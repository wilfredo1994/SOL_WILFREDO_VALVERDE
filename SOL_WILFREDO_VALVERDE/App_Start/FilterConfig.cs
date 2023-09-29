using System.Web;
using System.Web.Mvc;

namespace SOL_WILFREDO_VALVERDE
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
