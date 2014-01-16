using Leitor.Utilities;
using Leitor.Dao;
using Leitor.Model;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Leitor.Document;
using Leitor.Core;
using System.Data.SqlClient;
using Leitor.Tools;
using System.Threading.Tasks;

namespace Leitor.Email
{
    public class EmailManager
    {
        public List<IEmailLoader> GetPostalBoxes()
        {
            List<IEmailLoader> loaderList = new List<IEmailLoader>();

            foreach (EmailInfo ei in new EmailDAO().GetCaixasPostais())
            {
                IEmailLoader loader;

                switch (ei.Provider)
                {
                    case "EXCHANGE":
                        loader = new EmailExchange
                        {
                            Info = ei,
                            RequestDate = ei.LastReceipt
                        };

                        loaderList.Add(loader);
                        break;
                    case "POP":
                        loader = new EmailPop
                        {
                            Info = ei,
                            RequestDate = ei.LastReceipt
                        };

                        loaderList.Add(loader);
                        break;
                }
            }

            return loaderList;
        }

        public void UpdatePostalBoxes(ref List<IEmailLoader> emailList)
        {
            foreach (EmailInfo ei in new EmailDAO().GetCaixasPostais())
            {
                bool updated = false;

                IEmailLoader loader;

                for (int i = 0; i < emailList.Count; i++)
                {
                    if (ei.Id == emailList[i].Info.Id)
                    {
                        updated = true;

                        emailList[i].Info.Url = ei.Url;
                        emailList[i].Info.Provider = ei.Provider;
                        emailList[i].Info.Password = ei.Password;
                        emailList[i].Info.EmailAddress = ei.EmailAddress;
                        emailList[i].Info.Domain = ei.Domain;
                        emailList[i].Info.Id = ei.Id;
                        break;
                    }
                }

                if (!updated)
                {
                    switch (ei.Provider)
                    {
                        case "EXCHANGE":
                            loader = new EmailExchange
                            {
                                Info = ei,
                                RequestDate = ei.LastReceipt
                            };

                            emailList.Add(loader);
                            break;
                        case "POP":
                            loader = new EmailPop
                            {
                                Info = ei,
                                RequestDate = ei.LastReceipt
                            };

                            emailList.Add(loader);
                            break;
                        case "FOLDER":
                            loader = new EmailFolder
                            {
                                Info = ei,
                                RequestDate = ei.LastReceipt
                            };

                            emailList.Add(loader);
                            break;
                    }
                }
            }
        }

        public void Download(IEmailLoader loader)
        {
            List<EmailData> result = loader.LoadEmails(ReadEmail);

            if (result.Count > 0)
                Log.SaveTxt("Emails Lidos: " + result.Count.ToString(), Log.LogType.Debug);
        }

