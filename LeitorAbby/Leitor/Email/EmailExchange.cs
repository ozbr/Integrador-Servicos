using Leitor.Core;
using Leitor.Dao;
using Leitor.Model;
using Leitor.Utilities;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Leitor.Email
{
    public class EmailExchange : IEmailLoader
    {
        public List<EmailData> LoadEmails()
        {
            List<EmailData> emails = new List<EmailData>();

            try
            {
                // Criar objeto de serviço do Exchange
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010);
                service.Credentials = new NetworkCredential(Info.EmailAddress, Info.Password, Info.Domain);
                service.Url = new Uri(Info.Url);

                // Escolher o horário a partir do qual os arquivos serão baixados
                DateTime dateTimeReceivedFilter = LastRequestStartedOn > RequestDate ? LastRequestStartedOn : RequestDate;

                // Criar coleção de regras de filtragem
                List<SearchFilter> filterCollection = new List<SearchFilter>();
                filterCollection.Add(new SearchFilter.IsGreaterThan(EmailMessageSchema.DateTimeReceived, dateTimeReceivedFilter.AddSeconds(1)));

                // Criar filtro baseado nas regras de filtragem
                SearchFilter filter = new SearchFilter.SearchFilterCollection(LogicalOperator.And, filterCollection);

                // Resultado dos itens
                FindItemsResults<Item> itemsResults = null; ;

                // Preencher resultado dos itens
                itemsResults = service.FindItems(WellKnownFolderName.Inbox, filter, new ItemView(200));
                
                if (itemsResults.Items != null && itemsResults.Items.Count > 0)
                {
                    service.LoadPropertiesForItems(itemsResults, PropertySet.FirstClassProperties);

                    for (int i = itemsResults.Items.Count - 1; i > -1; i--)
                    {
                        Item item = itemsResults.Items[i];

                        // Atualizar horário do último Request
                        LastRequestStartedOn = item.DateTimeReceived;

                        EmailMessage message = 
                            EmailMessage.Bind(service, item.Id, new PropertySet(BasePropertySet.IdOnly, ItemSchema.Attachments, ItemSchema.HasAttachments));

                        // Pegar o endereço do remetente
                        String remetente = ((Microsoft.Exchange.WebServices.Data.EmailAddress)item[EmailMessageSchema.From]).Address;

                        // Guardar possíveis remetentes no corpo do email (caso algum seja encaminhado)
                        List<String> remetentePotencial = new List<String>();

                        if (item.Body.BodyType == BodyType.HTML)
                        {
                            MatchCollection mCollection = Regex.Matches(item.Body.ToString(), @"(?i)(?:De|From):.*?(\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,6}\b)");

                            for (int x = 0; x < mCollection.Count; x++)
                            {
                                remetentePotencial.Add(mCollection[x].Groups[1].Value.ToLower());
                            }
                        }

                        // Guardar informações dos anexos
                        List<Anexo> informacaoAnexos = new List<Anexo>();
                        string path = FileManager.GetCaminho(CaminhoPara.AnexosProcessando);

                        foreach (Attachment attachment in message.Attachments)
                        {
                            if (attachment.Name.ToLower().Contains("pdf"))
                            {
                                FileAttachment fileAttachment = attachment as FileAttachment;

                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }

                                string fileName = Path.GetFileNameWithoutExtension(fileAttachment.Name) + "_E" + DateTime.Now.ToString("ddMMyyyy-hhmmssfff") + Path.GetExtension(fileAttachment.Name);
                                string filePath = Path.Combine(path, fileName);

                                using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                                {
                                    fileAttachment.Load(fileStream);
                                    fileStream.Flush();
                                }

                                informacaoAnexos.Add(new Anexo { CaminhoArquivo = filePath, NomeArquivo = Path.GetFileName(filePath) });

                                Log.SaveTxt("EmailExchange.LoadEmails", "Anexo armazenado: " + filePath, Log.LogType.Processo);
                            }
                        }

                        emails.Add(new EmailData()
                            {
                                Anexos = informacaoAnexos,
                                Assunto = item.Subject,
                                Corpo = item.Body,
                                Data = item.DateTimeReceived,
                                IdEnderecoEmail = Info.Id,
                                Remetente = remetente,
                                RemetentesPotenciais = remetentePotencial
                            });
                    }
                }
                else
                {
                    Log.SaveTxt("EmailExchange.LoadEmails", "Não há novos emails em: " + Info.EmailAddress, Log.LogType.Processo);
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("EmailExchange.LoadEmails", e.Message, Log.LogType.Erro);
                return new List<EmailData>();
            }

            return emails;
        }

        private static object _lockBecauseOfLastRequestDateTime = new object();

        public DateTime RequestDate
        {
            get;
            set;
        }

        public EmailInfo Info
        {
            get;
            set;
        }

        public DateTime LastRequestStartedOn
        {
            get;
            set;
        }
    }
}
