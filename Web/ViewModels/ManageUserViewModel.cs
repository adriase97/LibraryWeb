using Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Models;

namespace Web.ViewModels
{
    public class ManageUserViewModel
    {
        public RolesModel Roles { get; set; } = new RolesModel();
        public ClaimsModel Claims { get; set; } = new ClaimsModel();
    }
}
