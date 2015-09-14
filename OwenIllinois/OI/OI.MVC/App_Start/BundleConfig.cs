using System.Web;
using System.Web.Optimization;

namespace OI.MVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
                        "~/Scripts/plugins/datatables/jquery.dataTables.js",
                        "~/Scripts/plugins/datatables/dataTables.bootstrap.js"                        
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/AdminLTE.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/plugins").Include(
                    "~/Content/plugins/datatables/dataTables.bootstrap.css",
                    "~/Content/plugins/pace-master/themes/green/pace-theme-flash.css",
                    "~/Content/plugins/uniform/css/uniform.default.css",
                    "~/Content/plugins/bootstrap/css/bootstrap.css",
                    "~/Content/plugins/fontawesome/css/font-awesome.css",
                    "~/Content/plugins/line-icons/simple-line-icons.css",
                    "~/Content/plugins/offcanvasmenueffects/css/menu_cornerbox.css",
                    "~/Content/plugins/waves/waves.css",
                    "~/Content/plugins/switchery/switchery.css",
                    "~/Content/plugins/3d-bold-navigation/css/style.css",
                    "~/Content/plugins/metrojs/MetroJs.css",
                    "~/Content/plugins/toastr/toastr.css"));

            bundles.Add(new StyleBundle("~/Content/theme").Include(
                      "~/Content/theme/modern.css",
                      "~/Content/theme/green.css",
                      "~/Content/theme/custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                    "~/Content/plugins/3d-bold-navigation/js/modernizr.js",
                    "~/Content/plugins/offcanvasmenueffects/js/snap.svg-min.js",
                    "~/Content/plugins/jquery/jquery-2.1.3.js",
                    "~/Content/plugins/jquery-ui/jquery-ui.js",
                    "~/Content/plugins/pace-master/pace.js",
                    "~/Content/plugins/jquery-blockui/jquery.blockui.js",
                    "~/Content/plugins/bootstrap/js/bootstrap.js",
                    "~/Content/plugins/jquery-slimscroll/jquery.slimscroll.js",
                    "~/Content/plugins/switchery/switchery.js",
                    "~/Content/plugins/uniform/jquery.uniform.js",
                    "~/Content/plugins/offcanvasmenueffects/js/classie.js",
                    "~/Content/plugins/offcanvasmenueffects/js/main.js",
                    "~/Content/plugins/waves/waves.js",
                    "~/Content/plugins/3d-bold-navigation/js/main.js",
                    "~/Content/plugins/waypoints/jquery.waypoints.js",
                    "~/Content/plugins/jquery-counterup/jquery.counterup.js",
                    "~/Content/plugins/toastr/toastr.js",
                    "~/Content/plugins/flot/jquery.flot.js",
                    "~/Content/plugins/flot/jquery.flot.time.js",
                    "~/Content/plugins/flot/jquery.flot.symbol.js",
                    "~/Content/plugins/flot/jquery.flot.resize.js",
                    "~/Content/plugins/flot/jquery.flot.tooltip.js",
                    "~/Content/plugins/curvedlines/curvedlines.js",
                    "~/Content/plugins/metrojs/metrojs.js",
                    "~/Scrpts/modern.js"                    
                    ));
        }
    }
}

