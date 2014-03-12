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
    public class EmailFolder : IEmailLoader
    {
        public List<Model.EmailData> LoadEmails(ReadEmailHandler readHandler)
        {
            List<EmailData> emails = new List<EmailData>();

            try
            {
                
                if (Directory.Exists(Info.Url))
                {
                    // Escolher o horário a partir do qual os arquivos serão baixados
                    DateTime dateTimeReceivedFilter = LastRequestStartedOn > RequestDate ? LastRequestStartedOn : RequestDate;

                    // Trazer mensagens a partir do horário escolhido
                    List<string> fetchedMessages =
                        FetchMessagesByDateTime(Info.Url, dateTimeReceivedFilter);

                    for (int i = fetchedMessages.Count - 1; i > -1 ; i--)
                    {
                        string folderfileName = fetchedMessages[i];

                        // Guardar possíveis remetentes no corpo do email (caso algum seja encaminhado)
                        List<String> remetentePotencial = new List<String>();
                        
                        // Guardar informações dos anexos
                        List<Anexo> informacaoAnexos = new List<Anexo>();

                        string path = FileManager.GetCaminho(CaminhoPara.AnexosProcessando);

                        
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        string attachmentFileName = Regex.Replace(Path.GetFileName(folderfileName), @"[^\w\-.\s]", "");
                        string fileName = Path.GetFileNameWithoutExtension(attachmentFileName) + "_P" + DateTime.Now.ToString("ddMMyyyy-hhmmssfff") + Path.GetExtension(attachmentFileName);
                        string filePath = Path.Combine(path, fileName);

                        File.Copy(folderfileName,filePath);

                        string originalFileName = string.Empty;
                        if (Directory.Exists(Path.Combine(Info.Url, "Original")))
                            originalFileName = Directory.GetFiles(Path.Combine(Info.Url, "Original"), Path.GetFileNameWithoutExtension(folderfileName) + ".*").FirstOrDefault<string>();

                        informacaoAnexos.Add(new Anexo { CaminhoArquivo = filePath, NomeArquivo = Path.GetFileName(filePath), CaminhoOriginalOCR = originalFileName });

                        Log.SaveTxt("EmailExchange.LoadEmails", "Anexo armazenado: " + filePath, Log.LogType.Processo);
                        

                        emails.Add(new EmailData()
                        {
                            Anexos = informacaoAnexos,
                            Assunto = "Carga de diretório",
                            Corpo = informacaoAnexos[0].CaminhoArquivo,
                            Data = DateTime.Now,
                            IdEnderecoEmail = Info.Id,
                            Remetente = Info.EmailAddress,
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
        /// <param name="urlPath">Hostname of the server. For example: pop3.live.com</param>
        /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
        /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
        /// <param name="username">Username of the user on the server</param>
        /// <param name="password">Password of the user on the server</param>
        /// <returns>All Messages on the POP3 server</returns>
        public List<string> FetchMessagesByDateTime(string urlPath, DateTime date)
        {

            //int messageCount = 0;

            // Get the number of messages in the inbox
            //messageCount = client.GetMessageCount();

            List<string> fetchedMessages = new List<string>();

            string pathProcessed = Path.Combine(urlPath,"Lidos");

            if (!Directory.Exists(pathProcessed))
                Directory.CreateDirectory(pathProcessed);

            // Messages are numbered in the interval: [1, messageCount]
            // Most servers give the latest message the highest number
            foreach(var filaName in Directory.GetFiles(urlPath))
            {
                
                // Atualizar horário do último Request
                string moveFilePath = Path.Combine(pathProcessed, Path.GetFileName(filaName));
                int ind = 0;

                while (File.Exists(moveFilePath))
                    moveFilePath = Path.Combine(pathProcessed, (ind++).ToString() + "_" + Path.GetFileName(filaName));

                File.Move(filaName, moveFilePath);

                fetchedMessages.Add(moveFilePath);

                //if (DateTime.Compare(message.Headers.DateSent.ToLocalTime(), date) > 0)
                //{
                //    fetchedMessages.Add(message);
                //}
                //else
                //{
                //    break;
                //}
            }

            // Now return the fetched messages
            return fetchedMessages;

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
