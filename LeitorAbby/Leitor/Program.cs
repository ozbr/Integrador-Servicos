using System.IO;
using Leitor.Dao;
using System;
using Leitor.Utilities;
using System.Threading;
using Leitor.Tools;
using System.Threading.Tasks;
using Leitor.Email;
using System.Collections.Generic;
using System.Diagnostics;
using Leitor.Document;
using System.Data.SqlClient;
using Leitor.Core;
using Leitor.Model;

namespace Leitor
{
    internal class Program
    {
        public static Timer listenEmailTaskTimer;
        public static Timer listenReadDocumentTaskTimer;

        private static void Main()
        {
            Log.SaveTxt("Start", Log.LogType.Debug);
            bool ok = true;// CheckUp.Start();

            if (ok)
            {
                EmailManager manager = new EmailManager();
                List<IEmailLoader> emailList = new List<IEmailLoader>();

                if (System.Configuration.ConfigurationManager.AppSettings["ExecuteOnce"] == "true")
                {
                    Jobs.ListenEmailTask(emailList);
                    Jobs.ListenReadDocumentTask(null);
                }
                else
                {
                    TimerCallback callbackListenEmailTask = new TimerCallback(Jobs.ListenEmailTask);
                    listenEmailTaskTimer = new Timer(callbackListenEmailTask, emailList, TimeSpan.Zero, TimeSpan.FromSeconds(60.0));

                    TimerCallback callbackListenReadDocumentTask = new TimerCallback(Jobs.ListenReadDocumentTask);
                    listenReadDocumentTaskTimer = new Timer(callbackListenReadDocumentTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(30.0));

                    //TimerCallback callbackListenSendDocumentTask = new TimerCallback(Jobs.ListenReadDocumentTask);
                    //listenReadDocumentTaskTimer = new Timer(callbackListenReadDocumentTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(60.0));

                }
                Console.Read();
            }

            #region Antigo
            //Launcher l = new Launcher();
            //l.Run = true;
            //l.LauchThreads();

            //while (true)
            //{
            //    String s = Console.ReadLine();
            //    if (s.Equals("-end"))
            //    {
            //        l.Run = false;
            //    }
            //}
            #endregion
        }
    }

    public static class Jobs
    {
        private static EmailManager manager = new EmailManager();

        private static LimitedConcurrencyLevelTaskScheduler lcts1 = new LimitedConcurrencyLevelTaskScheduler(2);
        private static LimitedConcurrencyLevelTaskScheduler lcts2 = new LimitedConcurrencyLevelTaskScheduler(8);

        public static void ListenEmailTask(object state)
        {
            List<IEmailLoader> emailList = (List<IEmailLoader>)state;
            manager.UpdatePostalBoxes(ref emailList);

            TaskFactory factory = new TaskFactory(lcts1);
            List<Task> tasksList = new List<Task>();

            for (int i = 0; i < emailList.Count; i++)
            {
                tasksList.Add(factory.StartNew((object loader) =>
                {
                    manager.Download((IEmailLoader)loader);
                }
                , emailList[i]));
            }

            Task.WaitAll(tasksList.ToArray());

            for (int i = tasksList.Count - 1; i > -1 ; i--)
            {
                tasksList[i].Dispose();
            }
        }

        public static void ListenReadDocumentTask(object state)
        {
            ReadOutputOCRFiles();
            
            EmailDataDAO dao = new EmailDataDAO();

            List<object[]> docLoaderInfo = new List<object[]>();
            docLoaderInfo = dao.SelectEmailData();

            TaskFactory factory = new TaskFactory(lcts2);
            List<Task> tasksList = new List<Task>();

            foreach (object[] info in docLoaderInfo)
            {
                EmailData e = (EmailData)info[0];
                Prefeitura p = (Prefeitura)info[1];

                tasksList.Add(factory.StartNew((object param) =>
                {
                    DocumentLoader docLoader = new DocumentLoader();
                    docLoader.Load((Prefeitura)((object[])param)[1], (EmailData)((object[])param)[0]);
                }
                , info));
            }

            Task.WaitAll(tasksList.ToArray());

            for (int i = tasksList.Count - 1; i > -1; i--)
            {
                tasksList[i].Dispose();
            }
        }

        private static void ReadOutputOCRFiles()
        {
            if (Directory.Exists(FileManager.CaminhoOCR_Output))
            {
                ReadFolderTreeOutputOCR(FileManager.CaminhoOCR_Output);
            }
        }

        private static void ReadFolderTreeOutputOCR(string directory)
        {
            foreach (var fileName in Directory.GetFiles(directory))
            {
                string controleORC = Path.GetFileNameWithoutExtension(fileName);
                ArquivoDAO dao = new ArquivoDAO();

                var anexo = dao.SelecionarArquivosControleOCR(controleORC);

                if (anexo != null)
                {
                    string newFile = anexo.CaminhoArquivo.Replace(Path.GetExtension(anexo.CaminhoArquivo), Path.GetExtension(fileName));

                    File.Move(fileName, newFile);

                    anexo.CaminhoArquivo = newFile;

                    dao.AtualizaAnexoOCRProcessado(anexo);
                }
            }
            foreach (var dir in Directory.GetDirectories(directory))
            {
                ReadFolderTreeOutputOCR(dir);
            }
        }

        public static void ListenSendDocumentTask(object state)
        {
            using (SqlConnection connection = new SqlConnection(Repository._connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = @"
                            SELECT EDA.EDA_ID, EDA.EDA_ASSUNTO, EDA.EDA_LOCAL_LOTE
	                        FROM EMAIL_DATA EDA
	                        WHERE EDA_STATUS = " + (int)Helper.FlowStatus.Processed + @"
	                        ORDER BY EDA_ID";

                    command.Connection.Open();

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            EmailData email = new EmailData();

                            email.Id = (int)dataReader["EDA_ID"];
                            email.Assunto = dataReader["EDA_ASSUNTO"] == DBNull.Value ? null : (string)dataReader["EDA_ASSUNTO"];
                            email.CaminhoLote = dataReader["EDA_LOCAL_LOTE"] == DBNull.Value ? null : (string)dataReader["EDA_LOCAL_LOTE"];

                            string[] dirs = Directory.GetFiles(email.CaminhoLote, "*.zip");

                            for (int i = 0; i < dirs.Length; i++)
                            {
                                IntegracaoManager.EnviarParaWebService(dirs[i], email);
                            }
                        }

                        dataReader.Close();
                    }

                    command.Connection.Close();
                }
            }
        }
        
    }

    public static class CheckUp
    {
        public static bool Start()
        {
            bool success = false;

            try
            {
                Log.SaveTxt("CheckUp.Start", "Entrando", Log.LogType.Debug);

                using (SqlConnection connection = new SqlConnection(Repository._connectionString))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText += @"UPDATE EMAIL_DATA SET EDA_STATUS = 1 WHERE EDA_STATUS = 2 OR EDA_STATUS = 3; ";
                        command.CommandText += @"UPDATE EMAIL_DATA SET EDA_STATUS = 4 WHERE EDA_STATUS = 5 OR EDA_STATUS = 6; ";

                        command.Connection.Open();
                        command.ExecuteNonQuery();
                        command.Connection.Close();

                        success = true;
                    }
                }

                Log.SaveTxt("CheckUp.Start", "Resultado " + success.ToString(), Log.LogType.Debug);
            }
            catch(Exception e)
            {
                Log.SaveTxt("Start", e.Message, Log.LogType.Erro);
                Console.WriteLine("Impossível iniciar.");
            }

            return success;
        }
    }
}
