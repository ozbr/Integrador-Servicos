using Leitor.Dao;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace EditorLayout
{
    public partial class EditorLayout : Form
    {
        public EditorLayout()
        {
            InitializeComponent();
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        }

        List<Leitor.Model.RegexModel> regexes;
        Leitor.Document.DocumentXml refDoc;
        XmlDocument doc;

        private void btShow_Click(object sender, EventArgs e)
        {

            RegexesDAO dao = new RegexesDAO();
            regexes = dao.SelecionarRegexPossiveis("GOIANIA");

            refDoc = new Leitor.Document.DocumentXml();
            refDoc.Prefeitura = new Leitor.Model.Prefeitura { Nome = "GOIANIA" };
            DataTable dt = new DataTable();
            dt.Columns.Add("Campo");
            dt.Columns.Add("RegraOrigem");
            dt.Columns.Add("Valor");

            doc = new XmlDocument();
            doc.Load(txtXmlOrigem.Text);

            Dictionary<string,string> dic = new Dictionary<string,string>();
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
                        var field = m.Value.Split('#')[0].Replace("//*/","");
                        if (dic.ContainsKey(field))
                            dic.Remove(field);

                        var result = refDoc.XpathSingleNode(doc, m.Value);
                        dt.Rows.Add(m.Key, m.Value, result);
                    }
                }
            }
            
            dgView.DataSource = dt;

            //dgDisponiveis.DataSource = dic.ToList<KeyValuePair<string,string>>();

            dgDisponiveis.DataSource = (from KeyValuePair<string, string> val in dic
                                       select new { Campo = val.Key, Valor = val.Value }).ToList<object>();

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


    }
}