        private bool ReadEmail(EmailData email)
        {
            Log.SaveTxt("Lendo Email: " + email.Assunto, Log.LogType.Debug);

            String prefeitura;

            Boolean sucesso;
            string caminhoDiretorioDestino = string.Empty;

            sucesso = ImportantMatches.TryGetCity(out prefeitura, email.Remetente, email.RemetentesPotenciais.ToArray());
            
            if (sucesso)
            {
                email.Prefeitura = prefeitura;

                string url;
                string[] parametrosUrl;

                sucesso = ImportantMatches.TryGetTaxDocumentUrl(out url, out parametrosUrl, email.Prefeitura, email.Corpo);
                
                if (sucesso)
                {
                    DocumentDownloader.BaixarArquivo(ref email, url, parametrosUrl[0], parametrosUrl[1]);
                }
            }
            else
            {
                string url;
                string[] parametrosUrl;

                sucesso = ImportantMatches.TryGetCityAndTaxDocumentUrl(out prefeitura, out parametrosUrl, out url, email.Corpo);

                if (sucesso)
                {
                    email.Prefeitura = prefeitura;
                    DocumentDownloader.BaixarArquivo(ref email, url, parametrosUrl[0], parametrosUrl[1]);
                }
                else
                {
                    for (int i = email.Anexos.Count - 1; i > -1; i--)
                    {
                        bool isNf;
                        sucesso = ImportantMatches.TryGetCityAndIsTaxDoc(out prefeitura, out isNf, email.Anexos[i].CaminhoArquivo);

                        if (sucesso)
                        {
                            email.Prefeitura = prefeitura;
                            break;
                        }
                        else
                        {
                            if (!isNf)
                            {
                                if (File.Exists(email.Anexos[i].CaminhoArquivo))
                                    File.Delete(email.Anexos[i].CaminhoArquivo);

                                email.Anexos.RemoveAt(i);
                            }
                        }
                    }
                }
            }

            if (email.Anexos.Count > 0)
            {
                Leitor.Helper.FlowStatus initialStatus = Helper.FlowStatus.Downloaded;

                var tpAnexos = new PrefeituraDAO().SelecionarPrefeituraTpAnexos(email.Prefeitura);

                for (int i = 0; i < email.Anexos.Count; i++)
                {
                    var ext = Path.GetExtension(email.Anexos[i].NomeArquivo).TrimStart('.').ToUpper();
                    var tpAnexo = (from tp in tpAnexos where tp.Extensao.ToUpper() == ext select tp).FirstOrDefault<TpAnexo>();
                    
                    if (tpAnexo == null)
                        tpAnexo = (from tp in tpAnexos where tp.Extensao == "*" select tp).FirstOrDefault<TpAnexo>();

                    if (tpAnexo == null)
                    {
                        tpAnexo = new TpAnexo();
                        initialStatus = Helper.FlowStatus.Stored;
                    }

                    //string originalPath
                    //if (Directory.Exists(Path.Combine(Path.GetDirectoryName(email.Anexos[i].CaminhoArquivo), "Original")))
                    //{
                        
                    //}

                    if (!String.IsNullOrEmpty(email.Prefeitura))
                    {
                        email.CaminhoLote = String.Format(FileManager.GetCaminho(CaminhoPara.Lote), email.Prefeitura, email.Data.ToString("dd-MM-yyyy hh-mm-ss"));
                        int ind = 0;
                        while (Directory.Exists(email.CaminhoLote))
                            email.CaminhoLote = String.Format(FileManager.GetCaminho(CaminhoPara.Lote), email.Prefeitura, email.Data.ToString("dd-MM-yyyy hh-mm-ss") + "_" + (++ind).ToString());
                    }
                    else
                    {
                        email.CaminhoLote = String.Format(FileManager.GetCaminho(CaminhoPara.AnexosDeixados));
                    }


                    caminhoDiretorioDestino = Path.Combine(email.CaminhoLote, @"anexos\");
                    
                    if (!Directory.Exists(caminhoDiretorioDestino))
                    {
                        Directory.CreateDirectory(caminhoDiretorioDestino);
                    }

                    //Move o arquivo para a pasta de anexos
                    string caminhoArquivoDestino = Path.Combine(caminhoDiretorioDestino, Path.GetFileName(email.Anexos[i].CaminhoArquivo));
                    File.Move(email.Anexos[i].CaminhoArquivo, caminhoArquivoDestino);
                    email.Anexos[i].CaminhoArquivo = caminhoArquivoDestino;

                    if (!string.IsNullOrEmpty(email.Anexos[i].CaminhoOriginalOCR))
                    {
                        //Move o anexo identificado como arquivo original para a pasta de anexos do email
                        string caminhoArquivoOriginalDestino = Path.Combine(caminhoDiretorioDestino, Path.GetFileNameWithoutExtension(email.Anexos[i].CaminhoArquivo) + Path.GetExtension(email.Anexos[i].CaminhoOriginalOCR));
                        File.Move(email.Anexos[i].CaminhoOriginalOCR, caminhoArquivoOriginalDestino);
                        email.Anexos[i].CaminhoOriginalOCR = caminhoArquivoOriginalDestino;
                    }

                    if (tpAnexo.UseOCR)
                    {
                        string ocrFilePath = FileManager.CaminhoOCR_Input.Replace("[PREFEITURA]", email.Prefeitura);
                        if (!Directory.Exists(ocrFilePath))
                            Directory.CreateDirectory(ocrFilePath);

                        email.Anexos[i].ControleOCR = Guid.NewGuid().ToString();
                        File.Copy(caminhoArquivoDestino, Path.Combine(ocrFilePath, email.Anexos[i].ControleOCR + Path.GetExtension(caminhoArquivoDestino)));
                        initialStatus = Helper.FlowStatus.OCR;
                    }

                    FileManager.SaveEmail(email.Prefeitura, email, email.Assunto, ".html", email.Corpo);
                }

                EmailDataDAO dao = new EmailDataDAO();
                return dao.SalvarEmailData(email, initialStatus);
            }
            else
            {
                LogDAO dao = new LogDAO();
                return dao.InserirLog("Não foram encontrados notas fiscais.", email.Remetente, email.Assunto, email.Corpo, email.IdEnderecoEmail);
            }
        }
    }
}
