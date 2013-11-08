using System.Collections.Generic;
using System.Xml.Serialization;

namespace Leitor.Model
{
    public class NF
    {
        private ProtNFe _protNFe = new ProtNFe();
        private InfNFe _infNFe = new InfNFe();

        public InfNFe infNFe
        {
            get { return _infNFe; }
            set { _infNFe = value; }
        }

        public ProtNFe protNFe
        {
            get { return _protNFe; }
            set { _protNFe = value; }
        }
    }


    public class InfNFe
    {
        private Ide _ide = new Ide();
        private Emit _emit = new Emit();
        private Dest _dest = new Dest();
        private Total _total = new Total();
        private Transp _transp = new Transp();
        private Cobr _cobr = new Cobr();
        private InfAdic _infAdic = new InfAdic();
        private List<Det> _det = new List<Det>();

        [XmlAttribute]
        public string versao { get; set; }

        [XmlAttribute]
        public string Id { get; set; }

        public string NFeNFSe { get; set; }

        public Ide ide
        {
            get { return _ide; }
            set { _ide = value; }
        }

        public Emit emit
        {
            get { return _emit; }
            set { _emit = value; }
        }

        public Dest dest
        {
            get { return _dest; }
            set { _dest = value; }
        }

        //[XmlArray, XmlArrayItem(typeof(Det), ElementName = "det")]
        [XmlElement("det")]
        public List<Det> det
        {
            get { return _det; }
            set { _det = value; }
        }

        public Total total
        {
            get { return _total; }
            set { _total = value; }
        }

        public Transp transp
        {
            get { return _transp; }
            set { _transp = value; }
        }

        public Cobr cobr
        {
            get { return _cobr; }
            set { _cobr = value; }
        }

        public InfAdic infAdic
        {
            get { return _infAdic; }
            set { _infAdic = value; }
        }
    }

    public class Ide
    {
        public string cUf { get; set; }
        public string cNF { get; set; }
        public string natOp { get; set; }
        public string indPag { get; set; }
        public string mod { get; set; }
        public string serie { get; set; }
        public string nNF { get; set; }
        public string dEmi { get; set; }
        public string tpNF { get; set; }
        public string cMunFG { get; set; }
        public string tpImp { get; set; }
        public string tpEmis { get; set; }
        public string cDV { get; set; }
        public string tpAmb { get; set; }
        public string finNFe { get; set; }
        public string procEmi { get; set; }
        public string NumeroNfseSubstituida { get; set; }
        public string NumeroRps { get; set; }
        public string SerieRps { get; set; }
        public string TipoRps { get; set; }
        public string DataEmissaoRps { get; set; }
        public string NumeroRpsSubstituido { get; set; }
        public string SerieRpsSubstituido { get; set; }
        public string TipoRpsSubstituido { get; set; }
    }

    public class Emit
    {
        private Ender _enderEmit = new Ender();
        public string CNPJ { get; set; }
        public string xNome { get; set; }
        public string xFant { get; set; }

        public Ender enderEmit
        {
            get { return _enderEmit; }
            set { _enderEmit = value; }
        }

        public string IE { get; set; }
        public string IM { get; set; }
        public string CRT { get; set; }
    }

    public class Ender
    {
        public string xLgr { get; set; }
        public string xBairro { get; set; }
        public string cMun { get; set; }
        public string xMun { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
        public string cPais { get; set; }
        public string xPais { get; set; }
        public string fone { get; set; }
    }

    public class Dest
    {
        private Ender _enderDest = new Ender();
        public string CNPJ { get; set; }
        public string xNome { get; set; }

        public Ender enderDest
        {
            get { return _enderDest; }
            set { _enderDest = value; }
        }

        public string IE { get; set; }
        public string IM { get; set; }
        public string email { get; set; }
    }

    public class Det
    {
        private Prod _prod = new Prod();
        private Imposto _imposto = new Imposto();

        [XmlAttribute]
        public string nItem { get; set; }

        public Prod prod
        {
            get { return _prod; }
            set { _prod = value; }
        }

        public Imposto imposto
        {
            get { return _imposto; }
            set { _imposto = value; }
        }

        public string infAdic { get; set; }
    }

