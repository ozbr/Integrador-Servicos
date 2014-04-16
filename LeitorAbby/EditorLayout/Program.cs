using Leitor.Core;
using Leitor.Document;
using Leitor.Model;
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

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(@"http://www.notamaisfacil.novaiguacu.rj.gov.br/NotaFiscal/visualizarNota.php?id_nota_fiscal=MzQ2MTc2Nw==&temPrestador=Tg==&codCidIni=5869&rDecId=056839");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            DocumentDownloader.LerRespostaPagina(res, null);

            //DocumentXml teste = new DocumentXml();
            //teste.Local = @"C:\KeepTrue\Documentos\Abbyy\empty_name_P19032014-105114142.xml";
            //teste.Prefeitura = new Prefeitura();
            //teste.Prefeitura.Id = 1047;
            //teste.Prefeitura.Nome = "CANOAS";

            //NF result = teste.Read();

            //string a = result.ToString();

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

