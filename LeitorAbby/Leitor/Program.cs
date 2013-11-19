//using System.IO;
//using Leitor.Dao;
//using System;
//using Leitor.Utilities;
//using System.Threading;
//using Leitor.Tools;
//using System.Threading.Tasks;
//using Leitor.Email;
//using System.Collections.Generic;
//using System.Diagnostics;
//using Leitor.Document;
//using System.Data.SqlClient;
//using Leitor.Core;
//using Leitor.Model;

//namespace Leitor
//{
//    internal class Program
//    {
//        public static Timer listenEmailTaskTimer;
//        public static Timer listenReadDocumentTaskTimer;

//        private static void Main()
//        {
//            Log.SaveTxt("Start", Log.LogType.Debug);
//            bool ok = true;// CheckUp.Start();

//            if (ok)
//            {
//                EmailManager manager = new EmailManager();
//                List<IEmailLoader> emailList = new List<IEmailLoader>();

//                if (System.Configuration.ConfigurationManager.AppSettings["ExecuteOnce"] == "true")
//                {
//                    Jobs.ListenEmailTask(emailList);
//                    Jobs.ListenReadDocumentTask(null);
//                }
//                else
//                {
//                    TimerCallback callbackListenEmailTask = new TimerCallback(Jobs.ListenEmailTask);
//                    listenEmailTaskTimer = new Timer(callbackListenEmailTask, emailList, TimeSpan.Zero, TimeSpan.FromSeconds(60.0));

//                    TimerCallback callbackListenReadDocumentTask = new TimerCallback(Jobs.ListenReadDocumentTask);
//                    listenReadDocumentTaskTimer = new Timer(callbackListenReadDocumentTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(30.0));

//                    //TimerCallback callbackListenSendDocumentTask = new TimerCallback(Jobs.ListenReadDocumentTask);
//                    //listenReadDocumentTaskTimer = new Timer(callbackListenReadDocumentTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(60.0));

//                }
//                Console.Read();
//            }

//        }
//    }
  

    
//}
