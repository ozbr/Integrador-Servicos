using System;
using System.Collections.Generic;

namespace Leitor.Model
{
    public class RegexModel
    {
        public int Id { get; set; }
        public string Geral { get; set; }
        public string Item { get; set; }
        public Dictionary<string, string> Groups { get; set; }
        public bool IsXpath { get; set; }

        public RegexModel()
        {
            Groups = new Dictionary<string, string>();
        }

        public bool IsValid()
        {
            return (!string.IsNullOrEmpty(Geral) /*&& !string.IsNullOrEmpty(Item) REMOVIDO POR CAUSA DE NOTA QUE NÃO POSSUIA ITENS*/ && Groups.Count > 1);
        }

        public void AddRegex(string key, int value)
        {
            if (value != 0)
            {
                Groups.Add(key, value + "");
            }
        }

        public void AddXPath(string key, string xpath)
        {
            if (xpath != string.Empty)
            {
                Groups.Add(key, xpath);
            }
        }

        public int GetKeyValue(string key)
        {
            int result = -1;
            if (Groups.ContainsKey(key))
            {
                result = Convert.ToInt32(Groups[key]);
            }
            if (result == 0) { result = -1; }
            return result;
        }

        public string GetKeyXPath(string key)
        {
            string result = string.Empty;
            if (Groups.ContainsKey(key))
            {
                result = Groups[key];
            }
            return result;
        }

        /// <summary>
        /// 0 - Xpath
        /// 1 - Regex
        /// 2 - Grupo
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        //public String GetKeyXPathRegex(HtmlDocument doc, string key)
        //{
        //    string result = String.Empty;
        //    string[] aux = new string[3];
        //    try
        //    {
        //        if (Groups.ContainsKey(key))
        //        {
        //            if (Groups[key].Contains("#"))
        //            {
        //                aux[0] = Groups[key].Split('#')[0];
        //                aux[1] = Groups[key].Split('#')[1];
        //                aux[2] = Groups[key].Split('#')[2];
        //                HtmlNode xpath = doc.DocumentNode.SelectSingleNode(aux[0]);
        //                if (xpath != null)
        //                {
        //                    Match m = Regex.Match(xpath.InnerText.Trim(), aux[1], RegexOptions.Singleline);
        //                    if (m.Success)
        //                    {
        //                        result = m.Groups[Convert.ToInt32(aux[2])].Value;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var b = doc.DocumentNode.SelectSingleNode(Groups[key]);
        //                if (b != null)
        //                {
        //                    result = b.InnerText.Trim();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    result = result.Replace("&amp", "&");
        //    result = result.Replace("&nbsp;", "");
        //    //ALTERADO POR MATEUS EM 15/07: ALGUMAS NOTAS ESTÃO VINDO DESFORMATADAS
        //    if (result.Contains("&#"))
        //        result = HttpUtility.HtmlDecode(result);
        //    //if (Prefeitura.Nome.Contains("PIRASS"))
        //    //    result = Encoding.UTF8.GetString(Encoding.Default.GetBytes(result));
        //    return result.Trim();
        //}

        //public String GetKeyXPathRegex(HtmlDocument doc, string key, string index)
        //{
        //    string result = String.Empty;
        //    string[] aux = new string[3];
        //    if (Groups.ContainsKey(key))
        //    {
        //        if (Groups[key].Contains("#"))
        //        {
        //            aux[0] = String.Format(Groups[key].Split('#')[0], index);
        //            aux[1] = Groups[key].Split('#')[1];
        //            aux[2] = Groups[key].Split('#')[2];
        //            Match m = Regex.Match(doc.DocumentNode.SelectSingleNode(aux[0]).InnerText.Trim(), aux[1]);
        //            if (m.Success)
        //            {
        //                result = m.Groups[Convert.ToInt32(aux[2])].Value;
        //            }
        //        }
        //        else
        //        {
        //            string xpath = String.Format(Groups[key], index);
        //            if (!String.IsNullOrEmpty(xpath))
        //            {
        //                var b = doc.DocumentNode.SelectSingleNode(xpath);
        //                if (b != null)
        //                {
        //                    result = b.InnerText.Trim();
        //                }
        //            }
        //        }
        //        result = result.Replace("&nbsp;", "");
        //    }
        //    return result.Trim();
        //}
    }
}