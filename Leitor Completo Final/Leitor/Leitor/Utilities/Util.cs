using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Leitor.Utilities
{
    public class Util
    {
        public static String LimpaCampos(String s)
        {
            if (String.IsNullOrEmpty(s))
                return s;

            s = Regex.Replace(s, @"^\+\d*?\(", "(");
            return s.Replace("-", "").Replace(".", "-").Replace(" ", "").Replace("/", "").Replace("\\", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("\r\n", " ");
        }

        public static String RetiraQuebraDeLinha(String s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            return s.Replace("\r\n", " ").Replace("\n\r"," ");
        }

        public static String LimpaDiscriminacao(String s)
        {
            if (!String.IsNullOrEmpty(s))
            {
                s = s.Replace("\r\n", "|");
                while (Regex.Match(s, @"\|[\s]*\|").Success)
                {
                    s = Regex.Replace(s, @"\|[\s]*\|", "|");
                }
                while (Regex.Match(s, @"[\s\t]{2,}").Success)
                {
                    s = Regex.Replace(s, @"[\s\t]{2,}", " ");
                }
            }
            return s;
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
                result = m.Groups[3] + "-" + (m.Groups[2].Length == 2 ? m.Groups[2].Value : "0" + m.Groups[2].Value) + "-" + (m.Groups[1].Length == 2 ? m.Groups[1].Value : "0" + m.Groups[1].Value);
            }
            else
            {
                rxData = new Regex("(\\d{1,2}).(\\w{3}).(\\d{1,4})", RegexOptions.Singleline);
                if (rxData.IsMatch(s))
                {
                    Match m = rxData.Match(s);
                    switch (m.Groups[2].Value.ToUpperInvariant())
                    {
                        case "JAN":
                            result = m.Groups[3] + "-01-" + m.Groups[1];
                            break;
                        case "FEV":
                            result = m.Groups[3] + "-02-" + m.Groups[1];
                            break;
                        case "MAR":
                            result = m.Groups[3] + "-03-" + m.Groups[1];
                            break;
                        case "ABR":
                            result = m.Groups[3] + "-04-" + m.Groups[1];
                            break;
                        case "MAI":
                            result = m.Groups[3] + "-05-" + m.Groups[1];
                            break;
                        case "JUN":
                            result = m.Groups[3] + "-06-" + m.Groups[1];
                            break;
                        case "JUL":
                            result = m.Groups[3] + "-07-" + m.Groups[1];
                            break;
                        case "AGO":
                            result = m.Groups[3] + "-08-" + m.Groups[1];
                            break;
                        case "SET":
                            result = m.Groups[3] + "-09-" + m.Groups[1];
                            break;
                        case "OUT":
                            result = m.Groups[3] + "-10-" + m.Groups[1];
                            break;
                        case "NOV":
                            result = m.Groups[3] + "-11-" + m.Groups[1];
                            break;
                        case "DEZ":
                            result = m.Groups[3] + "-12-" + m.Groups[1];
                            break;
                    }
                }
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
                Decimal.TryParse(s.Replace("R$", "").Replace("%", "").Replace("VALOR TOTAL DA NOTA =", "").Replace("(","").Replace(")","").Replace("%","").Trim(), NumberStyles.Any, new CultureInfo("pt-BR"), out dc);
                if (dc != -1)
                {
                    result = dc.ToString(new CultureInfo("en-US"));
                }
            }
            //Console.WriteLine(s + "\t" + result);
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

        public static string RemoverAcentos(string texto)
        {
            string result = String.Empty;
            if (!String.IsNullOrEmpty(texto))
            {
                byte[] bytes = Encoding.GetEncoding("iso-8859-8").GetBytes(texto);
                result = Encoding.UTF8.GetString(bytes);
                result = Regex.Replace(result, "([\x80-\xFF]*)", "");
            }
            return result;
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
