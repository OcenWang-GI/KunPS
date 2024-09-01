using GameServer.Systems.Entity.Component;
using GameServer.Systems.Notify;
using Protocol;

namespace GameServer.Systems.Entity;
internal class MonsterEntity : EntityBase
{
    public MonsterEntity(long id, int configId, IGameActionListener listener) : base(id, listener)
    {
        ConfigId = configId;
        DynamicId = configId;
    }

    public int ConfigId { get; }

    public override EEntityType Type => EEntityType.Monster;
    public override EntityConfigType ConfigType => EntityConfigType.Level;


public override void OnCreate()
{
    base.OnCreate();

    var attributeComponent = ComponentSystem.Create<EntityAttributeComponent>();
    attributeComponent.SetAttribute(EAttributeType.LifeMax, 100);
    attributeComponent.SetAttribute(EAttributeType.Life, 100);

    State = EntityState.Born;

    var aiComponent = ComponentSystem.Create<EntityMonsterAiComponent>();
    aiComponent.AiTeamInitId = 100;

    var fsm = ComponentSystem.Create<EntityFsmComponent>();

    var mainFsmBattleBranching = new DFsm
    {
        FsmId = 10007, //  ID
        CurrentState = 10013 // 当前状态为战斗分支
    };
    fsm.Fsms.Add(mainFsmBattleBranching);

    var mainFsmMovingCombat = new DFsm
    {
        FsmId = 10007, //  ID
        CurrentState = 10015 // 当前状态为移动战斗
    };
    fsm.Fsms.Add(mainFsmMovingCombat);

    // 一些怪物需要武器
    var weaponFsm = new DFsm
    {
        FsmId = 100,
        CurrentState = 9 // [9 - 空手, 10 - 扳手, 11 - 喷火器, 12 - 链锯, 13 - 电刃, 14 - 狙击枪]
    };
    fsm.Fsms.Add(weaponFsm);
}

public override EntityPb Pb
{
    get
    {
        var pb = new EntityPb
        {
            Id = Id,
            EntityType = (int)Type,
            ConfigType = (int)ConfigType,
            EntityState = (int)State,
            ConfigId = ConfigId,
            Pos = Pos,
            Rot = Rot,
            LivingStatus = (int)LivingStatus,
            IsVisible = IsVisible,
            InitLinearVelocity = new(), // 可能需要具体值
            InitPos = new() // 可能需要具体值
        };

        pb.ComponentPbs.AddRange(ComponentSystem.Pb);

        return pb;
    }
}
}
