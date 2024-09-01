using Core.Config;
using GameServer.Controllers.Attributes;
using GameServer.Extensions.Logic;
using GameServer.Models;
using GameServer.Network;
using GameServer.Settings;
using GameServer.Systems.Entity;
using GameServer.Systems.Entity.Component;
using GameServer.Systems.Event;
using GameServer.Systems.Notify;
using Microsoft.Extensions.Options;
using Protocol;
using System.Linq;

namespace GameServer.Controllers;
internal class CreatureController : Controller
{
    private const float DynamicSpawnRadius = 5000;
    private const float DynamicSpawnPositionDelta = 2500;

    private readonly EntitySystem _entitySystem;
    private readonly EntityFactory _entityFactory;
    private readonly ModelManager _modelManager;
    private readonly ConfigManager _configManager;
    private readonly IGameActionListener _listener;

    private readonly GameplayFeatureSettings _gameplayFeatures;
    private readonly Vector _lastDynamicSpawnPos;

    public CreatureController(PlayerSession session, EntitySystem entitySystem, EntityFactory entityFactory, ModelManager modelManager, ConfigManager configManager, IOptions<GameplayFeatureSettings> gameplayFeatures, IGameActionListener listener) : base(session)
    {
        _entitySystem = entitySystem;
        _entityFactory = entityFactory;
        _modelManager = modelManager;
        _configManager = configManager;
        _listener = listener;
        _gameplayFeatures = gameplayFeatures.Value;

        _lastDynamicSpawnPos = new();
    }

    public async Task JoinScene(int instanceId)
    {
        _modelManager.Creature.SetSceneLoadingData(instanceId);
        CreateTeamPlayerEntities();
        CreateWorldEntities();

        await _listener.OnJoinedScene(CreateSceneInfo(), TransitionType.Empty);

        // add AfterJoinSceneNotify
        await Task.Delay(TimeSpan.FromSeconds(3));
        await Session.Push(MessageId.JoinSceneFinishNotify, new JoinSceneFinishNotify());
    }

    [NetEvent(MessageId.EntityActiveRequest)]
    public async Task<RpcResult> OnEntityActiveRequest(EntityActiveRequest request)
    {
        EntityActiveResponse response;

        EntityBase? entity = _entitySystem.Get<EntityBase>(request.EntityId);
        if (entity != null)
        {
            _entitySystem.Activate(entity);
            response = new EntityActiveResponse
            {
                ErrorCode = (int)ErrorCode.Success,
                IsVisible = entity.IsVisible,
                Pos = _modelManager.Player.Position,
                Rot = _modelManager.Player.Rotation
            };

            response.ComponentPbs.AddRange(entity.ComponentSystem.Pb);
        }
        else
        {
            response = new EntityActiveResponse { ErrorCode = (int)ErrorCode.ErrActionEntityNoExist };
        }

        await OnVisionSkillChanged();
        return Response(MessageId.EntityActiveResponse, response);
    }

    [NetEvent(MessageId.SceneLoadingFinishRequest)]
    public async Task<RpcResult> OnSceneLoadingFinishRequest()
    {
        _modelManager.Creature.OnWorldDone();
        await UpdateAiHate();

        return Response(MessageId.SceneLoadingFinishResponse, new SceneLoadingFinishResponse());
    }

    [GameEvent(GameEventType.FormationUpdated)]
    public async Task OnFormationUpdated()
    {
        _entitySystem.Destroy(GetPlayerEntities().ToArray());
        CreateTeamPlayerEntities();

        _modelManager.Creature.PlayerEntityId = GetPlayerEntities().First().Id;
        await _listener.OnPlayerFightRoleInfoUpdated(_modelManager.Player.Id, GetFightRoleInfos());

        await UpdateAiHate();
    }

    [GameEvent(GameEventType.PlayerPositionChanged)]
    public void OnPlayerPositionChanged()
    {
        _modelManager.Player.Position.MergeFrom(GetPlayerEntity()!.Pos);
        _modelManager.Player.Rotation.MergeFrom(GetPlayerEntity()!.Rot);
        if (_lastDynamicSpawnPos.GetDistance(_modelManager.Player.Position) >= DynamicSpawnPositionDelta)
        {
            _lastDynamicSpawnPos.MergeFrom(_modelManager.Player.Position);

            ClearInactiveEntities();
            SpawnDynamicEntities();
        }
    }

