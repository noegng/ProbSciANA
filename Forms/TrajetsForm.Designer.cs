namespace ProbSciANA
{
    partial class TrajetsForm
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
            this.btnCalculerTrajet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCalculerTrajet
            // 
            this.btnCalculerTrajet.Location = new System.Drawing.Point(12, 12);
            this.btnCalculerTrajet.Name = "btnCalculerTrajet";
            this.btnCalculerTrajet.Size = new System.Drawing.Size(75, 23);
            this.btnCalculerTrajet.TabIndex = 0;
            this.btnCalculerTrajet.Text = "Calculer";
            this.btnCalculerTrajet.UseVisualStyleBackColor = true;
            this.btnCalculerTrajet.Click += new System.EventHandler(this.btnCalculerTrajet_Click);
            // 
            // TrajetsForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnCalculerTrajet);
            this.Name = "TrajetsForm";
            this.Load += new System.EventHandler(this.TrajetsForm_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnCalculerTrajet;
    }
}
