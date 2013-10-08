using System.Configuration;
using System.Data;
using System.Diagnostics;
using Leitor.Dao;
using Leitor.Document;
using Leitor.Email;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Leitor.Utilities;

namespace Leitor
{
    public class Launcher : BaseAdoDAO
    {
        public bool Run { get; set; }
        
        public int SleepTime
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["SleepTime"]) > 30
                           ? (Convert.ToInt32(ConfigurationManager.AppSettings["SleepTime"])) * 1000
                           : 30000;
            }
        }

        public Launcher()
        {
            _consoleHandler = ConsoleEventHandler;
            SetConsoleCtrlHandler(_consoleHandler, true);
        }

        public void LauchThreads()
        {
            ThreadStart ts = Launch;
            Thread t = new Thread(ts) {IsBackground = true};
            t.Start();
        }

        private void Launch()
        {
            Stopwatch timerMeasurer = new Stopwatch();

            while (MustRun())
            {
                bool funciona = false;
                Console.Write(".");
                //ALTERADO 24/07: testa a conexão com o banco.
                try
                {
                    _conn.Open();
                    funciona = true;
                }
                catch(Exception e)
                {
                    Log.SaveTxt("Laucher", "Não foi possível conectar com o banco de dados: " + e.Message, Log.LogType.Erro);
                }
                finally
                {
                    if(_conn.State==ConnectionState.Open)
                    {
                        _conn.Close();
                    }
                }
                if (funciona)
                {
                    //timerMeasurer.Start();
                    ////new EmailManager().Load(); // Guarda todos os e-mails localmente e salva em EMAIL_DATA o caminho
                    //timerMeasurer.Stop();
                    //Console.WriteLine("Tempo Gasto PRIMEIRA Etapa: " + timerMeasurer.Elapsed);

                    //timerMeasurer.Reset();
                    //timerMeasurer.Start();
                    //EmailDataManager.LoadPrefeituras(); // Carrega todos os e-mails que foram salvos localmente e tem Status 2 (Processando) no EMAIL_DATA
                    //timerMeasurer.Stop();
                    //Console.WriteLine("Tempo Gasto SEGUNDA Etapa: " + timerMeasurer.Elapsed);

                    //timerMeasurer.Reset();

                    timerMeasurer.Start();
                    DocumentManager.Load(); // Lê os documentos que estão marcados para serem lidos
                    timerMeasurer.Stop();
                    Console.WriteLine("Tempo Gasto TECEIRA Etapa: " + timerMeasurer.Elapsed);

                }

                Thread.Sleep(SleepTime);
            }
        }

        private bool MustRun()
        {
            return Run;
        }

        enum ConsoleCtrlHandlerCode : uint
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        delegate bool ConsoleCtrlHandlerDelegate(ConsoleCtrlHandlerCode eventCode);
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandlerDelegate handlerProc, bool add);
        static ConsoleCtrlHandlerDelegate _consoleHandler;

        bool ConsoleEventHandler(ConsoleCtrlHandlerCode eventCode)
        {
            switch (eventCode)
            {
                case ConsoleCtrlHandlerCode.CTRL_CLOSE_EVENT:
                case ConsoleCtrlHandlerCode.CTRL_BREAK_EVENT:
                case ConsoleCtrlHandlerCode.CTRL_LOGOFF_EVENT:
                case ConsoleCtrlHandlerCode.CTRL_SHUTDOWN_EVENT:
                    Run = false;
                    Environment.Exit(0);
                    break;
            }

            return (false);
        }
    }
}