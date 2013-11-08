using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Model
{
    public class Prefeitura
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public string RgxLink { get; set; }
        public string RgxLinkSecundario { get; set; }
        public string RgxLinkFormat { get; set; }
        public bool UseOCR { get; set; }
    }
}
