using System.Web.Optimization;

namespace FamilyBudget.Www
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery*"
                ));

            bundles.Add(new ScriptBundle("~/bundles/globalize").Include(
                "~/Scripts/globalize/globalize.js",
                "~/Scripts/globalize/cultures/globalize.culture.ru.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*",
                "~/Scripts/globalize/jquery.validate*"
                ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/moment*",
                "~/Scripts/bootstrap*"
                ));

            bundles.Add(new ScriptBundle("~/bundles/initialize").Include(
                "~/Scripts/initialize*"));

            bundles.Add(new ScriptBundle("~/bundles/ladda").Include(
                "~/Scripts/plugins/ladda/spin*",
                "~/Scripts/plugins/ladda/ladda*"
                ));

            bundles.Add(new ScriptBundle("~/bundles/widget-engine").Include(
                "~/Scripts/widget-engine*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
                "~/Scripts/plugins/dataTables/jquery.dataTables*",
                "~/Scripts/plugins/dataTables/dataTables.bootstrap*"
                ));

            bundles.Add(new ScriptBundle("~/bundles/metisMenu").Include(
                "~/Scripts/metisMenu*"
                ));

            bundles.Add(new ScriptBundle("~/bundles/morris").Include(
                "~/Scripts/plugins/morris/morris*"
                ));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/bootstrap*",
                "~/Content/metisMenu*",
                "~/font-awesome/css/font-awesome*",
                "~/Content/plugins/morris/morris*",
                "~/Content/plugins/timeline/timeline*",
                "~/Content/plugins/dataTables/dataTables.bootstrap*",
                "~/Content/familybudget*",
                "~/Content/ladda*"
                ));
        }
    }
}