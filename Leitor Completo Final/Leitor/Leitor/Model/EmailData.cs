using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Model
{
    public class EmailData
    {
        public EmailData()
        {
            Anexos = new List<Anexo>();
            RemetentesPotenciais = new List<string>();
        }

        public Int32 Id 
        { 
            get; 
            set; 
        }

        public String Remetente 
        { 
            get; 
            set; 
        }

        public List<String> RemetentesPotenciais 
        { 
            get; 
            set; 
        }

        public String Assunto 
        { 
            get; 
            set; 
        }

        public String Corpo 
        { 
            get; 
            set; 
        }

        public String Prefeitura 
        { 
            get; 
            set; 
        }

        public List<Anexo> Anexos 
        { 
            get; 
            set; 
        }

        public DateTime Data 
        { 
            get; 
            set; 
        }

        public String CaminhoLote 
        { 
            get; 
            set; 
        }

        public Int32 IdEnderecoEmail 
        { 
            get; 
            set; 
        }
    }

    public class Anexo
    {
        public string NomeArquivo 
        { 
            get; 
            set; 
        }

        public string CaminhoArquivo 
        { 
            get; 
            set; 
        }
    }
}
