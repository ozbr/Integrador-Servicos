using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Core
{
    public static class Repository
    {
        public static string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
    }
}
