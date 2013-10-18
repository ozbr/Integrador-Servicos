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
        private static LogType _cacheLogsHabilitados;
        private static bool _logCached;

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
            if (LogsHabilitado(tipo))
            {
                LogDAO.SalvarLog(classe, mensagem, tipo);
            }
        }
        public static void SaveTxt(String mensagem, LogType tipo)
        {
            if (LogsHabilitado(tipo))
            {
                var method = new System.Diagnostics.StackFrame(1).GetMethod();
                string classe = string.Join(".", method.DeclaringType.FullName, method.Name);
                LogDAO.SalvarLog(classe, mensagem, tipo);
            }
        }

        private static bool LogsHabilitado(LogType tipo)
        {
            if (!_logCached)
            {
                int habilitados;
                if (Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["LogsHabilitados"], out habilitados))
                    _cacheLogsHabilitados = (LogType)habilitados;
                else
                    _cacheLogsHabilitados = LogType.Todos;

                _logCached = true;
            }

            return _cacheLogsHabilitados.HasFlag(tipo);
        }

        [Flags]
        public enum LogType
        {
            Nenhum = 0,
            Erro = 1,
            Processo = 2,
            Debug = 4,
            Todos = Erro | Processo| Debug
        }
    }
}
