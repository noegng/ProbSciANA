namespace ProbSciANA
{
    partial class Form1
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
            btnClients = new Button();
            btnCuisiniers = new Button();
            btnCommandes = new Button();
            btnTrajets = new Button();
            btnStatistiques = new Button();
            SuspendLayout();
            // 
            // btnClients
            // 
            btnClients.Location = new Point(12, 12);
            btnClients.Name = "btnClients";
            btnClients.Size = new Size(75, 23);
            btnClients.TabIndex = 0;
            btnClients.Text = "Clients";
            btnClients.UseVisualStyleBackColor = true;
            btnClients.Click += btnClients_Click;
            // 
            // btnCuisiniers
            // 
            btnCuisiniers.Location = new Point(12, 41);
            btnCuisiniers.Name = "btnCuisiniers";
            btnCuisiniers.Size = new Size(75, 23);
            btnCuisiniers.TabIndex = 1;
            btnCuisiniers.Text = "Cuisiniers";
            btnCuisiniers.UseVisualStyleBackColor = true;
            btnCuisiniers.Click += btnCuisiniers_Click;
            // 
            // btnCommandes
            // 
            btnCommandes.Location = new Point(12, 70);
            btnCommandes.Name = "btnCommandes";
            btnCommandes.Size = new Size(75, 23);
            btnCommandes.TabIndex = 2;
            btnCommandes.Text = "Commandes";
            btnCommandes.UseVisualStyleBackColor = true;
            btnCommandes.Click += btnCommandes_Click;
            // 
            // btnTrajets
            // 
            btnTrajets.Location = new Point(12, 99);
            btnTrajets.Name = "btnTrajets";
            btnTrajets.Size = new Size(75, 23);
            btnTrajets.TabIndex = 3;
            btnTrajets.Text = "Trajets";
            btnTrajets.UseVisualStyleBackColor = true;
            btnTrajets.Click += btnTrajets_Click;
            // 
            // btnStatistiques
            // 
            btnStatistiques.Location = new Point(12, 128);
            btnStatistiques.Name = "btnStatistiques";
            btnStatistiques.Size = new Size(75, 23);
            btnStatistiques.TabIndex = 4;
            btnStatistiques.Text = "Statistiques";
            btnStatistiques.UseVisualStyleBackColor = true;
            btnStatistiques.Click += btnStatistiques_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(716, 477);
            Controls.Add(btnStatistiques);
            Controls.Add(btnTrajets);
            Controls.Add(btnCommandes);
            Controls.Add(btnCuisiniers);
            Controls.Add(btnClients);
            Name = "Form1";
            Load += UI_Load;
            ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnClients;
        private System.Windows.Forms.Button btnCuisiniers;
        private System.Windows.Forms.Button btnCommandes;
        private System.Windows.Forms.Button btnTrajets;
        private System.Windows.Forms.Button btnStatistiques;
    }
}
