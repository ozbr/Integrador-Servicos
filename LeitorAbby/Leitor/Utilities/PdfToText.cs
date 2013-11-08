using System;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

namespace Leitor.Utilities
{
    public class PdfToText
    {
        public static string ExtrairTextoDoPdf(string nomeArquivo)
        {
            string resultado = ConverterComPdfBox(nomeArquivo);
            return resultado;
        }

        private static string ConverterComPdfBox(string nomeArquivo)
        {
            try
            {
                //if (!Directory.Exists("C:\\Temp\\Erratas\\"))
                //{
                //    Directory.CreateDirectory("C:\\Temp\\Erratas\\");
                //}

                //if (!File.Exists("C:\\Temp\\Erratas\\" + nomeArquivo.Split('\\')[nomeArquivo.Split('\\').Length - 1]))
                //{
                //    FileInfo f = new FileInfo(nomeArquivo);
                //    f.CopyTo("C:\\Temp\\Erratas\\" + nomeArquivo.Split('\\')[nomeArquivo.Split('\\').Length - 1]);
                //}
                //PDDocument doc = PDDocument.load("C:\\Temp\\Erratas\\" + nomeArquivo.Split('\\')[nomeArquivo.Split('\\').Length - 1]);
                
                var doc = PDDocument.load(nomeArquivo);
                PDFTextStripper stripper = new PDFTextStripper();

                string texto = stripper.getText(doc);
                doc.close();
                return texto;
            }
            catch (Exception e)
            {
                Log.SaveTxt("PdfToText.ConverterComPdfBox", e.Message, Log.LogType.Erro);
            }
            return "";
        }
    }
}
