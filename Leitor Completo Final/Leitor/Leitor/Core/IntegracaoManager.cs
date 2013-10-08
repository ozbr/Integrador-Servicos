using Leitor.Dao;
using Leitor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leitor.ServiceReference1;
using Leitor.Utilities;
using System.Threading;

namespace Leitor.Core
{
    public class IntegracaoManager
    {
        public static void EnviarParaWebService(String path, EmailData email)
        {
            EmailDataDAO dao = new EmailDataDAO();
            ConsumoArquivosClient consumo = new ConsumoArquivosClient();

            object[] state = new object[3];
            state[0] = path;
            state[1] = email;
            state[2] = consumo;

            byte[] fileByteArray = GetBytesFromFile(path);
            String filename = Path.GetFileName(path);

            dao.AtualizarEmailData(email, (int)Helper.FlowStatus.Sending);

            Log.SaveTxt("IntegracaoManager", "Enviando Nota: " + path, Log.LogType.Processo);
            Console.WriteLine("ENVIANDO NOTA: " + path);

            AsyncCallback callback = new AsyncCallback(HandleCallback);
            consumo.BeginUploadZipFile(fileByteArray, filename, callback, state);
        }

        private static void HandleCallback(IAsyncResult result)
        {
            string path = (string)((object[])result.AsyncState)[0];
            EmailData email = (EmailData)((object[])result.AsyncState)[1];
            ConsumoArquivosClient consumo = (ConsumoArquivosClient)((object[])result.AsyncState)[2];

            try
            {
                EmailDataDAO dao = new EmailDataDAO();
                string resposta = consumo.EndUploadZipFile(result);

                dao.AtualizarEnvio(email, path, resposta, (int)Helper.FlowStatus.Sent);
                
                Log.SaveTxt("IntegracaoManager", "Retorno: " + result, Log.LogType.Processo);
                Console.WriteLine("RETORNO: " + resposta);
            }
            catch (Exception ex)
            {
                Log.SaveTxt("Falha ao enviar nota. " + ex.Message, "Nota: " + path, Log.LogType.Erro);
            }
        }

        public static byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)

            FileStream fs = File.OpenRead(fullFilePath);
            try
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                return bytes;
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
