using Es2al.Models.Entites;
using Es2al.Services.ViewModels;

namespace Es2al.Services.ExtensionMethods
{
    public static class QuestionQueryableExtensions
    {
        public static IQueryable<Question> ApplyFilters(this IQueryable<Question> questions, QuestionFilterVM? questionFilterVM)
        {
            if (questionFilterVM == null)
                return questions.OrderBy(e => e.Date);

            questions = questionFilterVM.SortOrder == "desc"
                         ? questions.OrderByDescending(e => e.Date)
                         : questions.OrderBy(e => e.Date);


            if (questionFilterVM.DateFrom.HasValue)
                questions = questions.Where(e => e.Date >= questionFilterVM.DateTimeFrom);


            if (questionFilterVM.DateEnd.HasValue)
                questions = questions.Where(e => e.Date <= questionFilterVM.DateTimeEnd);

            if (!string.IsNullOrWhiteSpace(questionFilterVM.SearchKeyword))
                questions = questions.Where(e => e.Text.ToLower().Contains(questionFilterVM.SearchKeyword));

            if (questionFilterVM.Tags != null && questionFilterVM.Tags.Any())
                questions = questions.Where(q => q.Tags.Any(tag => questionFilterVM.Tags.Contains(tag.Tag.Name)));

            return questions;
        }
    }
}
