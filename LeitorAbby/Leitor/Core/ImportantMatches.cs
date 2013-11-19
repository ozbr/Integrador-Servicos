using iTextSharp.text.pdf;
using Leitor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Leitor.Core
{
    public static class ImportantMatches
    {
        private static Stopwatch _watchEmailAssociation = new Stopwatch();
        private static Stopwatch _watchCities = new Stopwatch();
        private static Stopwatch _watchTaxDocUrlAssociation = new Stopwatch();

        private static Dictionary<string, string> _emailAssociation = new Dictionary<string, string>();
        private static List<string> _cities = new List<string>();
        private static List<string[]> _taxDocUrlAssociation = new List<string[]>();

        private static object _lockEmailAssociation = new object();
        private static object _lockCities = new object();
        private static object _lockTaxDocUrlAssociation = new object();

        private static void FillEmailAssociation(bool enforce)
        {
            if (_emailAssociation.Count == 0 || enforce || _watchEmailAssociation.Elapsed.Minutes >= 5)
            {
                lock (_lockEmailAssociation)
                {
                    if (_emailAssociation.Count == 0 || enforce || _watchEmailAssociation.Elapsed.Minutes >= 5)
                    {
                        _emailAssociation.Clear();

                        using (SqlConnection connection = new SqlConnection(Repository._connectionString))
                        {
                            using (SqlCommand command = connection.CreateCommand())
                            {
                                command.CommandType = System.Data.CommandType.Text;
                                command.CommandText = @"
                                SELECT PRE.PRE_ENDERECO_EMAIL, PRE.PRE_NOME
                                FROM PREFEITURA PRE
                                WHERE PRE.PRE_ENDERECO_EMAIL IS NOT NULL";

                                command.Connection.Open();

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        _emailAssociation.Add((string)reader[0], (string)reader[1]);
                                    }
                                }

                                command.Connection.Close();
                                _watchEmailAssociation.Restart();
                            }
                        }
                    }
                }
            }
        }

        private static void FillCities(bool enforce)
        {
            if (_cities.Count == 0 || enforce || _watchCities.Elapsed.Minutes >= 5)
            {
                lock (_lockCities)
                {
                    if (_cities.Count == 0 || enforce || _watchCities.Elapsed.Minutes >= 5)
                    {
                        _cities.Clear();

                        using (SqlConnection connection = new SqlConnection(Repository._connectionString))
                        {
                            using (SqlCommand command = connection.CreateCommand())
                            {
                                command.CommandType = System.Data.CommandType.Text;
                                command.CommandText = @"
                                SELECT P.[PRE_NOME]
                                FROM [PREFEITURA] P";

                                command.Connection.Open();

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        _cities.Add((string)reader[0]);
                                    }
                                }

                                command.Connection.Close();
                                _watchCities.Restart();
                            }
                        }
                    }
                }
            }
        }

        private static void FillTaxDocUrlAssociation(bool enforce)
        {
            if (_taxDocUrlAssociation.Count == 0 || enforce || _watchTaxDocUrlAssociation.Elapsed.Minutes >= 5)
            {
                lock (_lockTaxDocUrlAssociation)
                {
                    if (_taxDocUrlAssociation.Count == 0 || enforce || _watchTaxDocUrlAssociation.Elapsed.Minutes >= 5)
                    {
                        _taxDocUrlAssociation.Clear();

                        using (SqlConnection connection = new SqlConnection(Repository._connectionString))
                        {
                            using (SqlCommand command = connection.CreateCommand())
                            {
                                command.CommandType = System.Data.CommandType.Text;
                                command.CommandText = @"
                            SELECT PRE_NOME, PRE_RGXLINK, PRE_RGXLINK_SECUNDARIO, PRE_RGXLINK_FORMAT
                            FROM PREFEITURA PRE
                            WHERE PRE.PRE_RGXLINK IS NOT NULL";

                                command.Connection.Open();

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        _taxDocUrlAssociation.Add
                                            (new string[]
                                                { 
                                                    (string)reader[0], 
                                                    (string)reader[1], 
                                                    reader[2] == DBNull.Value ? null : (string)reader[2], 
                                                    reader[3] == DBNull.Value ? null : (string)reader[3]
                                                }
                                            );
                                    }
                                }

                                command.Connection.Close();
                                _watchTaxDocUrlAssociation.Restart();
                            }
                        }
                    }
                }
            }
        }

        public static bool TryGetCity(out string cityName, string sender, string[] possibleSenders)
        {
            FillEmailAssociation(false);

            bool success = false;

            cityName = string.Empty;

            if (_emailAssociation.ContainsKey(sender))
            {
                success = true;
                cityName = _emailAssociation[sender];
            }
            else
            {
                bool sure = false; 
                
                for (int i=0; i < possibleSenders.Length; i++)
                {
                    if (_emailAssociation.ContainsKey(possibleSenders[i]))
                    {
                        if (!sure)
                        {
                            cityName = _emailAssociation[possibleSenders[i]];
                            sure = true;
                            success = true;
                        }
                        else
                        {
                            cityName = string.Empty;
                            sure = false;
                            break;
                        }
                    }
                } 
            }

            return success;
        }

        public static bool TryGetCityAndIsTaxDoc(out string cityName, out bool isTaxDoc, string documentPath)
        {
            FillCities(false);

            bool success = false;
            cityName = string.Empty;
            isTaxDoc = true;

            string documentText = PdfToText.ExtrairTextoDoPdf(documentPath);

            Match m = Regex.Match(documentText, @"(?i)prefeitura.*?de\s{1,}\b([A-Z|a-z|À-ÿ| ]*)\b");
            Match n = Regex.Match(documentText, @"(?i)munic.pio.*?de\s{1,}\b([A-Z|a-z|À-ÿ| ]*)\b");

            if (m.Groups[1].Success || n.Groups[1].Success)
            {
                string foundName;

                if (!String.IsNullOrEmpty(m.Groups[1].Value))
                {
                    foundName = m.Groups[1].Value;
                }
                else
                {
                    foundName = n.Groups[1].Value;
                }

                foundName = Regex.Replace(foundName, @"(?i)[^A-Z|a-z| ]", ".?");

                for (int i = 0; i < _cities.Count; i++)
                {
                    if (Regex.IsMatch(_cities[i], foundName, RegexOptions.IgnoreCase))
                    {
                        cityName = _cities[i];
                        success = true;
                        break;
                    }
                }

                if (!success)
                {
                    Console.WriteLine("A PREFEITURA '{0}' AINDA NÃO FOI ADICIONADA", foundName);
                }
            }
            else
            {
                isTaxDoc = false;
            }

            return success;
        }

        public static bool TryGetCityAndTaxDocumentUrl(out string cityName, out string[] urlParams, out string url, string messageBody)
        {
            FillTaxDocUrlAssociation(false);

            bool success = false;

            cityName = string.Empty;
            url = string.Empty;
            urlParams = new string[0];
            
            for (int i = 0; i < _taxDocUrlAssociation.Count; i++)
            {
                Match m = Regex.Match(messageBody, _taxDocUrlAssociation[i][1], RegexOptions.IgnoreCase);

                if (m.Success)
                {
                    success = true;

                    cityName = _taxDocUrlAssociation[i][0];
                    url = m.Groups[1].Value;
                    urlParams = new string[] { _taxDocUrlAssociation[i][2], _taxDocUrlAssociation[i][3] };
                    break;
                }
            }

            return success;
        }

        public static bool TryGetTaxDocumentUrl(out string url, out string[] urlParams, string cityName, string messageBody)
        {
            FillTaxDocUrlAssociation(false);

            bool success = false;

            url = string.Empty;
            urlParams = new string[0];

            for (int i = 0; i < _taxDocUrlAssociation.Count; i++)
            {
                if (cityName == _taxDocUrlAssociation[i][0])
                {
                    Match m = Regex.Match(messageBody, _taxDocUrlAssociation[i][1], RegexOptions.IgnoreCase);

                    if (m.Success)
                    {
                        success = true;

                        url = m.Groups[1].Value.Replace("&amp;","&");
                        urlParams = new string[] { _taxDocUrlAssociation[i][2], _taxDocUrlAssociation[i][3] };
                    }
                }
            }

            return success;
        }
    }
}
