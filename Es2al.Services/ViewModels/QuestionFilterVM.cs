using System.ComponentModel.DataAnnotations;


namespace Es2al.Services.ViewModels
{
    public class QuestionFilterVM
    {
        private string? _searchKeyword = null;
        [DataType(DataType.Date)]
        public DateOnly? DateFrom { get; set; } = null;
        [DataType(DataType.Date)]
        public DateOnly? DateEnd { get; set; } = null;
      
        public HashSet<string>? Tags { get; set; } = null;
        public string? SortOrder { get; set; } = null;
        public string? SearchKeyword { get => _searchKeyword; set { _searchKeyword = value?.ToLower(); } }
        public DateTime? DateTimeFrom => DateFrom?.ToDateTime(TimeOnly.MinValue);
        public DateTime? DateTimeEnd => DateEnd?.ToDateTime(TimeOnly.MinValue).AddDays(1).AddTicks(-1);

    }
}