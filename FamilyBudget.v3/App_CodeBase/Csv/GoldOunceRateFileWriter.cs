using System;
using System.IO;
using System.Web;

namespace FamilyBudget.v3.App_CodeBase.Csv
{
    public static class GoldOunceRateFileWriter
    {
        private static readonly string FileNameFormat = HttpContext.Current.Server.MapPath(@"~/App_Data/gold-ounce-rates-{0}.csv");

        public static void SaveRatesToFile(string currency, string ratesCsvText)
        {
            try
            {
                string fileName = string.Format(FileNameFormat, currency);

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a file to write to.
                using (var sw = File.CreateText(string.Format(FileNameFormat, currency)))
                {
                    sw.WriteLine(ratesCsvText);
                }
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                throw;
            }
        }

        public static string ReadRatesFromFile(string currency)
        {
            try
            {
                // Open the file to read from.
                string s;
                using (StreamReader sr = File.OpenText(string.Format(FileNameFormat, currency)))
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