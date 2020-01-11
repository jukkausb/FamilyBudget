using System.Web.Optimization;

namespace FamilyBudget.v3
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Common JS libs

            bundles.Add(new ScriptBundle("~/bundles/lodash").Include(
                "~/Scripts/lodash.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-3.4.1.min.js",
                "~/js/jquery-easing/jquery.easing.min.js",
                "~/Scripts/jquery-ui-1.12.1.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/globalize").Include(
                "~/Scripts/cldr.js",
                "~/Scripts/cldr/event.js",
                "~/Scripts/cldr/supplemental.js",
                "~/Scripts/cldr/unresolved.js",
                "~/Scripts/globalize.js",
                "~/Scripts/globalize/number.js",
                "~/Scripts/globalize/date.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalglobalize").Include(
                "~/Scripts/jquery.validate.globalize.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/moment-with-locales.min.js",
                "~/vendor/bootstrap/js/bootstrap.bundle.min.js",
                "~/Scripts/tempusdominus-bootstrap-4.min.js"
                ));

            //// Custom JS scripts

            bundles.Add(new ScriptBundle("~/bundles/initialize").Include(
                "~/Scripts/initialize*"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                "~/vendor/jquery-easing/jquery.easing.min.js",
                "~/js/sb-admin-2.min.js",
                "~/vendor/chart.js/Chart.min.js"
                ));

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

            bundles.Add(new ScriptBundle("~/bundles/morris").Include(
                "~/Scripts/plugins/morris/morris*"
                ));

            bundles.Add(new ScriptBundle("~/bundles/charts").Include(
                "~/js/charts.js/Chart.min.js"
                ));

            // CSS

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/vendor/fontawesome-free/css/all.min.css",
                "~/Content/bootstrap.min.css",
                "~/css/sb-admin-2.min.css",
                "~/css/fb.css",
                "~/font-awesome/css/font-awesome*",
                "~/Content/plugins/morris/morris*",
                "~/Content/plugins/timeline/timeline*",
                "~/Content/plugins/dataTables/dataTables.bootstrap*",
                "~/Content/ladda*",
                "~/Content/tempusdominus-bootstrap-4.min.css"
                ));
        }
    }
}