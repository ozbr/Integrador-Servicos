using Leitor.Core;
using Leitor.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace EditorLayout
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //XmlDocument doc = new XmlDocument();
            //doc.Load("C:\\KeepTrue\\Documentos\\Abbyy\\NFSe 4765 - CURITIBA - Cópia_P18022014-101446675.xml");
            //string path = @"//*/_PRE_Endereco#SplitScore#.*BAIRRO.(?<Bairro>[\x20-\xFC]+)|.*CEP.*(?<CEP>\d{5}.?\d{3})|(?<Log>[\x20-\xFC]+)#3";
            //XpathSingleNodeTeste(doc, path);

            //var collection = GetCertificate(StoreLocation.CurrentUser);

            //X509Certificate2 certificate = collection[0];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(@"http://www.issnetonline.com.br/cascavel/online/NotaDigital/NovoLayoutNovaNota.aspx?EF+4E+50+D1+44+B1+18+A8+DF+5C+59+97+6E+CD+6B+EA+28+C1+48+96+F3+B9+7A+84+A4+F9+24+D9+AD+FF+88+E3+CE+A5+FD+2+27+17+A8+B5+69+90+64+61+B2+96+DE+72+26+CC+D1+5A+8A+FD+50+C8+D5+79+CC+69+CD+CC+59+AD+BF+4C+B7+2+D2+C3+44+94+F+B9+A1+5F+E9+62+4C+88+B2+16+D2+C6+D5+A1+1B+8E+1F+24+A+6C+ED+39+32+D0+DA+53+82+71+78+CD+47+9B+55+52+6A+AB+1A+32+CC+FC+38+95+BF+9E+1E+56+79+86+A+92+BD+1B+6E+EF+E2+2F+F2+7B+B6+46+AB+9A+2B+3C+9C+A2+49+98+E6+43+E7+B0+");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            DocumentDownloader.LerRespostaPagina(res, null);

            //string teste = "11.905.15";
            //int a = teste.LastIndexOf(',');
            //if (a != teste.Length - 3)
            //{
            //    a = teste.LastIndexOf('.');
            //    teste = teste.Remove(a, 1);
            //    teste = teste.Insert(a, ",");
            //}
            //teste.ToString();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EditorLayout());

        }

        public static String XpathSingleNodeTeste(XmlDocument doc, string xpath)
        {
            string result = "";
            if (!string.IsNullOrEmpty(xpath))
            {
                try
                {

                    string[] aux = xpath.Split('#');

                    var node = doc.DocumentElement.SelectSingleNode(aux[0]);
                    if (node != null)
                        result = node.InnerText.Trim();

                    switch (aux.Length)
                    {
                        case 2:
                            if (aux[1] == "OCRNUMBER")
                            {
                                //Efetua tratamento para leitura de números reconhecidos por OCR, que costuma confundir pontos e vírgulas e também incluir os carascteres de moeda.
                                //result = FormatOCRNumber(result);
                            }
                            break;
                        case 3:
                        case 4:
                            if (!String.IsNullOrEmpty(result) && !String.IsNullOrEmpty(aux[1]) && !String.IsNullOrEmpty(aux[2]))
                            {
                                if (node != null)
                                {
                                    result = result.Replace((char)8232, ' '); //Remove newline
                                    if (aux[1].Equals("SplitScore"))
                                    {
                                        var resultCortado = result.Split('-');
                                        if (resultCortado.Length >= Convert.ToInt16(aux[3]))
                                        {
                                            Match m = Regex.Match(resultCortado[Convert.ToInt16(aux[3]) - 1], aux[2], RegexOptions.Singleline);
                                            if (m.Success)
                                                result = m.Groups[Convert.ToInt32(aux[3]) - 1].Value;
                                            else
                                                result = string.Empty;
                                        }
                                        else
                                            result = string.Empty;
                                    }
                                    else
                                    {
                                        Match m = Regex.Match(result, aux[1], RegexOptions.Singleline);
                                        if (m.Success)
                                            result = m.Groups[Convert.ToInt32(aux[2])].Value;
                                        else
                                            result = string.Empty;
                                    }
                                }
                            }
                            if (aux.Length == 4)
                            {
                                if (aux[3] == "OCRNUMBER")
                                    //result = FormatOCRNumber(result);
                                    ;
                            }
                            break;
                        default:
                            break;
                    }

                }
                catch (Exception e)
                {
                    //Log.SaveTxt("DocumentXml.XpathSingleNode", e.Message, Log.LogType.Erro);
                }
                result = result.Replace("&amp", "&");
                result = result.Replace("&nbsp;", "");
                //ALTERADO POR MATEUS EM 15/07: ALGUMAS NOTAS ESTÃO VINDO DESFORMATADAS
                //if (result.Contains("&#"))
                //    result = HttpUtility.HtmlDecode(result);
                //if (Prefeitura.Nome.Contains("PIRASS"))
                //    result = Encoding.UTF8.GetString(Encoding.Default.GetBytes(result));
            }
            return result;
        }
    }
}

