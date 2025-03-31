namespace ProbSciANA
{
    partial class CommandesForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnAjouterCommande = new System.Windows.Forms.Button();
            this.btnModifierCommande = new System.Windows.Forms.Button();
            this.btnVisualiserChemin = new System.Windows.Forms.Button();
            this.btnEvaluerCommande = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAjouterCommande
            // 
            this.btnAjouterCommande.Location = new System.Drawing.Point(12, 12);
            this.btnAjouterCommande.Name = "btnAjouterCommande";
            this.btnAjouterCommande.Size = new System.Drawing.Size(75, 23);
            this.btnAjouterCommande.TabIndex = 0;
            this.btnAjouterCommande.Text = "Ajouter";
            this.btnAjouterCommande.UseVisualStyleBackColor = true;
            this.btnAjouterCommande.Click += new System.EventHandler(this.btnAjouterCommande_Click);
            // 
            // btnModifierCommande
            // 
            this.btnModifierCommande.Location = new System.Drawing.Point(12, 41);
            this.btnModifierCommande.Name = "btnModifierCommande";
            this.btnModifierCommande.Size = new System.Drawing.Size(75, 23);
            this.btnModifierCommande.TabIndex = 1;
            this.btnModifierCommande.Text = "Modifier";
            this.btnModifierCommande.UseVisualStyleBackColor = true;
            this.btnModifierCommande.Click += new System.EventHandler(this.btnModifierCommande_Click);
            // 
            // btnVisualiserChemin
            // 
            this.btnVisualiserChemin.Location = new System.Drawing.Point(12, 70);
            this.btnVisualiserChemin.Name = "btnVisualiserChemin";
            this.btnVisualiserChemin.Size = new System.Drawing.Size(75, 23);
            this.btnVisualiserChemin.TabIndex = 2;
            this.btnVisualiserChemin.Text = "Visualiser";
            this.btnVisualiserChemin.UseVisualStyleBackColor = true;
            this.btnVisualiserChemin.Click += new System.EventHandler(this.btnVisualiserChemin_Click);
            // 
            // btnEvaluerCommande
            // 
            this.btnEvaluerCommande.Location = new System.Drawing.Point(12, 99);
            this.btnEvaluerCommande.Name = "btnEvaluerCommande";
            this.btnEvaluerCommande.Size = new System.Drawing.Size(75, 23);
            this.btnEvaluerCommande.TabIndex = 3;
            this.btnEvaluerCommande.Text = "Évaluer";
            this.btnEvaluerCommande.UseVisualStyleBackColor = true;
            this.btnEvaluerCommande.Click += new System.EventHandler(this.btnEvaluerCommande_Click);
            // 
            // CommandesForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnEvaluerCommande);
            this.Controls.Add(this.btnVisualiserChemin);
            this.Controls.Add(this.btnModifierCommande);
            this.Controls.Add(this.btnAjouterCommande);
            this.Name = "CommandesForm";
            this.Load += new System.EventHandler(this.CommandesForm_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnAjouterCommande;
        private System.Windows.Forms.Button btnModifierCommande;
        private System.Windows.Forms.Button btnVisualiserChemin;
        private System.Windows.Forms.Button btnEvaluerCommande;
    }
}
