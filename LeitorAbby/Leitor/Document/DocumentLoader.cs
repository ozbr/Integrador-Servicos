using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Leitor.Core;
using Leitor.Dao;
using Leitor.Model;
using Leitor.Utilities;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Leitor.Document
{
    public class DocumentLoader
    {
        private Prefeitura Prefeitura { get; set; }
        private EmailData Email { get; set; }

        public void Load(Prefeitura p, EmailData email)
        {
            new EmailDataDAO().AtualizarEmailData(email, (int)Helper.FlowStatus.Processing);
            Prefeitura = p;
            Email = email;

            SalvarLerEnviar();
        }

        private void SalvarLerEnviar()
        {
            Dao.EmailDataDAO dao = new EmailDataDAO();
            List<IDocument> doc = GetDocument(Prefeitura, Email);

            if (doc.Count > 0)
            {
                foreach (IDocument document in doc)
                {
                    if (document is DocumentXml || Core.EnsureTaxDocument.IsTaxDoc(document.Arquivo))
                    {
                        if (Prefeitura.Nome == "ATIBAIA")
                        {
                            Regex regex = new Regex(@"(Data\s*de\s*Emissão\s*(\d{2}.\d{2}.\d{4}).*)\s*(Número\s*da\s*Nota\s*(\d*)\s*)", RegexOptions.IgnoreCase);

                            Match m = regex.Match(document.Arquivo);

                            if (m.Success)
                            {
                                document.Arquivo = regex.Replace(document.Arquivo, m.Groups[3].Value + m.Groups[1].Value + "\r\n");
                            }
                        }

                        Log.SaveTxt("DocumentLoader.SalvarLerEnviar", "Lendo nota de: " + Prefeitura.Nome, Log.LogType.Processo);
                        NF result = document.Read();
                        if (result != null)
                        {
                            var originalFileName = (from anexo in Email.Anexos
                                                      where anexo.CaminhoArquivo == document.Local
                                                      select anexo.CaminhoOriginalOCR).FirstOrDefault<string>();

                            String local = LotManager.CreateLot(Prefeitura, Email, originalFileName ?? document.Local, result, "E");
                            new EmailDataDAO().AtualizarLocalLote(Email);
                            IntegracaoManager.EnviarParaWebService(local, Email);
                        }
                        else
                        {
                            dao.AtualizarEmailData(Email, (int)Helper.FlowStatus.Failed);
                            Log.SaveTxt("DocumentLoader.SalverLerEnviar", "Não foi possível ler o arquivo: " + document.Local, Log.LogType.Erro);
                        }
                    }
                    else
                    {
                        Log.SaveTxt("DocumentLoader.SalverLerEnviar", "Não é documento fiscal. Arquivo: " + document.Local, Log.LogType.Erro);
                    }
                }
            }
            else
            {
                dao.AtualizarEmailData(Email, (int)Helper.FlowStatus.Failed);
            }
        }

        public IDocument GetSpecificDocument(Prefeitura p, EmailData e)
        {
            String local = FileManager.GetLocalArquivo(p.Nome, e);
            IDocument result = null;
            if (Directory.Exists(local))
            {
                DirectoryInfo di = new DirectoryInfo(local);
                FileInfo f = di.GetFiles()[0];
                if (f != null)
                {
                    String arquivo;
                    if (f.Extension.Contains("html"))
                    {
                        arquivo = File.ReadAllText(f.FullName);
                        result = new DocumentHtml
                            {Arquivo = arquivo, Local = FileManager.GetLocalArquivo(p.Nome, e) + "pagina.html"};
                    }
                    else
                    {
                        if (f.Extension.Contains("pdf"))
                        {
                            arquivo = ConversorPdf.ExtrairTexto(f.FullName).Trim();
                            result = new DocumentPdf {Arquivo = arquivo};
                        }
                    }
                }
            }

            if (result != null)
            {
                result.Prefeitura = p;
                RegexesDAO dao = new RegexesDAO();
                result.Parser = dao.SelecionarRegexPossiveis(p.Nome);
            }

            return result;
        }

        public List<IDocument> GetDocument(Prefeitura p, EmailData e)
        {
            List<IDocument> result = new List<IDocument>();

            String local = e.CaminhoLote;

            if (Directory.Exists(local))
            {
                for (int i = 0; i < e.Anexos.Count; i++)
                {
                    IDocument document;

                    string extension = Path.GetExtension(e.Anexos[i].CaminhoArquivo).ToUpperInvariant();

                    if (extension.Contains("HTML"))
                    {
                        document = new DocumentHtml
                        {
                            Arquivo = File.ReadAllText(e.Anexos[i].CaminhoArquivo),
                            Local = e.Anexos[i].CaminhoArquivo,
                            Prefeitura = p
                        };

                        RegexesDAO dao = new RegexesDAO();
                        document.Parser = dao.SelecionarRegexPossiveis(p.Nome);
                        result.Add(document);
                    }
                    else if (extension.Contains("PDF"))
                    {
                        document = new DocumentPdf { Arquivo = ConversorPdf.ExtrairTexto(e.Anexos[i].CaminhoArquivo).Trim(), Prefeitura = p, Local = e.Anexos[i].CaminhoArquivo };
                        RegexesDAO dao = new RegexesDAO();
                        document.Parser = dao.SelecionarRegexPossiveis(p.Nome);
                        result.Add(document);
                    }
                    else if (extension.Contains("XML"))
                    {
                        document = new DocumentXml
                        {
                            Arquivo = File.ReadAllText(e.Anexos[i].CaminhoArquivo),
                            Local = e.Anexos[i].CaminhoArquivo,
                            Prefeitura = p
                        };

                        RegexesDAO dao = new RegexesDAO();
                        document.Parser = dao.SelecionarRegexPossiveis(p.Nome);
                        result.Add(document);
                    }
                }
            }
            else
            {
                Log.SaveTxt("DocumentLoader", "Caminho especificado não encontrado.");
            }

            return result;
        }
    }
}
