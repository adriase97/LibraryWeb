using Core.Enums;

namespace Web.Models
{
    public class RolesModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<Role> Roles { get; set; } = new List<Role>();
        public string UserRole { get; set; } = string.Empty;
        public string SelectedRole { get; set; } = string.Empty;
    }
}
