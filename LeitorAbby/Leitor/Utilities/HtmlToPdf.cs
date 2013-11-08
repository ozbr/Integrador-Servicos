using iTextSharp.text.pdf;
using Leitor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Utilities
{
    public class HtmlToPdf
    {
        public static string Convert(string local, string newLocal, Prefeitura p) 
        {
            string result = string.Empty;

            FileStream fsHTMLDocument = new FileStream(local, FileMode.Open, FileAccess.Read);
            StreamReader srHTMLDocument = new StreamReader(fsHTMLDocument);
            string html = srHTMLDocument.ReadToEnd();
            srHTMLDocument.Close();

            html = html.Replace("\r\n", "");
            html = html.Replace("\0", "");

            HTMLToPdf(html, newLocal);

            return newLocal;
        }

        public static void HTMLToPdf(string HTML, string FilePath)
        {
            iTextSharp.text.Document document = new iTextSharp.text.Document();

            PdfWriter.GetInstance(document, new FileStream(FilePath, FileMode.Create));
            document.Open();
            iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
            hw.Parse(new StringReader(HTML));
            document.Dispose();

        }

    }
}
