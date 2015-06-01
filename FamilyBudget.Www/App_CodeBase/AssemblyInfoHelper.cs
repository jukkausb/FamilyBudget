using System.Reflection;

namespace FamilyBudget.Www.App_CodeBase
{
    public static class AssemblyInfoHelper
    {
        public static string GetAssemblyInfo()
        {
            Assembly assembly = typeof (AssemblyInfoHelper).Assembly;
            return assembly.GetName().Version.ToString();
        }
    }
}