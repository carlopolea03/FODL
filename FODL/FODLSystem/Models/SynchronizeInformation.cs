using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace FODLSystem.Models
{
    public class SynchronizeInformation
    {
        public int Id { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime? LastDownloaded { get; set; }

    }

    public static class ExtensionMethods
    {
        public static IQueryable<T> OrderByField<T>(this IQueryable<T> q, string SortField, string Ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, SortField);
            var exp = Expression.Lambda(prop, param);
            string method = Ascending.ToUpper() == "ASC" ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }
    }
}
