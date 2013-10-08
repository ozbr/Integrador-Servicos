using Leitor.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Utilities
{
    public class Log
    {
        public static void SaveTxt(String classe, String mensagem, int remetenteId)
        {
            LogDAO.SalvarLog(classe, mensagem, remetenteId);
        }

        public static void SaveTxt(String classe, String mensagem)
        {
            LogDAO.SalvarLog(classe, mensagem, 0);
        }

        public static void SaveTxt(String classe, String mensagem, LogType tipo)
        {
            LogDAO.SalvarLog(classe, mensagem, tipo);
        }

        public enum LogType
        {
            Erro,
            Processo,
            Debug
        }
    }
}
