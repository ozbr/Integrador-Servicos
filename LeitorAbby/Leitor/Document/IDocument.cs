using Leitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Document
{
    public interface IDocument
    {
        String Arquivo { get; set; }
        String Local { get; set; }
        List<RegexModel> Parser { get; set; }
        Prefeitura Prefeitura { get; set; }
        NF Read();
    }
}
