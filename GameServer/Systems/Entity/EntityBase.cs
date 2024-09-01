using Core.Config;
using GameServer.Systems.Entity.Component;
using GameServer.Systems.Notify;
using Protocol;

namespace GameServer.Systems.Entity;
internal abstract class EntityBase
{
// 定义一个公开的只读属性 Id，用于存储实体的唯一标识符
public long Id { get; }

// 定义一个公开的只读属性 ComponentSystem，用于存储实体的组件系统
public EntityComponentSystem ComponentSystem { get; }

// 定义一个公开的可读写属性 Pos，用于存储实体的位置
public Vector Pos { get; set; }

// 定义一个公开的可读写属性 Rot，用于存储实体的旋转角度
public Rotator Rot { get; set; }

// 定义一个公开的可读写属性 Active，用于表示实体是否处于活动状态
public bool Active { get; set; }

// 定义一个受保护的可读写属性 DynamicId，用于存储实体的动态 ID
public int DynamicId { get; protected set; }

// 定义一个受保护的可读写属性 State，用于存储实体的状态
public EntityState State { get; protected set; }

// 定义一个只读属性 IsConcomitant，用于判断实体是否伴随其他实体
// 使用 ComponentSystem 尝试获取 EntitySummonerComponent 组件，如果存在则返回 true，否则返回 false
public bool IsConcomitant => ComponentSystem.TryGet<EntitySummonerComponent>(out _);

// 定义一个受保护的只读属性 ActionListener，用于存储游戏动作监听器
protected IGameActionListener ActionListener { get; }

// 定义一个公开的构造函数，用于创建 EntityBase 实例
public EntityBase(long id, IGameActionListener listener)
{
    // 设置实体的唯一标识符
    Id = id;

    // 初始化实体的位置
    Pos = new Vector();

    // 初始化实体的旋转角度
    Rot = new Rotator();

    // 创建实体的组件系统
    ComponentSystem = new EntityComponentSystem();

    // 设置游戏动作监听器
    ActionListener = listener;
}
// 定义一个公开的虚拟方法，用于在实体创建时执行初始化操作
public virtual void OnCreate()
{
    // 设置实体的状态为 "Born"（出生）
    State = EntityState.Born;

    // 创建 EntityLogicStateComponent 组件，并忽略返回值
    _ = ComponentSystem.Create<EntityLogicStateComponent>();

    // 创建 EntityFightBuffComponent 组件，并忽略返回值
    _ = ComponentSystem.Create<EntityFightBuffComponent>();
}

    public virtual void Activate()
    {
        // Activate.
    }

// 定义一个公开的方法，用于改变实体装备的武器 ID
public void ChangeEquipment(int weaponId)
{
    // 尝试从 ComponentSystem 中获取 EntityEquipComponent 组件
    if (ComponentSystem.TryGet(out EntityEquipComponent? equipComponent))
    {
        // 如果成功获取到 EntityEquipComponent 组件，则设置武器 ID
        equipComponent.WeaponId = weaponId;

        // 触发事件 OnEntityEquipmentChanged，通知其他系统或组件实体的装备已更改
        // 第一个参数 Id 表示实体的唯一标识符
        // 第二个参数 EquipComponent 表示更新后的装备组件
        _ = ActionListener.OnEntityEquipmentChanged(Id, equipComponent.Pb.EquipComponent);
    }
}

// 定义一个公开的方法，用于改变实体的游戏玩法属性
public void ChangeGameplayAttributes(IEnumerable<GameplayAttributeData> attributes)
{
    // 尝试从 ComponentSystem 中获取 EntityAttributeComponent 组件
    if (ComponentSystem.TryGet(out EntityAttributeComponent? attrComponent))
    {
        // 如果成功获取到 EntityAttributeComponent 组件，则设置所有传入的游戏玩法属性
        attrComponent.SetAll(attributes);

        // 触发事件 OnEntityAttributesChanged，通知其他系统或组件实体的游戏玩法属性已更改
        // 第一个参数 Id 表示实体的唯一标识符
        // 第二个参数 GameAttributes 表示更新后的游戏玩法属性集合
        _ = ActionListener.OnEntityAttributesChanged(Id, attrComponent.Pb.AttributeComponent.GameAttributes);
    }
}

    public virtual LivingStatus LivingStatus => LivingStatus.Alive;
    public virtual bool IsVisible => true;

    public abstract EEntityType Type { get; }
    public abstract EntityConfigType ConfigType { get; }

    public abstract EntityPb Pb { get; }

