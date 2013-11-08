using Leitor.Dao;
using Leitor.Model;
using Leitor.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Document
{
    public class DocumentManager
    {
        public static void Load()
        {
            EmailDataDAO dao = new EmailDataDAO();

            List<object[]> docLoaderInfo = new List<object[]>();
            docLoaderInfo = dao.SelectEmailData();

            foreach (object[] info in docLoaderInfo)
            {
                EmailData email = (EmailData)info[0];
                Prefeitura p = (Prefeitura)info[1];
                
                new DocumentLoader().Load(p, email);
            }
        }
    }
}
