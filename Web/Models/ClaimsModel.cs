using Core.Enums;
using Web.ViewModels;

namespace Web.Models
{
    public class ClaimsModel
    {
        public string UserId { get; set; } = string.Empty;
        public List<UserClaimViewModel> Claims { get; set; } = new List<UserClaimViewModel>();
        public List<UserClaims> UserClaims { get; set; } = new List<UserClaims>();
        public string NewClaimType { get; set; } = string.Empty;
        public string NewClaimValue { get; set; } = string.Empty;
    }
}
