using System.ComponentModel.DataAnnotations;

namespace Es2al.Services.ViewModels
{
    public class LoginVM
    {
        public string EmailOrUserName { get; set; } = null!;
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
