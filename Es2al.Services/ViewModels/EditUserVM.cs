using System.ComponentModel.DataAnnotations;

namespace Es2al.Services.ViewModels { 
    public class EditUserVM
    {
        public string UserName { get; set; } = null!;
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        public string? Bio { get; set; } = null!;
        public byte[]? Image { get; set; } = null!;
        public HashSet<int>? Tags { get; set; } = null!;
       
    }
}
