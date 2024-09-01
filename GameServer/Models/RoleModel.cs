using Protocol;

namespace GameServer.Models
{
    internal class RoleModel
    {
        public List<RoleInfo> Roles { get; } = new List<RoleInfo>();

        public RoleInfo Create(int id)
        {
            RoleInfo info = new RoleInfo
            {
                RoleId = id,
                Level = 1,
            };

            Roles.Add(info);
            return info;
        }

        public RoleInfo? GetRoleById(int roleId)
        {
            return Roles.SingleOrDefault(role => role.RoleId == roleId);
        }
    }
}