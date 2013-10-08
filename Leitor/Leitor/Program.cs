using System.Text.RegularExpressions;
using Leitor.Core;
using Leitor.Dao;
using Leitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Leitor
{
    internal class Program
    {
        private static void Main()
        {
            carregarEmails(new DateTime(2013, 06, 10));//CARREGA TODOS OS EMAILS A PARTIR DA DATA ESPECIFICADA

            //lerArquivos("redher.wetzl@dot-insight.net");//LÊ OS ARQUIVOS RECEBIDOS DO EMAIL DO REMETENTE ESPECIFICADO
            //lerArquivos("hering.beckedorff@pqspe.com.br");
            //lerArquivos("hering.beckedorff@pqspe.com.br");            
            //uploadXml("hering.beckedorff@pqspe.com.br");
            //uploadXml("nfe.noreply@barueri.sp.gov.br");
            //uploadXml("noreply@sefaz.salvador.ba.gov.br");
            //uploadXml("caixa_dpedro@armandoveiculos.com.br");
            //uploadXml("mateus.rocha@dot-insight.net");
            //uploadXml("redher.wetzl@dot-insight.net");


            lerArquivos();//LÊ TODOS OS ARQUIVOS RECEBIDOS
            uploadXml();

            //Console.Read();
        }

        #region métodos

        /// <summary>
        /// Lê todos os e-mails a partir da data especificada
        /// </summary>
        /// <param name="dateTime">data de início da leitura</param>
        private static void carregarEmails(DateTime dateTime)
        {
            #region carrega todos os emails enviados após 10/06/2013
            VarredorEmails varredor = new VarredorEmails();
            varredor.VerificaEmail(dateTime);
            #endregion
        }

        /// <summary>
        /// Faz a leitura de todos os arquivos recolhidos pelo método carregarEmails()
        /// </summary>
        private static void lerArquivos()
        {
            RemetenteDAO dao = new RemetenteDAO();
            #region leitura de todos

            List<Remetente> remetentes = dao.SelecionarRemetenteTodos();
            for (int c = 0; c < remetentes.Count; c++)
            {
                if (!remetentes[c].ArquivoNoCorpo && !String.IsNullOrEmpty(remetentes[c].RgxLink))
                {
                    LinksManager.SalvarLinksRemetente(remetentes[c]);
                }

                String arquivo = ArquivosManager.LerArquivo(remetentes[c].Emails).Trim();
                Parametrizador pd = new Parametrizador();
                String final = pd.GerarXml(arquivo, remetentes[c], "0" + c);
                LeitorArquivos.AtualizarPrefeitura(remetentes[c], arquivo, false);
                //LINHA RESPONSÁVEL POR ENVIAR O ARQUIVO GERADO AO WEBSERVICE
                ////IntegracaoManager.EnviarParaWebService(final);
            }
            #endregion
        }

        /// <summary>
        /// Faz a leitura dos arquivos enviados pelo remetente especificado
        /// </summary>
        /// <param name="p">email do remetente</param>
        private static void lerArquivos(string p)
        {
            RemetenteDAO dao = new RemetenteDAO();

            #region leitura individual

            Remetente r = dao.SelecionarRemetentePorEmail(p);
            if (!r.ArquivoNoCorpo && !String.IsNullOrEmpty(r.RgxLink))
            {
                LinksManager.SalvarLinksRemetente(r);
            }

            Parametrizador pd = new Parametrizador();
            String arquivo = ArquivosManager.LerArquivo(r.Emails).Trim();
            String final = pd.GerarXml(arquivo, r, "01");
            //LINHA RESPONSÁVEL POR ENVIAR O ARQUIVO GERADO AO WEBSERVICE
            ////IntegracaoManager.EnviarParaWebService(final, r.Id);

            #endregion
        }

        /// <summary>
        /// Faz upload de todos os arquivos ainda não enviados, de acordo com a tabela [DOT_LEITOR].[dbo].[ARQUIVO]
        /// </summary>
        private static void uploadXml()
        {
            ArquivoDAO dao = new ArquivoDAO();
            Dictionary<String, int> d = dao.SelecionarArquivosNaoLidos();

            foreach (String s in d.Keys)
            {
                IntegracaoManager.EnviarParaWebService(s, d[s]);
            }
        }


        /// <summary>
        /// Faz upload dos arquivos não enviados do remetente especificado
        /// </summary>
        /// <param name="p">e-mail do remetente</param>
        private static void uploadXml(string p)
        {
            ArquivoDAO dao = new ArquivoDAO();
            RemetenteDAO rdao = new RemetenteDAO();

            Dictionary<String, int> d = dao.SelecionarArquivosNaoLidos(rdao.SelecionarRemetentePorEmail(p).Id);

            foreach (String s in d.Keys)
            {
                IntegracaoManager.EnviarParaWebService(s, d[s]);
            }
        }


        #endregion

    }
}