    public class Prod
    {
        private Servico _servico = new Servico();
        public string cProd { get; set; }
        public string cEAN { get; set; }
        public string xProd { get; set; }
        public string NCM { get; set; }
        public string CFOP { get; set; }
        public string uCom { get; set; }
        public string qCom { get; set; }
        public string vUnCom { get; set; }
        public string vProd { get; set; }
        public string cEANTrib { get; set; }
        //public string uTrib { get; set; }
        //public string qTrib { get; set; }
        //public string vUnTrib { get; set; }
        public string indTot { get; set; }

        public Servico servico
        {
            get { return _servico; }
            set { _servico = value; }
        }
    }

    public class Servico
    {
        public string competencia { get; set; }
        public string ItemListaServico { get; set; }
        public string CodigoCnae { get; set; }
        public string CodigoTributacaoMunicipio { get; set; }
        public string Discriminacao { get; set; }
        public string CodigoMunicipio { get; set; }
        public string MunicipioIncidencia { get; set; }
    }

    public class Imposto
    {
        private Icms _icms = new Icms();
        private Pis _pis = new Pis();
        private Cofins _cofins = new Cofins();
        private Ipi _ipi = new Ipi();
        private Ii _ii = new Ii();
        private Issqn _issqn = new Issqn();

        public Icms ICMS
        {
            get { return _icms; }
            set { _icms = value; }
        }

        public Ipi IPI
        {
            get { return _ipi; }
            set { _ipi = value; }
        }

        public Ii II
        {
            get { return _ii; }
            set { _ii = value; }
        }

        public Pis PIS
        {
            get { return _pis; }
            set { _pis = value; }
        }

        public Cofins COFINS
        {
            get { return _cofins; }
            set { _cofins = value; }
        }

        public Issqn ISSQN
        {
            get { return _issqn; }
            set { _issqn = value; }
        }
    }

    public class Icms
    {
        public string orig { get; set; }
        public string CST { get; set; }
        public string CSOSN { get; set; }
        public string modBC { get; set; }
        public string vBC { get; set; }
        public string pRedBC { get; set; }
        public string pICMS { get; set; }
        public string vICMS { get; set; }
        public string modBCST { get; set; }
        public string pMVAST { get; set; }
        public string pRedBCST { get; set; }
        public string vBCST { get; set; }
        public string pICMSST { get; set; }
        public string vICMSST { get; set; }
        public string pCredSN { get; set; }
        public string vCredICMSSN { get; set; }
        public string motDesICMS { get; set; }
    }

    public class Ipi
    {
        private IpiTrib _ipiTrib = new IpiTrib();

        public string cIEnq { get; set; }
        public string CNPJProd { get; set; }
        public string cSelo { get; set; }
        public string qSelo { get; set; }
        public string cEnq { get; set; }

        public IpiTrib IPITrib
        {
            get { return _ipiTrib; }
            set { _ipiTrib = value; }
        }
    }

    public class IpiTrib
    {
        public string CST { get; set; }
        public string vBC { get; set; }
        public string pIPI { get; set; }
        public string qUnid { get; set; }
        public string vUnid { get; set; }
        public string vIPI { get; set; }
    }

    public class Ii
    {
        public string vBC { get; set; }
        public string vDespAdu { get; set; }
        public string vII { get; set; }
        public string vIOF { get; set; }
    }

    public class Pis
    {
        public string CST { get; set; }
        public string vBC { get; set; }
        public string pPIS { get; set; }
        public string vPIS { get; set; }
    }

    public class Cofins
    {
        public string CST { get; set; }
        public string vBC { get; set; }
        public string pCOFINS { get; set; }
        public string vCOFINS { get; set; }
    }

    public class Issqn
    {
        public string vBC { get; set; }
        public string vAliq { get; set; }
        public string vISSQN { get; set; }
        public string cMunFG { get; set; }
        public string cListServ { get; set; }
        public string cSitTrib { get; set; }
    }

    public class Total
    {
        private IcmsTot _icmsTot = new IcmsTot();

        public IcmsTot ICMSTot
        {
            get { return _icmsTot; }
            set { _icmsTot = value; }
        }
    }

