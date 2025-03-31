namespace ProbSciANA
{
    partial class StatistiquesForm
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
            this.btnGenererStats = new System.Windows.Forms.Button();
            this.btnExporterStats = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGenererStats
            // 
            this.btnGenererStats.Location = new System.Drawing.Point(12, 12);
            this.btnGenererStats.Name = "btnGenererStats";
            this.btnGenererStats.Size = new System.Drawing.Size(75, 23);
            this.btnGenererStats.TabIndex = 0;
            this.btnGenererStats.Text = "Générer";
            this.btnGenererStats.UseVisualStyleBackColor = true;
            this.btnGenererStats.Click += new System.EventHandler(this.btnGenererStats_Click);
            // 
            // btnExporterStats
            // 
            this.btnExporterStats.Location = new System.Drawing.Point(12, 41);
            this.btnExporterStats.Name = "btnExporterStats";
            this.btnExporterStats.Size = new System.Drawing.Size(75, 23);
            this.btnExporterStats.TabIndex = 1;
            this.btnExporterStats.Text = "Exporter";
            this.btnExporterStats.UseVisualStyleBackColor = true;
            this.btnExporterStats.Click += new System.EventHandler(this.btnExporterStats_Click);
            // 
            // StatistiquesForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnExporterStats);
            this.Controls.Add(this.btnGenererStats);
            this.Name = "StatistiquesForm";
            this.Load += new System.EventHandler(this.StatistiquesForm_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnGenererStats;
        private System.Windows.Forms.Button btnExporterStats;
    }
}
