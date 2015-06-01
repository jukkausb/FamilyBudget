using FamilyBudget.Www.App_CodeBase;

namespace FamilyBudget.Www.App_Helpers
{
    public static class JsonUtils
    {
        public static string ModelToJson(object model)
        {
            if (model == null) return null;
            return new JsonDotNetResult().ConvertToJson(model);
        }
    }
}