using System.ComponentModel.DataAnnotations;

namespace Es2al.Services.ViewModels
{
    public class RegisterVM
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;
        public string UserName { get; set; } = null!;

    }
}
