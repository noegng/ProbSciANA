using System;
using System.Collections.Generic;

namespace LivinParis
{
    public class Utilisateur
    {
        private int id_utilisateur;
        private string nom;
        private string prenom;
        private string adresse;
        private string telephone;
        private string email;
        private string station;
        private DateTime date_inscription;
        private string mdp;

        public Utilisateur(string nom, string prenom, string adresse, string telephone, string email, string station, string mdp)
        {
            this.id_utilisateur = Requetes.utilisateurs.Count + 1;
            this.nom = nom;
            this.prenom = prenom;
            this.adresse = adresse;
            this.telephone = telephone;
            this.email = email;
            this.station = station;
            this.date_inscription = DateTime.Now();
            this.mdp = mdp;

            Insert(nom, prenom, adresse, telephone, email, station, mdp);
        }

        #region GetSet
        public string Id_utilisateur
        {
            get { return id_utilisateur; }
        }
        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }
        public string Prenom
        {
            get { return prenom; }
            set { prenom = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string Adresse
        {
            get { return adresse; }
            set { adresse = value; }
        }
        public string Role
        {
            get { return role; }
            set { role = value; }
        }
        #endregion GetSet
    }

    public class Client : Utilisateur
    {
        private id_utilisateur;
        public Client(string nom, string prenom, string email, string adresse, string role, string telephone, string dateNaissance) : base(nom, prenom, email, adresse, role)
        {
            this.telephone = telephone;
            this.dateNaissance = dateNaissance;
        }
        #region GetSet
        public string Telephone
        {
            get { return telephone; }
            set { telephone = value; }
        }
        public string DateNaissance
        {
            get { return dateNaissance; }
            set { dateNaissance = value; }
        }
        #endregion GetSet
    }

    public class Cuisinier : Utilisateur
    {
        private string specialite;
        private string experience;

        public Cuisinier(string nom, string prenom, string email, string adresse, string role, string specialite, string experience) : base(nom, prenom, email, adresse, role)
        {
            this.specialite = specialite;
            this.experience = experience;
        }

        #region GetSet
        public string Specialite
        {
            get { return specialite; }
            set { specialite = value; }
        }
        public string Experience
        {
            get { return experience; }
            set { experience = value; }
        }
        #endregion GetSet
    }
}