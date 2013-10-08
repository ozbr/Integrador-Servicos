using System.IO.Compression;
using Leitor.Model;
using Leitor.Utilities;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Leitor.Document
{
    public class DocumentDownloader
    {
        //public static String GetLink(Prefeitura p, EmailData e)
        //{
        //    String result = String.Empty;
        //    if (Directory.Exists(FileManager.GetLocalEmail(p.Nome, e)))
        //    {
        //        foreach (string file in Directory.EnumerateFiles(FileManager.GetLocalEmail(p.Nome, e), "*.html"))
        //        {
        //            Regex rxLink = new Regex(p.RgxLink, RegexOptions.Singleline);
        //            Regex rxAuxiliar = new Regex(p.RgxLinkSecundario, RegexOptions.Singleline);
        //            String parameterAuxiliar = p.RgxLinkFormat;
        //            String link = string.Empty;

        //            string contents = File.ReadAllText(file);
        //            contents = contents.Replace("amp;", "");

        //            link = rxLink.Match(contents).Groups[1].Value;
        //            result = link;
        //            if (!String.IsNullOrEmpty(link) && !String.IsNullOrEmpty(p.RgxLinkSecundario))
        //            {
        //                CookieContainer cookies = new CookieContainer();
        //                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(link);
        //                req.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
        //                req.AllowAutoRedirect = true;
        //                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //                req.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
        //                req.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
        //                req.CookieContainer = cookies;

        //                HttpWebResponse res = (HttpWebResponse)req.GetResponse();

        //                if (!String.IsNullOrEmpty(p.RgxLinkSecundario) || res.ContentType.Contains("pdf"))
        //                {
        //                    if (!String.IsNullOrEmpty(p.RgxLinkSecundario))
        //                    {
        //                        cookies.Add(res.Cookies);
        //                        StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("UTF-8"));

        //                        String html = sr.ReadToEnd();
        //                        String id = rxAuxiliar.Match(html).Groups[1].Value;
        //                        String urlSecundaria = String.Format(parameterAuxiliar, id);
        //                        link = urlSecundaria;
        //                        req = (HttpWebRequest)WebRequest.Create(urlSecundaria);
        //                        try
        //                        {
        //                            res = (HttpWebResponse)req.GetResponse();
        //                        }
        //                        catch (WebException we) //timeout
        //                        {
        //                            result = string.Empty;
        //                        }
        //                        sr.Close();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        //public static String GetLinkPorCorpo(Prefeitura p, EmailData e)
        //{
        //    String result = String.Empty;

        //    Regex rxLink = new Regex(p.RgxLink, RegexOptions.Singleline);
        //    Regex rxAuxiliar = new Regex(p.RgxLinkSecundario, RegexOptions.Singleline);
        //    String parameterAuxiliar = p.RgxLinkFormat;
        //    String link = string.Empty;

        //    string contents = e.Corpo;
        //    contents = contents.Replace("amp;", "");

        //    link = rxLink.Match(contents).Groups[1].Value;
        //    result = link;
        //    if (!String.IsNullOrEmpty(link) && !String.IsNullOrEmpty(p.RgxLinkSecundario))
        //    {
        //        CookieContainer cookies = new CookieContainer();
        //        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(link);
        //        req.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
        //        req.AllowAutoRedirect = true;
        //        req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //        req.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
        //        req.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
        //        req.CookieContainer = cookies;

        //        HttpWebResponse res = (HttpWebResponse)req.GetResponse();

        //        if (!String.IsNullOrEmpty(p.RgxLinkSecundario) || res.ContentType.Contains("pdf"))
        //        {
        //            if (!String.IsNullOrEmpty(p.RgxLinkSecundario))
        //            {
        //                cookies.Add(res.Cookies);
        //                StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("UTF-8"));

        //                String html = sr.ReadToEnd();
        //                String id = rxAuxiliar.Match(html).Groups[1].Value;
        //                String urlSecundaria = String.Format(parameterAuxiliar, id);
        //                link = urlSecundaria;
        //                req = (HttpWebRequest)WebRequest.Create(urlSecundaria);
        //                res = (HttpWebResponse)req.GetResponse();
        //                sr.Close();
        //            }
        //        }
        //    }
        //    return result;
        //}

        //public static void SalvarLinks(Prefeitura p, EmailData e)
        //{
        //    if (Directory.Exists(e.CaminhoLote + @"\email"))
        //    {
        //        foreach (string file in Directory.EnumerateFiles(FileManager.GetLocalEmail(p.Nome, e), "*.html"))
        //        {
        //            Regex rxLink = new Regex(p.RgxLink, RegexOptions.Singleline);
        //            Regex rxAuxiliar = new Regex(p.RgxLinkSecundario, RegexOptions.Singleline);
        //            String parameterAuxiliar = p.RgxLinkFormat;

        //            string contents = File.ReadAllText(file);
        //            contents = contents.Replace("amp;", "");

        //            string link = rxLink.Match(contents).Groups[1].Value;

        //            if (!String.IsNullOrEmpty(link))
        //            {
        //                CookieContainer cookies = new CookieContainer();
        //                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(link);
        //                req.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
        //                req.AllowAutoRedirect = true;
        //                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //                req.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
        //                req.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
        //                req.CookieContainer = cookies;

        //                HttpWebResponse res = (HttpWebResponse)req.GetResponse();

        //                if (!String.IsNullOrEmpty(p.RgxLinkSecundario) || res.ContentType.Contains("pdf"))
        //                {
        //                    if (!String.IsNullOrEmpty(p.RgxLinkSecundario))
        //                    {
        //                        cookies.Add(res.Cookies);
        //                        StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("UTF-8"));

        //                        String html = sr.ReadToEnd();
        //                        String id = rxAuxiliar.Match(html).Groups[1].Value;
        //                        String urlSecundaria = String.Format(parameterAuxiliar, id);
        //                        req = (HttpWebRequest)WebRequest.Create(urlSecundaria);
        //                        res = (HttpWebResponse)req.GetResponse();
        //                        sr.Close();
        //                    }
        //                    if (res.ContentType.StartsWith("image"))
        //                        LerImagemResponse(res, p, e);
        //                    else if (res.ContentType.Contains("pdf"))
        //                        LerPdfResponse(res, p, e);
        //                    else
        //                        LerPaginaResponse(res, p, e);
        //                }
        //                else
        //                    LerPaginaResponse(res, p, e);
        //            }
        //        }
        //    }
        //}

        //public static void SalvarLinkPrefeituraDesconhecida(Prefeitura p, EmailData e)
        //{
        //    Regex rxLink = new Regex(p.RgxLink, RegexOptions.Singleline);
        //    Regex rxAuxiliar = new Regex(p.RgxLinkSecundario, RegexOptions.Singleline);
        //    String parameterAuxiliar = p.RgxLinkFormat;
        //    String link = string.Empty;

        //    string contents = e.Corpo;
        //    contents = contents.Replace("amp;", "");

        //    link = rxLink.Match(contents).Groups[1].Value;

        //    if (!String.IsNullOrEmpty(link))
        //    {
        //        CookieContainer cookies = new CookieContainer();
        //        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(link);
        //        req.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
        //        req.AllowAutoRedirect = true;
        //        req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //        req.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
        //        req.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
        //        req.CookieContainer = cookies;

        //        try
        //        {
        //            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

        //            if (!String.IsNullOrEmpty(p.RgxLinkSecundario) || res.ContentType.Contains("pdf"))
        //            {
        //                if (!String.IsNullOrEmpty(p.RgxLinkSecundario))
        //                {
        //                    cookies.Add(res.Cookies);
        //                    StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("UTF-8"));

        //                    String html = sr.ReadToEnd();
        //                    String id = rxAuxiliar.Match(html).Groups[1].Value;
        //                    String urlSecundaria = String.Format(parameterAuxiliar, id);
        //                    req = (HttpWebRequest)WebRequest.Create(urlSecundaria);
        //                    if (link.Contains("sjc"))
        //                    {
        //                        req.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
        //                        req.AllowAutoRedirect = true;
        //                        req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //                        req.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
        //                        req.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
        //                    }
        //                    try
        //                    {
        //                        res = (HttpWebResponse)req.GetResponse();
        //                        sr.Close();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                    }
        //                }

        //                if (res.ContentType.StartsWith("image"))
        //                {
        //                    LerImagemResponse(res, p, e);
        //                }
        //                else
        //                {
        //                    if (res.ContentType.Contains("text/html"))
        //                    {
        //                        LerPaginaResponse(res, p, e);
        //                    }
        //                    else
        //                    {
        //                        LerPdfResponse(res, p, e);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                LerPaginaResponse(res, p, e);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //    }

        //}

        public static void BaixarArquivo(ref EmailData email, string url, string rgxSecundario, string parametro)
        {
            if (!String.IsNullOrEmpty(url))
            {
                url = url.Replace("&amp;", "&");
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
                req.AllowAutoRedirect = true;
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                req.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");

                req.CookieContainer = new CookieContainer();

                try
                {
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                    if (!String.IsNullOrEmpty(rgxSecundario) || res.ContentType.Contains("pdf"))
                    {
                        if (!String.IsNullOrEmpty(rgxSecundario))
                        {
                            req.CookieContainer.Add(res.Cookies);

                            StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(res.CharacterSet));

                            String html = sr.ReadToEnd();
                            sr.Close(); 
                            String id = Regex.Match(html, rgxSecundario).Groups[1].Value;
                            String urlSecundaria = String.Format(parametro, id);
                            
                            req = (HttpWebRequest)WebRequest.Create(urlSecundaria);
                            req.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
                            req.AllowAutoRedirect = true;
                            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                            req.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
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
                        else
                        {
                            if (res.ContentType.Contains("text/html"))
                            {
                                string caminhoArquivo = LerRespostaPagina(res, email);
                                email.Anexos.Add(new Anexo { CaminhoArquivo = caminhoArquivo, NomeArquivo = Path.GetFileName(caminhoArquivo) });
                            }
                            else
                            {
                                string caminhoArquivo = LerRespostaPdf(res, email);
                                email.Anexos.Add(new Anexo { CaminhoArquivo = caminhoArquivo, NomeArquivo = Path.GetFileName(caminhoArquivo) });
                            }
                        }
                    }
                    else
                    {
                        string caminhoArquivo = LerRespostaPagina(res, email);
                        email.Anexos.Add(new Anexo { CaminhoArquivo = caminhoArquivo, NomeArquivo = Path.GetFileName(caminhoArquivo) });
                    }
                }
                catch (Exception ex)
                {
                    string x = ex.Message;
                }
            }
        }

        private static string LerRespostaPagina(HttpWebResponse res, EmailData email)
        {
            StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(res.CharacterSet));

            string caminhoArquivo = FileManager.GetCaminho(CaminhoPara.AnexosProcessando);

            if (!Directory.Exists(caminhoArquivo))
                Directory.CreateDirectory(caminhoArquivo);

            caminhoArquivo = Path.Combine(caminhoArquivo, "B_" + DateTime.Now.ToString("ddMMyyyy-hhmmssfff") + ".html");

            File.WriteAllText(caminhoArquivo, sr.ReadToEnd(), Encoding.GetEncoding(res.CharacterSet));

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
                    }   while (bytesRead != 0);

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
