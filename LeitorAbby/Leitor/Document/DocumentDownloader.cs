using System.IO.Compression;
using Leitor.Model;
using Leitor.Utilities;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using EO.Pdf;
using System.Drawing;

namespace Leitor.Document
{
    public class DocumentDownloader
    {

        public static void BaixarArquivo(ref EmailData email, string url, string rgxSecundario, string parametro, string aceptEncoding = null)
        {
            if (!String.IsNullOrEmpty(url))
            {
                if (string.IsNullOrEmpty(aceptEncoding))
                    aceptEncoding = "gzip,deflate,sdch";

                url = url.Replace("&amp;", "&");
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
                req.AllowAutoRedirect = true;
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //req.Headers.Add("Accept-Encoding", aceptEncoding);
                req.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");

                req.CookieContainer = new CookieContainer();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                
                try
                {
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                    //if (!String.IsNullOrEmpty(rgxSecundario) || res.ContentType.Contains("pdf"))
                    //{
                    if (!String.IsNullOrEmpty(rgxSecundario))
                    {
                        req.CookieContainer.Add(res.Cookies);

                        StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(res.CharacterSet));

                        String html = sr.ReadToEnd();
                        sr.Close();
                        String id = Regex.Match(html, rgxSecundario).Groups[1].Value;
                        String urlSecundaria = String.Format(parametro, id);

                        urlSecundaria = urlSecundaria.Replace("&amp;", "&");
                        req = (HttpWebRequest)WebRequest.Create(urlSecundaria);
                        req.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
                        req.AllowAutoRedirect = true;
                        req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                        //req.Headers.Add("Accept-Encoding", aceptEncoding);
                        req.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");

                        req.CookieContainer = new CookieContainer();
                        req.CookieContainer.Add(res.Cookies);

                        res = (HttpWebResponse)req.GetResponse();

                    }

                    if (res.ContentType.StartsWith("image"))
                    {
                        string caminhoArquivo = LerRespostaImagem(res, email);
                        email.Anexos.Add(new Anexo { CaminhoArquivo = caminhoArquivo, NomeArquivo = Path.GetFileName(caminhoArquivo) });
                    }
                    else if (res.ContentType.Contains("text/html"))
                    {
                        string caminhoArquivo = LerRespostaPagina(res, email);
                        email.Anexos.Add(new Anexo { CaminhoArquivo = caminhoArquivo, NomeArquivo = Path.GetFileName(caminhoArquivo) });
                    }
                    else
                    {
                        string caminhoArquivo = LerRespostaPdf(res, email);
                        email.Anexos.Add(new Anexo { CaminhoArquivo = caminhoArquivo, NomeArquivo = Path.GetFileName(caminhoArquivo) });
                    }
                    //}
                    //}
                    //else
                    //{
                    //    string caminhoArquivo = LerRespostaPagina(res, email);
                    //    email.Anexos.Add(new Anexo { CaminhoArquivo = caminhoArquivo, NomeArquivo = Path.GetFileName(caminhoArquivo) });
                    //}
                }
                catch (Exception ex)
                {
                    string x = ex.Message;
                }
            }
        }

        public static string LerRespostaPagina(HttpWebResponse res, EmailData email)
        {
            StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(res.CharacterSet));

            string caminhoArquivo = FileManager.GetCaminho(CaminhoPara.AnexosProcessando);

            if (!Directory.Exists(caminhoArquivo))
                Directory.CreateDirectory(caminhoArquivo);

            caminhoArquivo = Path.Combine(caminhoArquivo, "B_" + DateTime.Now.ToString("ddMMyyyy-hhmmssfff") + ".pdf");
            HtmlToPdf.Options.OutputArea = new RectangleF(0.25f, 0.25f, 7.5f, 10f);
            HtmlToPdf.ConvertHtml(sr.ReadToEnd(), caminhoArquivo);

