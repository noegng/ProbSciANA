using System;
using System.Collections.Generic;

namespace LivinParis
{
    public class Utilisateur
    {
        private int id_utilisateur;
        private string nom;
        private string prenom;
        private string email;
        private string adresse;
        private string role;

        public Utilisateur(string nom, string prenom, string email, string adresse, string role)
        {
            this.nom = nom;
            this.prenom = prenom;
            this.email = email;
            this.adresse = adresse;
            this.role = role;
            
        }

        #region GetSet
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