    [GameEvent(GameEventType.VisionSkillChanged)]
    public async Task OnVisionSkillChanged()
    {
        PlayerEntity? playerEntity = GetPlayerEntity();
        if (playerEntity == null) return;

        EntityVisionSkillComponent visionSkillComponent = playerEntity.ComponentSystem.Get<EntityVisionSkillComponent>();

        VisionSkillChangeNotify skillChangeNotify = new() { EntityId = playerEntity.Id };
        skillChangeNotify.VisionSkillInfos.AddRange(visionSkillComponent.Skills);

        await Session.Push(MessageId.VisionSkillChangeNotify, skillChangeNotify);
    }

    public PlayerEntity? GetPlayerEntity()
    {
        return _entitySystem.Get<PlayerEntity>(_modelManager.Creature.PlayerEntityId);
    }

    public PlayerEntity? GetPlayerEntityByRoleId(int roleId)
    {
        return _entitySystem.EnumerateEntities()
        .FirstOrDefault(e => e is PlayerEntity playerEntity && playerEntity.ConfigId == roleId && playerEntity.PlayerId == _modelManager.Player.Id) as PlayerEntity;
    }

    public IEnumerable<PlayerEntity> GetPlayerEntities()
    {
        return _entitySystem.EnumerateEntities()
        .Where(e => e is PlayerEntity entity && entity.PlayerId == _modelManager.Player.Id)
        .Cast<PlayerEntity>();
    }

    public async Task SwitchPlayerEntity(int roleId)
    {
        PlayerEntity? prevEntity = GetPlayerEntity();
        if (prevEntity == null) return;

        prevEntity.IsCurrentRole = false;

        if (_entitySystem.EnumerateEntities().FirstOrDefault(e => e is PlayerEntity playerEntity && playerEntity.ConfigId == roleId) is not PlayerEntity newEntity) return;

        _modelManager.Creature.PlayerEntityId = newEntity.Id;
        newEntity.IsCurrentRole = true;

        newEntity.Pos.MergeFrom(prevEntity.Pos);
        newEntity.Rot.MergeFrom(prevEntity.Rot);

        await UpdateAiHate();
    }

    public async Task UpdateAiHate()
    {
        IEnumerable<EntityBase> monsters = _entitySystem.EnumerateEntities().Where(e => e is MonsterEntity);
        if (!monsters.Any()) return;

        await Session.Push(MessageId.CombatReceivePackNotify, new CombatReceivePackNotify
        {
            Data =
            {
                monsters.Select(monster => new CombatReceiveData
                {
                    CombatNotifyData = new()
                    {
                        CombatCommon = new() { EntityId = monster.Id },
                        AiHateNotify = new()
                        {
                            HateList =
                            {
                                GetPlayerEntities().Select(player => new AiHateEntity
                                {
                                    EntityId = player.Id,
                                    HatredValue = 99999 // currently this, TODO!
                                })
                            }
                        }
                    }
                })
            }
        });
    }

    private SceneInformation CreateSceneInfo() => new()
    {
        InstanceId = _modelManager.Creature.InstanceId,
        OwnerId = _modelManager.Creature.OwnerId,
        CurContextId = _modelManager.Player.Id,
        TimeInfo = new(),
        AoiData = new PlayerSceneAoiData
        {
            Entities = { _entitySystem.Pb }
        },
        PlayerInfos =
        {
            new ScenePlayerInformation
            {
                PlayerId = _modelManager.Player.Id,
                Level = 1,
                IsOffline = false,
                Rotation = _modelManager.Player.Rotation,
                Location = _modelManager.Player.Position,
                PlayerName = _modelManager.Player.Name,
                Crs = { GetCrsInfos()},
                GroupType = 1,
                CurRole = _modelManager.Formation.RoleIds[0],
                //FightRoleInfos = { GetFightRoleInfos() }
            }
        }
    };

    private IEnumerable<FightRoleInformation> GetFightRoleInfos()
    {
        return GetPlayerEntities().Select(playerEntity => new FightRoleInformation
        {
            EntityId = playerEntity.Id,
            CurHp = playerEntity.Health,
            MaxHp = playerEntity.HealthMax,
            IsControl = playerEntity.Id == _modelManager.Creature.PlayerEntityId,
            RoleId = playerEntity.ConfigId,
            RoleLevel = 1,
        });
    }

    private IEnumerable<CRs> GetCrsInfos()
    {
        return GetPlayerEntities().Select(playerEntity => new CRs
        {
            GroupType = 1,
            FightRoleInfos =
            {
                new NFs
                {
                    RoleId = playerEntity.ConfigId,
                    EntityId = playerEntity.Id,
                }
            },
            CurRole = _modelManager.Formation.RoleIds[0],
            LivingStatus = 1,
            Nda = false,
        });
    }

