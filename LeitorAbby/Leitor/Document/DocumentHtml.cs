using System.Text.RegularExpressions;
using System.Web;
using System.Text;
using HtmlAgilityPack;
using Leitor.Dao;
using Leitor.Model;
using Leitor.Utilities;
using System;
using System.Collections.Generic;

namespace Leitor.Document
{
    public class DocumentHtml : IDocument
    {
        private NF _nota = new NF();

        #region props
        private string _arquivo;

        public string Arquivo
        {
            get { return _arquivo; }
            set { _arquivo = value; }
        }

        private string _local;

        public string Local
        {
            get { return _local; }
            set { _local = value; }
        }

        private List<RegexModel> _rgxModel;

        public List<RegexModel> Parser
        {
            get { return _rgxModel; }
            set { _rgxModel = value; }
        }

        private Prefeitura _prefeitura;

        public Prefeitura Prefeitura
        {
            get { return _prefeitura; }
            set { _prefeitura = value; }
        }

        #endregion

        public NF Read()
        {
            RegexesDAO dao = new RegexesDAO();
            _rgxModel = dao.SelecionarRegexPossiveis(_prefeitura.Nome);

            #region xpath
            for (int i = 0; i < _rgxModel.Count; i++)
            {
                var rgxModel = _rgxModel[i];

                if (rgxModel.IsXpath)
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(Local);
                    //TODO 
                    //doc.Load(String.Format(ArquivosManager.LocalArquivos, r.Emails) + "pagina.html");

                    //ver se funciona com um, depois aplicar nos demais
                    _nota.infNFe.ide.nNF = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_nNF"));

                    #region _nota.infNFe

                    #region _nota.infNFe.cobr
                    XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ES"));
                    _nota.infNFe.NFeNFSe = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_NFeNFSe"));
                    //0 para mercadoria
                    //1 para serviço
                    _nota.infNFe.NFeNFSe = (_arquivo.ToUpperInvariant().Contains("SERVIÇO") || _arquivo.ToUpperInvariant().Contains("SERVI")) ? "1" : "0";

                    _nota.infNFe.cobr.dup.dVenc = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dup_dVenc"));
                    _nota.infNFe.cobr.dup.nDup = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dup_nDup"));
                    _nota.infNFe.cobr.dup.vDup = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dup_vDup")));

                    _nota.infNFe.cobr.fat.nFat = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_fat_nFat"));
                    _nota.infNFe.cobr.fat.vDesc = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_fat_vDesc")));
                    _nota.infNFe.cobr.fat.vLiq = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_fat_vLiq")));
                    _nota.infNFe.cobr.fat.vOrig = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_fat_vOrig")));
                    #endregion

                    #region _nota.infNFe.dest

                    _nota.infNFe.dest.CNPJ = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_CNPJ")));
                    _nota.infNFe.dest.Email = Util.SeparaEmails(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_email")));

                    //_nota.infNFe.dest.enderDest = "";
                    _nota.infNFe.dest.enderDest.CEP = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_CEP")));
                    _nota.infNFe.dest.enderDest.cMun = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_cMun")));
                    _nota.infNFe.dest.enderDest.cPais = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_cPais")));
                    _nota.infNFe.dest.enderDest.fone = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_fone")));
                    _nota.infNFe.dest.enderDest.UF = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_UF"));

                    _nota.infNFe.dest.enderDest.xBairro = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_xBairro"));
                    _nota.infNFe.dest.enderDest.xLgr = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_xLgr"));
                    _nota.infNFe.dest.enderDest.xMun = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_xMun"));
                    _nota.infNFe.dest.enderDest.xPais = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_xPais"));

                    _nota.infNFe.dest.IE = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_IE")));
                    _nota.infNFe.dest.IM = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_IM")));
                    _nota.infNFe.dest.xNome = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_dest_xNome"));
                    #endregion

                    #region _nota.infNFe.emit

                    _nota.infNFe.emit.CNPJ = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_CNPJ")));
                    _nota.infNFe.emit.CRT = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_CRT"));

                    _nota.infNFe.emit.enderEmit.CEP = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_CEP")));
                    _nota.infNFe.emit.enderEmit.cMun = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_cMun")));
                    _nota.infNFe.emit.enderEmit.cPais = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_cPais")));
                    _nota.infNFe.emit.enderEmit.fone = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_fone")));

                    _nota.infNFe.emit.enderEmit.UF = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_UF"));
                    _nota.infNFe.emit.enderEmit.xBairro = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_xBairro"));
                    _nota.infNFe.emit.enderEmit.xLgr = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_xLgr"));
                    _nota.infNFe.emit.enderEmit.xMun = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_xMun"));
                    _nota.infNFe.emit.enderEmit.xPais = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_xPais"));

                    _nota.infNFe.emit.IE = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_IE")));
                    _nota.infNFe.emit.IM = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_IM")));
                    _nota.infNFe.emit.xFant = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_xFant"));
                    _nota.infNFe.emit.xNome = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_emit_xNome"));

                    #endregion

                    #region _nota.infNFe.ide

                    _nota.infNFe.ide.cDV = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_cDV"));
                    _nota.infNFe.ide.cMunFG = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_cMunFG")));
                    _nota.infNFe.ide.cNF = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_cNF"));
                    _nota.infNFe.ide.cUf = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_cUf")));
                    _nota.infNFe.ide.DataEmissaoRps = Util.FormataData(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_DataEmissaoRps")));
                    _nota.infNFe.ide.dEmi = Util.FormataData(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_dEmi")));
                    _nota.infNFe.ide.finNFe = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_finNFe"));
                    _nota.infNFe.ide.indPag = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_indPag"));
                    _nota.infNFe.ide.mod = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_mode"));
                    _nota.infNFe.ide.natOp = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_natOp"));
                    _nota.infNFe.ide.nNF = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_nNF"));
                    _nota.infNFe.ide.NumeroNfseSubstituida = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_NumeroNfseSubstituida"));
                    _nota.infNFe.ide.NumeroRps = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_NumeroRps"));
                    _nota.infNFe.ide.NumeroRpsSubstituido = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_NumeroRpsSubstituido"));
                    _nota.infNFe.ide.procEmi = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_procEmi"));
                    _nota.infNFe.ide.serie = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_serie"));
                    _nota.infNFe.ide.SerieRps = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_SerieRps"));
                    _nota.infNFe.ide.SerieRpsSubstituido = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_SerieRspSubstituido"));
                    _nota.infNFe.ide.TipoRps = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_TipoRpsSubstituido"));
                    _nota.infNFe.ide.TipoRpsSubstituido = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_TipoRpsSubstituido"));
                    _nota.infNFe.ide.tpAmb = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_tpAmb"));
                    _nota.infNFe.ide.tpEmis = "0";//Util.FormataData(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_dEmi")));
                    _nota.infNFe.ide.tpImp = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_tpImp"));
                    _nota.infNFe.ide.tpNF = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ide_tpNF"));

                    #endregion

                    #region d

                    int k = 1;

                    //if (!String.IsNullOrEmpty(rgxModel.GetKeyXPath("RGX_prod_xProd")))
                    if (_nota.infNFe.NFeNFSe.Equals("0"))
                    {
                        int beginIndex = 4;
                        while (!String.IsNullOrEmpty(XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_xProd"), beginIndex))))
                        {

                            Det d = new Det();

                            d.infAdic = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_det_infAdic"), beginIndex + ""));

                            //produtos
                            d.nItem = k++.ToString();
                            d.prod.cEAN = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_cEAN"), beginIndex + ""));
                            d.prod.cEANTrib = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_cEANTrib"), beginIndex + ""));
                            d.prod.CFOP = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_CFOP"), beginIndex + ""));
                            d.prod.cProd = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_cProd"), beginIndex + ""));
                            d.prod.indTot = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_indTot"), beginIndex + ""));
                            d.prod.NCM = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_NCM"), beginIndex + ""));
                            d.prod.qCom = Util.FormataDecimal(XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_qCom"), beginIndex + "")));

                            //servicos
                            d.prod.servico.CodigoCnae = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_servico_CodigoCnae"), beginIndex + ""));
                            d.prod.servico.CodigoMunicipio = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_servico_CodigoMunicipio"), beginIndex + ""));
                            d.prod.servico.CodigoTributacaoMunicipio = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_servico_CodigoTributacaoMunicipio"), beginIndex + ""));
                            d.prod.servico.competencia = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_servico_competencia"), beginIndex + ""));
                            d.prod.servico.Discriminacao = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_servico_Discriminacao"), beginIndex + ""));
                            d.prod.servico.ItemListaServico = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_servico_ItemListaServico"));
                            d.prod.servico.MunicipioIncidencia = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_servico_MunicipioIncidencia"), beginIndex + ""));

                            d.prod.uCom = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_uCom"), beginIndex + ""));
                            d.prod.vProd = Util.FormataDecimal(XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_vProd"), beginIndex + "")));
                            d.prod.vUnCom = Util.FormataDecimal(XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_vUnCom"), beginIndex + "")));
                            d.prod.cProd = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_prod_xProd"), beginIndex + ""));
                            _nota.infNFe.det.Add(d);

                            beginIndex++;
                            if (!rgxModel.GetKeyXPath("RGX_prod_xProd").Contains("{0}"))
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        Det d = new Det();
                        d.prod.xProd = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_prod_xProd"));
                        d.prod.servico.CodigoCnae = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_servico_CodigoCnae"));
                        d.prod.servico.CodigoMunicipio = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_servico_CodigoMunicipio")));
                        d.prod.servico.CodigoTributacaoMunicipio = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_servico_CodigoTributacaoMunicipio")));
                        d.prod.servico.competencia = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_servico_competencia")));
                        d.prod.servico.Discriminacao = Util.LimpaDiscriminacao(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_servico_Discriminacao")));
                        d.prod.servico.ItemListaServico = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_servico_ItemListaServico"));
                        d.prod.servico.MunicipioIncidencia = XpathSingleNode(doc, String.Format(rgxModel.GetKeyXPath("RGX_servico_MunicipioIncidencia")));
                        _nota.infNFe.det.Add(d);
                    }

                    #endregion

                    #region _nota.infNFe.infAdic

                    _nota.infNFe.infAdic.infAdFisco = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_infAdic_infAdFisco"));
                    _nota.infNFe.infAdic.infCpl = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_infAdic_infCpl"));

                    #endregion

                    #region _nota.infNFe.total

                    _nota.infNFe.total.ICMSTot.Aliquota = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_Aliquota")));
                    _nota.infNFe.total.ICMSTot.DescontoCondicionado = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_DescontoCondicionado")));
                    _nota.infNFe.total.ICMSTot.DescontoIncondicionado = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_DescontoIncondicionado")));
                    _nota.infNFe.total.ICMSTot.ISSRetido = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_ISSRetido")));
                    _nota.infNFe.total.ICMSTot.OutrasRetencoes = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_OutrasRetencoes")));
                    _nota.infNFe.total.ICMSTot.ValorIss = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_ValorIss")));
                    _nota.infNFe.total.ICMSTot.vBC = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vBC")));
                    _nota.infNFe.total.ICMSTot.vBCST = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vBCST")));
                    _nota.infNFe.total.ICMSTot.vCOFINS = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vCOFINS")));
                    _nota.infNFe.total.ICMSTot.vCSLL = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vCSLL")));
                    _nota.infNFe.total.ICMSTot.vDeducoes = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vDeducoes")));
                    _nota.infNFe.total.ICMSTot.vDesc = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vDesc")));
                    _nota.infNFe.total.ICMSTot.vFrete = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vFrete")));
                    _nota.infNFe.total.ICMSTot.vICMS = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vICMS")));
                    _nota.infNFe.total.ICMSTot.vII = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vII")));
                    _nota.infNFe.total.ICMSTot.vINSS = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vINSS")));
                    _nota.infNFe.total.ICMSTot.vIPI = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vIPI")));
                    _nota.infNFe.total.ICMSTot.vIR = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vIR")));
                    _nota.infNFe.total.ICMSTot.vLiquidoNfse = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vLiquidoNfse")));
                    _nota.infNFe.total.ICMSTot.vNF = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vNF")));
                    _nota.infNFe.total.ICMSTot.vPIS = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vPIS")));
                    _nota.infNFe.total.ICMSTot.vProd = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vProd")));
                    _nota.infNFe.total.ICMSTot.vSeg = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vSeg")));
                    _nota.infNFe.total.ICMSTot.vServicos = Util.FormataDecimal(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_ICMSTot_vServicos")));
                    #endregion

                    #region _nota.infNFe.transp
                    _nota.infNFe.transp.modFrete = "";
                    _nota.infNFe.transp.transporta.IE = Util.LimpaCampos(XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_transporta_IE")));
                    _nota.infNFe.transp.transporta.UF = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_transporta_UF"));
                    _nota.infNFe.transp.transporta.xEnder = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_transporta_xEnder"));
                    _nota.infNFe.transp.transporta.xMun = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_transporta_xMun"));
                    _nota.infNFe.transp.transporta.xNome = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_transporta_xNome"));

                    _nota.infNFe.transp.vol.esp = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_vol_esp"));

                    #endregion

                    #region _nota.infNFe.versao

                    _nota.infNFe.versao = XpathSingleNode(doc, rgxModel.GetKeyXPath("RGX_transp_modFrete"));

                    #endregion

                    //brasil para pais padrão
                    if (String.IsNullOrEmpty(_nota.infNFe.emit.enderEmit.cPais)) _nota.infNFe.emit.enderEmit.cPais = "1058";
                    if (String.IsNullOrEmpty(_nota.infNFe.emit.enderEmit.xPais)) _nota.infNFe.emit.enderEmit.xPais = "Brasil";
                    if (String.IsNullOrEmpty(_nota.infNFe.dest.enderDest.cPais)) _nota.infNFe.dest.enderDest.cPais = "1058";
                    if (String.IsNullOrEmpty(_nota.infNFe.dest.enderDest.xPais)) _nota.infNFe.dest.enderDest.xPais = "Brasil";

                    #endregion

                    break;
                }
                else
                {
                    continue;
                }
            }
            #endregion

            return _nota;
        }

        /*
         * ALTERADO 16/07 POR MATEUS: UNICO METÓDO PARA XPATH E XPATHREGEX
         */
        private String XpathSingleNode(HtmlDocument doc, string xpath)
        {
            string result = "";
            if (!string.IsNullOrEmpty(xpath))
            {
                try
                {
                    if (xpath.Contains("#"))
                    {
                        string[] aux = new string[3];
                        aux[0] = xpath.Split('#')[0];
                        aux[1] = xpath.Split('#')[1];
                        aux[2] = xpath.Split('#')[2];
                        if(!String.IsNullOrEmpty(aux[0]) || !String.IsNullOrEmpty(aux[1]) || !String.IsNullOrEmpty(aux[2]))
                        {
                            var node = doc.DocumentNode.SelectSingleNode(aux[0]);
                            if(node != null)
                            {
                                Match m = Regex.Match(node.InnerText.Trim(), aux[1], RegexOptions.Singleline);
                                if (m.Success)
                                {
                                    result = m.Groups[Convert.ToInt32(aux[2])].Value;
                                }
                            }
                        }
                    }
                    else
                    {
                        var aux = doc.DocumentNode.SelectSingleNode(xpath);
                        if (aux != null)
                        {
                            result = aux.InnerText.Trim();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.SaveTxt("DocumentHtml.XpathSingleNode", e.Message, Log.LogType.Erro);
                }
                result = result.Replace("&amp", "&");
                result = result.Replace("&nbsp;", "");
                //ALTERADO POR MATEUS EM 15/07: ALGUMAS NOTAS ESTÃO VINDO DESFORMATADAS
                if (result.Contains("&#"))
                    result = HttpUtility.HtmlDecode(result);
                if (Prefeitura.Nome.Contains("PIRASS"))
                    result = Encoding.UTF8.GetString(Encoding.Default.GetBytes(result));
            }
            return result;
        }
    }
}
