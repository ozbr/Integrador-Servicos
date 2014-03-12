using EO.Pdf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Leitor.Core
{
    public static class EnsureTaxDocument
    {
        public static bool IsTaxDoc(string documentContent)
        {
            bool success = false;

            Match m = Regex.Match(documentContent, @"(?i)prefeitura.*?de\s{1,}\b(.*)\b");
            Match n = Regex.Match(documentContent, @"(?i)munic.pio.*?de\s{1,}\b(.*)\b");

            if (m.Success || n.Success)
            {
                Match j = Regex.Match(documentContent, @"(?i)nota\s*fiscal");

                if (j.Success)
                {
                    success = true;
                }
            }

            return success;
        }
    }
}
