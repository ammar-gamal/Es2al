namespace Es2al.Models.Abstractions
{
    public abstract class BaseText:BaseDate
    {
        virtual public int Id { get; set; }
        virtual public string Text { get; set; } = String.Empty;
    }
}
