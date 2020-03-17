using System;
using System.IO;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public interface ITinkoffOpenApiTokenReader
    {
        string ReadToken(string fileName);
    }

    public class TinkoffOpenApiTokenReader : ITinkoffOpenApiTokenReader
    {
        public string ReadToken(string fileName)
        {
            try
            {
                string s;
                using (StreamReader sr = File.OpenText(fileName))
                {
                    s = sr.ReadLine();
                }
                return s;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
            }

            return "";
        }
    }
}