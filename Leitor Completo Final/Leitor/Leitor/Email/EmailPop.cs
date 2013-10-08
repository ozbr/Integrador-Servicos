using System.Threading;
using Leitor.Dao;
using Leitor.Model;
using Leitor.Utilities;
using Microsoft.Exchange.WebServices.Data;
using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Leitor.Core;

namespace Leitor.Email
{
    public class EmailPop : IEmailLoader
    {
        public List<Model.EmailData> LoadEmails()
        {
            List<EmailData> emails = new List<EmailData>();

            try
            {
                Match match = Regex.Match(Info.Url, @"(?:http://)?(?:www\.)?(.*):(\d{1,4})");

                if (match.Success)
                {
                    // Escolher o horário a partir do qual os arquivos serão baixados
                    DateTime dateTimeReceivedFilter = LastRequestStartedOn > RequestDate ? LastRequestStartedOn : RequestDate;

                    // Trazer mensagens a partir do horário escolhido
                    List<Message> fetchedMessages =
                        FetchMessagesByDateTime(match.Groups[1].Value, Convert.ToInt32(match.Groups[2].Value), false, Info.EmailAddress, Info.Password, dateTimeReceivedFilter);

                    for (int i = fetchedMessages.Count - 1; i > -1 ; i--)
                    {
                        Message message = fetchedMessages[i];
                        MailMessage mailMessage = message.ToMailMessage();

                        // Guardar possíveis remetentes no corpo do email (caso algum seja encaminhado)
                        List<String> remetentePotencial = new List<String>();

                        if (mailMessage.IsBodyHtml)
                        {
                            MatchCollection mCollection = Regex.Matches(mailMessage.Body.ToString(), @"(?i)(?:De|From):.*?(\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,6}\b)");

                            for (int x = 0; x < mCollection.Count; x++)
                            {
                                remetentePotencial.Add(mCollection[x].Groups[1].Value.ToLower());
                            }
                        }

                        // Guardar informações dos anexos
                        List<Anexo> informacaoAnexos = new List<Anexo>();

                        string path = FileManager.GetCaminho(CaminhoPara.AnexosProcessando);

                        foreach (System.Net.Mail.Attachment attachment in mailMessage.Attachments)
                        {
                            if (attachment.ContentType.ToString().ToLower().Contains("pdf"))
                            {
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }

                                string fileName = Path.GetFileNameWithoutExtension(attachment.Name) + "_P" + DateTime.Now.ToString("ddMMyyyy-hhmmssfff") + Path.GetExtension(attachment.Name);
                                string filePath = Path.Combine(path, fileName);

                                using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                                {
                                    byte[] allBytes = new byte[attachment.ContentStream.Length];
                                    int bytesRead = attachment.ContentStream.Read(allBytes, 0, (int)attachment.ContentStream.Length);

                                    fileStream.Write(allBytes, 0, bytesRead);
                                    fileStream.Flush();
                                }

                                informacaoAnexos.Add(new Anexo { CaminhoArquivo = filePath, NomeArquivo = Path.GetFileName(filePath) });

                                Log.SaveTxt("EmailExchange.LoadEmails", "Anexo armazenado: " + filePath, Log.LogType.Processo);
                            }
                        }

                        emails.Add(new EmailData()
                        {
                            Anexos = informacaoAnexos,
                            Assunto = message.Headers.Subject,
                            Corpo = mailMessage.Body,
                            Data = message.Headers.DateSent.ToLocalTime(),
                            IdEnderecoEmail = Info.Id,
                            Remetente = mailMessage.From.Address.ToString(),
                            RemetentesPotenciais = remetentePotencial
                        });
                    }
                }
                else
                {
                    Log.SaveTxt("EmailPop.LoadEmails", "Não foi possível conectar ao pop: o endereço especificado está incorreto", Log.LogType.Erro);
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("EmailPop.LoadEmails", e.Message, Log.LogType.Erro);
            }

            return emails;
        }

        /// <summary>
        /// Fetch Messages from Pop3Client
        /// </summary>
        /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
        /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
        /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
        /// <param name="username">Username of the user on the server</param>
        /// <param name="password">Password of the user on the server</param>
        /// <returns>All Messages on the POP3 server</returns>
        public List<Message> FetchMessagesByDateTime(string hostname, int port, bool useSsl, string username, string password, DateTime date)
        {
            // The client disconnects from the server when being disposed
            using (Pop3Client client = new Pop3Client())
            {
                // Connect to the server
                client.Connect(hostname, port, useSsl);

                // Authenticate ourselves towards the server
                client.Authenticate(username, password);

                int messageCount = 0;

                // Get the number of messages in the inbox
                messageCount = client.GetMessageCount();

                List<Message> fetchedMessages = new List<Message>(messageCount);

                // Messages are numbered in the interval: [1, messageCount]
                // Most servers give the latest message the highest number
                for (int i = messageCount; i > 0; i--)
                {
                    var message = client.GetMessage(i);

                    // Atualizar horário do último Request
                    if (i == messageCount)
                    {
                        LastRequestStartedOn = message.Headers.DateSent.ToLocalTime();
                    }

                    if (DateTime.Compare(message.Headers.DateSent.ToLocalTime(), date) > 0)
                    {
                        fetchedMessages.Add(message);
                    }
                    else
                    {
                        break;
                    }
                }

                // Now return the fetched messages
                return fetchedMessages;
            }
        }

        private static object _lockBecauseOfLastRequestDateTime = new object();

        public DateTime LastRequestStartedOn
        {
            get;
            set;
        }

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
    }
}
