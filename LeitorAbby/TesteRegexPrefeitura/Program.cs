using iTextSharp.text;
using iTextSharp.text.pdf;
using Leitor;
using Leitor.Dao;
using Leitor.Document;
using Leitor.Email;
using Leitor.Model;
using Leitor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TesteRegexPrefeitura
{
    class Program
    {
        public static Timer listenEmailTaskTimer;
        public static Timer listenReadDocumentTaskTimer;

        static void Main(string[] args)
        {

            //string local = @"C:\temp\B_11112013-101413186.xml";

            //var document = new DocumentXml
            //{
            //    Arquivo = System.IO.File.ReadAllText(local),
            //    Local = local,
            //    Prefeitura = new Prefeitura { Nome = "CURITIBA" }
            //};           
            //var teste = document.Read();

            Log.SaveTxt("Start", Log.LogType.Debug);
            bool ok = CheckUp.Start();

            if (ok)
            {
                EmailManager manager = new EmailManager();
                List<IEmailLoader> emailList = new List<IEmailLoader>();

                if (System.Configuration.ConfigurationManager.AppSettings["ExecuteOnce"] == "true")
                {
                    Jobs.ListenEmailTask(emailList);
                    Jobs.ListenReadDocumentTask(null);
                    Jobs.ListenSendDocumentTask(null);
                }
                else
                {
                    TimerCallback callbackListenEmailTask = new TimerCallback(Jobs.ListenEmailTask);
                    listenEmailTaskTimer = new Timer(callbackListenEmailTask, emailList, TimeSpan.Zero, TimeSpan.FromSeconds(60.0));

                    TimerCallback callbackListenReadDocumentTask = new TimerCallback(Jobs.ListenReadDocumentTask);
                    listenReadDocumentTaskTimer = new Timer(callbackListenReadDocumentTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(30.0));

                    //TimerCallback callbackListenSendDocumentTask = new TimerCallback(Jobs.ListenSendDocumentTask);
                    //listenReadDocumentTaskTimer = new Timer(callbackListenReadDocumentTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(60.0));

                }
                Console.Read();
            }


            //string outro = ConversorPdf.ExtrairTexto(@"C:\Leitor\ARACATUBA\16-07-2013 12-14-25\anexos\Nota Fisc..[1].pdf AVF7977_E22082013-041230712.pdf");
            //string texto1 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\ribeiraopreto558.pdf");
            //string texto2 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\paulinia428.pdf");
            //string texto3 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\paulinia343.pdf");
            //string texto4 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza16730.pdf");
            //string texto5 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza14157.pdf");
            //string texto6 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza9977.pdf");
            //string texto7 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza9763.pdf");
            //string texto8 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza9078.pdf");
            //string texto9 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza9075.pdf");
            //string texto10 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza9035.pdf");
            //string texto11 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza383.pdf");
        }
    }
}
