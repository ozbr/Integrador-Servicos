using Leitor.Dao;
using Leitor.Model;
using Leitor.Utilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Leitor.Document
{
    public class DocumentPdf : IDocument
    {
        private NF _nota = new NF();

        #region props
        public string arquivo;
        public string Arquivo
        {
            get
            {
                return arquivo;
            }
            set
            {
                arquivo = value;
            }
        }

        public string _local { get; set; }
        public string Local
        {
            get
            {
                return _local;
            }
            set
            {
                _local = value;
            }
        }

        public List<RegexModel> _rgxModel { get; set; }
        public List<RegexModel> Parser
        {
            get
            {
                return _rgxModel;
            }
            set
            {
                _rgxModel = value;
            }
        }

        public Prefeitura _prefeitura { get; set; }
        public Prefeitura Prefeitura
        {
            get
            {
                return _prefeitura;
            }
            set
            {
                _prefeitura = value;
            }
        }
        #endregion

        public Model.NF Read()
        {
            bool temErro = false;

            String EouS = string.Empty;

            RegexesDAO dao = new RegexesDAO();
            _rgxModel = dao.SelecionarRegexPossiveis(Prefeitura.Nome);

            for (int i = 0; i < _rgxModel.Count; i++)
            {
                var rgxModel = _rgxModel[i];

                if (rgxModel.IsValid() && !rgxModel.IsXpath)
                {
                    temErro = false;

                    //==============================================

                    #region _nota.infNFe

                    #region _nota.infNFe.cobr

                    /*
                 ,[RGX_fat_nFat]
                  ,[RGX_fat_vOrig]
                  ,[RGX_fat_vLiq]
                  ,[RGX_fat_vDesc]
                  ,[RGX_dup_nDup]
                  ,[RGX_dup_dVenc]
                  ,[RGX_dup_vDup]
                 */

                    // Usar um Match diferente de Singleline cai em loop infinito em vários casos (ex. BARUERI)
                    Match geral = new Regex(rgxModel.Geral, RegexOptions.Singleline).Match(arquivo);
                    if (geral.Success)
                    {

                        EouS = geral.Groups[rgxModel.GetKeyValue("RGX_ES")].Value.Trim();
                        _nota.infNFe.NFeNFSe = geral.Groups[rgxModel.GetKeyValue("RGX_NFeNFSe")].Value.Trim();
                        //0 para mercadoria
                        //1 para serviço
                        if (arquivo.ToUpperInvariant().Contains("SERVIÇO"))
                        {
                            _nota.infNFe.NFeNFSe = "1";
                        }
                        else
                        {
                            _nota.infNFe.NFeNFSe = "0";
                        }

                        string prodGroup = geral.Groups[rgxModel.GetKeyValue("RGX_PROD")].Value.Trim();
                        _nota.infNFe.cobr.dup.dVenc =
                            Util.FormataData(geral.Groups[rgxModel.GetKeyValue("RGX_dup_dVenc")].Value.Trim());
                        _nota.infNFe.cobr.dup.nDup = geral.Groups[rgxModel.GetKeyValue("RGX_dup_nDup")].Value.Trim();
                        _nota.infNFe.cobr.dup.vDup =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_dup_vDup")].Value.Trim());

                        _nota.infNFe.cobr.fat.nFat = geral.Groups[rgxModel.GetKeyValue("RGX_fat_nFat")].Value.Trim();
                        _nota.infNFe.cobr.fat.vDesc =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_fat_vDesc")].Value.Trim());
                        _nota.infNFe.cobr.fat.vLiq =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_fat_vLiq")].Value.Trim());
                        _nota.infNFe.cobr.fat.vOrig =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_fat_vOrig")].Value.Trim());

                    #endregion

                        #region _nota.infNFe.dest

                        _nota.infNFe.dest.CNPJ =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_dest_CNPJ")].Value.Trim());
                        _nota.infNFe.dest.Email =
                            Util.SeparaEmails(geral.Groups[rgxModel.GetKeyValue("RGX_dest_email")].Value.Trim());

                        //_nota.infNFe.dest.enderDest = "";
                        _nota.infNFe.dest.enderDest.CEP =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_dest_CEP")].Value.Trim());
                        _nota.infNFe.dest.enderDest.cMun =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_dest_cMun")].Value.Trim());
                        _nota.infNFe.dest.enderDest.cPais =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_dest_cPais")].Value.Trim());
                        _nota.infNFe.dest.enderDest.fone =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_dest_fone")].Value.Trim());
                        _nota.infNFe.dest.enderDest.UF = geral.Groups[rgxModel.GetKeyValue("RGX_dest_UF")].Value.Trim();
                        _nota.infNFe.dest.enderDest.xBairro =
                            geral.Groups[rgxModel.GetKeyValue("RGX_dest_xBairro")].Value.Trim();
                        _nota.infNFe.dest.enderDest.xLgr =
                            Util.RetiraQuebraDeLinha(geral.Groups[rgxModel.GetKeyValue("RGX_dest_xLgr")].Value.Trim());
                        _nota.infNFe.dest.enderDest.xMun = geral.Groups[rgxModel.GetKeyValue("RGX_dest_xMun")].Value.Trim();
                        _nota.infNFe.dest.enderDest.xPais =
                            geral.Groups[rgxModel.GetKeyValue("RGX_dest_xPais")].Value.Trim();

                        _nota.infNFe.dest.IE =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_dest_IE")].Value.Trim());
                        _nota.infNFe.dest.IM =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_dest_IM")].Value.Trim());
                        _nota.infNFe.dest.xNome = geral.Groups[rgxModel.GetKeyValue("RGX_dest_xNome")].Value.Trim();

                        #endregion

                        #region d

                        if (_nota.infNFe.NFeNFSe.Equals("0"))
                        {
                            int k = 1;
                            foreach (Match item in new Regex(rgxModel.Item, RegexOptions.Singleline).Matches(prodGroup))
                            {
                                Det d = new Det();

                                //d.imposto = "";
                                //d.imposto.COFINS = "";
                                d.imposto.COFINS.CST = item.Groups[rgxModel.GetKeyValue("RGX_COFINS_CST")].Value.Trim();
                                d.imposto.COFINS.pCOFINS =
                                    item.Groups[rgxModel.GetKeyValue("RGX_COFINS_pCOFINS")].Value.Trim();
                                d.imposto.COFINS.vBC = item.Groups[rgxModel.GetKeyValue("RGX_COFINS_vBC")].Value.Trim();
                                d.imposto.COFINS.vCOFINS =
                                    item.Groups[rgxModel.GetKeyValue("RGX_COFINS_vCOFINS")].Value.Trim();

                                //d.imposto.ICMS = "";
                                d.imposto.ICMS.CSOSN = item.Groups[rgxModel.GetKeyValue("RGX_ICMS_CSOSN")].Value.Trim();
                                d.imposto.ICMS.CST = item.Groups[rgxModel.GetKeyValue("RGX_ICMS_CST")].Value.Trim();
                                d.imposto.ICMS.modBC = item.Groups[rgxModel.GetKeyValue("RGX_ICMS_modBC")].Value.Trim();
                                d.imposto.ICMS.modBCST =
                                    item.Groups[rgxModel.GetKeyValue("RGX_ICMS_modBCST")].Value.Trim();
                                d.imposto.ICMS.motDesICMS =
                                    item.Groups[rgxModel.GetKeyValue("RGX_ICMS_motDesICMS")].Value.Trim();
                                d.imposto.ICMS.orig = item.Groups[rgxModel.GetKeyValue("RGX_ICMS_orig")].Value.Trim();
                                d.imposto.ICMS.pCredSN =
                                    item.Groups[rgxModel.GetKeyValue("RGX_ICMS_pCredSN")].Value.Trim();
                                d.imposto.ICMS.pICMSST =
                                    item.Groups[rgxModel.GetKeyValue("RGX_ICMS_vICMSST")].Value.Trim();
                                d.imposto.ICMS.pMVAST = item.Groups[rgxModel.GetKeyValue("RGX_ICMS_pMVAST")].Value.Trim();
                                d.imposto.ICMS.pRedBC = item.Groups[rgxModel.GetKeyValue("RGX_ICMS_pRedBC")].Value.Trim();
                                d.imposto.ICMS.pRedBCST =
                                    item.Groups[rgxModel.GetKeyValue("RGX_ICMS_pRedBCST")].Value.Trim();
                                d.imposto.ICMS.vBC =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_ICMS_vBC")].Value.Trim());
                                d.imposto.ICMS.vBCST =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_ICMS_vBCST")].Value.Trim());
                                d.imposto.ICMS.vCredICMSSN =
                                    item.Groups[rgxModel.GetKeyValue("RGX_ICMS_vCredICMSSN")].Value.Trim();
                                d.imposto.ICMS.vICMS =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_ICMS_vICMS")].Value.Trim());
                                d.imposto.ICMS.vICMSST =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_ICMS_vICMSST")].Value.Trim());

                                /*
                                 * [RGX_II_vBC]
                                  ,[RGX_II_vDespAdu]
                                  ,[RGX_II_vII]
                                  ,[RGX_II_vIOF]
                                 */
                                //d.imposto.II = "";
                                d.imposto.II.vBC = item.Groups[rgxModel.GetKeyValue("RGX_II_vBC")].Value.Trim();
                                d.imposto.II.vDespAdu = item.Groups[rgxModel.GetKeyValue("RGX_II_vDespAdu")].Value.Trim();
                                d.imposto.II.vII = item.Groups[rgxModel.GetKeyValue("RGX_II_vII")].Value.Trim();
                                d.imposto.II.vIOF = item.Groups[rgxModel.GetKeyValue("RGX_II_vIOF")].Value.Trim();

                                /*
                                 ,[RGX_IPI_cIEnq]
                                  ,[RGX_IPI_CNPJProd]
                                  ,[RGX_IPI_cSelo]
                                  ,[RGX_IPI_qSelo]
                                  ,[RGX_IPI_cEnq]
                                  ,[RGX_IPITrib_CST]
                                  ,[RGX_IPITrib_vBC]
                                  ,[RGX_IPITrib_pIPI]
                                  ,[RGX_IPITrib_qUnid]
                                  ,[RGX_IPITrib_vUnid]
                                  ,[RGX_IPITrib_vIPI]
                                 */
                                //d.imposto.IPI = "";
                                d.imposto.IPI.cEnq = item.Groups[rgxModel.GetKeyValue("RGX_IPI_cEnq")].Value.Trim();
                                d.imposto.IPI.cIEnq = item.Groups[rgxModel.GetKeyValue("RGX_IPI_cIEnq")].Value.Trim();
                                d.imposto.IPI.CNPJProd =
                                    item.Groups[rgxModel.GetKeyValue("RGX_IPI_CNPJProd")].Value.Trim();
                                d.imposto.IPI.cSelo = item.Groups[rgxModel.GetKeyValue("RGX_IPI_cSelo")].Value.Trim();
                                //d.imposto.IPI.IPITrib = "";
                                d.imposto.IPI.IPITrib.CST =
                                    item.Groups[rgxModel.GetKeyValue("RGX_IPITrib_CST")].Value.Trim();
                                d.imposto.IPI.IPITrib.pIPI =
                                    item.Groups[rgxModel.GetKeyValue("RGX_IPITrib_pIPI")].Value.Trim();
                                d.imposto.IPI.IPITrib.qUnid =
                                    item.Groups[rgxModel.GetKeyValue("RGX_IPITrib_qUnid")].Value.Trim();
                                d.imposto.IPI.IPITrib.vBC =
                                    item.Groups[rgxModel.GetKeyValue("RGX_IPITrib_vBC")].Value.Trim();
                                d.imposto.IPI.IPITrib.vIPI =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_IPITrib_vIPI")].Value.Trim());
                                d.imposto.IPI.IPITrib.vUnid =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_IPITrib_vUnid")].Value.Trim());

                                d.imposto.IPI.qSelo = item.Groups[rgxModel.GetKeyValue("RGX_IPI_qSelo")].Value.Trim();

                                /*
                                 [RGX_ISSQN_vBC]
                                  ,[RGX_ISSQN_vAliq]
                                  ,[RGX_ISSQN_vISSQN]
                                  ,[RGX_ISSQN_cMunFG]
                                  ,[RGX_ISSQN_cListServ]
                                  ,[RGX_ISSQN_cSitTrib]
                                 */

                                //d.imposto.ISSQN = "";
                                d.imposto.ISSQN.cListServ =
                                    item.Groups[rgxModel.GetKeyValue("RGX_ISSQN_cListServ")].Value.Trim();
                                d.imposto.ISSQN.cMunFG =
                                    Util.LimpaCampos(item.Groups[rgxModel.GetKeyValue("RGX_ISSQN_cMunFG")].Value.Trim());
                                d.imposto.ISSQN.cSitTrib =
                                    item.Groups[rgxModel.GetKeyValue("RGX_ISSQN_cSitTrib")].Value.Trim();
                                d.imposto.ISSQN.vAliq =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_ISSQN_vAliq")].Value.Trim());
                                d.imposto.ISSQN.vBC =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_ISSQN_vBC")].Value.Trim());
                                d.imposto.ISSQN.vISSQN =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_ISSQN_vISSQN")].Value.Trim());

                                /*
                                 * [RGX_PIS_CST]
                                  ,[RGX_PIS_vBC]
                                  ,[RGX_PIS_pPIS]
                                  ,[RGX_PIS_vPIS]
                                 */
                                //d.imposto.PIS = "";
                                d.imposto.PIS.CST = item.Groups[rgxModel.GetKeyValue("RGX_PIS_CS")].Value.Trim();
                                d.imposto.PIS.pPIS = item.Groups[rgxModel.GetKeyValue("RGX_PIS_pPIS")].Value.Trim();
                                d.imposto.PIS.vBC =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_PIS_vBC")].Value.Trim());
                                d.imposto.PIS.vPIS =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_PIS_vPIS")].Value.Trim());

                                d.infAdic = item.Groups[rgxModel.GetKeyValue("RGX_det_infAdic")].Value.Trim();


                                //produtos
                                d.nItem = k++.ToString();
                                d.prod.cEAN = item.Groups[rgxModel.GetKeyValue("RGX_prod_cEAN")].Value.Trim();
                                d.prod.cEANTrib = item.Groups[rgxModel.GetKeyValue("RGX_prod_cEANTrib")].Value.Trim();
                                d.prod.CFOP = item.Groups[rgxModel.GetKeyValue("RGX_prod_CFOP")].Value.Trim();
                                d.prod.cProd = item.Groups[rgxModel.GetKeyValue("RGX_prod_cProd")].Value.Trim();
                                d.prod.indTot = item.Groups[rgxModel.GetKeyValue("RGX_prod_indTot")].Value.Trim();
                                d.prod.NCM = item.Groups[rgxModel.GetKeyValue("RGX_prod_NCM")].Value.Trim();
                                d.prod.qCom =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_prod_qCom")].Value.Trim());

                                //servicos
                                d.prod.servico.CodigoCnae =
                                    item.Groups[rgxModel.GetKeyValue("RGX_servico_CodigoCnae")].Value.Trim();
                                d.prod.servico.CodigoMunicipio =
                                    item.Groups[rgxModel.GetKeyValue("RGX_servico_CodigoMunicipio")].Value.Trim();
                                d.prod.servico.CodigoTributacaoMunicipio =
                                    item.Groups[rgxModel.GetKeyValue("RGX_servico_CodigoTributacaoMunicipio")].Value.Trim();
                                d.prod.servico.competencia =
                                    item.Groups[rgxModel.GetKeyValue("RGX_servico_competencia")].Value.Trim();
                                d.prod.servico.Discriminacao =
                                    item.Groups[rgxModel.GetKeyValue("RGX_servico_Discriminacao")].Value.Trim();
                                d.prod.servico.ItemListaServico =
                                    item.Groups[rgxModel.GetKeyValue("RGX_servico_ItemListaServico")].Value.Trim();
                                d.prod.servico.MunicipioIncidencia =
                                    item.Groups[rgxModel.GetKeyValue("RGX_servico_MunicipioIncidencia")].Value.Trim();

                                d.prod.uCom = item.Groups[rgxModel.GetKeyValue("RGX_prod_uCom")].Value.Trim();
                                d.prod.vProd =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_prod_vProd")].Value.Trim());
                                d.prod.vUnCom =
                                    Util.FormataDecimal(item.Groups[rgxModel.GetKeyValue("RGX_prod_vUnCom")].Value.Trim());
                                d.prod.xProd = item.Groups[rgxModel.GetKeyValue("RGX_prod_xProd")].Value.Trim();
                                _nota.infNFe.det.Add(d);
                            }
                        }
                        else
                        {
                            Det d = new Det();

                            #region impostos

                            d.imposto.COFINS.CST = geral.Groups[rgxModel.GetKeyValue("RGX_COFINS_CST")].Value.Trim();
                            d.imposto.COFINS.pCOFINS =
                                geral.Groups[rgxModel.GetKeyValue("RGX_COFINS_pCOFINS")].Value.Trim();
                            d.imposto.COFINS.vBC = geral.Groups[rgxModel.GetKeyValue("RGX_COFINS_vBC")].Value.Trim();
                            d.imposto.COFINS.vCOFINS =
                                geral.Groups[rgxModel.GetKeyValue("RGX_COFINS_vCOFINS")].Value.Trim();

                            d.imposto.ICMS.CSOSN = geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_CSOSN")].Value.Trim();
                            d.imposto.ICMS.CST = geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_CST")].Value.Trim();
                            d.imposto.ICMS.modBC = geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_modBC")].Value.Trim();
                            d.imposto.ICMS.modBCST =
                                geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_modBCST")].Value.Trim();
                            d.imposto.ICMS.motDesICMS =
                                geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_motDesICMS")].Value.Trim();
                            d.imposto.ICMS.orig = geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_orig")].Value.Trim();
                            d.imposto.ICMS.pCredSN =
                                geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_pCredSN")].Value.Trim();
                            d.imposto.ICMS.pICMSST =
                                geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_vICMSST")].Value.Trim();
                            d.imposto.ICMS.pMVAST = geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_pMVAST")].Value.Trim();
                            d.imposto.ICMS.pRedBC = geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_pRedBC")].Value.Trim();
                            d.imposto.ICMS.pRedBCST =
                                geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_pRedBCST")].Value.Trim();
                            d.imposto.ICMS.vBC =
                                Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_vBC")].Value.Trim());
                            d.imposto.ICMS.vBCST =
                                Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_vBCST")].Value.Trim());
                            d.imposto.ICMS.vCredICMSSN =
                                geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_vCredICMSSN")].Value.Trim();
                            d.imposto.ICMS.vICMS =
                                Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_vICMS")].Value.Trim());
                            d.imposto.ICMS.vICMSST =
                                Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMS_vICMSST")].Value.Trim());

                            d.imposto.II.vBC = geral.Groups[rgxModel.GetKeyValue("RGX_II_vBC")].Value.Trim();
                            d.imposto.II.vDespAdu = geral.Groups[rgxModel.GetKeyValue("RGX_II_vDespAdu")].Value.Trim();
                            d.imposto.II.vII = geral.Groups[rgxModel.GetKeyValue("RGX_II_vII")].Value.Trim();
                            d.imposto.II.vIOF = geral.Groups[rgxModel.GetKeyValue("RGX_II_vIOF")].Value.Trim();

                            d.imposto.IPI.cEnq = geral.Groups[rgxModel.GetKeyValue("RGX_IPI_cEnq")].Value.Trim();
                            d.imposto.IPI.cIEnq = geral.Groups[rgxModel.GetKeyValue("RGX_IPI_cIEnq")].Value.Trim();
                            d.imposto.IPI.CNPJProd =
                                geral.Groups[rgxModel.GetKeyValue("RGX_IPI_CNPJProd")].Value.Trim();
                            d.imposto.IPI.cSelo = geral.Groups[rgxModel.GetKeyValue("RGX_IPI_cSelo")].Value.Trim();

                            d.imposto.IPI.IPITrib.CST =
                                geral.Groups[rgxModel.GetKeyValue("RGX_IPITrib_CST")].Value.Trim();
                            d.imposto.IPI.IPITrib.pIPI =
                                geral.Groups[rgxModel.GetKeyValue("RGX_IPITrib_pIPI")].Value.Trim();
                            d.imposto.IPI.IPITrib.qUnid =
                                geral.Groups[rgxModel.GetKeyValue("RGX_IPITrib_qUnid")].Value.Trim();
                            d.imposto.IPI.IPITrib.vBC =
                                geral.Groups[rgxModel.GetKeyValue("RGX_IPITrib_vBC")].Value.Trim();
                            d.imposto.IPI.IPITrib.vIPI =
                                Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_IPITrib_vIPI")].Value.Trim());
                            d.imposto.IPI.IPITrib.vUnid =
                                Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_IPITrib_vUnid")].Value.Trim());

                            d.imposto.IPI.qSelo = geral.Groups[rgxModel.GetKeyValue("RGX_IPI_qSelo")].Value.Trim();

                            d.imposto.ISSQN.cListServ =
                                geral.Groups[rgxModel.GetKeyValue("RGX_ISSQN_cListServ")].Value.Trim();
                            d.imposto.ISSQN.cMunFG =
                                Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_ISSQN_cMunFG")].Value.Trim());
                            d.imposto.ISSQN.cSitTrib =
                                geral.Groups[rgxModel.GetKeyValue("RGX_ISSQN_cSitTrib")].Value.Trim();
                            d.imposto.ISSQN.vAliq =
                                Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ISSQN_vAliq")].Value.Trim());
                            d.imposto.ISSQN.vBC =
                                Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ISSQN_vBC")].Value.Trim());
                            d.imposto.ISSQN.vISSQN =
                                Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ISSQN_vISSQN")].Value.Trim());

                            d.imposto.PIS.CST = geral.Groups[rgxModel.GetKeyValue("RGX_PIS_CS")].Value.Trim();
                            d.imposto.PIS.pPIS = geral.Groups[rgxModel.GetKeyValue("RGX_PIS_pPIS")].Value.Trim();
                            d.imposto.PIS.vBC =
                                Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_PIS_vBC")].Value.Trim());
                            d.imposto.PIS.vPIS =
                                Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_PIS_vPIS")].Value.Trim());

                            d.infAdic = geral.Groups[rgxModel.GetKeyValue("RGX_det_infAdic")].Value.Trim();

                            #endregion

                            //produtos
                            d.prod.uCom =
                                d.prod.vProd =
                                d.prod.vUnCom =
                                d.prod.cEAN =
                                d.prod.cEANTrib =
                                d.prod.CFOP = d.prod.cProd = d.prod.indTot = d.prod.NCM = d.prod.qCom = String.Empty;

                            d.prod.xProd =
                                Util.RetiraQuebraDeLinha(geral.Groups[rgxModel.GetKeyValue("RGX_prod_xProd")].Value.Trim());
                            d.prod.servico.CodigoCnae =
                                geral.Groups[rgxModel.GetKeyValue("RGX_servico_CodigoCnae")].Value.Trim();
                            d.prod.servico.CodigoMunicipio =
                                geral.Groups[rgxModel.GetKeyValue("RGX_servico_CodigoMunicipio")].Value.Trim();
                            d.prod.servico.CodigoTributacaoMunicipio =
                                geral.Groups[rgxModel.GetKeyValue("RGX_servico_CodigoTributacaoMunicipio")].Value.Trim();
                            d.prod.servico.competencia =
                                geral.Groups[rgxModel.GetKeyValue("RGX_servico_competencia")].Value.Trim();
                            d.prod.servico.Discriminacao = Util.LimpaDiscriminacao(
                                geral.Groups[rgxModel.GetKeyValue("RGX_servico_Discriminacao")].Value.Trim());
                            d.prod.servico.ItemListaServico =
                                geral.Groups[rgxModel.GetKeyValue("RGX_servico_ItemListaServico")].Value.Trim();
                            d.prod.servico.MunicipioIncidencia =
                                geral.Groups[rgxModel.GetKeyValue("RGX_servico_MunicipioIncidencia")].Value.Trim();
                            _nota.infNFe.det.Add(d);
                        }

                        #endregion

                        #region _nota.infNFe.emit

                        _nota.infNFe.emit.CNPJ =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_emit_CNPJ")].Value.Trim());
                        _nota.infNFe.emit.CRT = geral.Groups[rgxModel.GetKeyValue("RGX_emit_CRT")].Value.Trim();

                        //_nota.infNFe.emit.enderEmit = "";
                        _nota.infNFe.emit.enderEmit.CEP =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_emit_CEP")].Value.Trim());
                        _nota.infNFe.emit.enderEmit.cMun =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_emit_cMun")].Value.Trim());
                        _nota.infNFe.emit.enderEmit.cPais =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_emit_cPais")].Value.Trim());
                        _nota.infNFe.emit.enderEmit.fone =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_emit_fone")].Value.Trim());
                        _nota.infNFe.emit.enderEmit.UF = geral.Groups[rgxModel.GetKeyValue("RGX_emit_UF")].Value.Trim();
                        _nota.infNFe.emit.enderEmit.xBairro =
                            geral.Groups[rgxModel.GetKeyValue("RGX_emit_xBairro")].Value.Trim();
                        _nota.infNFe.emit.enderEmit.xLgr =
                            Util.RetiraQuebraDeLinha(geral.Groups[rgxModel.GetKeyValue("RGX_emit_xLgr")].Value.Trim());
                        _nota.infNFe.emit.enderEmit.xMun = geral.Groups[rgxModel.GetKeyValue("RGX_emit_xMun")].Value.Trim();
                        _nota.infNFe.emit.enderEmit.xPais =
                            geral.Groups[rgxModel.GetKeyValue("RGX_emit_xPais")].Value.Trim();

                        _nota.infNFe.emit.IE =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_emit_IE")].Value.Trim());
                        _nota.infNFe.emit.IM =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_emit_IM")].Value.Trim());
                        _nota.infNFe.emit.xFant = geral.Groups[rgxModel.GetKeyValue("RGX_emit_xFant")].Value.Trim();
                        _nota.infNFe.emit.xNome = geral.Groups[rgxModel.GetKeyValue("RGX_emit_xNome")].Value.Trim();

                        #endregion

                        #region _nota.infNFe.ide

                        _nota.infNFe.ide.cDV = geral.Groups[rgxModel.GetKeyValue("RGX_ide_cDV")].Value.Trim();
                        _nota.infNFe.ide.cMunFG =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_ide_cMunFG")].Value.Trim());
                        _nota.infNFe.ide.cNF = geral.Groups[rgxModel.GetKeyValue("RGX_ide_cNF")].Value.Trim();
                        _nota.infNFe.ide.cUf =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_ide_cUf")].Value.Trim());
                        _nota.infNFe.ide.DataEmissaoRps =
                            Util.FormataData(geral.Groups[rgxModel.GetKeyValue("RGX_ide_DataEmissaoRps")].Value.Trim());
                        _nota.infNFe.ide.dEmi =
                            Util.FormataData(geral.Groups[rgxModel.GetKeyValue("RGX_ide_dEmi")].Value.Trim());
                        _nota.infNFe.ide.finNFe = geral.Groups[rgxModel.GetKeyValue("RGX_ide_finNFe")].Value.Trim();
                        _nota.infNFe.ide.indPag = geral.Groups[rgxModel.GetKeyValue("RGX_ide_indPag")].Value.Trim();
                        _nota.infNFe.ide.mod = geral.Groups[rgxModel.GetKeyValue("RGX_ide_mode")].Value.Trim();
                        _nota.infNFe.ide.natOp = geral.Groups[rgxModel.GetKeyValue("RGX_ide_natOp")].Value.Trim();
                        _nota.infNFe.ide.nNF = geral.Groups[rgxModel.GetKeyValue("RGX_ide_nNF")].Value.Trim();
                        _nota.infNFe.ide.NumeroNfseSubstituida =
                            geral.Groups[rgxModel.GetKeyValue("RGX_ide_NumeroNfseSubstituida")].Value.Trim();
                        _nota.infNFe.ide.NumeroRps = geral.Groups[rgxModel.GetKeyValue("RGX_ide_NumeroRps")].Value.Trim();
                        _nota.infNFe.ide.NumeroRpsSubstituido =
                            geral.Groups[rgxModel.GetKeyValue("RGX_ide_NumeroRpsSubstituido")].Value.Trim();
                        _nota.infNFe.ide.procEmi = geral.Groups[rgxModel.GetKeyValue("RGX_ide_procEmi")].Value.Trim();
                        _nota.infNFe.ide.serie = geral.Groups[rgxModel.GetKeyValue("RGX_ide_serie")].Value.Trim();
                        _nota.infNFe.ide.SerieRps = geral.Groups[rgxModel.GetKeyValue("RGX_ide_SerieRps")].Value.Trim();
                        _nota.infNFe.ide.SerieRpsSubstituido =
                            geral.Groups[rgxModel.GetKeyValue("RGX_ide_SerieRspSubstituido")].Value.Trim();
                        _nota.infNFe.ide.TipoRps = geral.Groups[rgxModel.GetKeyValue("RGX_ide_NumeroRps")].Value.Trim();
                        _nota.infNFe.ide.TipoRpsSubstituido =
                            geral.Groups[rgxModel.GetKeyValue("RGX_ide_TipoRpsSubstituido")].Value.Trim();
                        _nota.infNFe.ide.tpAmb = geral.Groups[rgxModel.GetKeyValue("RGX_ide_tpAmb")].Value.Trim();
                        _nota.infNFe.ide.tpEmis = "0";
                        //alteração pedida por fábio em 10/07 via skype//Util.FormataData(geral.Groups[rgxModel.GetKeyValue("RGX_ide_dEmi")].Value.Trim());
                        _nota.infNFe.ide.tpImp = geral.Groups[rgxModel.GetKeyValue("RGX_ide_tpImp")].Value.Trim();
                        _nota.infNFe.ide.tpNF = geral.Groups[rgxModel.GetKeyValue("RGX_ide_tpNF")].Value.Trim();

                        #endregion

                        #region _nota.infNFe.infAdic

                        _nota.infNFe.infAdic.infAdFisco =
                            geral.Groups[rgxModel.GetKeyValue("RGX_infAdic_infAdFisco")].Value.Trim();
                        _nota.infNFe.infAdic.infCpl = geral.Groups[rgxModel.GetKeyValue("RGX_infAdic_infCpl")].Value.Trim();

                        #endregion

                        #region _nota.infNFe.total

                        _nota.infNFe.total.ICMSTot.Aliquota =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_Aliquota")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.DescontoCondicionado =
                            Util.FormataDecimal(
                                geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_DescontoCondicionado")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.DescontoIncondicionado =
                            Util.FormataDecimal(
                                geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_DescontoIncondicionado")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.ISSRetido =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_ISSRetido")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.OutrasRetencoes =
                            Util.FormataDecimal(
                                geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_OutrasRetencoes")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.ValorIss =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_ValorIss")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vBC =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vBC")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vBCST =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vBCST")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vCOFINS =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vCOFINS")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vCSLL =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vCSLL")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vDeducoes =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vDeducoes")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vDesc =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vDesc")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vFrete =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vFrete")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vICMS =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vICMS")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vII =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vII")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vINSS =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vINSS")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vIPI =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vIPI")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vIR =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vIR")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vLiquidoNfse =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vLiquidoNfse")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vNF =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vNF")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vPIS =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vPIS")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vProd =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vProd")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vSeg =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vSeg")].Value.Trim());
                        _nota.infNFe.total.ICMSTot.vServicos =
                            Util.FormataDecimal(geral.Groups[rgxModel.GetKeyValue("RGX_ICMSTot_vServicos")].Value.Trim());

                        #endregion

                        #region _nota.infNFe.transp

                        //_nota.infNFe.transp = "";
                        _nota.infNFe.transp.modFrete = "";
                        //_nota.infNFe.transp.transporta = "";
                        _nota.infNFe.transp.transporta.IE =
                            Util.LimpaCampos(geral.Groups[rgxModel.GetKeyValue("RGX_transporta_IE")].Value.Trim());
                        _nota.infNFe.transp.transporta.UF =
                            geral.Groups[rgxModel.GetKeyValue("RGX_transporta_UF")].Value.Trim();
                        _nota.infNFe.transp.transporta.xEnder =
                            geral.Groups[rgxModel.GetKeyValue("RGX_transporta_xEnder")].Value.Trim();
                        _nota.infNFe.transp.transporta.xMun =
                            geral.Groups[rgxModel.GetKeyValue("RGX_transporta_xMun")].Value.Trim();
                        _nota.infNFe.transp.transporta.xNome =
                            geral.Groups[rgxModel.GetKeyValue("RGX_transporta_xNome")].Value.Trim();

                        _nota.infNFe.transp.vol.esp = geral.Groups[rgxModel.GetKeyValue("RGX_vol_esp")].Value.Trim();

                        #endregion

                        #region _nota.infNFe.versao

                        _nota.infNFe.versao = geral.Groups[rgxModel.GetKeyValue("RGX_transp_modFrete")].Value.Trim();

                        #endregion

                        //brasil para pais padrão
                        if (String.IsNullOrEmpty(_nota.infNFe.emit.enderEmit.cPais))
                            _nota.infNFe.emit.enderEmit.cPais = "1058";
                        if (String.IsNullOrEmpty(_nota.infNFe.emit.enderEmit.xPais))
                            _nota.infNFe.emit.enderEmit.xPais = "Brasil";
                        if (String.IsNullOrEmpty(_nota.infNFe.dest.enderDest.cPais))
                            _nota.infNFe.dest.enderDest.cPais = "1058";
                        if (String.IsNullOrEmpty(_nota.infNFe.dest.enderDest.xPais))
                            _nota.infNFe.dest.enderDest.xPais = "Brasil";

                    #endregion

                        //==============================================

                        //==============================================

                        #region _nota.protNFe

                        #region _nota.protNFe.infProt;

                        _nota.protNFe.infProt.chNFe = geral.Groups[rgxModel.GetKeyValue("RGX_infProt_chNFe")].Value.Trim();
                        _nota.protNFe.infProt.dhRecbto =
                            geral.Groups[rgxModel.GetKeyValue("RGX_infProt_dhRecbto")].Value.Trim();
                        _nota.protNFe.infProt.tpAmb = geral.Groups[rgxModel.GetKeyValue("RGX_infProt_tpAmb")].Value.Trim();

                        #endregion

                        #region _nota.protNFe.versao;

                        _nota.protNFe.versao = "";

                        #endregion

                        #endregion

                        //==============================================
                    }
                    else
                    {
                        temErro = true;
                    }

                    if (temErro)
                    {
                        break;
                    }
                }
                else
                {
                    temErro = true;
                }
            }

            if (temErro)
                _nota = null;

            return _nota;
        }
    }
}
