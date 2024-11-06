using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Model.Enum
{
    public enum Role
    {
        NEOVERENY = 0,    
        PACIENT = 1,      
        ZAMESTNANEC = 2,  
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
    }
}
