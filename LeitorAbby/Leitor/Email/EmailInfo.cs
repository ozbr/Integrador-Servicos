using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Email
{
    public class EmailInfo
    {
        public int Id 
        { 
            get; 
            set; 
        }

        public String Url 
        { 
            get; 
            set; 
        }

        public String EmailAddress 
        { 
            get; 
            set; 
        }

        public String Password 
        { 
            get; 
            set; 
        }

        public bool UseSSL
        {
            get;
            set;
        }

        public String Domain 
        { 
            get; 
            set; 
        }

        public String Provider 
        { 
            get; 
            set; 
        }

        public DateTime LastReceipt 
        { 
            get; 
            set; 
        }

        public string ConsumoServicoURL
        {
            get;
            set;
        }
    }
}
