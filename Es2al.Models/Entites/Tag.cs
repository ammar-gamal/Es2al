namespace Es2al.Models.Entites
{
    public class Tag
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        public ICollection<QuestionTag> Questions { get; set; } = null!;
        public ICollection<UserTag> Users { get; set; } = null!;
        public override bool Equals(object? obj)
        {
            if (obj is not Tag)
                return false;
            var tag = obj as Tag;
            if (tag is null)
                return false;
            return Id.Equals(tag.Id);
        }
        public override int GetHashCode()
        {
            return Id;
        }

    }
}
