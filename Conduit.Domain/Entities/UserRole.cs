using System.ComponentModel.DataAnnotations;

namespace Conduit.Domain.Entities
{
    public class UserRole
    {
        [Key]
        public Guid UserID { get; set; }
        [Key]
        public int Role { get; set; }
        public virtual User User { get; set; }

        public UserRole(Guid userID, int role)
        {
            UserID = userID;
            Role = role;
        }

        public UserRole(Guid userID, RoleList role)
        {
            UserID = userID;
            Role = (int)role;
        }
    }

    public enum RoleList
    {
        SystemAdmin,
        SystemUser,
    }
}
