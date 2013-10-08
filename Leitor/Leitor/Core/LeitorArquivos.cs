using Leitor.Dao;
using Leitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Leitor.Core
{
    public class LeitorArquivos
    {
        public static String LerArquivo(Remetente remetente)
        {
            String arquivo = ArquivosManager.LerArquivo(remetente.Emails);
            return arquivo;
        }

        public static void AtualizarPrefeitura(Remetente r, String arquivo, bool isEmail)
        {
            if (r.Nome == r.Emails)
            {
                String final = String.Empty;
                Regex rxPrefeitura = new Regex(".*?(PREFEIT.*)", RegexOptions.Singleline);
                String match = rxPrefeitura.Match(arquivo.ToUpperInvariant()).Groups[1].Value;
                match = match.Substring(0, (match.Length > 200 ? 200 : match.Length));
                String[] arr = match.Split(' ');
                int interessantes = 0;//palavras com mais de 4 caracteres
                for (int k = 0; k < arr.Length && interessantes < 3; k++)
                {
                    if (arr[k].Length > 4 && arr[k]!="SANTO")
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


                if (isEmail)
                {
                    final.Replace(".", "");
                }

                if (final.Length > 10 && !final.Contains("\"") && !final.Contains('.'))
                {
                    new RemetenteDAO().AtualizarNomePrefeitura(r.Id, final);
                }
            }
        }
    }
}
