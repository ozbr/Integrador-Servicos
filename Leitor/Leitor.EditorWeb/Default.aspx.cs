using Leitor.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Leitor.EditorWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDocumento.Text = @"C:\Leitor\Exemplos\SORANA_101125829_61088795000207_E_001.pdf";

                RegexesDAO dao = new RegexesDAO();
                ddlPrefeituras.DataTextField = "Nome";
                ddlPrefeituras.DataValueField = "Id";
                ddlPrefeituras.DataSource = dao.ListarRegexes();
                ddlPrefeituras.DataBind();
            }
        }

        protected void btParse_Click(object sender, EventArgs e)
        {

            string arquivo = Leitor.Core.ConversorPdf.ExtrairTexto(txtDocumento.Text).Trim();

            RegexesDAO dao = new RegexesDAO();
            var rm = dao.SelecionarRegexPorRemetenteId(Int32.Parse(ddlPrefeituras.SelectedValue));

            #region Obtem XML
            Leitor.Core.Parametrizador pd = new Leitor.Core.Parametrizador();
            txtXML.Text = pd.ObterXml(arquivo, rm.Id, "");
            #endregion

            System.Text.RegularExpressions.Match match = null;
            try
            {
                var reg = new System.Text.RegularExpressions.Regex(txtRegex.Text);
                match = reg.Match(arquivo);
            }
            catch(Exception ex)
            {
                litControl.Text = "Expressão regular inválida: <p />" + ex.Message;
                return;
            }

            if (match.Success)
            {

                //SortedDictionary<int, System.Text.RegularExpressions.Group> groups = new SortedDictionary<int, System.Text.RegularExpressions.Group>();
                StringBuilder sb = new StringBuilder();

                int pos = 0;

                for (int i = 1; i < match.Groups.Count; i++)
                {

                    var coluna = (from g in rm.Groups
                                  where g.Value == i.ToString()
                                  select g.Key).FirstOrDefault<string>();

                    System.Text.RegularExpressions.Group group = match.Groups[i];
                    if (!string.IsNullOrEmpty(group.Value))
                    {
                        int posFind = arquivo.IndexOf(group.Value, pos);

                        if (pos < posFind)
                        {
                            sb.Append(arquivo.Substring(pos, posFind - pos));
                            pos = group.Index;
                        }

                        //
                        if (!string.IsNullOrEmpty(coluna))
                            sb.Append("<b><div title=\"Informção no XML: " + coluna + "\" class=\"matchField\">");
                        else
                            sb.Append("<b><div title=\"Sem correspondente\" class=\"matchWithoutField\">");

                        sb.Append(arquivo.Substring(posFind, group.Length));
                        pos += group.Length;

                        sb.Append("</div></b>");
                    }

                }

                if (pos < arquivo.Length)
                {
                    sb.Append(arquivo.Substring(pos, arquivo.Length - pos));
                    pos = arquivo.Length;
                }

                litControl.Text = sb.ToString().Replace("\r\n", "<p />");
            }
            else
            {
                litControl.Text = "Não foi possível parsear o arquivo";
            }
        }

        protected void ddlPrefeituras_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 val = 0;

            if (Int32.TryParse(ddlPrefeituras.SelectedValue, out val))
            {
                RegexesDAO dao = new RegexesDAO();
                var rm = dao.SelecionarRegexPorRemetenteId(val);
                txtRegex.Text = rm.Geral;
            }
        }
    }
}