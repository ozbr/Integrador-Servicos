using System.ServiceProcess;

namespace LeitorService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] servicesToRun = new ServiceBase[] 
                { 
                    new LeitorNFeService()  
                };

            ServiceBase.Run(servicesToRun);
        }
    }
}
