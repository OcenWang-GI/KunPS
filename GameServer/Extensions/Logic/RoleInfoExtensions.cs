using Core.Config;
using Protocol;

namespace GameServer.Extensions.Logic
{
    internal static class RoleInfoExtensions
    {
        /// <summary>
        /// Generates a list of gameplay attributes for a role.
        /// </summary>
        /// <param name="role">The role whose attributes are to be generated.</param>
        /// <returns>An enumerable collection of gameplay attribute data.</returns>
        public static IEnumerable<GameplayAttributeData> GetAttributeList(this RoleInfo role)
        {
            return role.BaseProp.Select(prop => new GameplayAttributeData
            {
                AttributeType = prop.Key,
                BaseValue = prop.Value,
                CurrentValue = prop.Value + (role.AddProp.FirstOrDefault(p => p.Key == prop.Key)?.Value ?? 0),
            });
        }

        /// <summary>
        /// Applies weapon properties to a role's additional properties.
        /// </summary>
        /// <param name="role">The role to apply the properties to.</param>
        /// <param name="weaponConf">The weapon configuration to use.</param>
        public static void ApplyWeaponProperties(this RoleInfo role, WeaponConfig weaponConf)
        {
            role.AddProp.Clear();

            if (weaponConf.FirstPropId != null)
            {
                role.AddProp.Add(new ArrayIntInt
                {
                    Key = weaponConf.FirstPropId.Id,
                    Value = (int)weaponConf.FirstPropId.Value
                });
            }

            if (weaponConf.SecondPropId != null)
            {
                role.AddProp.Add(new ArrayIntInt
                {
                    Key = weaponConf.SecondPropId.Id,
                    Value = (int)weaponConf.SecondPropId.Value
                });
            }
        }
    }
}