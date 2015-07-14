using System.Web.Optimization;

namespace FamilyBudget.Spa
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            var libJs = new ScriptBundle("~/Scripts/lib.js")
                .Include("~/Scripts/jquery-1.9.1.min.js")
                .Include("~/Scripts/globalize/globalize.js", "~/Scripts/globalize/cultures/globalize.culture.ru.js")
                .Include("~/Scripts/jquery.validate.min.js")
                .Include("~/Scripts/angular.min.js")
                .Include("~/Scripts/moment.min.js", "~/Scripts/bootstrap.min.js")
                .Include("~/Scripts/angular-ui/ui-bootstrap.min.js")
                .Include("~/Scripts/angular-ui/ui-bootstrap-tpls.min.js");
            bundles.Add(libJs);

            var pluginJs = new ScriptBundle("~/Scripts/plugin.js")
                .Include("~/Scripts/plugins/ladda/spin.js", "~/Scripts/plugins/ladda/ladda.js")
                .Include("~/Scripts/plugins/dataTables/jquery.dataTables.js", "~/Scripts/plugins/dataTables/dataTables.bootstrap.js")
                .Include("~/Scripts/metisMenu.js")
                .Include("~/Scripts/plugins/morris/morris.js")
                .Include("~/Scripts/plugins/morris/raphael-2.1.0.min.js"
                );
            bundles.Add(pluginJs);

            var appJs = new ScriptBundle("~/Scripts/fb.js")
                .Include("~/Scripts/fb.js");
            bundles.Add(appJs);
        }
    }
}