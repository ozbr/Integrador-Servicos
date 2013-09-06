using Leitor.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Core
{
    public class Log
    {
        public static void Save(String classe, String mensagem, int remetenteId)
        {
            new LogDAO().InserirLog(classe, mensagem, remetenteId);
        }
        public static void Save(String classe, String mensagem)
        {
            new LogDAO().InserirLog(classe, mensagem, 0);
        }
    }
}
