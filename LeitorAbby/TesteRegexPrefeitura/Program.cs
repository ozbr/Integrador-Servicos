using Leitor.Document;
using Leitor.Model;
using Leitor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteRegexPrefeitura
{
    class Program
    {
        static void Main(string[] args)
        {
            //string outro = ConversorPdf.ExtrairTexto(@"C:\Leitor\ARACATUBA\16-07-2013 12-14-25\anexos\Nota Fisc..[1].pdf AVF7977_E22082013-041230712.pdf");
            string texto1 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\ribeiraopreto558.pdf");
            string texto2 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\paulinia428.pdf");
            string texto3 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\paulinia343.pdf");
            string texto4 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza16730.pdf");
            string texto5 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza14157.pdf");
            string texto6 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza9977.pdf");
            string texto7 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza9763.pdf");
            string texto8 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza9078.pdf");
            string texto9 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza9075.pdf");
            string texto10 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza9035.pdf");
            string texto11 = PdfToText.ExtrairTextoDoPdf(@"C:\NOTAS\FORMATO1\fortaleza383.pdf");
        }
    }
}
