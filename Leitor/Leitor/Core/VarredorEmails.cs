using Leitor.Dao;
using Leitor.Model;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Leitor.Core
{
    public class VarredorEmails
    {
        public string VerificaEmail(DateTime requestedDate)
        {
            string emailUrl = string.Empty;

            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010);
            service.Credentials = new NetworkCredential("nfemonitor", "Senha@@", "dotinsight");
            //service.Credentials = new NetworkCredential("tabelas", "Tud@S&u$%", "dotinsight");

            service.Url = new Uri("http://mimzy.dotinsight.corp/EWS/Exchange.asmx");

            List<SearchFilter> terms = new List<SearchFilter>();
            //terms.Add(new SearchFilter.IsEqualTo(EmailMessageSchema.Sender, "tabela-preco@alcateia.com.br"));
            terms.Add(new SearchFilter.IsGreaterThanOrEqualTo(EmailMessageSchema.DateTimeReceived, requestedDate));

            SearchFilter searchTerms = new SearchFilter.SearchFilterCollection(LogicalOperator.And, terms);

            FindItemsResults<Item> findResults = service.FindItems(
               WellKnownFolderName.Inbox,
               searchTerms,
               new ItemView(200));

            if (findResults != null && findResults.Items != null && findResults.Items.Count > 0)
            {
                service.LoadPropertiesForItems(findResults, PropertySet.FirstClassProperties);

                foreach (Item item in findResults.Items)
                {
                    EmailMessage message = EmailMessage.Bind(service, item.Id, new PropertySet(BasePropertySet.IdOnly, ItemSchema.Attachments, ItemSchema.HasAttachments));

                    String remetente = String.Empty;
                    if (item.Body.BodyType.ToString().Equals("HTML"))
                    {
                        #region recolhe remetente
                        Regex rxEncaminhadas = new Regex("<br>De: .*?<a href.*?>(.*?)</a>", RegexOptions.Singleline);

                        if (rxEncaminhadas.Match(item.Body.ToString()) != null && !String.IsNullOrEmpty(rxEncaminhadas.Match(item.Body.ToString()).Groups[1].Value))
                        {
                            remetente = rxEncaminhadas.Match(item.Body.ToString()).Groups[1].Value;
                        }
                        else
                        {
                            remetente = ((Microsoft.Exchange.WebServices.Data.EmailAddress)item[EmailMessageSchema.From]).Address;
                        }
                        #endregion

                        if (new RemetenteDAO().SalvarCorpoEmail(remetente))
                        {
                            ArquivosManager.SalvarArquivo(remetente, item.Subject, ".html", item.Body.ToString());
                        }
                        ArquivosManager.SalvarEmail(remetente, item.Subject, ".html", item.Body.ToString());
                    }

                    foreach (Attachment attachment in message.Attachments)
                    {
                        if (attachment is FileAttachment)
                        {
                            FileAttachment fileAttachment = attachment as FileAttachment;

                            // Load the file attachment into memory and print out its file name.
                            fileAttachment.Load();

                            // Load attachment contents into a file. fileAttachment.Load("C:\\temp\\" + fileAttachment.Name); // Stream attachment contents into a file.
                            if (!Directory.Exists(String.Format(ArquivosManager.LocalArquivos, remetente)))
                            {
                                Directory.CreateDirectory(String.Format(ArquivosManager.LocalArquivos, remetente));
                            }

                            FileStream theStream = new FileStream(String.Format(ArquivosManager.LocalArquivos, remetente) + fileAttachment.Name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                            fileAttachment.Load(theStream);
                            theStream.Close();
                            theStream.Dispose();
                        }

                    }

                    #region atributos importantes
                    RemetenteDAO dao = new RemetenteDAO();
                    Remetente r = dao.SelecionarRemetentePorEmail(remetente);
                    if (r == null)
                    {
                        Remetente _r = new Remetente();
                        _r.Nome = remetente;
                        _r.Emails = remetente;
                        _r.Assuntos = item.Subject;
                        dao.InserirRemetente(_r);
                    }
                    else
                    {
                        LeitorArquivos.AtualizarPrefeitura(r, item.Body.ToString(), true);
                    }
                    //Console.WriteLine(item.Subject);
                    //Console.WriteLine(((Microsoft.Exchange.WebServices.Data.EmailAddress)item[EmailMessageSchema.From]).Address);
                    //Console.WriteLine(item.Body.ToString());
                    //Console.WriteLine(item.Body.BodyType.ToString());
                    #endregion


                }
            }
            else
            {
                Console.WriteLine("no items");
            }
            return emailUrl;
        }
    }
}
