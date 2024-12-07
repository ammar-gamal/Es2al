namespace Es2al.Services.ViewModels
{
    public class DisplayUserVM
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public byte[]? Image { get; set; } = null!;
    }
}