    private void CreateTeamPlayerEntities()
    {
        PlayerEntity[] playerEntities = new PlayerEntity[_modelManager.Formation.RoleIds.Length];

        for (int i = 0; i < _modelManager.Formation.RoleIds.Length; i++)
        {
            int roleId = _modelManager.Formation.RoleIds[i];

            PlayerEntity entity = _entityFactory.CreatePlayer(roleId, _modelManager.Player.Id);
            entity.Pos = _modelManager.Player.Position.Clone();
            entity.IsCurrentRole = i == 0;

            entity.ComponentSystem.Get<EntityAttributeComponent>().SetAll(_modelManager.Roles.GetRoleById(roleId)!.GetAttributeList());

            CreateConcomitants(entity);
            entity.WeaponId = _modelManager.Inventory.GetEquippedWeapon(roleId)?.Id ?? 0;

            if (i == 0) _modelManager.Creature.PlayerEntityId = entity.Id;

            if (_gameplayFeatures.UnlimitedEnergy)
            {
                EntityAttributeComponent attr = entity.ComponentSystem.Get<EntityAttributeComponent>();
                attr.SetAttribute(EAttributeType.EnergyMax, 0);
                attr.SetAttribute(EAttributeType.SpecialEnergy1Max, 0);
                attr.SetAttribute(EAttributeType.SpecialEnergy2Max, 0);
                attr.SetAttribute(EAttributeType.SpecialEnergy3Max, 0);
                attr.SetAttribute(EAttributeType.SpecialEnergy4Max, 0);
            }

            playerEntities[i] = entity;
        }

        _entitySystem.Add(playerEntities);
    }

    private void CreateConcomitants(PlayerEntity entity)
    {
        (int roleId, int summonConfigId) = entity.ConfigId switch
        {
            1302 => (5002, 10070301),
            _ => (-1, -1)
        };

        if (roleId != -1)
        {
            PlayerEntity concomitant = _entityFactory.CreatePlayer(roleId, 0);

            EntityConcomitantsComponent concomitants = entity.ComponentSystem.Create<EntityConcomitantsComponent>();
            concomitants.CustomEntityIds.Clear();
            concomitants.VisionEntityId = concomitant.Id;
            concomitants.CustomEntityIds.Add(concomitant.Id);

            EntitySummonerComponent summoner = concomitant.ComponentSystem.Create<EntitySummonerComponent>();
            summoner.SummonerId = entity.Id;
            summoner.SummonConfigId = summonConfigId;
            summoner.SummonType = ESummonType.ConcomitantCustom;
            summoner.PlayerId = _modelManager.Player.Id;

            concomitant.InitProps(_configManager.GetConfig<BasePropertyConfig>(roleId)!);
            _entitySystem.Add([concomitant]);
        }
    }

    private void CreateWorldEntities()
    {
        _lastDynamicSpawnPos.MergeFrom(_modelManager.Player.Position.Clone());
        SpawnDynamicEntities();
    }

    private void ClearInactiveEntities()
    {
        _entitySystem.Destroy(_entitySystem.EnumerateEntities()
            .Where(e => e is MonsterEntity && e.DynamicId != 0 &&
            e.Pos.GetDistance(_modelManager.Player.Position) > DynamicSpawnRadius).ToArray());
    }

    private void SpawnDynamicEntities()
    {
        Vector playerPos = _modelManager.Player.Position;

        // Currently only monsters
        IEnumerable<LevelEntityConfig> entitiesToSpawn = _configManager.Enumerate<LevelEntityConfig>()
        .Where(config => config.MapId == 8 && Math.Abs(config.Transform[0].X / 100 - playerPos.X) < DynamicSpawnRadius && Math.Abs(config.Transform[0].Y / 100 - playerPos.Y) < DynamicSpawnRadius &&
        config.BlueprintType.StartsWith("Monster"));

        List<MonsterEntity> spawnMonsters = [];
        foreach (LevelEntityConfig levelEntity in entitiesToSpawn)
        {
            if (_entitySystem.HasDynamicEntity(levelEntity.EntityId)) continue;

            MonsterEntity monster = _entityFactory.CreateMonster(levelEntity.EntityId);
            monster.Pos = new()
            {
                X = levelEntity.Transform[0].X / 100,
                Y = levelEntity.Transform[0].Y / 100,
                Z = levelEntity.Transform[0].Z / 100
            };

            monster.InitProps(_configManager.GetConfig<BasePropertyConfig>(600000100)!);
            spawnMonsters.Add(monster);
        }

        _entitySystem.Add(spawnMonsters);
    }
}
