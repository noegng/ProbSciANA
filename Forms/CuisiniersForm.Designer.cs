namespace ProbSciANA
{
    partial class CuisiniersForm
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
            this.btnAjouterCuisinier = new System.Windows.Forms.Button();
            this.btnNotationCuisiniers = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAjouterCuisinier
            // 
            this.btnAjouterCuisinier.Location = new System.Drawing.Point(12, 12);
            this.btnAjouterCuisinier.Name = "btnAjouterCuisinier";
            this.btnAjouterCuisinier.Size = new System.Drawing.Size(75, 23);
            this.btnAjouterCuisinier.TabIndex = 0;
            this.btnAjouterCuisinier.Text = "Ajouter";
            this.btnAjouterCuisinier.UseVisualStyleBackColor = true;
            this.btnAjouterCuisinier.Click += new System.EventHandler(this.btnAjouterCuisinier_Click);
            // 
            // btnNotationCuisiniers
            // 
            this.btnNotationCuisiniers.Location = new System.Drawing.Point(12, 41);
            this.btnNotationCuisiniers.Name = "btnNotationCuisiniers";
            this.btnNotationCuisiniers.Size = new System.Drawing.Size(75, 23);
            this.btnNotationCuisiniers.TabIndex = 1;
            this.btnNotationCuisiniers.Text = "Notations";
            this.btnNotationCuisiniers.UseVisualStyleBackColor = true;
            this.btnNotationCuisiniers.Click += new System.EventHandler(this.btnNotationCuisiniers_Click);
            // 
            // CuisiniersForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnNotationCuisiniers);
            this.Controls.Add(this.btnAjouterCuisinier);
            this.Name = "CuisiniersForm";
            this.Load += new System.EventHandler(this.CuisiniersForm_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnAjouterCuisinier;
        private System.Windows.Forms.Button btnNotationCuisiniers;
    }
}
