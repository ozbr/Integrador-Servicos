using System.ServiceProcess;
using Leitor;
using Leitor.Email;
using System.Threading;
using System.Collections.Generic;
using System;

namespace LeitorService
{
    partial class LeitorNFeService : ServiceBase
    {
        private static Timer listenEmailTaskTimer;
        private static Timer listenReadDocumentTaskTimer;

        public LeitorNFeService()
        {
            EventLog.Log = "Application";
            ServiceName = "LeitorNFe";
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            bool ok = CheckUp.Start();

            if (ok)
            {
                Thread threadResume = new Thread(CheckUp.Retomar);

                EmailManager manager = new EmailManager();
                List<IEmailLoader> emailList = manager.GetPostalBoxes();

                TimerCallback callbackListenEmailTask = new TimerCallback(Jobs.ListenEmailTask);
                listenEmailTaskTimer = new Timer(callbackListenEmailTask, emailList, TimeSpan.Zero, TimeSpan.FromSeconds(30.0));

                TimerCallback callbackListenReadDocumentTask = new TimerCallback(Jobs.ListenReadDocumentTask);
                listenReadDocumentTaskTimer = new Timer(callbackListenReadDocumentTask, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

                Console.Read();
            }
        }

        protected override void OnStop()
        {
            listenEmailTaskTimer.Change(Timeout.Infinite, Timeout.Infinite);
            listenReadDocumentTaskTimer.Change(Timeout.Infinite, Timeout.Infinite);

            base.OnStop();
        }
    }
}
