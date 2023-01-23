namespace CI_Platform_Project.Models
{
    public class Pager
    {
        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public int StartPage { get; set; }

        public int EndPage { get; set; }


        public Pager() { }

        public Pager(int totalItems, int page, int pagesize = 1)
        {
            int totalPages = (int)Math.Ceiling((float)totalItems / (float)pagesize);
            int currentPage = page;

            int startPage = currentPage - 3;
            int endPage = currentPage + 2;

            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 6)
                {
                    startPage = endPage - 5;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pagesize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
