using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

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
                if (Groups[key].Contains("#"))
                {
                    result = Groups[key].Split('#')[0];
                }
                else
                {
                    result = (Groups[key]);
                  
                }
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
        public String GetKeyXPathRegex(HtmlDocument doc, string key)
        {
            string result = String.Empty;
            string[] aux = new string[3];
            if (Groups.ContainsKey(key))
            {
                if (Groups[key].Contains("#"))
                {
                    aux[0] = Groups[key].Split('#')[0];
                    aux[1] = Groups[key].Split('#')[1];
                    aux[2] = Groups[key].Split('#')[2];
                    Match m = Regex.Match(doc.DocumentNode.SelectSingleNode(aux[0]).InnerText.Trim(), aux[1]);
                    if (m.Success)
                    {
                        result = m.Groups[Convert.ToInt32(aux[2])].Value;
                    }
                }
                else
                {
                    var b = doc.DocumentNode.SelectSingleNode(Groups[key]);
                    if (b != null)
                    {
                        result = b.InnerText.Trim();
                    }
                }
            }
            return result.Trim();
        }
    }
}