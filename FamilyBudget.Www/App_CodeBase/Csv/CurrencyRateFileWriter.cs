using FileHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FamilyBudget.Www.App_CodeBase.Csv
{
    public static class CurrencyRateFileWriter
    {
        private static string FileNameFormat = HttpContext.Current.Server.MapPath(@"~/App_Data/currency-rates-{0}-{1}.csv");

        public static void SaveRatesToFile(string sellCurrencyCode, string purchaseCurrencyCode, string ratesCsvText)
        {
            string fileName = string.Format(FileNameFormat, sellCurrencyCode, purchaseCurrencyCode);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(string.Format(FileNameFormat, sellCurrencyCode, purchaseCurrencyCode)))
            {
                sw.WriteLine(ratesCsvText);
            }
        }

        public static string ReadRatesFromFile(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            string s = "";

            // Open the file to read from.
            using (StreamReader sr = File.OpenText(string.Format(FileNameFormat, sellCurrencyCode, purchaseCurrencyCode)))
            {
                s = sr.ReadLine();
            }

            return s;
        }
    }
}