using Leitor.Dao;
using Leitor.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace EditorLayout
{
    public partial class EditorLayout : Form
    {
        List<Leitor.Model.RegexModel> regexes;
        Leitor.Document.DocumentXml refDoc;
        XmlDocument doc;
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;

        public EditorLayout()
        {
            InitializeComponent();

            PrefeituraDAO preDao = new PrefeituraDAO();
            List<Prefeitura> prefeituras = preDao.SelecionarPrefeituras();
            cmbPrefeitura.DataSource = prefeituras;
            cmbPrefeitura.DisplayMember = "Nome";
            cmbPrefeitura.SelectedIndex = -1;

            cmbPrefeitura.SelectedIndexChanged += cmbPrefeitura_SelectedIndexChanged;
            btnSalvar.Click += btnSalvar_Click;
            dgView.DragDrop += dgView_DragDrop;
            dgDisponiveis.MouseMove += dgDisponiveis_MouseMove;
            dgDisponiveis.MouseDown += dgDisponiveis_MouseDown;
            dgView.DragOver += dgView_DragOver;
        }

        private void IniciarlizarGrid(DataTable dt)
        {
            DataGridViewComboBoxColumn campos = new DataGridViewComboBoxColumn();

            if (!dgView.Columns.Contains("Campo"))
            {
                var list11 = new List<string>() 
            { "RGX_GERAL",
              "RGX_ITEM",
              "RGX_ide_cUf",
              "RGX_ide_cNF",
              "RGX_ide_indPag",
              "RGX_ide_natOp",
              "RGX_ide_mode",
              "RGX_ide_serie",
              "RGX_ide_nNF",
              "RGX_ide_dEmi",
              "RGX_ide_tpNF",
              "RGX_ide_cMunFG",
              "RGX_ide_tpImp",
              "RGX_ide_tpEmi",
              "RGX_ide_cDV",
              "RGX_ide_tpAmb",
              "RGX_ide_finNFe",
              "RGX_ide_procEmi",
              "RGX_ide_NumeroNfseSubstituida",
              "RGX_ide_NumeroRps",
              "RGX_ide_SerieRps",
              "RGX_ide_DataEmissaoRps",
              "RGX_ide_NumeroRpsSubstituido",
              "RGX_ide_SerieRspSubstituido",
              "RGX_ide_TipoRpsSubstituido",
              "RGX_ide_OutrasInformacoes",
              "RGX_emit_CNPJ",
              "RGX_emit_xNome",
              "RGX_emit_xFant",
              "RGX_emit_xLgr",
              "RGX_emit_xNro",
              "RGX_emit_xCpl",
              "RGX_emit_xBairro",
              "RGX_emit_cMun",
              "RGX_emit_xMun",
              "RGX_emit_UF",
              "RGX_emit_CEP",
              "RGX_emit_cPais",
              "RGX_emit_xPais",
              "RGX_emit_fone",
              "RGX_emit_email",
              "RGX_emit_IE",
              "RGX_emit_IM",
              "RGX_emit_CRT",
              "RGX_dest_CNPJ",
              "RGX_dest_xNome",
              "RGX_dest_xLgr",
              "RGX_dest_xNro",
              "RGX_dest_xCpl",
              "RGX_dest_xBairro",
              "RGX_dest_cMun",
              "RGX_dest_xMun",
              "RGX_dest_UF",
              "RGX_dest_CEP",
              "RGX_dest_cPais",
              "RGX_dest_xPais",
              "RGX_dest_fone",
              "RGX_dest_IE",
              "RGX_dest_IM",
              "RGX_dest_email",
              "RGX_det_infAdic",
              "RGX_prod_cProd",
              "RGX_prod_cEAN",
              "RGX_prod_xProd",
              "RGX_prod_NCM",
              "RGX_prod_CFOP",
              "RGX_prod_uCom",
              "RGX_prod_qCom",
              "RGX_prod_vUnCom",
              "RGX_prod_vProd",
              "RGX_prod_cEANTrib",
              "RGX_prod_indTot",
              "RGX_servico_competencia",
              "RGX_servico_ItemListaServico",
              "RGX_servico_CodigoCnae",
              "RGX_servico_CodigoTributacaoMunicipio",
              "RGX_servico_Discriminacao",
              "RGX_servico_CodigoMunicipio",
              "RGX_servico_MunicipioIncidencia",
              "RGX_servico_Quantidade",
              "RGX_servico_PrecoUnit",
              "RGX_ICMS_orig",
              "RGX_ICMS_CST",
              "RGX_ICMS_CSOSN",
              "RGX_ICMS_modBC",
              "RGX_ICMS_vBC",
              "RGX_ICMS_pRedBC",
              "RGX_ICMS_pICMS",
              "RGX_ICMS_vICMS",
              "RGX_ICMS_modBCST",
              "RGX_ICMS_pMVAST",
              "RGX_ICMS_pRedBCST",
              "RGX_ICMS_vBCST",
              "RGX_ICMS_pICMSST",
              "RGX_ICMS_vICMSST",
              "RGX_ICMS_pCredSN",
              "RGX_ICMS_vCredICMSSN",
              "RGX_ICMS_motDesICMS",
              "RGX_IPI_cIEnq",
              "RGX_IPI_CNPJProd",
              "RGX_IPI_cSelo",
              "RGX_IPI_qSelo",
              "RGX_IPI_cEnq",
              "RGX_IPITrib_CST",
              "RGX_IPITrib_vBC",
              "RGX_IPITrib_pIPI",
              "RGX_IPITrib_qUnid",
              "RGX_IPITrib_vUnid",
              "RGX_IPITrib_vIPI",
              "RGX_II_vBC",
              "RGX_II_vDespAdu",
              "RGX_II_vII",
              "RGX_II_vIOF",
              "RGX_PIS_CST",
              "RGX_PIS_vBC",
              "RGX_PIS_pPIS",
              "RGX_PIS_vPIS",
              "RGX_COFINS_CST",
              "RGX_COFINS_vBC",
              "RGX_COFINS_pCOFINS",
              "RGX_COFINS_vCOFINS",
              "RGX_ISSQN_vBC",
              "RGX_ISSQN_vAliq",
              "RGX_ISSQN_vISSQN",
              "RGX_ISSQN_cMunFG",
              "RGX_ISSQN_cListServ",
              "RGX_ISSQN_cSitTrib",
              "RGX_ICMSTot_ValorCreditoGerado",
              "RGX_ICMSTot_vBC",
              "RGX_ICMSTot_vICMS",
              "RGX_ICMSTot_vBCST",
              "RGX_ICMSTot_vProd",
              "RGX_ICMSTot_vFrete",
              "RGX_ICMSTot_vSeg",
              "RGX_ICMSTot_vDesc",
              "RGX_ICMSTot_vII",
              "RGX_ICMSTot_vIPI",
              "RGX_ICMSTot_vPIS",
              "RGX_ICMSTot_vCOFINS",
              "RGX_ICMSTot_vNF",
              "RGX_ICMSTot_vServicos",
              "RGX_ICMSTot_vDeducoes",
              "RGX_ICMSTot_vINSS",
              "RGX_ICMSTot_vIR",
              "RGX_ICMSTot_vCSLL",
              "RGX_ICMSTot_ValorIss",
              "RGX_ICMSTot_BcRetencaoISS",
              "RGX_ICMSTot_OutrasRetencoes",
              "RGX_ICMSTot_Aliquota",
              "RGX_ICMSTot_DescontoIncondicionado",
              "RGX_ICMSTot_DescontoCondicionado",
              "RGX_ICMSTot_ISSRetido",
              "RGX_ICMSTot_vLiquidoNfse",
              "RGX_transporta_xNome",
              "RGX_transporta_IE",
              "RGX_transporta_xEnder",
              "RGX_transporta_xMun",
              "RGX_transporta_UF",
              "RGX_vol_esp",
              "RGX_fat_nFat",
              "RGX_fat_vOrig",
              "RGX_fat_vLiq",
              "RGX_fat_vDesc",
              "RGX_dup_nDup",
              "RGX_dup_dVenc",
              "RGX_dup_vDup",
              "RGX_infAdic_infAdFisco",
              "RGX_infAdic_infCpl",
              "RGX_infProt_tpAmb",
              "RGX_infProt_chNFe",
              "RGX_infProt_dhRecbto",
              "RGX_PROD",
              "RGX_transp_modFrete",
              "RGX_ISXPATH",
              "RGX_NFeNFSe",
              "RGX_ES",
              "RGX_protNFe_infProt_chNFe" };

                campos.DataSource = list11;
                campos.Name = "Campo";
                campos.HeaderText = "Campo";
                campos.DataPropertyName = "Campo";

                dgView.Columns.AddRange(campos);
                dgView.Sort(campos, System.ComponentModel.ListSortDirection.Ascending);
            }
            dgView.DataSource = dt;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        }

        private void btShow_Click(object sender, EventArgs e)
        {

            RegexesDAO dao = new RegexesDAO();
            regexes = dao.SelecionarRegexPossiveis(cmbPrefeitura.Text);

            refDoc = new Leitor.Document.DocumentXml();
            refDoc.Prefeitura = new Leitor.Model.Prefeitura { Nome = cmbPrefeitura.Text };
            DataTable dt = new DataTable();
            dt.Columns.Add(("Campo"));
            dt.Columns.Add("RegraOrigem");
            dt.Columns.Add("Valor");

            doc = new XmlDocument();
            doc.Load(txtXmlOrigem.Text);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (XmlNode node in doc.DocumentElement.FirstChild.FirstChild.ChildNodes)
            {
                dic.Add(node.Name, node.ChildNodes.Count > 0 ? node.ChildNodes[0].Value : string.Empty);
            }


            foreach (var model in regexes)
            {
                foreach (var m in model.Groups)
                {
                    if (m.Value != null)
                    {
                        var field = m.Value.Split('#')[0].Replace("//*/", "");
                        if (dic.ContainsKey(field))
                            dic.Remove(field);

                        var result = refDoc.XpathSingleNode(doc, m.Value);
                        dt.Rows.Add(m.Key, m.Value, result);
                    }
                }
            }
            //new DataGridViewComboBoxColumn() { Name = "Campo" }

            dgView.DataSource = dt;

            //dgDisponiveis.DataSource = dic.ToList<KeyValuePair<string,string>>();

            dgDisponiveis.DataSource = (from KeyValuePair<string, string> val in dic
                                        select new { Campo = val.Key, Valor = val.Value }).ToList<object>();

        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dgView.DataSource;
            Dictionary<string, string> regexes = new Dictionary<string, string>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string valor = (string.IsNullOrEmpty(dgView[1, i].Value.ToString()) ? null : dgView[1, i].Value.ToString());
                regexes.Add(dgView[0, i].Value.ToString(), valor);
            }
            RegexesDAO dao = new RegexesDAO();

            var prefeitura = (Prefeitura)cmbPrefeitura.SelectedItem;

            if (dao.AtualizarRegex(prefeitura.Id, regexes))
            {
                MessageBox.Show("Parametrização salva com sucesso.");
                cmbPrefeitura_SelectedIndexChanged(new object(), new EventArgs());
            }
            else
                MessageBox.Show("Erro ao salvar a parametrização.");
        }

        private void dgView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                var row = dgView.Rows[e.RowIndex];
                try
                {
                    row.Cells[2].Value = refDoc.XpathSingleNode(doc, row.Cells[e.ColumnIndex].Value.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro na formula " + ex.Message);
                }
            }
        }

        private void dgView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void dgDisponiveis_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = dgDisponiveis.HitTest(e.X, e.Y).RowIndex;
            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void dgDisponiveis_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dgDisponiveis.DoDragDrop(dgDisponiveis.Rows[rowIndexFromMouseDown], DragDropEffects.Copy);
                }
            }
        }

        private void dgView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(typeof(DataGridViewRow)))
                {

                    Point clientPoint = dgView.PointToClient(new Point(e.X, e.Y));

                    int rowIndexOfItemUnderMouseToDrop = dgView.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

                    if (e.Effect == DragDropEffects.Copy)
                    {
                        DataGridViewRow Row = (DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow));
                        dgView.Rows[rowIndexOfItemUnderMouseToDrop].Cells[1].Value = string.Concat("//*/", Row.Cells[0].Value);
                    }
                }
            }
            catch
            {
            }
        }

        private void cmbPrefeitura_SelectedIndexChanged(object sender, EventArgs e)
        {
            RegexesDAO dao = new RegexesDAO();
            regexes = dao.SelecionarRegexPossiveis(cmbPrefeitura.Text);

            refDoc = new Leitor.Document.DocumentXml();
            refDoc.Prefeitura = new Leitor.Model.Prefeitura { Nome = cmbPrefeitura.SelectedText };
            DataTable dt = new DataTable();
            dt.Columns.Add("Campo");
            dt.Columns.Add("RegraOrigem");
            dt.Columns.Add("Valor");

            doc = new XmlDocument();

            foreach (var model in regexes)
            {
                foreach (var m in model.Groups)
                {
                    if (m.Value != null)
                    {
                        dt.Rows.Add(m.Key, m.Value, string.Empty);
                    }
                }
            }

            IniciarlizarGrid(dt);


            //for (int row = 0; row < dgView.Columns.Count; row++)
            //{
            //    DataGridViewComboBoxCell cell =
            //        (DataGridViewComboBoxCell)(dgView.Rows[row].Cells["Campo"]);
            //    cell.DataSource = new string[] { "f", "g" };
            //}


        }

        private void btnSelecionar_Click(object sender, EventArgs e)
        {
            var result = ofdAbrirXML.ShowDialog();
            if (result == DialogResult.OK)
                txtXmlOrigem.Text = ofdAbrirXML.FileName;
        }

    }
}
