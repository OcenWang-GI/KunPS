using Protocol;

namespace GameServer.Systems.Entity.Component
{
    internal class EntityLogicStateComponent : EntityComponentBase
    {
        public List<int> States { get; set; }

        public EntityLogicStateComponent()
        {
            // 使用 new List<int>() 初始化 States，确保 States 不为 null
            States = new List<int>();
        }
        public override EntityComponentType Type => EntityComponentType.LogicState;
        public override EntityComponentPb Pb
        {
            get
            {
                return new EntityComponentPb
                {
                    LogicStateComponentPb = new LogicStateComponentPb
                    {
                        States = { States.ToArray() }
                    }
                };
            }
        }
    }
}