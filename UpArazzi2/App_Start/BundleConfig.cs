

using System.Web.Optimization;

namespace UpArazzi2.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {


            bundles.Add(new StyleBundle("~/bundles/Css").Include(
                "~/Theme/css/bootstrap.css",
                "~/Theme/css/fancybox.css",
                "~/Theme/style.css",
                "~/Theme/css/dataTables.bootstrap.min.css"


                ));

            bundles.Add(new ScriptBundle("~/bundles/Scripts").Include(              
                "~/Theme/js/plugins.js",
                "~/Theme/js/bootstrap-slider.min.js",
                "~/Theme/js/jquery.main.js",
                "~/Theme/js/init.js",
                "~/Theme/js/jquery.dataTables.js",
                "~/Theme/js/dataTables.bootstrap.min.js",
                "~/Theme/js/table.js",
                "~/Theme/js/ilceler.js"
               ));
            BundleTable.EnableOptimizations = true;
        }
    }
}