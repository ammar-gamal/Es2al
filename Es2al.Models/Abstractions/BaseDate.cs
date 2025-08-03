namespace Es2al.Models.Abstractions
{
    public class BaseDate
    {
        public DateTime Date { get; set; }

        public string GetDate() => Date.ToString("MMM dd, yyyy hh:mm tt");
    }
}
