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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.txtXmlOrigem = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgDisponiveis = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDisponiveis)).BeginInit();
            this.SuspendLayout();
            // 
            // dgView
            // 
            this.dgView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgView.Location = new System.Drawing.Point(12, 129);
            this.dgView.Name = "dgView";
            this.dgView.Size = new System.Drawing.Size(418, 313);
            this.dgView.TabIndex = 0;
            this.dgView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgView_CellValueChanged);
            // 
            // btShow
            // 
            this.btShow.Location = new System.Drawing.Point(483, 74);
            this.btShow.Name = "btShow";
            this.btShow.Size = new System.Drawing.Size(75, 23);
            this.btShow.TabIndex = 1;
            this.btShow.Text = "Comparar";
            this.btShow.UseVisualStyleBackColor = true;
            this.btShow.Click += new System.EventHandler(this.btShow_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(350, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 2;
            // 
            // txtXmlOrigem
            // 
            this.txtXmlOrigem.Location = new System.Drawing.Point(93, 48);
            this.txtXmlOrigem.Name = "txtXmlOrigem";
            this.txtXmlOrigem.Size = new System.Drawing.Size(465, 20);
            this.txtXmlOrigem.TabIndex = 3;
            this.txtXmlOrigem.Text = "C:\\temp\\Goiânia_P13012014-023139369.xml";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "XML Origem";
            // 
            // dgDisponiveis
            // 
            this.dgDisponiveis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgDisponiveis.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgDisponiveis.Location = new System.Drawing.Point(450, 129);
            this.dgDisponiveis.Name = "dgDisponiveis";
            this.dgDisponiveis.Size = new System.Drawing.Size(417, 313);
            this.dgDisponiveis.TabIndex = 0;
            this.dgDisponiveis.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgView_CellValueChanged);
            // 
            // EditorLayout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 471);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtXmlOrigem);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.btShow);
            this.Controls.Add(this.dgDisponiveis);
            this.Controls.Add(this.dgView);
            this.Name = "EditorLayout";
            this.Text = "Parmetrização";
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgDisponiveis)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgView;
        private System.Windows.Forms.Button btShow;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox txtXmlOrigem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgDisponiveis;
    }
}

