using System.Web.Mvc;

namespace OI.MVC.Helpers
{
    public static class MvcExtensionHelper
    {
        public static MvcHtmlString If(this MvcHtmlString value, bool evaluation, MvcHtmlString falseValue = default(MvcHtmlString))
        {
            return evaluation ? value : falseValue;
        }
    }
}