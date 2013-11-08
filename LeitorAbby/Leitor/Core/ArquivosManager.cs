using Leitor.Utilities;
using System;
using System.IO;
using System.Text;

namespace Leitor.Core
{
    public class ArquivosManager
    {
        public static String Local = @"C:\Leitor\";
        public static String LocalEmails = Local + "{0}" + @"\" + DateTime.Now.ToString("dd-MM-yyyy") + @"\emails\";
        //public static String localAnexos = local + "{0}" + @"\" + DateTime.Now.ToString("dd-MM-yyyy") + "\\anexos\\";
        public static String LocalArquivos = Local + "{0}" + @"\" + DateTime.Now.ToString("dd-MM-yyyy") + @"\arquivos\";
        //public static String localLinks = local + "{0}" + @"\links\" + DateTime.Now.ToString("dd-MM-yyyy") + @"\";
        public static String LocalXml = Local + "{0}" + @"\" + DateTime.Now.ToString("dd-MM-yyyy") + "\\";

        public static void SalvarEmail(String remetente, String nomeArquivo, String extensao, String arquivo)
        {
            if (!Directory.Exists(String.Format(LocalEmails, remetente)))
            {
                Directory.CreateDirectory(String.Format(LocalEmails, remetente));
            }
            File.WriteAllText(String.Format(LocalEmails, remetente) + @"\" + nomeArquivo.Replace(":", "_") + extensao, arquivo);
        }

        public static void SalvarEmailPrefeitura(String remetente, String nomeArquivo, String extensao, String arquivo)
        {
            if (!Directory.Exists(String.Format(LocalEmails, remetente)))
            {
                Directory.CreateDirectory(String.Format(LocalEmails, remetente));
            }
            File.WriteAllText(String.Format(LocalEmails, remetente) + @"\" + nomeArquivo.Replace(":", "_") + extensao, arquivo);
        }

        //public static void SalvarAnexo(String remetente, String nomeArquivo, String extensao, String arquivo)
        //{
        //    if (!Directory.Exists(String.Format(localAnexos, remetente)))
        //    {
        //        Directory.CreateDirectory(String.Format(localAnexos, remetente));
        //    }
        //    File.WriteAllText(String.Format(localAnexos, remetente) + @"\" + nomeArquivo.Replace(":", "_") + extensao, arquivo);
        //}

        public static void SalvarArquivo(String remetente, String nomeArquivo, String extensao, String arquivo)
        {
            if (!Directory.Exists(String.Format(LocalArquivos, remetente)))
            {
                Directory.CreateDirectory(String.Format(LocalArquivos, remetente));
            }
            File.WriteAllText(String.Format(LocalArquivos, remetente) + @"\" + nomeArquivo.Replace(":", "_") + extensao, arquivo, Encoding.GetEncoding("UTF-8"));
        }

        internal static void SalvarArquivoPrefeitura(String remetente, String nomeArquivo, String extensao, String arquivo)
        {
            if (!Directory.Exists(String.Format(LocalArquivos, remetente)))
            {
                Directory.CreateDirectory(String.Format(LocalArquivos, remetente));
            }
            File.WriteAllText(String.Format(LocalArquivos, remetente) + @"\" + nomeArquivo.Replace(":", "_") + extensao, arquivo, Encoding.GetEncoding("UTF-8"));
        }

        public static String LerArquivo(String remetente)
        {
            String result = String.Empty;

            if (Directory.Exists(String.Format(LocalArquivos, remetente)))
            {
                DirectoryInfo di = new DirectoryInfo(String.Format(LocalArquivos, remetente));
                FileInfo f = di.GetFiles()[0];
                if (f != null)
                {
                    if (f.Extension.Contains("html"))
                    {
                        result = File.ReadAllText(f.FullName);
                    }
                    else
                    {
                        if (f.Extension.Contains("pdf"))
                        {
                            result = ConversorPdf.ExtrairTexto(f.FullName).Trim();

                        }
                    }
                }
            }

            return result;
        }

        public static String LerArquivoTemporario(String localArquivo)
        {
            String result = String.Empty;

            if (Directory.Exists(localArquivo))
            {
                DirectoryInfo di = new DirectoryInfo(localArquivo);
                FileInfo f = di.GetFiles()[0];
                if (f != null)
                {
                    if (f.Extension.Contains("html"))
                    {
                        result = File.ReadAllText(f.FullName);
                    }
                    else
                    {
                        if (f.Extension.Contains("pdf"))
                        {
                            result = ConversorPdf.ExtrairTexto(f.FullName).Trim();

                        }
                    }
                }
            }

            return result;
        }

        public static void SalvarImagem()
        {
            string fullFileName = string.Empty;
        }

        public static String Lote(String numeroLote, String cnpj, String qtdArquivos)
        {
            return DateTime.Now.ToString("ddMMyyyy") + numeroLote + "_" + cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "") + "_" + qtdArquivos;
        }

        public static String LocalZip(String lote, String email)
        {
            return String.Format("C:\\Temp\\Lotes\\" + lote + ".zip", email);
        }

        public static String BaseLocal(String numeroLote, String cnpj, String tipo, String numero) 
        {
            return DateTime.Now.ToString("ddMMyyyy") + numeroLote + "_" + cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "") + "_" + (String.IsNullOrEmpty(tipo) ? "E" : tipo.ToUpper()) + "_" + numero;
        }



    }
}
