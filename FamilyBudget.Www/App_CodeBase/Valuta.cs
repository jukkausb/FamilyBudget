
namespace FamilyBudget.Www.App_CodeBase
{
    public class Valuta
    {
        public const string ValutaListCacheKey = "ValutaListCacheKey";

        public string Vcode { get; set; }
        public string Vname { get; set; }
        public string VEngname { get; set; }
        public decimal Vnom { get; set; }
        public string VcommonCode { get; set; }
        public string VcharCode { get; set; }
    }
}