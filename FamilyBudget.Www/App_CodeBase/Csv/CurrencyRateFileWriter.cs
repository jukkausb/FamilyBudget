using System;
using System.IO;
using System.Web;

namespace FamilyBudget.Www.App_CodeBase.Csv
{
    public static class CurrencyRateFileWriter
    {
        private static readonly string FileNameCsvFormat = HttpContext.Current.Server.MapPath(@"~/App_Data/currency-rates-{0}-{1}.csv");
        private static readonly string FileNameJsonFormat = HttpContext.Current.Server.MapPath(@"~/App_Data/currency-rates-{0}-{1}.json");

        public static void SaveRatesToCsvFile(string sellCurrencyCode, string purchaseCurrencyCode, string ratesCsvText)
        {
            SaveRatesFile(sellCurrencyCode, purchaseCurrencyCode, FileNameCsvFormat, ratesCsvText);
        }

        public static void SaveRatesToJsonFile(string sellCurrencyCode, string purchaseCurrencyCode, string ratesJsonText)
        {
            SaveRatesFile(sellCurrencyCode, purchaseCurrencyCode, FileNameJsonFormat, ratesJsonText);
        }

        public static string ReadRatesFromCsvFile(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            return ReadRatesFromFile(sellCurrencyCode, purchaseCurrencyCode, FileNameCsvFormat);
        }

        public static string ReadRatesFromJsonFile(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            return ReadRatesFromFile(sellCurrencyCode, purchaseCurrencyCode, FileNameJsonFormat);
        }

        private static void SaveRatesFile(string sellCurrencyCode, string purchaseCurrencyCode, string fileFormat, string text)
        {
            try
            {
                string fileName = string.Format(fileFormat, sellCurrencyCode, purchaseCurrencyCode);

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a file to write to.
                using (var sw = File.CreateText(string.Format(fileFormat, sellCurrencyCode, purchaseCurrencyCode)))
                {
                    sw.WriteLine(text);
                }
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                throw;
            }
        }

        private static string ReadRatesFromFile(string sellCurrencyCode, string purchaseCurrencyCode, string fileFormat)
        {
            try
            {
                // Open the file to read from.
                string s;
                using (StreamReader sr = File.OpenText(string.Format(fileFormat, sellCurrencyCode, purchaseCurrencyCode)))
                {
                    s = sr.ReadLine();
                }

                return s;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                throw;
            }
        }
    }
}