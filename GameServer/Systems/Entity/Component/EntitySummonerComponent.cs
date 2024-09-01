using Protocol;

namespace GameServer.Systems.Entity.Component
{
    internal class EntitySummonerComponent : EntityComponentBase
    {
        public int SummonConfigId { get; set; }
        public ESummonType SummonType { get; set; }
        public long SummonerId { get; set; }
        public int PlayerId { get; set; }
        public int SummonSkillId { get; set; }
        public int Level { get; set; } // 添加 Level 属性
        public long Timestamp { get; set; } // 添加 Timestamp 属性

        public override EntityComponentType Type => EntityComponentType.Summoner;

        public override EntityComponentPb Pb => new()
        {
            SummonerComponent = new()
            {
                SummonCfgId = SummonConfigId,
                Type = (int)SummonType,
                SummonerId = SummonerId,
                PlayerId = PlayerId,
                SummonSkillId = SummonSkillId,
            }
        };
    }
}