namespace EditorLayout
{
    partial class EditorLayout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgView = new System.Windows.Forms.DataGridView();
            this.btShow = new System.Windows.Forms.Button();
            this.cmbPrefeitura = new System.Windows.Forms.ComboBox();
            this.txtXmlOrigem = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgDisponiveis = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.ofdAbrirXML = new System.Windows.Forms.OpenFileDialog();
            this.btnSelecionar = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDisponiveis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgView
            // 
            this.dgView.AllowDrop = true;
            this.dgView.AllowUserToDeleteRows = false;
            this.dgView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgView.Location = new System.Drawing.Point(0, 0);
            this.dgView.Name = "dgView";
            this.dgView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgView.Size = new System.Drawing.Size(414, 341);
            this.dgView.TabIndex = 0;
            this.dgView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgView_CellValueChanged);
            // 
            // btShow
            // 
            this.btShow.Location = new System.Drawing.Point(501, 83);
            this.btShow.Name = "btShow";
            this.btShow.Size = new System.Drawing.Size(75, 23);
            this.btShow.TabIndex = 1;
            this.btShow.Text = "Comparar";
            this.btShow.UseVisualStyleBackColor = true;
            this.btShow.Click += new System.EventHandler(this.btShow_Click);
            // 
            // cmbPrefeitura
            // 
            this.cmbPrefeitura.FormattingEnabled = true;
            this.cmbPrefeitura.Location = new System.Drawing.Point(368, 22);
            this.cmbPrefeitura.Name = "cmbPrefeitura";
            this.cmbPrefeitura.Size = new System.Drawing.Size(208, 21);
            this.cmbPrefeitura.TabIndex = 2;
            // 
            // txtXmlOrigem
            // 
            this.txtXmlOrigem.Location = new System.Drawing.Point(111, 57);
            this.txtXmlOrigem.Name = "txtXmlOrigem";
            this.txtXmlOrigem.Size = new System.Drawing.Size(465, 20);
            this.txtXmlOrigem.TabIndex = 3;
            this.txtXmlOrigem.Text = "C:\\temp\\Goiânia_P13012014-023139369.xml";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "XML Origem";
            // 
            // dgDisponiveis
            // 
            this.dgDisponiveis.AllowDrop = true;
            this.dgDisponiveis.AllowUserToAddRows = false;
            this.dgDisponiveis.AllowUserToDeleteRows = false;
            this.dgDisponiveis.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgDisponiveis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgDisponiveis.Location = new System.Drawing.Point(0, 0);
            this.dgDisponiveis.Name = "dgDisponiveis";
            this.dgDisponiveis.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgDisponiveis.Size = new System.Drawing.Size(461, 341);
            this.dgDisponiveis.TabIndex = 0;
            this.dgDisponiveis.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgView_CellValueChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 121);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgDisponiveis);
            this.splitContainer1.Size = new System.Drawing.Size(879, 341);
            this.splitContainer1.SplitterDistance = 414;
            this.splitContainer1.TabIndex = 5;
            // 
            // btnSalvar
            // 
            this.btnSalvar.Location = new System.Drawing.Point(379, 6);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(75, 23);
            this.btnSalvar.TabIndex = 6;
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.UseVisualStyleBackColor = true;
            // 
            // ofdAbrirXML
            // 
            this.ofdAbrirXML.Filter = "Xml files|*.xml";
            // 
            // btnSelecionar
            // 
            this.btnSelecionar.Location = new System.Drawing.Point(583, 53);
            this.btnSelecionar.Name = "btnSelecionar";
            this.btnSelecionar.Size = new System.Drawing.Size(75, 23);
            this.btnSelecionar.TabIndex = 7;
            this.btnSelecionar.Text = "Selecionar";
            this.btnSelecionar.UseVisualStyleBackColor = true;
            this.btnSelecionar.Click += new System.EventHandler(this.btnSelecionar_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSalvar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 462);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(879, 36);
            this.panel1.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btnSelecionar);
            this.panel2.Controls.Add(this.btShow);
            this.panel2.Controls.Add(this.cmbPrefeitura);
            this.panel2.Controls.Add(this.txtXmlOrigem);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(879, 121);
            this.panel2.TabIndex = 9;
            // 
            // EditorLayout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 498);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "EditorLayout";
            this.Text = "Parmetrização";
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDisponiveis)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgView;
        private System.Windows.Forms.Button btShow;
        private System.Windows.Forms.ComboBox cmbPrefeitura;
        private System.Windows.Forms.TextBox txtXmlOrigem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgDisponiveis;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.OpenFileDialog ofdAbrirXML;
        private System.Windows.Forms.Button btnSelecionar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}