    public void InitProps(BasePropertyConfig config)
    {
        EntityAttributeComponent attributeComponent = ComponentSystem.Get<EntityAttributeComponent>();

        attributeComponent.SetAttribute(EAttributeType.Lv, config.Lv);
        attributeComponent.SetAttribute(EAttributeType.LifeMax, config.LifeMax);
        attributeComponent.SetAttribute(EAttributeType.Life, config.Life);
        attributeComponent.SetAttribute(EAttributeType.Sheild, config.Sheild);
        attributeComponent.SetAttribute(EAttributeType.SheildDamageChange, config.SheildDamageChange);
        attributeComponent.SetAttribute(EAttributeType.SheildDamageReduce, config.SheildDamageReduce);
        attributeComponent.SetAttribute(EAttributeType.Atk, config.Atk);
        attributeComponent.SetAttribute(EAttributeType.Crit, config.Crit);
        attributeComponent.SetAttribute(EAttributeType.CritDamage, config.CritDamage);
        attributeComponent.SetAttribute(EAttributeType.Def, config.Def);
        attributeComponent.SetAttribute(EAttributeType.EnergyEfficiency, config.EnergyEfficiency);
        attributeComponent.SetAttribute(EAttributeType.CdReduse, config.CdReduse);
        attributeComponent.SetAttribute(EAttributeType.ReactionEfficiency, config.ReactionEfficiency);
        attributeComponent.SetAttribute(EAttributeType.DamageChangeNormalSkill, config.DamageChangeNormalSkill);
        attributeComponent.SetAttribute(EAttributeType.DamageChange, config.DamageChange);
        attributeComponent.SetAttribute(EAttributeType.DamageReduce, config.DamageReduce);
        attributeComponent.SetAttribute(EAttributeType.DamageChangeAuto, config.DamageChangeAuto);
        attributeComponent.SetAttribute(EAttributeType.DamageChangeCast, config.DamageChangeCast);
        attributeComponent.SetAttribute(EAttributeType.DamageChangeUltra, config.DamageChangeUltra);
        attributeComponent.SetAttribute(EAttributeType.DamageChangeQte, config.DamageChangeQte);
        attributeComponent.SetAttribute(EAttributeType.DamageChangePhys, config.DamageChangePhys);
        attributeComponent.SetAttribute(EAttributeType.DamageChangeElement1, config.DamageChangeElement1);
        attributeComponent.SetAttribute(EAttributeType.DamageChangeElement2, config.DamageChangeElement2);
        attributeComponent.SetAttribute(EAttributeType.DamageChangeElement3, config.DamageChangeElement3);
        attributeComponent.SetAttribute(EAttributeType.DamageChangeElement4, config.DamageChangeElement4);
        attributeComponent.SetAttribute(EAttributeType.DamageChangeElement5, config.DamageChangeElement5);
        attributeComponent.SetAttribute(EAttributeType.DamageChangeElement6, config.DamageChangeElement6);
        attributeComponent.SetAttribute(EAttributeType.DamageResistancePhys, config.DamageResistancePhys);
        attributeComponent.SetAttribute(EAttributeType.DamageResistanceElement1, config.DamageResistanceElement1);
        attributeComponent.SetAttribute(EAttributeType.DamageResistanceElement2, config.DamageResistanceElement2);
        attributeComponent.SetAttribute(EAttributeType.DamageResistanceElement3, config.DamageResistanceElement3);
        attributeComponent.SetAttribute(EAttributeType.DamageResistanceElement4, config.DamageResistanceElement4);
        attributeComponent.SetAttribute(EAttributeType.DamageResistanceElement5, config.DamageResistanceElement5);
        attributeComponent.SetAttribute(EAttributeType.DamageResistanceElement6, config.DamageResistanceElement6);
        attributeComponent.SetAttribute(EAttributeType.HealChange, config.HealChange);
        attributeComponent.SetAttribute(EAttributeType.HealedChange, config.HealedChange);
        attributeComponent.SetAttribute(EAttributeType.DamageReducePhys, config.DamageReducePhys);
        attributeComponent.SetAttribute(EAttributeType.DamageReduceElement1, config.DamageReduceElement1);
        attributeComponent.SetAttribute(EAttributeType.DamageReduceElement2, config.DamageReduceElement2);
        attributeComponent.SetAttribute(EAttributeType.DamageReduceElement3, config.DamageReduceElement3);
        attributeComponent.SetAttribute(EAttributeType.DamageReduceElement4, config.DamageReduceElement4);
        attributeComponent.SetAttribute(EAttributeType.DamageReduceElement5, config.DamageReduceElement5);
        attributeComponent.SetAttribute(EAttributeType.DamageReduceElement6, config.DamageReduceElement6);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange1, config.ReactionChange1);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange2, config.ReactionChange2);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange3, config.ReactionChange3);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange4, config.ReactionChange4);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange5, config.ReactionChange5);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange6, config.ReactionChange6);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange7, config.ReactionChange7);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange8, config.ReactionChange8);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange9, config.ReactionChange9);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange10, config.ReactionChange10);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange11, config.ReactionChange11);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange12, config.ReactionChange12);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange13, config.ReactionChange13);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange14, config.ReactionChange14);
        attributeComponent.SetAttribute(EAttributeType.ReactionChange15, config.ReactionChange15);
        attributeComponent.SetAttribute(EAttributeType.EnergyMax, config.EnergyMax);
        attributeComponent.SetAttribute(EAttributeType.Energy, config.Energy);
        attributeComponent.SetAttribute(EAttributeType.SpecialEnergy1Max, config.SpecialEnergy1Max);
        attributeComponent.SetAttribute(EAttributeType.SpecialEnergy1, config.SpecialEnergy1);
        attributeComponent.SetAttribute(EAttributeType.SpecialEnergy2Max, config.SpecialEnergy2Max);
        attributeComponent.SetAttribute(EAttributeType.SpecialEnergy2, config.SpecialEnergy2);
        attributeComponent.SetAttribute(EAttributeType.SpecialEnergy3Max, config.SpecialEnergy3Max);
        attributeComponent.SetAttribute(EAttributeType.SpecialEnergy3, config.SpecialEnergy3);
        attributeComponent.SetAttribute(EAttributeType.SpecialEnergy4Max, config.SpecialEnergy4Max);
        attributeComponent.SetAttribute(EAttributeType.SpecialEnergy4, config.SpecialEnergy4);
        attributeComponent.SetAttribute(EAttributeType.StrengthMax, config.StrengthMax);
        attributeComponent.SetAttribute(EAttributeType.Strength, config.Strength);
        attributeComponent.SetAttribute(EAttributeType.StrengthRecover, config.StrengthRecover);
        attributeComponent.SetAttribute(EAttributeType.StrengthPunishTime, config.StrengthPunishTime);
        attributeComponent.SetAttribute(EAttributeType.StrengthRun, config.StrengthRun);
        attributeComponent.SetAttribute(EAttributeType.StrengthSwim, config.StrengthSwim);
        attributeComponent.SetAttribute(EAttributeType.StrengthFastSwim, config.StrengthFastSwim);
        attributeComponent.SetAttribute(EAttributeType.StrengthClimb, config.StrengthClimb);
        attributeComponent.SetAttribute(EAttributeType.StrengthFastClimb, config.StrengthFastClimb);
        attributeComponent.SetAttribute(EAttributeType.HardnessMax, config.HardnessMax);
        attributeComponent.SetAttribute(EAttributeType.Hardness, config.Hardness);
        attributeComponent.SetAttribute(EAttributeType.HardnessRecover, config.HardnessRecover);
        attributeComponent.SetAttribute(EAttributeType.HardnessPunishTime, config.HardnessPunishTime);
        attributeComponent.SetAttribute(EAttributeType.HardnessChange, config.HardnessChange);
        attributeComponent.SetAttribute(EAttributeType.HardnessReduce, config.HardnessReduce);
        attributeComponent.SetAttribute(EAttributeType.RageMax, config.RageMax);
        attributeComponent.SetAttribute(EAttributeType.Rage, config.Rage);
        attributeComponent.SetAttribute(EAttributeType.RageRecover, config.RageRecover);
        attributeComponent.SetAttribute(EAttributeType.RagePunishTime, config.RagePunishTime);
        attributeComponent.SetAttribute(EAttributeType.RageChange, config.RageChange);
        attributeComponent.SetAttribute(EAttributeType.RageReduce, config.RageReduce);
        attributeComponent.SetAttribute(EAttributeType.ToughMax, config.ToughMax);
        attributeComponent.SetAttribute(EAttributeType.Tough, config.Tough);
        attributeComponent.SetAttribute(EAttributeType.ToughRecover, config.ToughRecover);
        attributeComponent.SetAttribute(EAttributeType.ToughChange, config.ToughChange);
        attributeComponent.SetAttribute(EAttributeType.ToughReduce, config.ToughReduce);
        attributeComponent.SetAttribute(EAttributeType.ToughRecoverDelayTime, config.ToughRecoverDelayTime);
        attributeComponent.SetAttribute(EAttributeType.ElementPower1, config.ElementPower1);
        attributeComponent.SetAttribute(EAttributeType.ElementPower2, config.ElementPower2);
        attributeComponent.SetAttribute(EAttributeType.ElementPower3, config.ElementPower3);
        attributeComponent.SetAttribute(EAttributeType.ElementPower4, config.ElementPower4);
        attributeComponent.SetAttribute(EAttributeType.ElementPower5, config.ElementPower5);
        attributeComponent.SetAttribute(EAttributeType.ElementPower6, config.ElementPower6);
        attributeComponent.SetAttribute(EAttributeType.SpecialDamageChange, config.SpecialDamageChange);
        attributeComponent.SetAttribute(EAttributeType.StrengthFastClimbCost, config.StrengthFastClimbCost);
        attributeComponent.SetAttribute(EAttributeType.ElementPropertyType, config.ElementPropertyType);
        attributeComponent.SetAttribute(EAttributeType.WeakTime, config.WeakTime);
        attributeComponent.SetAttribute(EAttributeType.IgnoreDefRate, config.IgnoreDefRate);
        attributeComponent.SetAttribute(EAttributeType.IgnoreDamageResistancePhys, config.IgnoreDamageResistancePhys);
        attributeComponent.SetAttribute(EAttributeType.IgnoreDamageResistanceElement1, config.IgnoreDamageResistanceElement1);
        attributeComponent.SetAttribute(EAttributeType.IgnoreDamageResistanceElement2, config.IgnoreDamageResistanceElement2);
        attributeComponent.SetAttribute(EAttributeType.IgnoreDamageResistanceElement3, config.IgnoreDamageResistanceElement3);
        attributeComponent.SetAttribute(EAttributeType.IgnoreDamageResistanceElement4, config.IgnoreDamageResistanceElement4);
        attributeComponent.SetAttribute(EAttributeType.IgnoreDamageResistanceElement5, config.IgnoreDamageResistanceElement5);
        attributeComponent.SetAttribute(EAttributeType.IgnoreDamageResistanceElement6, config.IgnoreDamageResistanceElement6);
        attributeComponent.SetAttribute(EAttributeType.SkillToughRatio, config.SkillToughRatio);
        attributeComponent.SetAttribute(EAttributeType.StrengthClimbJump, config.StrengthClimbJump);
        attributeComponent.SetAttribute(EAttributeType.StrengthGliding, config.StrengthGliding);
        attributeComponent.SetAttribute(EAttributeType.Mass, config.Mass);
        attributeComponent.SetAttribute(EAttributeType.BrakingFrictionFactor, config.BrakingFrictionFactor);
        attributeComponent.SetAttribute(EAttributeType.GravityScale, config.GravityScale);
        attributeComponent.SetAttribute(EAttributeType.SpeedRatio, config.SpeedRatio);
        attributeComponent.SetAttribute(EAttributeType.DamageChangePhantom, config.DamageChangePhantom);
        attributeComponent.SetAttribute(EAttributeType.AutoAttackSpeed, config.AutoAttackSpeed);
        attributeComponent.SetAttribute(EAttributeType.CastAttackSpeed, config.CastAttackSpeed);
        attributeComponent.SetAttribute(EAttributeType.StatusBuildUp1Max, config.StatusBuildUp1Max);
        attributeComponent.SetAttribute(EAttributeType.StatusBuildUp1, config.StatusBuildUp1);
        attributeComponent.SetAttribute(EAttributeType.StatusBuildUp2Max, config.StatusBuildUp2Max);
        attributeComponent.SetAttribute(EAttributeType.StatusBuildUp2, config.StatusBuildUp2);
        attributeComponent.SetAttribute(EAttributeType.StatusBuildUp3Max, config.StatusBuildUp3Max);
        attributeComponent.SetAttribute(EAttributeType.StatusBuildUp3, config.StatusBuildUp3);
        attributeComponent.SetAttribute(EAttributeType.StatusBuildUp4Max, config.StatusBuildUp4Max);
        attributeComponent.SetAttribute(EAttributeType.StatusBuildUp4, config.StatusBuildUp4);
        attributeComponent.SetAttribute(EAttributeType.StatusBuildUp5Max, config.StatusBuildUp5Max);
        attributeComponent.SetAttribute(EAttributeType.StatusBuildUp5, config.StatusBuildUp5);
        attributeComponent.SetAttribute(EAttributeType.ParalysisTimeMax, config.ParalysisTimeMax);
        attributeComponent.SetAttribute(EAttributeType.ParalysisTime, config.ParalysisTime);
        attributeComponent.SetAttribute(EAttributeType.ParalysisTimeRecover, config.ParalysisTimeRecover);
    }
}