    public class IcmsTot
    {
        public string vBC { get; set; }
        public string vICMS { get; set; }
        public string vBCST { get; set; }
        //public string vST { get; set; }
        public string vProd { get; set; }
        public string vFrete { get; set; }
        public string vSeg { get; set; }
        public string vDesc { get; set; }
        public string vII { get; set; }
        public string vIPI { get; set; }
        public string vPIS { get; set; }
        public string vCOFINS { get; set; }
        //public string vOutro { get; set; }
        public string vNF { get; set; }
        public string vServicos { get; set; }
        public string vDeducoes { get; set; }
        public string vINSS { get; set; }
        public string vIR { get; set; }
        public string vCSLL { get; set; }
        public string ValorIss { get; set; }
        public string OutrasRetencoes { get; set; }
        public string Aliquota { get; set; }
        public string DescontoIncondicionado { get; set; }
        public string DescontoCondicionado { get; set; }
        public string ISSRetido { get; set; }
        public string vLiquidoNfse { get; set; }
    }

    public class Transp
    {
        private Vol _vol = new Vol();
        private Transporta _transporta = new Transporta();
        public string modFrete { get; set; }

        public Transporta transporta
        {
            get { return _transporta; }
            set { _transporta = value; }
        }

        public Vol vol
        {
            get { return _vol; }
            set { _vol = value; }
        }
    }

    public class Transporta
    {
        public string xNome { get; set; }
        public string IE { get; set; }
        public string xEnder { get; set; }
        public string xMun { get; set; }
        public string UF { get; set; }
    }

    public class Vol
    {
        public string esp { get; set; }
    }

    public class Cobr
    {
        private Fat _fat = new Fat();
        private Dup _dup = new Dup();

        public Fat fat
        {
            get { return _fat; }
            set { _fat = value; }
        }

        public Dup dup
        {
            get { return _dup; }
            set { _dup = value; }
        }
    }

    public class Fat
    {
        public string nFat { get; set; }
        public string vOrig { get; set; }
        public string vLiq { get; set; }
        public string vDesc { get; set; }
    }

    public class Dup
    {
        public string nDup { get; set; }
        public string dVenc { get; set; }
        public string vDup { get; set; }
    }

    public class InfAdic
    {
        public string infAdFisco { get; set; }
        public string infCpl { get; set; }
    }

    public class Signature
    {
        private SignedInfo _signedInfo = new SignedInfo();

        [XmlAttribute]
        public string xmlns { get; set; }

        public SignedInfo SignedInfo
        {
            get { return _signedInfo; }
            set { _signedInfo = value; }
        }

        public string SignatureValue { get; set; }
    }

    public class SignedInfo
    {
        private CanonicalizationMethod _canonicalizationMethod = new CanonicalizationMethod();
        private SignatureMethod _signatureMethod = new SignatureMethod();
        private Reference _reference = new Reference();

        public CanonicalizationMethod CanonicalizationMethod
        {
            get { return _canonicalizationMethod; }
            set { _canonicalizationMethod = value; }
        }

        public SignatureMethod SignatureMethod
        {
            get { return _signatureMethod; }
            set { _signatureMethod = value; }
        }

        public Reference Reference
        {
            get { return _reference; }
            set { _reference = value; }
        }
    }

    public class CanonicalizationMethod
    {
        [XmlAttribute]
        public string Algorithm { get; set; }
    }

    public class SignatureMethod
    {
        [XmlAttribute]
        public string Algorithm { get; set; }
    }

    public class Reference
    {
        [XmlAttribute]
        public string URI { get; set; }
    }

    public class ProtNFe
    {
        private InfProt _infProt = new InfProt();

        [XmlAttribute]
        public string versao { get; set; }

        public InfProt infProt
        {
            get { return _infProt; }
            set { _infProt = value; }
        }
    }

    public class InfProt
    {
        public string tpAmb { get; set; }
        //public string verAplic { get; set; }
        public string chNFe { get; set; }
        public string dhRecbto { get; set; }
        //public string nProt { get; set; }
        //public string digVal { get; set; }
        //public string cStat { get; set; }
        //public string xMotivo { get; set; }
    }
}