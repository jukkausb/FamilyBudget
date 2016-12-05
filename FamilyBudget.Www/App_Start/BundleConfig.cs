using System.Web.Optimization;

namespace FamilyBudget.Www
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Common JS libs

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

            // Custom JS scripts

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

            // CSS

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

            AddMobileSpaBundles(bundles);
        }

        private static void AddMobileSpaBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/lib-spa.js")
                .Include("~/Scripts/moment.js")
                .Include("~/Scripts/angular.js")
                .Include("~/Scripts/angular-animate.js")
                .Include("~/Scripts/angular-loader.js")
                .Include("~/Scripts/angular-resource.js")
                .Include("~/Scripts/angular-sanitize.js")
                .Include("~/Scripts/angular-touch.js")
                .Include("~/Scripts/angular-route.js")
                .Include("~/Scripts/angular-ui-router.js")
                .Include("~/Scripts/angular-messages.js")
                .Include("~/Scripts/modernizr-2.6.2.js",
                "~/Scripts/lodash.js"));

            var dashboardJs = new Bundle("~/bundles/dashboard.js")
                .Include("~/Scripts/app/core/core.module.js")
                .IncludeDirectory("~/Scripts/app/core", "*.js", true)
                .Include("~/Scripts/app/dashboard/dashboard.module.js")
                .IncludeDirectory("~/Scripts/app/dashboard", "*.js", true);

            bundles.Add(dashboardJs);
        }
    }
}