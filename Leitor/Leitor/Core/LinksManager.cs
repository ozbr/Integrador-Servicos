using Leitor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Leitor.Core
{
    public class LinksManager
    {
        public static void SalvarLinksRemetente(Remetente remetente)
        {
            if (Directory.Exists(String.Format(ArquivosManager.LocalEmails, remetente.Emails)))
            {
                foreach (string file in Directory.EnumerateFiles(String.Format(ArquivosManager.LocalEmails, remetente.Emails), "*.html"))
                {
                    Regex rxLink = new Regex(remetente.RgxLink, RegexOptions.Singleline);
                    Regex rxAuxiliar = new Regex(remetente.RgxSecundario, RegexOptions.Singleline);
                    String parameterAuxiliar = remetente.Parametro;
                    String link = string.Empty;

                    string contents = File.ReadAllText(file);
                    if (remetente.Emails.Contains("barueri"))
                    {
                        contents = contents.Replace("amp;", "");
                    }

                    link = rxLink.Match(contents).Groups[1].Value;

                    if (!String.IsNullOrEmpty(link))
                    {
                        CookieContainer cookies = new CookieContainer();
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(link);
                        req.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
                        req.AllowAutoRedirect = true;
                        req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                        req.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                        req.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
                        req.CookieContainer = cookies;

                        HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                        if (!String.IsNullOrEmpty(remetente.RgxSecundario) || res.ContentType.Contains("pdf"))
                        {
                            if (!String.IsNullOrEmpty(remetente.RgxSecundario))
                            {
                                cookies.Add(res.Cookies);
                                StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("UTF-8"));

                                String html = sr.ReadToEnd();
                                String id = rxAuxiliar.Match(html).Groups[1].Value;
                                String urlSecundaria = String.Format(parameterAuxiliar, id);
                                req = (HttpWebRequest)WebRequest.Create(urlSecundaria);
                                res = (HttpWebResponse)req.GetResponse();
                                sr.Close();
                            }
                            if (res.ContentType.StartsWith("image"))
                            {
                                LerImagemResponse(res, remetente);
                            }
                            else
                            {
                                LerPdfResponse(res, remetente);
                            }
                        }
                        else
                        {
                            LerPaginaResponse(res, remetente);
                        }
                    }
                }
            }
        }

        private static void LerPaginaResponse(HttpWebResponse res, Remetente remetente)
        {
            Encoding encode;

            if (remetente.Id >= 11)
            {
                encode = Encoding.GetEncoding("ISO-8859-1");
            }
            else 
            {
                encode = Encoding.GetEncoding("UTF-8");
            }

            StreamReader sr = new StreamReader(res.GetResponseStream(), encode);

            String html = sr.ReadToEnd();

            if (!Directory.Exists(String.Format(ArquivosManager.LocalArquivos, remetente.Emails)))
            {
                Directory.CreateDirectory(String.Format(ArquivosManager.LocalArquivos, remetente.Emails));
            }
            ArquivosManager.SalvarArquivo(remetente.Emails, "pagina", ".html", html);
        }

        private static void LerImagemResponse(HttpWebResponse res, Remetente remetente)
        {
            if (!Directory.Exists(String.Format(ArquivosManager.LocalArquivos, remetente.Emails)))
            {
                Directory.CreateDirectory(String.Format(ArquivosManager.LocalArquivos, remetente.Emails));
            }

            using (Stream inputStream = res.GetResponseStream())
            using (Stream outputStream = File.OpenWrite(String.Format(ArquivosManager.LocalArquivos, remetente.Emails) + "imagem" + res.ContentType.ToString().Replace("image/", ".")))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                do
                {
                    bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                    outputStream.Write(buffer, 0, bytesRead);
                } while (bytesRead != 0);
                outputStream.Close();
                inputStream.Close();
            }
        }

        private static void LerPdfResponse(HttpWebResponse res, Remetente remetente)
        {

            if (!Directory.Exists(String.Format(ArquivosManager.LocalArquivos, remetente.Emails)))
            {
                Directory.CreateDirectory(String.Format(ArquivosManager.LocalArquivos, remetente.Emails));
            }

            using (Stream inputStream = res.GetResponseStream())
            using (Stream outputStream = File.OpenWrite(String.Format(ArquivosManager.LocalArquivos, remetente.Emails) + "pdf" + ".pdf"))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                do
                {
                    bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                    outputStream.Write(buffer, 0, bytesRead);
                } while (bytesRead != 0);
                outputStream.Dispose();
                inputStream.Dispose();
            }
        }
    }
}

