using System;
using System.ComponentModel;
using System.Reflection;

namespace BDAS2_SEM.Model.Enum
{
    public enum Role
    {
        [Description("neovereny")]
        NEOVERENY = 0,

        [Description("pacient")]
        PACIENT = 1,

        [Description("zamestnanec")]
        ZAMESTNANEC = 2,

        [Description("admin")]
        ADMIN = 3
    }

    public static class RoleService
    {
        public static Role GetRoleById(int roleId)
        {
            if (Role.IsDefined(typeof(Role), roleId))
            {
                return (Role)roleId;
            }
            else
            {
                throw new ArgumentException("Invalid role ID");
            }
        }

        public static string GetRoleName(Role role)
        {
            FieldInfo fi = role.GetType().GetField(role.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return role.ToString().ToLower();
        }
    }
}
