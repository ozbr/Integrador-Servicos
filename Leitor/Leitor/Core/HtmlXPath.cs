using HtmlAgilityPack;
using Leitor.Model;

namespace Leitor.Core
{
    class HtmlXPath
    {
        public HtmlDocument Arquivo { get; set; }
        public string Remetente { get; set; }
        NF xml = new NF();
        #region variaveis xpath
        public string NumeroNota { get; set; }
        public string DataEmissao { get; set; }
        public string CodigoVerificacao { get; set; }
        public string PrestCpfCnpj { get; set; }
        public string PrestInscMunicipal { get; set; }
        public string PrestNome { get; set; }
        public string PrestEndereco { get; set; }
        public string PrestEmail { get; set; }
        public string TomadCpfCnpj { get; set; }
        public string TomadtInscMunicipal { get; set; }
        public string TomadNome { get; set; }
        public string TomadEndereco { get; set; }
        public string TomadEmail { get; set; }
        public string Total { get; set; }
        public string Cnae { get; set; }
        #endregion

        public HtmlXPath(string localArquivo, string remetente)
        {
            Arquivo = new HtmlWeb().Load(localArquivo);
            Remetente = remetente;
        }

        public void GerarXPaths()
        {
            //conectar com banco e pegar xpaths e setar variaveis
            xml.infNFe.ide.nNF = Arquivo.DocumentNode.SelectSingleNode("//*[@id=\"cabecalho\"]/tbody/tr/td[3]/center[1]/label").InnerText;
            xml.infNFe.ide.dEmi = Arquivo.DocumentNode.SelectSingleNode("//*[@id=\"cabecalho\"]/tbody/tr/td[3]/center[2]/label").InnerText;
            //xml.infNFe.ide.codigoverificador = Arquivo.DocumentNode.SelectSingleNode("//*[@id=\"cabecalho\"]/tbody/tr/td[3]/center[3]/label").InnerText;
            xml.infNFe.ide.serie = Arquivo.DocumentNode.SelectSingleNode("//*[@id=\"cabecalho\"]/tbody/tr/td[2]/label[4]").InnerText;
        }

        public NF GetXml()
        {
            

            //xml.NFe.infNFe.ide.cNF = NumeroNota;
            //xml.NFe.infNFe.ide.dEmi = DataEmissao;
            //xml.NFe.infNFe.ide.cMunFG = xml.NFe.infNFe.emit.enderEmit.cMun = PrestInscMunicipal;
            //xml.NFe.infNFe.emit.CNPJ = PrestCpfCnpj;
            //xml.NFe.infNFe.emit.xNome = xml.NFe.infNFe.emit.xFant = PrestNome;
            ////tratar string de endereço para pegar todos os dados
            //xml.NFe.infNFe.emit.enderEmit.xLgr = PrestEndereco;
            //xml.NFe.infNFe.emit.enderEmit.cPais = xml.NFe.infNFe.dest.enderDest.cPais = "1058";
            //xml.NFe.infNFe.emit.enderEmit.xPais = xml.NFe.infNFe.dest.enderDest.xPais = "Brasil";
            //xml.NFe.infNFe.dest.CNPJ = TomadCpfCnpj;
            //xml.NFe.infNFe.dest.xNome = TomadNome;
            ////pegar todos os dados
            //xml.NFe.infNFe.dest.enderDest.xLgr = TomadEndereco;
            //xml.NFe.infNFe.dest.enderDest.cMun = TomadtInscMunicipal;


            return xml;
        }
    }
}
