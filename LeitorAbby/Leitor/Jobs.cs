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

            for (int i = tasksList.Count - 1; i > -1; i--)
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
                ReadFolderTreeOutputOCR(FileManager.CaminhoOCR_Output, false);
            }
        }

        private static void ReadFolderTreeOutputOCR(string directory, bool recursive)
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
                if (recursive)
                    ReadFolderTreeOutputOCR(dir, recursive);
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
                            SELECT EDA.EDA_ID, EDA.EDA_ASSUNTO, EDA.EDA_LOCAL_LOTE, EDA.EMA_ID
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
                            email.IdEnderecoEmail = dataReader["EMA_ID"] == DBNull.Value ? 0 : (int)dataReader["EMA_ID"];
                            email.CaminhoLote = dataReader["EDA_LOCAL_LOTE"] == DBNull.Value ? null : (string)dataReader["EDA_LOCAL_LOTE"];
                            if (!string.IsNullOrEmpty(Path.GetFileName(email.CaminhoLote)))
                                email.CaminhoLote = email.CaminhoLote.Replace(Path.GetFileName(email.CaminhoLote), string.Empty);
                            
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
}
