using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace WorldCities.Server.Data
{
    public class ApiResult<T>
    {
        #region Properties
        /// <summary>
        /// data result
        /// </summary>
        public List<T> Data { get; set; }
        /// <summary>
        /// zero-based index of current page
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// number of items on the page
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// total items
        /// </summary>
        public int TotalCount {  get; set; }
        /// <summary>
        /// total pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// true if the current page has a previous page 
        /// </summary>
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }
        /// <summary>
        /// true if the current page has a next page
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return ((PageIndex + 1) < TotalPages);
            }
        }
        #endregion
        public ApiResult(List<T> data, int count, int pageIndex, int pageSize)  
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        #region Methods
        public static async Task<ApiResult<T>> CreateAsync(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            //var count = await source.CountAsync();
            var count = source.Count();
            source = source.Skip(pageIndex * pageSize).Take(pageSize);
            var data = source.ToList();
            return new ApiResult<T>(
            data,
            count,
            pageIndex,
            pageSize);
        }
        #endregion
    }
}
