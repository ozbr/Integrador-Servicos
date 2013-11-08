using Leitor.Model;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Leitor.Utilities
{
    public enum CaminhoPara
    {
        Raiz, Lote, PrefeituraAnexos, PrefeituraEmail, AnexosProcessando, AnexosDeixados
    }

    public static class FileManager
    {
        
        public static string CaminhoRaiz
        {
            get
            {
                if (string.IsNullOrEmpty(_cacheCaminhoRaiz))
                    _cacheCaminhoRaiz = (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["CaminhoRaiz"])
                               ? ConfigurationManager.AppSettings["CaminhoRaiz"]
                               : @"C:\Leitor\");
                return _cacheCaminhoRaiz;
            }
        }

        public static string CaminhoOCR_Input
        {
            get
            {
                if (string.IsNullOrEmpty(_caminhoOCR_Input))
                    _caminhoOCR_Input = (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["CaminhoOCR_Input"])
                               ? ConfigurationManager.AppSettings["CaminhoOCR_Input"]
                               : @"C:\Leitor\OCR_Input");
                return _caminhoOCR_Input;
            }
        }

        public static string CaminhoOCR_Output
        {
            get
            {
                if (string.IsNullOrEmpty(_caminhoOCR_Output))
                    _caminhoOCR_Output = (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["CaminhoOCR_Output"])
                               ? ConfigurationManager.AppSettings["CaminhoOCR_Output"]
                               : @"C:\Leitor\OCR_Ouput");
                return _caminhoOCR_Output;
            }
        }

        private static String _cacheCaminhoRaiz = null;
        private static String _caminhoPrefeituraAnexos = CaminhoRaiz + @"{0}\{1}\anexos\";
        private static String _caminhoPrefeituraEmail = CaminhoRaiz + @"{0}\{1}\email\";
        private static String _caminhoLote = CaminhoRaiz + @"{0}\{1}\";
        private static String _caminhoAnexosProcessando = CaminhoRaiz + @"_AnexosProcessando\";
        private static String _caminhoAnexosDeixados = CaminhoRaiz + @"_AnexosDeixados\";
        private static String _caminhoOCR_Input;
        private static String _caminhoOCR_Output;

        public static String GetCaminho(CaminhoPara caminho)
        {
            string caminhoArquivo = null;

            switch (caminho)
            {
                case CaminhoPara.Raiz: caminhoArquivo = CaminhoRaiz; break;
                case CaminhoPara.Lote: caminhoArquivo = _caminhoLote; break;
                case CaminhoPara.PrefeituraEmail: caminhoArquivo = _caminhoPrefeituraEmail; break;
                case CaminhoPara.PrefeituraAnexos: caminhoArquivo = _caminhoPrefeituraAnexos; break;
                case CaminhoPara.AnexosDeixados: caminhoArquivo = _caminhoAnexosDeixados; break;
                case CaminhoPara.AnexosProcessando: caminhoArquivo = _caminhoAnexosProcessando; break;
            }

            return caminhoArquivo;
        }

        public static String CaminhoAnexosProcessando 
        { 
            get { return _caminhoAnexosProcessando; } 
        }


        /// <summary>
        /// Salva o email na pasta C:\Leitor\{Cidade}\{Data}\email
        /// </summary>
        /// <param name="prefeitura"></param>
        /// <param name="e"></param>
        /// <param name="nomeArquivo"></param>
        /// <param name="extensao"></param>
        /// <param name="arquivo"></param>
        public static void SaveEmail(String prefeitura, EmailData e, String nomeArquivo, String extensao, String arquivo)
        {
            String local = GetLocalEmail(prefeitura, e);
            if (!Directory.Exists(local))
                Directory.CreateDirectory(local);
            File.WriteAllText(local + nomeArquivo.Replace(":", "_").Replace("\\", "_").Replace("/", "_") + extensao, arquivo, Encoding.GetEncoding("UTF-8"));
            Log.SaveTxt("EmailManager.SaveEmail", "Email armazenado: " + local + nomeArquivo.Replace(":", "_") + extensao, Log.LogType.Processo);
        }

        /// <summary>
        /// Salva um arquivo de texto ou html.
        /// </summary>
        /// <param name="prefeitura"></param>
        /// <param name="e"></param>
        /// <param name="nomeArquivo"></param>
        /// <param name="extensao"></param>
        /// <param name="arquivo"></param>
        public static void SaveFile(String prefeitura, EmailData e, String nomeArquivo, String extensao, String arquivo)
        {
            String local = GetLocalArquivo(prefeitura, e);
            if (!Directory.Exists(local))
                Directory.CreateDirectory(local);
            File.WriteAllText(local + nomeArquivo.Replace(":", "_") + extensao, arquivo, Encoding.GetEncoding("UTF-8"));
            Log.SaveTxt("EmailManager.SaveFile", "Arquivo armazenado: " + local + nomeArquivo.Replace(":", "_") + extensao, Log.LogType.Processo);
        }

        public static void SaveNf(String prefeitura, EmailData e, NF nf)
        {
            String local = GetLocalNf(prefeitura, e);
            if (!Directory.Exists(local))
                Directory.CreateDirectory(local);

        }

        /// <summary>
        /// Com base na prefeitura e data de e-mail retorna o local do arquivo, ex:
        /// C:\Leitor\CIDADE\04-07-2013 10-30-22\arquivos\
        /// </summary>
        /// <param name="p"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetLocalArquivo(String p, EmailData e)
        {
            return String.Format(_caminhoPrefeituraAnexos, p, e.Data.ToString("dd-MM-yyyy hh-mm-ss"));
        }

        /// <summary>
        /// Com base na prefeitura e data de e-mail retorna o local do arquivo, ex:
        /// C:\Leitor\CIDADE\04-07-2013 10-30-22\email\
        /// </summary>
        /// <param name="p"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetLocalEmail(String p, EmailData e)
        {
            return String.Format(_caminhoPrefeituraEmail, p, e.Data.ToString("dd-MM-yyyy hh-mm-ss"));
        }

        public static String GetLocalNf(String p, EmailData e)
        {
            return String.Format(_caminhoLote, p, e.Data.ToString("dd-MM-yyyy hh-mm-ss"));
        }

        public static String GetArquivo(String p, EmailData e)
        {
            String result = String.Empty;
            if (Directory.Exists(GetLocalArquivo(p, e)))
            {
                DirectoryInfo di = new DirectoryInfo(GetLocalArquivo(p, e));
                FileInfo f = di.GetFiles()[0];
                if (f != null)
                {
                    result = f.FullName;
                }
            }
            return result;
        }

        public static String GetNf(String p, EmailData e)
        {
            String result = String.Empty;
            if (Directory.Exists(GetLocalNf(p, e)))
            {
                DirectoryInfo di = new DirectoryInfo(GetLocalNf(p, e));
                FileInfo f = di.GetFiles()[0];
                if (f != null && f.FullName.Contains("xml"))
                {
                    result = f.FullName;
                }
            }
            return result;
        }

        /// <summary>
        /// Com base em qualquer texto encontra o nome mais adequado para dar à prefeitura
        /// ACONSELHO NÃO ALTERAR
        /// Regras:
        /// 1 - Regex iniciando em "PREFEIT" até o fim do arquivo
        /// 2 - Limita a String pega em 200 caracteres
        /// 3 - Realiza Split por espaços
        /// 4 - Busca por "elementos interessantes" dentro do array resultante do split
        /// Elementos interessantes são todos exceto:
        ///     Elementos de até 4 letras (SÃO, DE, RIO)
        ///     Elementos != de SANTO, CAMPO, VOLTA, FEIRA, POÇOS, MONTES
        /// 5 - junta todos os elementos do arrays até ter somado 3 elementos interessantes (PREFEITURA MUNICIPAL DO RIO DE JANEIRO), PREFEITURA MUNICIPAL JANEIRO são os elementos interessantes
        /// 6 - Remove caracteres indesejados da String final
        /// 7 - As vezes encontrava PREFEITURA DE SALVADOR ou PREFEITURA MUNICIPAL DE SALVADOR, por isso, são removidos os excessos e resulta em SALVADOR somente
        /// </summary>
        /// <param name="arquivo"></param>
        /// <param name="isEmail"></param>
        /// <returns></returns>
        public static String RecolheNomePrefeitura(String arquivo, bool isEmail)
        {
            String final = String.Empty;

            Regex rxPrefeitura = new Regex(".*?(PREFEIT.*)", RegexOptions.Singleline);
            String match = rxPrefeitura.Match(arquivo.ToUpperInvariant()).Groups[1].Value;
            match = match.Substring(0, (match.Length > 200 ? 200 : match.Length));
            match = match.Replace("Ï¿½", "Ã");
            match = match.Replace("\r\n", " ");//alterado em 10/07 por Mateus Rocha vide prefeitura de cuiabá
            String[] arr = match.Split(' ');
            int interessantes = 0;//palavras com mais de 4 caracteres
            for (int k = 0; k < arr.Length && interessantes < 3; k++)
            {
                if (arr[k].Length > 4 && arr[k] != "SANTO" && arr[k] != "CAMPO" && arr[k] != "VOLTA" && arr[k] != "FEIRA" && arr[k] != "POÇOS" && arr[k] != "MONTES")
                {
                    interessantes++;
                }
                final += arr[k] + " ";
            }

            if (final.Contains("<"))
            {
                final = final.Substring(0, final.IndexOf('<'));
            }

            if (final.Contains("-"))
            {
                final = final.Substring(0, final.IndexOf('-'));
            }
            if(final.Contains("("))
            {
                final = final.Substring(0, final.IndexOf('('));
            }

            if (!(final.Length > 10) && final.Contains("\"") && final.Contains('.'))
            {
                final = string.Empty;
            }
            final = Util.RemoverAcentos(final);
            final = final.Trim().Replace("/", "").Replace("\\", "").Replace(",", "").Replace("[", "").Replace(".", "").Trim();
            final = final.Replace("PREFEITURA", "").Replace("MUNICIPAL", "").Replace("SECRETARIA", "").Replace("CIDADE", "").Replace("MUNICIPIO", "");
            final = final.Trim();
            //Algumas notas de goiania vem com caracteres estranhos no meio.
            Match goiania = Regex.Match(final, "GOI.*?NIA");
            if(goiania.Success)
                final = "GOIANIA";
            while (final.Length > 2 && final.Substring(2, 1).Equals(" "))
            {
                final = final.Substring(3).Trim();
            }

            return final.Trim();
        }

        /// <summary>
        /// Lê o arquivo (html ou pdf) especificado no local.
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public static String Read(String local)
        {
            String result = String.Empty;

            if (Directory.Exists(local))
            {
                DirectoryInfo di = new DirectoryInfo(local);
                if (di.GetFiles().Any())
                {
                    FileInfo f = new FileInfo(local);
                    if (f.Extension.ToUpper().Contains("HTML"))
                        result = File.ReadAllText(f.FullName);
                    else if (f.Extension.ToUpper().Contains("PDF"))
                        result = ConversorPdf.ExtrairTexto(f.FullName).Trim();
                }
            }
            else if (File.Exists(local))
            {
                FileInfo f = new FileInfo(local);
                if (f.Extension.ToUpper().Contains("HTML"))
                    result = File.ReadAllText(f.FullName);
                else if (f.Extension.ToUpper().Contains("PDF"))
                    result = ConversorPdf.ExtrairTexto(f.FullName).Trim();
            }
            return result;
        }

    }
}
