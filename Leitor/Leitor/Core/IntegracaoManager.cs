using Leitor.Dao;
using Leitor.Model;
using Leitor.ServiceReference1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Core
{
    public class IntegracaoManager
    {


        public static void EnviarParaWebService(String path, int remetenteId)
        {
            byte[] fileByteArray = GetBytesFromFile(path);
            String fileName = path.Split('\\')[path.Split('\\').Length - 1];
            UploadFileClient uf = new UploadFileClient();
            Console.WriteLine("Chamando WebService...");
            String result = uf.UploadZipFile(fileByteArray, fileName);
            Console.WriteLine("Chamado, reterno: " + result);
            //new ArquivoDAO().InserirArquivo(remetenteId, fileName, path, result);
            new ArquivoDAO().AtualizarStatusArquivo(remetenteId, path, result);
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
