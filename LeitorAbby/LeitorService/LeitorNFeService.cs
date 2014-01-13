using System.ServiceProcess;
using Leitor;
using Leitor.Email;
using System.Threading;
using System.Collections.Generic;
using System;
using Leitor.Utilities;

namespace LeitorService
{
    partial class LeitorNFeService : ServiceBase
    {
        private static Timer listenEmailTaskTimer;
        private static Timer listenReadDocumentTaskTimer;
        private static Timer listenSendDocumentTaskTimer;

        public LeitorNFeService()
        {
            EventLog.Log = "Application";
            ServiceName = "LeitorNFe";
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            try
            {
                Log.SaveTxt("Starting", Log.LogType.Debug);

                bool ok = CheckUp.Start();
                Jobs.ListenSendDocumentTask(null);

                if (ok)
                {
                    //Verificar a ausência do método Retomar
                    //Thread threadResume = new Thread(CheckUp.Retomar);   

                    EmailManager manager = new EmailManager();
                    List<IEmailLoader> emailList = manager.GetPostalBoxes();

                    TimerCallback callbackListenEmailTask = new TimerCallback(Jobs.ListenEmailTask);
                    listenEmailTaskTimer = new Timer(callbackListenEmailTask, emailList, TimeSpan.Zero, TimeSpan.FromSeconds(30.0));

                    TimerCallback callbackListenReadDocumentTask = new TimerCallback(Jobs.ListenReadDocumentTask);
                    listenReadDocumentTaskTimer = new Timer(callbackListenReadDocumentTask, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

                    TimerCallback callbackListenSendDocumentTask = new TimerCallback(Jobs.ListenSendDocumentTask);
                    listenSendDocumentTaskTimer = new Timer(callbackListenSendDocumentTask, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(30));

                    Log.SaveTxt("Started", Log.LogType.Debug);
                }
                else
                {
                    Log.SaveTxt("Serviço não iniciou com sucesso e será interrompido", Log.LogType.Debug);
                    base.Stop();
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("OnStart", e.Message, Log.LogType.Erro);
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (listenEmailTaskTimer != null)
                    listenEmailTaskTimer.Change(Timeout.Infinite, Timeout.Infinite);
                if (listenReadDocumentTaskTimer != null)
                    listenReadDocumentTaskTimer.Change(Timeout.Infinite, Timeout.Infinite);
                if(listenSendDocumentTaskTimer != null)
                    listenSendDocumentTaskTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            catch (Exception e)
            {
                Log.SaveTxt("OnStop", e.Message + e.StackTrace, Log.LogType.Erro);
            }
            base.OnStop();
        }
    }
}