            return caminhoArquivo;
        }

        private static string LerRespostaImagem(HttpWebResponse res, EmailData email)
        {
            string caminhoArquivo = FileManager.GetCaminho(CaminhoPara.AnexosProcessando);

            if (!Directory.Exists(caminhoArquivo))
            {
                Directory.CreateDirectory(caminhoArquivo);
            }

            caminhoArquivo = Path.Combine(caminhoArquivo, "B_" + DateTime.Now.ToString("ddMMyyyy-hhmmssfff") + res.ContentType.ToString().Replace("image/", "."));

            using (Stream inputStream = res.GetResponseStream())
            {
                using (Stream outputStream = File.OpenWrite(caminhoArquivo))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;

                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);

                    outputStream.Close();
                }
                inputStream.Close();
            }

            return caminhoArquivo;
        }

        private static string LerRespostaPdf(HttpWebResponse res, EmailData e)
        {
            string caminhoArquivo = FileManager.GetCaminho(CaminhoPara.AnexosProcessando);

            if (!Directory.Exists(caminhoArquivo))
            {
                Directory.CreateDirectory(caminhoArquivo);
            }

            caminhoArquivo = Path.Combine(caminhoArquivo, "B_" + DateTime.Now.ToString("ddMMyyyy-hhmmssfff") + ".pdf");

            Stream inputStream = res.GetResponseStream();

            if (res.ContentEncoding.ToUpperInvariant().Contains("GZIP"))
                inputStream = new GZipStream(res.GetResponseStream(), CompressionMode.Decompress);

            using (inputStream)
            {
                using (Stream outputStream = File.OpenWrite(caminhoArquivo))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);

                    outputStream.Close();
                }
                inputStream.Close();
            }

            return caminhoArquivo;
        }


        //private static void LerPaginaResponse(HttpWebResponse res, Prefeitura p, EmailData e)
        //{
        //    Encoding encode;

        //    if (p.Id >= 11 || p.Nome.Contains("JOS") || p.Nome.Contains("PIRASS"))
        //    {
        //        encode = Encoding.GetEncoding("ISO-8859-1");
        //    }
        //    else
        //    {
        //        encode = Encoding.GetEncoding("UTF-8");
        //    }

        //    StreamReader sr = new StreamReader(res.GetResponseStream(), encode);

        //    String html = sr.ReadToEnd();

        //    if (!Directory.Exists(FileManager.GetLocalArquivo(p.Nome, e)))
        //    {
        //        Directory.CreateDirectory(FileManager.GetLocalArquivo(p.Nome, e));
        //    }
        //    FileManager.SaveFile(p.Nome, e, "pagina", ".html", html);
        //}

        //private static void LerImagemResponse(HttpWebResponse res, Prefeitura p, EmailData e)
        //{
        //    if (!Directory.Exists(FileManager.GetLocalArquivo(p.Nome, e)))
        //    {
        //        Directory.CreateDirectory(FileManager.GetLocalArquivo(p.Nome, e));
        //    }

        //    using (Stream inputStream = res.GetResponseStream())
        //    using (Stream outputStream = File.OpenWrite(FileManager.GetLocalArquivo(p.Nome, e) + "imagem" + res.ContentType.ToString().Replace("image/", ".")))
        //    {
        //        byte[] buffer = new byte[4096];
        //        int bytesRead;
        //        do
        //        {
        //            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
        //            outputStream.Write(buffer, 0, bytesRead);
        //        } while (bytesRead != 0);
        //        outputStream.Close();
        //        inputStream.Close();
        //    }
        //}

        //private static void LerPdfResponse(HttpWebResponse res, Prefeitura p, EmailData e)
        //{

        //    if (!Directory.Exists(FileManager.GetLocalArquivo(p.Nome, e)))
        //    {
        //        Directory.CreateDirectory(FileManager.GetLocalArquivo(p.Nome, e));
        //    }
        //    Stream inputStream = res.GetResponseStream();
        //    if (res.ContentEncoding.ToUpperInvariant().Contains("GZIP"))
        //        inputStream = new GZipStream(res.GetResponseStream(), CompressionMode.Decompress);
        //    using (inputStream)
        //    using (Stream outputStream = File.OpenWrite(FileManager.GetLocalArquivo(p.Nome, e) + "pdf" + ".pdf"))
        //    {
        //        byte[] buffer = new byte[4096];
        //        int bytesRead;
        //        do
        //        {
        //            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
        //            outputStream.Write(buffer, 0, bytesRead);
        //        } while (bytesRead != 0);
        //        outputStream.Dispose();
        //        inputStream.Dispose();
        //        Log.SaveTxt("DocumentDownloader.LerPdfResponse", "Link armazenado: " + FileManager.GetLocalArquivo(p.Nome, e) + "pdf" + ".pdf", Log.LogType.Processo);
        //    }
        //}
    }
}
