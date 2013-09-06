using HtmlAgilityPack;
using Ionic.Zip;
using Leitor.Dao;
using Leitor.Model;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Leitor.Core
{
    public class Parametrizador
    {
        private NF _nota = new NF();
        private Remetente _remetente;
        private RegexModel _rgxModel = new RegexModel();
        private string EouS;

        private void Parametrizar(String arquivo, Remetente r)
        {
            _remetente = r;
            //                          1               2                           3                               4                           5                                       6       7               8           9       10          11          12          13          14          15      16          17          18      19      20                  21      22          23          24          25
            //Regex rxGeral = new Regex("Valor do ISS\\(R\\$\\)\\s*(.*?)\\s(\\d{1,2}/\\d{1,2}/\\d{1,4})\\s(\\d{1,2}:\\d{1,2}:\\d{1,2})\\s*(\\d{1,2}/\\d{1,2}/\\d{1,4})\\s(.*?)\\s*\\d{1,2}/\\d{1,2}/\\d{1,4}\\s*(\\d*)\\s*(.{2,20})\\s*(\\S*)\\s*(.*?)\\s\\s(.*?)\\s\\s(.*?)\\s\\s+(\\S*)\\s\\s+(.*?)\\s\\s(.*?)\\s\\s+(.*?)\\s+(.{1,3})\\s(.*?)\\s\\s(\\S+)\\s(.*?)\\s\\s+(\\S+)\\s\\s+(.{1,33})\\s(.*?)\\s\\s+(.*?)\\s\\s+(.{1,2})\\s(.*?)RPS: (.*?)\\sMês de Competência da Nota Fiscal: (.*?)\\s\\s+Local da Prestação do Serviço: (.*?)\\sRecolhimento: (.*?)\\s+Tributação: (.*?) RPS: (.*?) CNAE: (.*?) CODIGO SERVICO: (.*?) ", RegexOptions.Singleline);
            //String a = rxGeral.Match(arquivo).Groups[26].Value.Trim();
            RegexesDAO dao = new RegexesDAO();
            _rgxModel = dao.SelecionarRegexPorRemetenteId(r.Id);

            if (_rgxModel.IsValid() && !_rgxModel.IsXpath)
            {

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
                Match geral = new Regex(_rgxModel.Geral, RegexOptions.Singleline).Match(arquivo);

                EouS = geral.Groups[_rgxModel.GetKeyValue("RGX_ES")].Value.Trim();
                _nota.infNFe.NFeNFSe = geral.Groups[_rgxModel.GetKeyValue("RGX_NFeNFSe")].Value.Trim();
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

                string prodGroup = geral.Groups[_rgxModel.GetKeyValue("RGX_PROD")].Value.Trim();
                _nota.infNFe.cobr.dup.dVenc = Util.FormataData(geral.Groups[_rgxModel.GetKeyValue("RGX_dup_dVenc")].Value.Trim());
                _nota.infNFe.cobr.dup.nDup = geral.Groups[_rgxModel.GetKeyValue("RGX_dup_nDup")].Value.Trim();
                _nota.infNFe.cobr.dup.vDup = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_dup_vDup")].Value.Trim());

                _nota.infNFe.cobr.fat.nFat = geral.Groups[_rgxModel.GetKeyValue("RGX_fat_nFat")].Value.Trim();
                _nota.infNFe.cobr.fat.vDesc = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_fat_vDesc")].Value.Trim());
                _nota.infNFe.cobr.fat.vLiq = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_fat_vLiq")].Value.Trim());
                _nota.infNFe.cobr.fat.vOrig = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_fat_vOrig")].Value.Trim());
                #endregion

                #region _nota.infNFe.dest
                /*
                 [RGX_dest_CNPJ]
                  ,[RGX_dest_xNome]
                  ,[RGX_dest_xLgr]
                  ,[RGX_dest_xBairro]
                  ,[RGX_dest_cMun]
                  ,[RGX_dest_xMun]
                  ,[RGX_dest_UF]
                  ,[RGX_dest_CEP]
                  ,[RGX_dest_cPais]
                  ,[RGX_dest_xPais]
                  ,[RGX_dest_fone]
                  ,[RGX_dest_IE]
                  ,[RGX_dest_IM]
                  ,[RGX_dest_email]
                  ,[RGX_det_infAdic]
                 */

                _nota.infNFe.dest.CNPJ = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_dest_CNPJ")].Value.Trim());
                _nota.infNFe.dest.email = Util.SeparaEmails(geral.Groups[_rgxModel.GetKeyValue("RGX_dest_email")].Value.Trim());

                //_nota.infNFe.dest.enderDest = "";
                _nota.infNFe.dest.enderDest.CEP = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_dest_CEP")].Value.Trim());
                _nota.infNFe.dest.enderDest.cMun = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_dest_cMun")].Value.Trim());
                _nota.infNFe.dest.enderDest.cPais = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_dest_cPais")].Value.Trim());
                _nota.infNFe.dest.enderDest.fone = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_dest_fone")].Value.Trim());
                _nota.infNFe.dest.enderDest.UF = geral.Groups[_rgxModel.GetKeyValue("RGX_dest_UF")].Value.Trim();
                _nota.infNFe.dest.enderDest.xBairro = geral.Groups[_rgxModel.GetKeyValue("RGX_dest_xBairro")].Value.Trim();
                _nota.infNFe.dest.enderDest.xLgr = geral.Groups[_rgxModel.GetKeyValue("RGX_dest_xLgr")].Value.Trim();
                _nota.infNFe.dest.enderDest.xMun = geral.Groups[_rgxModel.GetKeyValue("RGX_dest_xMun")].Value.Trim();
                _nota.infNFe.dest.enderDest.xPais = geral.Groups[_rgxModel.GetKeyValue("RGX_dest_xPais")].Value.Trim();

                _nota.infNFe.dest.IE = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_dest_IE")].Value.Trim());
                _nota.infNFe.dest.IM = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_dest_IM")].Value.Trim());
                _nota.infNFe.dest.xNome = geral.Groups[_rgxModel.GetKeyValue("RGX_dest_xNome")].Value.Trim();
                #endregion

                #region d

                int k = 1;
                foreach (Match item in new Regex(_rgxModel.Item, RegexOptions.Singleline).Matches(prodGroup))
                {
                    Det d = new Det();

                    /*
                     [RGX_COFINS_CST]
                      ,[RGX_COFINS_vBC]
                      ,[RGX_COFINS_pCOFINS]
                      ,[RGX_COFINS_vCOFINS]
                     */

                    //d.imposto = "";
                    //d.imposto.COFINS = "";
                    d.imposto.COFINS.CST = item.Groups[_rgxModel.GetKeyValue("RGX_COFINS_CST")].Value.Trim();
                    d.imposto.COFINS.pCOFINS =
                        item.Groups[_rgxModel.GetKeyValue("RGX_COFINS_pCOFINS")].Value.Trim();
                    d.imposto.COFINS.vBC = item.Groups[_rgxModel.GetKeyValue("RGX_COFINS_vBC")].Value.Trim();
                    d.imposto.COFINS.vCOFINS =
                        item.Groups[_rgxModel.GetKeyValue("RGX_COFINS_vCOFINS")].Value.Trim();

                    /*
                     * [RGX_ICMS_orig]
                          ,[RGX_ICMS_CST]
                          ,[RGX_ICMS_CSOSN]
                          ,[RGX_ICMS_modBC]
                          ,[RGX_ICMS_vBC]
                          ,[RGX_ICMS_pRedBC]
                          ,[RGX_ICMS_pICMS]
                          ,[RGX_ICMS_vICMS]
                          ,[RGX_ICMS_modBCST]
                          ,[RGX_ICMS_pMVAST]
                          ,[RGX_ICMS_pRedBCST]
                          ,[RGX_ICMS_vBCST]
                          ,[RGX_ICMS_pICMSST]
                          ,[RGX_ICMS_vICMSST]
                          ,[RGX_ICMS_pCredSN]
                          ,[RGX_ICMS_vCredICMSSN]
                          ,[RGX_ICMS_motDesICMS]
                     */
                    //d.imposto.ICMS = "";
                    d.imposto.ICMS.CSOSN = item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_CSOSN")].Value.Trim();
                    d.imposto.ICMS.CST = item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_CST")].Value.Trim();
                    d.imposto.ICMS.modBC = item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_modBC")].Value.Trim();
                    d.imposto.ICMS.modBCST =
                        item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_modBCST")].Value.Trim();
                    d.imposto.ICMS.motDesICMS =
                        item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_motDesICMS")].Value.Trim();
                    d.imposto.ICMS.orig = item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_orig")].Value.Trim();
                    d.imposto.ICMS.pCredSN =
                        item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_pCredSN")].Value.Trim();
                    d.imposto.ICMS.pICMSST =
                        item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_vICMSST")].Value.Trim();
                    d.imposto.ICMS.pMVAST = item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_pMVAST")].Value.Trim();
                    d.imposto.ICMS.pRedBC = item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_pRedBC")].Value.Trim();
                    d.imposto.ICMS.pRedBCST =
                        item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_pRedBCST")].Value.Trim();
                    d.imposto.ICMS.vBC = Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_vBC")].Value.Trim());
                    d.imposto.ICMS.vBCST = Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_vBCST")].Value.Trim());
                    d.imposto.ICMS.vCredICMSSN =
                        item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_vCredICMSSN")].Value.Trim();
                    d.imposto.ICMS.vICMS = Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_vICMS")].Value.Trim());
                    d.imposto.ICMS.vICMSST =
                        Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_ICMS_vICMSST")].Value.Trim());

                    /*
                     * [RGX_II_vBC]
                      ,[RGX_II_vDespAdu]
                      ,[RGX_II_vII]
                      ,[RGX_II_vIOF]
                     */
                    //d.imposto.II = "";
                    d.imposto.II.vBC = item.Groups[_rgxModel.GetKeyValue("RGX_II_vBC")].Value.Trim();
                    d.imposto.II.vDespAdu = item.Groups[_rgxModel.GetKeyValue("RGX_II_vDespAdu")].Value.Trim();
                    d.imposto.II.vII = item.Groups[_rgxModel.GetKeyValue("RGX_II_vII")].Value.Trim();
                    d.imposto.II.vIOF = item.Groups[_rgxModel.GetKeyValue("RGX_II_vIOF")].Value.Trim();

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
                    d.imposto.IPI.cEnq = item.Groups[_rgxModel.GetKeyValue("RGX_IPI_cEnq")].Value.Trim();
                    d.imposto.IPI.cIEnq = item.Groups[_rgxModel.GetKeyValue("RGX_IPI_cIEnq")].Value.Trim();
                    d.imposto.IPI.CNPJProd =
                        item.Groups[_rgxModel.GetKeyValue("RGX_IPI_CNPJProd")].Value.Trim();
                    d.imposto.IPI.cSelo = item.Groups[_rgxModel.GetKeyValue("RGX_IPI_cSelo")].Value.Trim();
                    //d.imposto.IPI.IPITrib = "";
                    d.imposto.IPI.IPITrib.CST =
                        item.Groups[_rgxModel.GetKeyValue("RGX_IPITrib_CST")].Value.Trim();
                    d.imposto.IPI.IPITrib.pIPI =
                        item.Groups[_rgxModel.GetKeyValue("RGX_IPITrib_pIPI")].Value.Trim();
                    d.imposto.IPI.IPITrib.qUnid =
                        item.Groups[_rgxModel.GetKeyValue("RGX_IPITrib_qUnid")].Value.Trim();
                    d.imposto.IPI.IPITrib.vBC =
                        item.Groups[_rgxModel.GetKeyValue("RGX_IPITrib_vBC")].Value.Trim();
                    d.imposto.IPI.IPITrib.vIPI =
                        Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_IPITrib_vIPI")].Value.Trim());
                    d.imposto.IPI.IPITrib.vUnid =
                        Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_IPITrib_vUnid")].Value.Trim());

                    d.imposto.IPI.qSelo = item.Groups[_rgxModel.GetKeyValue("RGX_IPI_qSelo")].Value.Trim();

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
                        item.Groups[_rgxModel.GetKeyValue("RGX_ISSQN_cListServ")].Value.Trim();
                    d.imposto.ISSQN.cMunFG =
                        Util.LimpaCampos(item.Groups[_rgxModel.GetKeyValue("RGX_ISSQN_cMunFG")].Value.Trim());
                    d.imposto.ISSQN.cSitTrib =
                        item.Groups[_rgxModel.GetKeyValue("RGX_ISSQN_cSitTrib")].Value.Trim();
                    d.imposto.ISSQN.vAliq = Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_ISSQN_vAliq")].Value.Trim());
                    d.imposto.ISSQN.vBC = Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_ISSQN_vBC")].Value.Trim());
                    d.imposto.ISSQN.vISSQN =
                        Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_ISSQN_vISSQN")].Value.Trim());

                    /*
                     * [RGX_PIS_CST]
                      ,[RGX_PIS_vBC]
                      ,[RGX_PIS_pPIS]
                      ,[RGX_PIS_vPIS]
                     */
                    //d.imposto.PIS = "";
                    d.imposto.PIS.CST = item.Groups[_rgxModel.GetKeyValue("RGX_PIS_CS")].Value.Trim();
                    d.imposto.PIS.pPIS = item.Groups[_rgxModel.GetKeyValue("RGX_PIS_pPIS")].Value.Trim();
                    d.imposto.PIS.vBC = Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_PIS_vBC")].Value.Trim());
                    d.imposto.PIS.vPIS = Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_PIS_vPIS")].Value.Trim());

                    d.infAdic = item.Groups[_rgxModel.GetKeyValue("RGX_det_infAdic")].Value.Trim();


                    //produtos
                    d.nItem = k++.ToString();
                    d.prod.cEAN = item.Groups[_rgxModel.GetKeyValue("RGX_prod_cEAN")].Value.Trim();
                    d.prod.cEANTrib = item.Groups[_rgxModel.GetKeyValue("RGX_prod_cEANTrib")].Value.Trim();
                    d.prod.CFOP = item.Groups[_rgxModel.GetKeyValue("RGX_prod_CFOP")].Value.Trim();
                    d.prod.cProd = item.Groups[_rgxModel.GetKeyValue("RGX_prod_cProd")].Value.Trim();
                    d.prod.indTot = item.Groups[_rgxModel.GetKeyValue("RGX_prod_indTot")].Value.Trim();
                    d.prod.NCM = item.Groups[_rgxModel.GetKeyValue("RGX_prod_NCM")].Value.Trim();
                    d.prod.qCom = Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_prod_qCom")].Value.Trim());

                    //servicos
                    d.prod.servico.CodigoCnae = item.Groups[_rgxModel.GetKeyValue("RGX_servico_CodigoCnae")].Value.Trim();
                    d.prod.servico.CodigoMunicipio = item.Groups[_rgxModel.GetKeyValue("RGX_servico_CodigoMunicipio")].Value.Trim();
                    d.prod.servico.CodigoTributacaoMunicipio = item.Groups[_rgxModel.GetKeyValue("RGX_servico_CodigoTributacaoMunicipio")].Value.Trim();
                    d.prod.servico.competencia = item.Groups[_rgxModel.GetKeyValue("RGX_servico_competencia")].Value.Trim();
                    d.prod.servico.Discriminacao = item.Groups[_rgxModel.GetKeyValue("RGX_servico_Discriminacao")].Value.Trim();
                    d.prod.servico.ItemListaServico = item.Groups[_rgxModel.GetKeyValue("RGX_servico_ItemListaServico")].Value.Trim();
                    d.prod.servico.MunicipioIncidencia = item.Groups[_rgxModel.GetKeyValue("RGX_servico_MunicipioIncidencia")].Value.Trim();

                    d.prod.uCom = item.Groups[_rgxModel.GetKeyValue("RGX_prod_uCom")].Value.Trim();
                    d.prod.vProd = Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_prod_vProd")].Value.Trim());
                    d.prod.vUnCom = Util.FormataDecimal(item.Groups[_rgxModel.GetKeyValue("RGX_prod_vUnCom")].Value.Trim());
                    d.prod.xProd = item.Groups[_rgxModel.GetKeyValue("RGX_prod_xProd")].Value.Trim();
                    _nota.infNFe.det.Add(d);
                }
                if (string.IsNullOrEmpty(_nota.infNFe.det[0].prod.servico.Discriminacao))
                    _nota.infNFe.det[0].prod.servico.Discriminacao =
                        geral.Groups[_rgxModel.GetKeyValue("RGX_servico_Discriminacao")].Value.Trim();

                #endregion

                #region produtos
                //foreach (Match item in new Regex(_rgxModel.Item, RegexOptions.Singleline).Matches(arquivo))
                //{

                //    Prod prod = new Prod();

                //    //d.nItem = "";

                //    prod.cEAN = item.Groups[_rgxModel.GetKeyValue("RGX_prod_cEAN")].Value.Trim();
                //    prod.cEANTrib = item.Groups[_rgxModel.GetKeyValue("RGX_prod_cEANTrib")].Value.Trim();
                //    prod.CFOP = item.Groups[_rgxModel.GetKeyValue("RGX_prod_CFOP")].Value.Trim();
                //    prod.cProd = item.Groups[_rgxModel.GetKeyValue("RGX_prod_cProd")].Value.Trim();
                //    prod.indTot = item.Groups[_rgxModel.GetKeyValue("RGX_prod_indTot")].Value.Trim();
                //    prod.NCM = item.Groups[_rgxModel.GetKeyValue("RGX_prod_NCM")].Value.Trim();
                //    prod.qCom = item.Groups[_rgxModel.GetKeyValue("RGX_prod_qCom")].Value.Trim();

                //    prod.servico.CodigoCnae = item.Groups[_rgxModel.GetKeyValue("RGX_servico_CodigoCnae")].Value.Trim();
                //    prod.servico.CodigoMunicipio = item.Groups[_rgxModel.GetKeyValue("RGX_servico_CodigoMunicipio")].Value.Trim();
                //    prod.servico.CodigoTributacaoMunicipio = item.Groups[_rgxModel.GetKeyValue("RGX_servico_CodigoTributacaoMunicipio")].Value.Trim();
                //    prod.servico.competencia = item.Groups[_rgxModel.GetKeyValue("RGX_servico_competencia")].Value.Trim();
                //    prod.servico.Discriminacao = item.Groups[_rgxModel.GetKeyValue("RGX_servico_Discriminacao")].Value.Trim();
                //    prod.servico.ItemListaServico = item.Groups[_rgxModel.GetKeyValue("RGX_servico_ItemListaServico")].Value.Trim();
                //    prod.servico.MunicipioIncidencia = item.Groups[_rgxModel.GetKeyValue("RGX_servico_MunicipioIncidencia")].Value.Trim();

                //    prod.uCom = item.Groups[_rgxModel.GetKeyValue("RGX_prod_uCom")].Value.Trim();
                //    prod.vProd = item.Groups[_rgxModel.GetKeyValue("RGX_prod_vProd")].Value.Trim();
                //    prod.vUnCom = item.Groups[_rgxModel.GetKeyValue("RGX_prod_vUnCom")].Value.Trim();
                //    prod.xProd = item.Groups[_rgxModel.GetKeyValue("RGX_prod_xProd")].Value.Trim();

                //    //d.nItem = "";

                //    //d.prod = "";
                //    //d.prod.cEAN = item.Groups[_rgxModel.GetKeyValue("RGX_prod_cEAN")].Value.Trim();
                //    //d.prod.cEANTrib = item.Groups[_rgxModel.GetKeyValue("RGX_prod_cEANTrib")].Value.Trim();
                //    //d.prod.CFOP = item.Groups[_rgxModel.GetKeyValue("RGX_prod_CFOP")].Value.Trim();
                //    //d.prod.cProd = item.Groups[_rgxModel.GetKeyValue("RGX_prod_cProd")].Value.Trim();
                //    //d.prod.indTot = item.Groups[_rgxModel.GetKeyValue("RGX_prod_indTot")].Value.Trim();
                //    //d.prod.NCM = item.Groups[_rgxModel.GetKeyValue("RGX_prod_NCM")].Value.Trim();
                //    //d.prod.qCom = item.Groups[_rgxModel.GetKeyValue("RGX_prod_qCom")].Value.Trim();

                //    //d.prod.servico = "";
                //    //d.prod.servico.CodigoCnae = item.Groups[_rgxModel.GetKeyValue("RGX_servico_CodigoCnae")].Value.Trim();
                //    //d.prod.servico.CodigoMunicipio = item.Groups[_rgxModel.GetKeyValue("RGX_servico_CodigoMunicipio")].Value.Trim();
                //    //d.prod.servico.CodigoTributacaoMunicipio = item.Groups[_rgxModel.GetKeyValue("RGX_servico_CodigoTributacaoMunicipio")].Value.Trim();
                //    //d.prod.servico.competencia = item.Groups[_rgxModel.GetKeyValue("RGX_servico_competencia")].Value.Trim();
                //    //d.prod.servico.Discriminacao = item.Groups[_rgxModel.GetKeyValue("RGX_servico_Discriminacao")].Value.Trim();
                //    //d.prod.servico.ItemListaServico = item.Groups[_rgxModel.GetKeyValue("RGX_servico_ItemListaServico")].Value.Trim();
                //    //d.prod.servico.MunicipioIncidencia = item.Groups[_rgxModel.GetKeyValue("RGX_servico_MunicipioIncidencia")].Value.Trim();

                //    //d.prod.uCom = item.Groups[_rgxModel.GetKeyValue("RGX_prod_uCom")].Value.Trim();
                //    //d.prod.vProd = item.Groups[_rgxModel.GetKeyValue("RGX_prod_vProd")].Value.Trim();
                //    //d.prod.vUnCom = item.Groups[_rgxModel.GetKeyValue("RGX_prod_vUnCom")].Value.Trim();
                //    //d.prod.xProd = item.Groups[_rgxModel.GetKeyValue("RGX_prod_xProd")].Value.Trim();

                //    break;
                //}
                #endregion

                #region _nota.infNFe.emit

                /*
                 ,[RGX_emit_CNPJ]
                  ,[RGX_emit_xNome]
                  ,[RGX_emit_xFant]
                  ,[RGX_emit_xLgr]
                  ,[RGX_emit_xBairro]
                  ,[RGX_emit_cMun]
                  ,[RGX_emit_xMun]
                  ,[RGX_emit_UF]
                  ,[RGX_emit_CEP]
                  ,[RGX_emit_cPais]
                  ,[RGX_emit_xPais]
                  ,[RGX_emit_fone]
                  ,[RGX_emit_IE]
                  ,[RGX_emit_IM]
                  ,[RGX_emit_CRT]
                 */

                _nota.infNFe.emit.CNPJ = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_emit_CNPJ")].Value.Trim());
                _nota.infNFe.emit.CRT = geral.Groups[_rgxModel.GetKeyValue("RGX_emit_CRT")].Value.Trim();

                //_nota.infNFe.emit.enderEmit = "";
                _nota.infNFe.emit.enderEmit.CEP = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_emit_CEP")].Value.Trim());
                _nota.infNFe.emit.enderEmit.cMun = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_emit_cMun")].Value.Trim());
                _nota.infNFe.emit.enderEmit.cPais = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_emit_cPais")].Value.Trim());
                _nota.infNFe.emit.enderEmit.fone = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_emit_fone")].Value.Trim());
                _nota.infNFe.emit.enderEmit.UF = geral.Groups[_rgxModel.GetKeyValue("RGX_emit_UF")].Value.Trim();
                _nota.infNFe.emit.enderEmit.xBairro = geral.Groups[_rgxModel.GetKeyValue("RGX_emit_xBairro")].Value.Trim();
                _nota.infNFe.emit.enderEmit.xLgr = geral.Groups[_rgxModel.GetKeyValue("RGX_emit_xLgr")].Value.Trim();
                _nota.infNFe.emit.enderEmit.xMun = geral.Groups[_rgxModel.GetKeyValue("RGX_emit_xMun")].Value.Trim();
                _nota.infNFe.emit.enderEmit.xPais = geral.Groups[_rgxModel.GetKeyValue("RGX_emit_xPais")].Value.Trim();

                _nota.infNFe.emit.IE = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_emit_IE")].Value.Trim());
                _nota.infNFe.emit.IM = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_emit_IM")].Value.Trim());
                _nota.infNFe.emit.xFant = geral.Groups[_rgxModel.GetKeyValue("RGX_emit_xFant")].Value.Trim();
                _nota.infNFe.emit.xNome = geral.Groups[_rgxModel.GetKeyValue("RGX_emit_xNome")].Value.Trim();

                #endregion

                #region _nota.infNFe.ide

                /*
                 [RGX_ide_cUf]
                  ,[RGX_ide_cNF]
                  ,[RGX_ide_indPag]
                  ,[RGX_ide_natOp]
                  ,[RGX_ide_mode]
                  ,[RGX_ide_serie]
                  ,[RGX_ide_nNF]
                  ,[RGX_ide_dEmi]
                  ,[RGX_ide_tpNF]
                  ,[RGX_ide_cMunFG]
                  ,[RGX_ide_tpImp]
                  ,[RGX_ide_tpEmi]
                  ,[RGX_ide_cDV]
                  ,[RGX_ide_tpAmb]
                  ,[RGX_ide_finNFe]
                  ,[RGX_ide_procEmi]
                  ,[RGX_ide_NumeroNfseSubstituida]
                  ,[RGX_ide_NumeroRps]
                  ,[RGX_ide_SerieRps]
                  ,[RGX_ide_DataEmissaoRps]
                  ,[RGX_ide_NumeroRpsSubstituido]
                  ,[RGX_ide_SerieRspSubstituido]
                  ,[RGX_ide_TipoRpsSubstituido]
                 */

                _nota.infNFe.ide.cDV = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_cDV")].Value.Trim();
                _nota.infNFe.ide.cMunFG = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_ide_cMunFG")].Value.Trim());
                _nota.infNFe.ide.cNF = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_cNF")].Value.Trim();
                _nota.infNFe.ide.cUf = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_ide_cUf")].Value.Trim());
                _nota.infNFe.ide.DataEmissaoRps = Util.FormataData(geral.Groups[_rgxModel.GetKeyValue("RGX_ide_DataEmissaoRps")].Value.Trim());
                _nota.infNFe.ide.dEmi = Util.FormataData(geral.Groups[_rgxModel.GetKeyValue("RGX_ide_dEmi")].Value.Trim());
                _nota.infNFe.ide.finNFe = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_finNFe")].Value.Trim();
                _nota.infNFe.ide.indPag = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_indPag")].Value.Trim();
                _nota.infNFe.ide.mod = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_mode")].Value.Trim();
                _nota.infNFe.ide.natOp = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_natOp")].Value.Trim();
                _nota.infNFe.ide.nNF = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_nNF")].Value.Trim();
                _nota.infNFe.ide.NumeroNfseSubstituida = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_NumeroNfseSubstituida")].Value.Trim();
                _nota.infNFe.ide.NumeroRps = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_NumeroRps")].Value.Trim();
                _nota.infNFe.ide.NumeroRpsSubstituido = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_NumeroRpsSubstituido")].Value.Trim();
                _nota.infNFe.ide.procEmi = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_procEmi")].Value.Trim();
                _nota.infNFe.ide.serie = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_serie")].Value.Trim();
                _nota.infNFe.ide.SerieRps = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_SerieRps")].Value.Trim();
                _nota.infNFe.ide.SerieRpsSubstituido = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_SerieRspSubstituido")].Value.Trim();
                _nota.infNFe.ide.TipoRps = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_NumeroRps")].Value.Trim();
                _nota.infNFe.ide.TipoRpsSubstituido = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_TipoRpsSubstituido")].Value.Trim();
                _nota.infNFe.ide.tpAmb = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_tpAmb")].Value.Trim();
                _nota.infNFe.ide.tpEmis = Util.FormataData(geral.Groups[_rgxModel.GetKeyValue("RGX_ide_dEmi")].Value.Trim());
                _nota.infNFe.ide.tpImp = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_tpImp")].Value.Trim();
                _nota.infNFe.ide.tpNF = geral.Groups[_rgxModel.GetKeyValue("RGX_ide_tpNF")].Value.Trim();

                #endregion

                #region _nota.infNFe.infAdic

                _nota.infNFe.infAdic.infAdFisco = geral.Groups[_rgxModel.GetKeyValue("RGX_infAdic_infAdFisco")].Value.Trim();
                _nota.infNFe.infAdic.infCpl = geral.Groups[_rgxModel.GetKeyValue("RGX_infAdic_infCpl")].Value.Trim();

                #endregion

                #region _nota.infNFe.total

                /*
                 ,[RGX_ICMSTot_vBC]
                  ,[RGX_ICMSTot_vICMS]
                  ,[RGX_ICMSTot_vBCST]
                  ,[RGX_ICMSTot_vProd]
                  ,[RGX_ICMSTot_vFrete]
                  ,[RGX_ICMSTot_vSeg]
                  ,[RGX_ICMSTot_vDesc]
                  ,[RGX_ICMSTot_vII]
                  ,[RGX_ICMSTot_vIPI]
                  ,[RGX_ICMSTot_vPIS]
                  ,[RGX_ICMSTot_vCOFINS]
                  ,[RGX_ICMSTot_vNF]
                  ,[RGX_ICMSTot_vServicos]
                  ,[RGX_ICMSTot_vDeducoes]
                  ,[RGX_ICMSTot_vINSS]
                  ,[RGX_ICMSTot_vIR]
                  ,[RGX_ICMSTot_vCSLL]
                  ,[RGX_ICMSTot_ValorIss]
                  ,[RGX_ICMSTot_OutrasRetencoes]
                  ,[RGX_ICMSTot_Aliquota]
                  ,[RGX_ICMSTot_DescontoIncondicionado]
                  ,[RGX_ICMSTot_DescontoCondicionado]
                  ,[RGX_ICMSTot_ISSRetido]
                  ,[RGX_ICMSTot_vLiquidoNfse]
                 */
                _nota.infNFe.total.ICMSTot.Aliquota = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_Aliquota")].Value.Trim());
                _nota.infNFe.total.ICMSTot.DescontoCondicionado = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_DescontoCondicionado")].Value.Trim());
                _nota.infNFe.total.ICMSTot.DescontoIncondicionado = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_DescontoIncondicionado")].Value.Trim());
                _nota.infNFe.total.ICMSTot.ISSRetido = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_ISSRetido")].Value.Trim());
                _nota.infNFe.total.ICMSTot.OutrasRetencoes = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_OutrasRetencoes")].Value.Trim());
                _nota.infNFe.total.ICMSTot.ValorIss = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_ValorIss")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vBC = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vBC")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vBCST = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vBCST")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vCOFINS = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vCOFINS")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vCSLL = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vCSLL")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vDeducoes = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vDeducoes")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vDesc = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vDesc")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vFrete = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vFrete")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vICMS = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vICMS")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vII = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vII")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vINSS = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vINSS")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vIPI = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vIPI")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vIR = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vIR")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vLiquidoNfse = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vLiquidoNfse")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vNF = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vNF")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vPIS = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vPIS")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vProd = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vProd")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vSeg = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vSeg")].Value.Trim());
                _nota.infNFe.total.ICMSTot.vServicos = Util.FormataDecimal(geral.Groups[_rgxModel.GetKeyValue("RGX_ICMSTot_vServicos")].Value.Trim());
                #endregion

                #region _nota.infNFe.transp
                /*
                  ,[RGX_vol_esp]
                  ,[RGX_fat_nFat]
                  ,[RGX_fat_vOrig]
                  ,[RGX_fat_vLiq]
                  ,[RGX_fat_vDesc]
                  ,[RGX_dup_nDup]
                  ,[RGX_dup_dVenc]
                  ,[RGX_dup_vDup]
                  ,[RGX_infAdic_infAdFisco]
                  ,[RGX_infAdic_infCpl]
                  ,[RGX_infProt_tpAmb]
                  ,[RGX_infProt_chNFe]
                  ,[RGX_infProt_dhRecbto]
                 */

                /*
                 ,[RGX_transporta_xNome]
                  ,[RGX_transporta_IE]
                  ,[RGX_transporta_xEnder]
                  ,[RGX_transporta_xMun]
                  ,[RGX_transporta_UF]
                 */

                //_nota.infNFe.transp = "";
                _nota.infNFe.transp.modFrete = "";
                //_nota.infNFe.transp.transporta = "";
                _nota.infNFe.transp.transporta.IE = Util.LimpaCampos(geral.Groups[_rgxModel.GetKeyValue("RGX_transporta_IE")].Value.Trim());
                _nota.infNFe.transp.transporta.UF = geral.Groups[_rgxModel.GetKeyValue("RGX_transporta_UF")].Value.Trim();
                _nota.infNFe.transp.transporta.xEnder = geral.Groups[_rgxModel.GetKeyValue("RGX_transporta_xEnder")].Value.Trim();
                _nota.infNFe.transp.transporta.xMun = geral.Groups[_rgxModel.GetKeyValue("RGX_transporta_xMun")].Value.Trim();
                _nota.infNFe.transp.transporta.xNome = geral.Groups[_rgxModel.GetKeyValue("RGX_transporta_xNome")].Value.Trim();

                _nota.infNFe.transp.vol.esp = geral.Groups[_rgxModel.GetKeyValue("RGX_vol_esp")].Value.Trim();

                #endregion

                #region _nota.infNFe.versao

                _nota.infNFe.versao = geral.Groups[_rgxModel.GetKeyValue("RGX_transp_modFrete")].Value.Trim();

                #endregion

                //brasil para pais padrão
                if (String.IsNullOrEmpty(_nota.infNFe.emit.enderEmit.cPais)) _nota.infNFe.emit.enderEmit.cPais = "1058";
                if (String.IsNullOrEmpty(_nota.infNFe.emit.enderEmit.xPais)) _nota.infNFe.emit.enderEmit.xPais = "Brasil";
                if (String.IsNullOrEmpty(_nota.infNFe.dest.enderDest.cPais)) _nota.infNFe.dest.enderDest.cPais = "1058";
                if (String.IsNullOrEmpty(_nota.infNFe.dest.enderDest.xPais)) _nota.infNFe.dest.enderDest.xPais = "Brasil";

                #endregion
                //==============================================

                //==============================================
                #region _nota.protNFe

                #region _nota.protNFe.infProt;

                _nota.protNFe.infProt.chNFe = geral.Groups[_rgxModel.GetKeyValue("RGX_infProt_chNFe")].Value.Trim();
                _nota.protNFe.infProt.dhRecbto = geral.Groups[_rgxModel.GetKeyValue("RGX_infProt_dhRecbto")].Value.Trim();
                _nota.protNFe.infProt.tpAmb = geral.Groups[_rgxModel.GetKeyValue("RGX_infProt_tpAmb")].Value.Trim();

                #endregion

                #region _nota.protNFe.versao;

                _nota.protNFe.versao = "";

                #endregion

                #endregion
                //==============================================
            }
            else
            {
                #region xpath
                if (_rgxModel.IsXpath)
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(String.Format(ArquivosManager.LocalArquivos, r.Emails) + "pagina.html");

                    //ver se funciona com um, depois aplicar nos demais
                    _nota.infNFe.ide.nNF = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_nNF"));

                    #region _nota.infNFe

                    #region _nota.infNFe.cobr
                    EouS = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ES"));
                    _nota.infNFe.NFeNFSe = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_NFeNFSe"));
                    //0 para mercadoria
                    //1 para serviço
                    _nota.infNFe.NFeNFSe = arquivo.ToUpperInvariant().Contains("SERVIÇO") ? "1" : "0";

                    _nota.infNFe.cobr.dup.dVenc = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dup_dVenc"));
                    _nota.infNFe.cobr.dup.nDup = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dup_nDup"));
                    _nota.infNFe.cobr.dup.vDup = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dup_vDup")));

                    _nota.infNFe.cobr.fat.nFat = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_fat_nFat"));
                    _nota.infNFe.cobr.fat.vDesc = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_fat_vDesc")));
                    _nota.infNFe.cobr.fat.vLiq = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_fat_vLiq")));
                    _nota.infNFe.cobr.fat.vOrig = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_fat_vOrig")));
                    #endregion

                    #region _nota.infNFe.dest

                    _nota.infNFe.dest.CNPJ = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dest_CNPJ")));
                    _nota.infNFe.dest.email = Util.SeparaEmails(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dest_email")));

                    //_nota.infNFe.dest.enderDest = "";
                    _nota.infNFe.dest.enderDest.CEP = Util.LimpaCampos(_rgxModel.GetKeyXPathRegex(doc, "RGX_dest_CEP"));
                    _nota.infNFe.dest.enderDest.cMun = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dest_cMun")));
                    _nota.infNFe.dest.enderDest.cPais = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dest_cPais")));
                    _nota.infNFe.dest.enderDest.fone = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dest_fone")));
                    _nota.infNFe.dest.enderDest.UF = _rgxModel.GetKeyXPathRegex(doc, "RGX_dest_UF");

                    _nota.infNFe.dest.enderDest.xBairro = _rgxModel.GetKeyXPathRegex(doc, "RGX_dest_xBairro");
                    _nota.infNFe.dest.enderDest.xLgr = _rgxModel.GetKeyXPathRegex(doc, "RGX_dest_xLgr");
                    _nota.infNFe.dest.enderDest.xMun = _rgxModel.GetKeyXPathRegex(doc, "RGX_dest_xMun");
                    _nota.infNFe.dest.enderDest.xPais = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dest_xPais"));

                    _nota.infNFe.dest.IE = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dest_IE")));
                    _nota.infNFe.dest.IM = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dest_IM")));
                    _nota.infNFe.dest.xNome = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_dest_xNome"));
                    #endregion

                    #region _nota.infNFe.emit

                    _nota.infNFe.emit.CNPJ = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_emit_CNPJ")));
                    _nota.infNFe.emit.CRT = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_emit_CRT"));

                    _nota.infNFe.emit.enderEmit.CEP = Util.LimpaCampos(_rgxModel.GetKeyXPathRegex(doc, "RGX_emit_CEP"));
                    _nota.infNFe.emit.enderEmit.cMun = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_emit_cMun")));
                    _nota.infNFe.emit.enderEmit.cPais = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_emit_cPais")));
                    _nota.infNFe.emit.enderEmit.fone = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_emit_fone")));

                    _nota.infNFe.emit.enderEmit.UF = _rgxModel.GetKeyXPathRegex(doc, "RGX_emit_UF");
                    _nota.infNFe.emit.enderEmit.xBairro = _rgxModel.GetKeyXPathRegex(doc, "RGX_emit_xBairro");
                    _nota.infNFe.emit.enderEmit.xLgr = _rgxModel.GetKeyXPathRegex(doc, "RGX_emit_xLgr");
                    _nota.infNFe.emit.enderEmit.xMun = _rgxModel.GetKeyXPathRegex(doc, "RGX_emit_xMun");
                    _nota.infNFe.emit.enderEmit.xPais = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_emit_xPais"));

                    _nota.infNFe.emit.IE = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_emit_IE")));
                    _nota.infNFe.emit.IM = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_emit_IM")));
                    _nota.infNFe.emit.xFant = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_emit_xFant"));
                    _nota.infNFe.emit.xNome = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_emit_xNome"));

                    #endregion

                    #region _nota.infNFe.ide

                    _nota.infNFe.ide.cDV = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_cDV"));
                    _nota.infNFe.ide.cMunFG = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_cMunFG")));
                    _nota.infNFe.ide.cNF = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_cNF"));
                    _nota.infNFe.ide.cUf = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_cUf")));
                    _nota.infNFe.ide.DataEmissaoRps = Util.FormataData(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_DataEmissaoRps")));
                    _nota.infNFe.ide.dEmi = Util.FormataData(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_dEmi")));
                    _nota.infNFe.ide.finNFe = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_finNFe"));
                    _nota.infNFe.ide.indPag = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_indPag"));
                    _nota.infNFe.ide.mod = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_mode"));
                    _nota.infNFe.ide.natOp = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_natOp"));
                    _nota.infNFe.ide.nNF = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_nNF"));
                    _nota.infNFe.ide.NumeroNfseSubstituida = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_NumeroNfseSubstituida"));
                    _nota.infNFe.ide.NumeroRps = _rgxModel.GetKeyXPathRegex(doc, "RGX_ide_NumeroRps");
                    _nota.infNFe.ide.NumeroRpsSubstituido = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_NumeroRpsSubstituido"));
                    _nota.infNFe.ide.procEmi = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_procEmi"));
                    _nota.infNFe.ide.serie = _rgxModel.GetKeyXPathRegex(doc, "RGX_ide_serie");
                    _nota.infNFe.ide.SerieRps = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_SerieRps"));
                    _nota.infNFe.ide.SerieRpsSubstituido = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_SerieRspSubstituido"));
                    _nota.infNFe.ide.TipoRps = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_TipoRpsSubstituido"));
                    _nota.infNFe.ide.TipoRpsSubstituido = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_TipoRpsSubstituido"));
                    _nota.infNFe.ide.tpAmb = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_tpAmb"));
                    _nota.infNFe.ide.tpEmis = Util.FormataData(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_dEmi")));
                    _nota.infNFe.ide.tpImp = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_tpImp"));
                    _nota.infNFe.ide.tpNF = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ide_tpNF"));

                    #endregion

                    #region d

                    int k = 1;

                    if (!String.IsNullOrEmpty(_rgxModel.GetKeyXPath("RGX_prod_xProd")))
                    {
                        int beginIndex = 4;
                        while (!String.IsNullOrEmpty(XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_xProd"), beginIndex + ""))))
                        {

                            Det d = new Det();

                            d.infAdic = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_det_infAdic"), beginIndex + ""));

                            //produtos
                            d.nItem = k++.ToString();
                            d.prod.cEAN = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_cEAN"), beginIndex + ""));
                            d.prod.cEANTrib = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_cEANTrib"), beginIndex + ""));
                            d.prod.CFOP = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_CFOP"), beginIndex + ""));
                            d.prod.cProd = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_cProd"), beginIndex + ""));
                            d.prod.indTot = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_indTot"), beginIndex + ""));
                            d.prod.NCM = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_NCM"), beginIndex + ""));
                            d.prod.qCom = Util.FormataDecimal(XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_qCom"), beginIndex + "")));

                            //servicos
                            d.prod.servico.CodigoCnae = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_servico_CodigoCnae"), beginIndex + ""));
                            d.prod.servico.CodigoMunicipio = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_servico_CodigoMunicipio"), beginIndex + ""));
                            d.prod.servico.CodigoTributacaoMunicipio = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_servico_CodigoTributacaoMunicipio"), beginIndex + ""));
                            d.prod.servico.competencia = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_servico_competencia"), beginIndex + ""));
                            d.prod.servico.Discriminacao = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_servico_Discriminacao"), beginIndex + ""));
                            d.prod.servico.ItemListaServico = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_servico_ItemListaServico"), beginIndex + ""));
                            d.prod.servico.MunicipioIncidencia = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_servico_MunicipioIncidencia"), beginIndex + ""));

                            d.prod.uCom = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_uCom"), beginIndex + ""));
                            d.prod.vProd = Util.FormataDecimal(XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_vProd"), beginIndex + "")));
                            d.prod.vUnCom = Util.FormataDecimal(XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_vUnCom"), beginIndex + "")));
                            d.prod.xProd = XpathSingleNode(doc, String.Format(_rgxModel.GetKeyXPath("RGX_prod_xProd"), beginIndex + ""));

                            _nota.infNFe.det.Add(d);

                            beginIndex++;
                        }
                    }

                    #endregion

                    #region _nota.infNFe.infAdic

                    _nota.infNFe.infAdic.infAdFisco = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_infAdic_infAdFisco"));
                    _nota.infNFe.infAdic.infCpl = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_infAdic_infCpl"));

                    #endregion

                    #region _nota.infNFe.total

                    _nota.infNFe.total.ICMSTot.Aliquota = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_Aliquota")));
                    _nota.infNFe.total.ICMSTot.DescontoCondicionado = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_DescontoCondicionado")));
                    _nota.infNFe.total.ICMSTot.DescontoIncondicionado = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_DescontoIncondicionado")));
                    _nota.infNFe.total.ICMSTot.ISSRetido = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_ISSRetido")));
                    _nota.infNFe.total.ICMSTot.OutrasRetencoes = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_OutrasRetencoes")));
                    _nota.infNFe.total.ICMSTot.ValorIss = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_ValorIss")));
                    _nota.infNFe.total.ICMSTot.vBC = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vBC")));
                    _nota.infNFe.total.ICMSTot.vBCST = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vBCST")));
                    _nota.infNFe.total.ICMSTot.vCOFINS = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vCOFINS")));
                    _nota.infNFe.total.ICMSTot.vCSLL = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vCSLL")));
                    _nota.infNFe.total.ICMSTot.vDeducoes = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vDeducoes")));
                    _nota.infNFe.total.ICMSTot.vDesc = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vDesc")));
                    _nota.infNFe.total.ICMSTot.vFrete = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vFrete")));
                    _nota.infNFe.total.ICMSTot.vICMS = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vICMS")));
                    _nota.infNFe.total.ICMSTot.vII = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vII")));
                    _nota.infNFe.total.ICMSTot.vINSS = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vINSS")));
                    _nota.infNFe.total.ICMSTot.vIPI = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vIPI")));
                    _nota.infNFe.total.ICMSTot.vIR = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vIR")));
                    _nota.infNFe.total.ICMSTot.vLiquidoNfse = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vLiquidoNfse")));
                    _nota.infNFe.total.ICMSTot.vNF = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vNF")));
                    _nota.infNFe.total.ICMSTot.vPIS = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vPIS")));
                    _nota.infNFe.total.ICMSTot.vProd = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vProd")));
                    _nota.infNFe.total.ICMSTot.vSeg = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vSeg")));
                    _nota.infNFe.total.ICMSTot.vServicos = Util.FormataDecimal(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_ICMSTot_vServicos")));
                    #endregion

                    #region _nota.infNFe.transp
                    _nota.infNFe.transp.modFrete = "";
                    _nota.infNFe.transp.transporta.IE = Util.LimpaCampos(XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_transporta_IE")));
                    _nota.infNFe.transp.transporta.UF = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_transporta_UF"));
                    _nota.infNFe.transp.transporta.xEnder = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_transporta_xEnder"));
                    _nota.infNFe.transp.transporta.xMun = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_transporta_xMun"));
                    _nota.infNFe.transp.transporta.xNome = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_transporta_xNome"));

                    _nota.infNFe.transp.vol.esp = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_vol_esp"));

                    #endregion

                    #region _nota.infNFe.versao

                    _nota.infNFe.versao = XpathSingleNode(doc, _rgxModel.GetKeyXPath("RGX_transp_modFrete"));

                    #endregion

                    //brasil para pais padrão
                    if (String.IsNullOrEmpty(_nota.infNFe.emit.enderEmit.cPais)) _nota.infNFe.emit.enderEmit.cPais = "1058";
                    if (String.IsNullOrEmpty(_nota.infNFe.emit.enderEmit.xPais)) _nota.infNFe.emit.enderEmit.xPais = "Brasil";
                    if (String.IsNullOrEmpty(_nota.infNFe.dest.enderDest.cPais)) _nota.infNFe.dest.enderDest.cPais = "1058";
                    if (String.IsNullOrEmpty(_nota.infNFe.dest.enderDest.xPais)) _nota.infNFe.dest.enderDest.xPais = "Brasil";

                    #endregion
                }
                #endregion
            }
        }

        private String XpathSingleNode(HtmlDocument doc, string xpath)
        {
            string result = "";
            if (!string.IsNullOrEmpty(xpath))
            {
                var aux = doc.DocumentNode.SelectSingleNode(xpath);
                if (aux != null)
                {
                    result = aux.InnerText.Trim();
                }
            }
            result = result.Replace("&nbsp;", "");
            return result;
        }

        private String GerarXml(String numeroLote)
        {

            //String lote = DateTime.Now.ToString("ddMMyyyy") + numeroLote + "_" + _nota.infNFe.emit.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "") + "_" + "02";
            //String localZip = String.Format("C:\\Temp\\Lotes\\" + lote + ".zip", _remetente.Emails);
            String lote = ArquivosManager.Lote(numeroLote, _nota.infNFe.emit.CNPJ, "02");
            String localZip = ArquivosManager.LocalZip(lote, _remetente.Emails);


            if (!Directory.Exists("C:\\Temp\\Lotes\\"))
            {
                Directory.CreateDirectory("C:\\Temp\\Lotes\\");

            }

            if (!File.Exists(localZip))
            {
                //String nomeArquivo = DateTime.Now.ToString("ddMMyyyy") + numeroLote + "_" + _nota.infNFe.emit.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "") + "_" + (String.IsNullOrEmpty(EouS) ? "E" : EouS.ToUpper()) + "_" + "001";
                //String nomePdf = DateTime.Now.ToString("ddMMyyyy") + numeroLote + "_" + _nota.infNFe.emit.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "") + "_" + (String.IsNullOrEmpty(EouS) ? "E" : EouS.ToUpper()) + "_" + "002";

                String nomeArquivo = ArquivosManager.BaseLocal(numeroLote, _nota.infNFe.emit.CNPJ, Util.validateEouS(EouS), "001");
                String nomePdf = ArquivosManager.BaseLocal(numeroLote, _nota.infNFe.emit.CNPJ, Util.validateEouS(EouS), "002");

                System.Xml.Serialization.XmlSerializer serializadorXml = new System.Xml.Serialization.XmlSerializer(_nota.GetType());

                FileStream fs = File.Create(String.Format(ArquivosManager.LocalXml + nomeArquivo + ".xml", _remetente.Emails));
                serializadorXml.Serialize(fs, _nota);
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

        /// <summary>
        /// Gera o zip e retorna o local onde foi criado
        /// </summary>
        /// <param name="arquivo"></param>
        /// <param name="r"></param>
        /// <param name="numeroLote"></param>
        /// <returns>Local onde o zip foi criado</returns>
        public String GerarXml(String arquivo, Remetente r, String numeroLote)
        {
            String result = String.Empty;
            Parametrizar(arquivo, r);
            if (_nota.infNFe.emit.CNPJ != null)
            {
                result = GerarXml(numeroLote);
                ArquivoDAO dao = new ArquivoDAO();
                if (!dao.VerificarArquivoGerado(result.Split('\\')[result.Split('\\').Length - 1]))
                {
                    dao.InserirArquivo(r.Id, result.Split('\\')[result.Split('\\').Length - 1], result, String.Empty);
                }
                else
                {
                    dao.AtualizarStatusArquivo(r.Id, result, String.Empty);
                }
            }
            else
            {
                Console.WriteLine("Erro na nota de: " + r.Emails);
                //Log.Save(this.GetType().ToString(), "ERRO AO LER XML", remetenteId);
            }
            return result;
        }
    }
}