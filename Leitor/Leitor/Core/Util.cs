using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Leitor.Core
{
    public class Util
    {
        public static String LimpaCampos(String s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            return s.Replace("-", "").Replace(".", "-").Replace(" ", "").Replace("/", "").Replace("\\", "").Replace("(", "").Replace(")", "").Replace("-", "");
        }

        public static String FormataData(String s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            String result = s;
            Regex rxData = new Regex("(\\d{1,2}).(\\d{1,2}).(\\d{1,4})", RegexOptions.Singleline);
            if (rxData.IsMatch(s))
            {
                Match m = rxData.Match(s);
                result = m.Groups[3] + "-" + m.Groups[2] + "-" + m.Groups[1];
            }
            return result;
        }

        //cheio de returns...
        public static String SeparaEmails(String s)
        {
            if (String.IsNullOrEmpty(s))
                return s;

            if (s.Contains(";"))
            {
                return s.Split(';')[0];
            }
            if (s.Contains(","))
            {
                return s.Split(',')[0];
            }

            return s;
        }

        public static String FormataDecimal(String s)
        {
            String result = s;

            if (!string.IsNullOrEmpty(s))
            {
                Decimal dc = -1;
                Decimal.TryParse(s.Replace("R$","").Replace("%","").Replace("VALOR TOTAL DA NOTA =","").Trim(), out dc);
                if (dc != -1)
                {
                    result = dc.ToString(new CultureInfo("en-US"));
                }
            }
            Console.WriteLine(s + "\t" + result);
            return result;
        }

        public static String validateEouS(String s)
        {
            if (!s.Equals("E") || !s.Equals("S"))
            {
                if (s.Equals("1"))
                {
                    s = "S";
                }
            }
            return s;
        }


    }
}
