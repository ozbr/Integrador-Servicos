using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Model
{
    public class TpAnexo
    {
        public int Id { get; set; }
        public string Prefeitura { get; set; }
        public string Extensao { get; set; }
        public bool UseOCR { get; set; }
    }
}
