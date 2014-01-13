using EO.Pdf;
using Ionic.Zip;
using Leitor.Document;
using Leitor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Utilities
{
    public class LotManager
    {
        private static string _LocalLotes = "C:\\Lotes\\{0}\\{1}\\";

        private static String BaseLocal(String numeroLote, String cnpj, String tipo, String numero)
        {
            string result = String.Empty;
            if (numeroLote.Substring(0, 1).Equals("0"))
            {
                numeroLote = numeroLote.Substring(1);
            }
            try
            {
                result = numeroLote + "_" + cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "") +
                         "_" + (String.IsNullOrEmpty(tipo) ? "E" : tipo.ToUpper()) + "_" + numero;
            }catch(Exception e)
            {
                Log.SaveTxt("LotManager.BaseLote", e.Message, Log.LogType.Erro);
                result = numeroLote + "_" + "Erro";
            }
            return result;
        }

        public static String Lote(String numeroLote, String cnpj, String qtdArquivos)
        {
            string result = String.Empty;
            if (numeroLote.Substring(0, 1).Equals("0"))
            {
                numeroLote = numeroLote.Substring(1);
            }
            try
            {
                result =  numeroLote + "_" + cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "") + "_" +
                       qtdArquivos;
            }catch(Exception e)
            {
                Log.SaveTxt("LotManager.Lote", e.Message, Log.LogType.Erro);
                result = numeroLote + "_" + "Erro";
            }
            return result;
        }

        public static object lockObject = new object();

        public static String CreateLot(Prefeitura p, EmailData e, string caminhoArquivo, NF nf, String EouS)
        {
            String local = Path.GetDirectoryName(e.CaminhoLote);
            if (!Directory.Exists(local))
            {
                Directory.CreateDirectory(local);
            }
            string cnpj = string.IsNullOrEmpty(nf.infNFe.emit.CNPJ) ? new string('0',14) : nf.infNFe.emit.CNPJ;

            //ALTERADO PELA PRESENÇA DE MAIS DE UMA NF POR EMAIL
            String numeroLote = e.Data.ToString("MMddhh") + DateTime.Now.ToString("ffff");
            String nomeZip = Lote(numeroLote, cnpj, "02") + ".zip";
            String nomeArquivo = BaseLocal(numeroLote, cnpj, Util.validateEouS(EouS), "001") + ".xml";
            String nomePdf = BaseLocal(numeroLote, cnpj, Util.validateEouS(EouS), "001");

            System.Xml.Serialization.XmlSerializer serializadorXml = new System.Xml.Serialization.XmlSerializer(nf.GetType());

            FileStream fs = File.Create(e.CaminhoLote + nomeArquivo);
            serializadorXml.Serialize(fs, nf);
            fs.Dispose();

            String novoNome = string.Empty;
            String arquivoSalvo = caminhoArquivo;                

            if (!String.IsNullOrEmpty(arquivoSalvo))
            {
                FileInfo f = new FileInfo(arquivoSalvo);
                novoNome = e.CaminhoLote + nomePdf + ".pdf";

                if (f.Extension.Contains("pdf"))
                {
                    if (!File.Exists(novoNome))
                        f.CopyTo(novoNome);
                }
                else
                {
                    lock(lockObject)
                    {
                        if (!ConversorPdf.ConvertImageToPDF(arquivoSalvo, novoNome))
                            HtmlToPdf.ConvertUrl(arquivoSalvo, novoNome);
                    }
                }

            }

            if (!File.Exists(e.CaminhoLote + nomeZip))
            {
                using (ZipFile zip = new ZipFile(e.CaminhoLote + nomeZip))
                {
                    zip.AddFile(e.CaminhoLote + nomeArquivo, "/");
                    zip.AddFile(novoNome, "/");
                    zip.Save();
                    zip.Dispose();
                }
            }

            return e.CaminhoLote + nomeZip;
        }

        /*
         private String GerarXml(String numeroLote)
        {

            //String lote = DateTime.Now.ToString("ddMMyyyy") + numeroLote + "_" + nf.infNFe.emit.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "") + "_" + "02";
            //String localZip = String.Format("C:\\Temp\\Lotes\\" + lote + ".zip", _remetente.Emails);
            String lote = ArquivosManager.Lote(numeroLote, nf.infNFe.emit.CNPJ, "02");
            String localZip = ArquivosManager.LocalZip(lote, _remetente.Emails);


            if (!Directory.Exists("C:\\Temp\\Lotes\\"))
            {
                Directory.CreateDirectory("C:\\Temp\\Lotes\\");
            }

            if (!File.Exists(localZip))
            {
                //String nomeArquivo = DateTime.Now.ToString("ddMMyyyy") + numeroLote + "_" + nf.infNFe.emit.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "") + "_" + (String.IsNullOrEmpty(EouS) ? "E" : EouS.ToUpper()) + "_" + "001";
                //String nomePdf = DateTime.Now.ToString("ddMMyyyy") + numeroLote + "_" + nf.infNFe.emit.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "") + "_" + (String.IsNullOrEmpty(EouS) ? "E" : EouS.ToUpper()) + "_" + "002";

                String nomeArquivo = ArquivosManager.BaseLocal(numeroLote, nf.infNFe.emit.CNPJ, Util.validateEouS(EouS), "001");
                String nomePdf = ArquivosManager.BaseLocal(numeroLote, nf.infNFe.emit.CNPJ, Util.validateEouS(EouS), "002");

                System.Xml.Serialization.XmlSerializer serializadorXml = new System.Xml.Serialization.XmlSerializer(nf.GetType());

                FileStream fs = File.Create(String.Format(ArquivosManager.LocalXml + nomeArquivo + ".xml", _remetente.Emails));
                serializadorXml.Serialize(fs, nf);
                fs.Dispose();

                FileInfo f = null;

                if (Directory.Exists(String.Format(ArquivosManager.Local, _remetente)))
                {
                    DirectoryInfo di = new DirectoryInfo(String.Format(ArquivosManager.LocalArquivos, _remetente.Emails));
                    f = di.GetFiles()[0];
                    if (f != null)
                    {
                        String novoNome = String.Format(ArquivosManager.LocalXml + nomePdf + f.Extension, _remetente.Emails);
                        if (!File.Exists(novoNome))
                            f.MoveTo(novoNome);
                    }
                }

                using (ZipFile zip = new ZipFile(localZip))
                {
                    //zip.AddFile(String.Format(ArquivosManager.LocalXml + nomeArquivo + ".xml", _remetente.Emails));
                    //zip.AddFile(String.Format(ArquivosManager.LocalArquivos + nomePdf + ".pdf", _remetente.Emails));
                    zip.AddFile(String.Format(ArquivosManager.LocalXml + nomeArquivo + ".xml", _remetente.Emails), "/");
                    zip.AddFile(String.Format(ArquivosManager.LocalXml + nomePdf + f.Extension, _remetente.Emails), "/");
                    zip.Save();
                    zip.Dispose();
                }
            }

            return localZip;
        }
         */
    }
}
