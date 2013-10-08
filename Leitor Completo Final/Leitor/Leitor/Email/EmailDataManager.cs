using Leitor.Dao;
using Leitor.Document;
using Leitor.Model;
using Leitor.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Leitor.Email
{
    public class EmailDataManager
    {
        private static HashSet<string> _prefeituras = new HashSet<string>();

        public static void LoadPrefeituras()
        {
            String local = FileManager.CaminhoRaiz;
            DirectoryInfo dir = new DirectoryInfo(local);
            EmailDataDAO dao2 = new EmailDataDAO();
            // Busca automaticamente todos os arquivos em todos os subdiretórios
            if (Directory.Exists(local))
            {
                FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    if (file.FullName.ToUpperInvariant().Contains(".XML")) continue;
                    String info = file.FullName;
                    EmailData e = LoadEmailDataFromFile(info.Replace(local, ""));
                    if (info.Contains("pdf") || (info.Contains("html") && !info.Contains("email")))
                    {
                        //VERIFICAR SE E É BOLETO OU NÃO
                        //if (e != null && !IsBoletoOrDanfe(e.Attachment))
                        //    dao2.InserirEmailData(e, e.Prefeitura, 0);
                    }
                        //ALTERADO 15/07 POR MATEUS: LER ARQUIVOS NO CORPO DO EMAIL
                    //else if (info.Contains("html") && info.Contains("email"))
                    //{
                    //    if (e == null) continue;
                    //    if (
                    //        !String.IsNullOrEmpty(
                    //            DocumentDownloader.GetLink(new PrefeituraDAO().SelecionarPrefeitura(e.Prefeitura), e)))
                    //        dao2.InserirEmailData(e, e.Prefeitura, 0);
                    //}
                }
            }
            PrefeituraDAO dao = new PrefeituraDAO();
            foreach (string s in _prefeituras)
            {
                if (dao.InserirPrefeitura(s) != -1)
                {
                    Log.SaveTxt("EmailDataManager.LoadPrefeituras", "NOVA PREFEITURA: " + s, Log.LogType.Processo);
                }
            }
        }

        public static bool IsBoletoOrDanfe(String arquivo)
        {
            bool result = false;
            if (arquivo != null)
            {
                if (arquivo.ToUpperInvariant().Contains("CADAS") || arquivo.ToUpperInvariant().Contains("BL ") ||
                          arquivo.ToUpperInvariant().Contains("BOLETO "))
                    result = true;
                if (File.Exists(arquivo) && !result)
                {
                    Regex rgxBoleto =
                        new Regex("\\d{5}\\s*.\\s*\\d{5}\\s*\\d{5}\\s*.\\s*\\d{6}\\s*\\d{5}\\s*.\\s*\\d{6}\\s*\\d{1}\\s*\\d{14}", RegexOptions.Singleline);
                    Regex rgxLocais = new Regex("Local\\s*de\\s*Pagamento", RegexOptions.Singleline);
                    string texto = arquivo.Contains(".pdf")
                                       ? ConversorPdf.ExtrairTexto(arquivo).Trim()
                                       : File.ReadAllText(arquivo);
                    result = (rgxBoleto.IsMatch(texto) || rgxLocais.IsMatch(texto) ||
                              texto.ToUpperInvariant().Contains("DANFE"));
                }
            }
            return result;
        }

        /// <summary>
        /// EXEMPLO DE PARAMETRO:
        /// SALVADOR\\18-06-2013 12-46-28\\email\\ENC_ NFS-e Município de Salvador.html
        /// </summary>
        /// <param name="info">Ex: SALVADOR\\18-06-2013 12-46-28\\email\\ENC_ NFS-e Município de Salvador.html</param>
        /// <returns></returns>
        private static EmailData LoadEmailDataFromLocation(String info)
        {
            EmailData email = null;

            String[] arr = info.Split('\\');
            _prefeituras.Add(arr[0]);

            Regex rgxData = new Regex("(\\d{2})-(\\d{2})-(\\d{4})\\s(\\d{2})-(\\d{2})-(\\d{2})", RegexOptions.Singleline);
            if (rgxData.IsMatch(arr[1]))
            {
                email = new EmailData();

                #region coletando a data
                Match m = rgxData.Match(arr[1]);

                DateTime data = new DateTime(
                    Convert.ToInt32(m.Groups[3].Value),
                    Convert.ToInt32(m.Groups[2].Value),
                    Convert.ToInt32(m.Groups[1].Value),
                    Convert.ToInt32(m.Groups[4].Value),
                    Convert.ToInt32(m.Groups[5].Value),
                    Convert.ToInt32(m.Groups[6].Value));

                #endregion
                email.Prefeitura = arr[0];
                email.Data = data;
                email.Assunto = arr[3].Replace(".html", "");
            }

            return email;
        }

        private static EmailData LoadEmailDataFromFile(String info)
        {
            EmailData email = null;
            String[] arr = info.Split('\\');
            Regex rgxData = new Regex("(\\d{2})-(\\d{2})-(\\d{4})\\s(\\d{2})-(\\d{2})-(\\d{2})", RegexOptions.Singleline);
            if (rgxData.IsMatch(arr[1]))
            {
                _prefeituras.Add(arr[0]);
                email = new EmailData();
                #region coletando a data
                Match m = rgxData.Match(arr[1]);
                DateTime data = new DateTime(
                    Convert.ToInt32(m.Groups[3].Value),
                    Convert.ToInt32(m.Groups[2].Value),
                    Convert.ToInt32(m.Groups[1].Value),
                    Convert.ToInt32(m.Groups[4].Value),
                    Convert.ToInt32(m.Groups[5].Value),
                    Convert.ToInt32(m.Groups[6].Value));
                #endregion
                DirectoryInfo di = new DirectoryInfo(FileManager.CaminhoRaiz + arr[0] + "\\" + arr[1] + "\\email\\");
                if (di.GetFiles().Any())
                    email.Assunto = di.GetFiles()[0].FullName.Split('\\')[di.GetFiles()[0].FullName.Split('\\').Length - 1];
                email.Prefeitura = arr[0];
                email.Data = data;
                //email.Attachment = arr[3];
            }
            return email;
        }
    }
}
