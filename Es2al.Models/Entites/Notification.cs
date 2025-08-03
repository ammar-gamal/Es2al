using Es2al.Models.Abstractions;

namespace Es2al.Models.Entites
{
    public class Notification : BaseText
    {
        public AppUser? User { get; set; }
        public int UserId { get; set; }
        public string? RelatedUrl { get; set; }
        public string? AnchorText { get; set; }
        public bool IsMarkedAsReed { get; set; }
      


    }
}
