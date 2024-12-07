using Es2al.Services.CustomException;
using Microsoft.EntityFrameworkCore;


namespace Es2al.Services.Paging
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public PaginatedList(List<T> items, int totalPages, int pageIndex)
        {
            this.AddRange(items);
            TotalPages = totalPages;
            PageIndex = pageIndex;
        }
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            if (count > 0)
            {
                var totalPages = (int)Math.Ceiling((double)count / pageSize);
                if (pageIndex > totalPages || pageIndex < 1)
                {

                    throw new AppException();
                }

                var items = await source.Skip((pageIndex - 1) * pageSize)
                                        .Take(pageSize).ToListAsync();
                return new PaginatedList<T>(items, totalPages, pageIndex);
            }
            else
                return new PaginatedList<T>([], 0, 0);
        }
      
    }
}
