using Microsoft.AspNetCore.Components.Sections;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using WorldCities.Server.StaticClasses_Don_t_know_how_to_name_;

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
        /// ASC or DESC sorting
        /// </summary>
        public string? SortOrder { get; set; }
        /// <summary>
        /// exect column on which sorting is based (or null)
        /// </summary>
        public string? SortColumn { get; set; }

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
        /// <summary>
        /// Filter Column name or null
        /// </summary>
        public string? FilterColumn { get; set; }
        /// <summary>
        /// Filter Query string
        /// </summary>
        public string? FilterQuery { get; set; }

        #endregion
        public ApiResult(List<T> data, int count, int pageIndex, int pageSize, string? sortColumn, string? sortOrder, string? filterColumn, string? filterQuery)  
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            FilterColumn = filterColumn;
            FilterQuery = filterQuery;
        }

        #region Methods
        public static async Task<ApiResult<T>> CreateAsync(IEnumerable<T> source, int pageIndex, int pageSize, string? sortColumn = null, string? sortOrder = null, string? filterColumn = null, string? filterQuery = null)
        {
            IQueryable<T> queryableSource = source.AsQueryable();
            if(!String.IsNullOrEmpty(filterColumn) && !String.IsNullOrEmpty(filterQuery) 
                && IsValidProperty(filterColumn)) 
            {
                queryableSource = queryableSource.Where(ExpressionCreator<T>.CreateFilterExpression<T>(filterColumn, filterQuery));
            }

            if (!String.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn)) //нужно сделать красивие
            {
                sortOrder = !String.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "ASC"
                    ? "ASC"
                    : "DESC";
                if (sortOrder == "ASC")
                {
                    queryableSource = queryableSource.OrderBy(ExpressionCreator<T>.CreateSortExpression<T>(sortColumn));
                }
                else if (sortOrder == "DESC")
                {
                    queryableSource = queryableSource.OrderByDescending(ExpressionCreator<T>.CreateSortExpression<T>(sortColumn));
                }

            }

            var count = source.Count();
            queryableSource = queryableSource
                .Skip(pageIndex * pageSize)
                .Take(pageSize);
                
            var data = queryableSource.ToList();
            return new ApiResult<T>(
            data,
            count,
            pageIndex,
            pageSize,
            sortColumn,
            sortOrder,
            filterColumn,
            filterQuery);
        }

        public static bool IsValidProperty(string property)
        {
            var prop = typeof(T).GetProperty(
                property,
                BindingFlags.IgnoreCase |
                BindingFlags.Public |
                BindingFlags.Instance);
            if (prop == null)
                throw new NotSupportedException(string.Format($"ERROR: Property '{property}' does not exist."));

            return prop != null;
        }
        #endregion
    }
}
