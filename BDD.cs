using System;
using System.Collections.Generic;
using System.Windows.Input;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;


namespace ProbSciANA
{

public class Client
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Adresse { get; set; }
    public string Station { get; set; }
    public string MotDePasse { get; set; }

    public Client(int id, string nom, string prenom, string email, string adresse )
    {
        Id= id;
        Nom = nom;
        Prenom = prenom;
        Email = email;
        Adresse = adresse;

    }


    public Client (int id, string nom, string prenom, string email, string adresse, string station, string motDePasse)
    {
        Id = id;
        Nom = nom;
        Prenom = prenom;
        Email = email;
        Adresse = adresse;
        Station = station;
        MotDePasse = motDePasse;
    }
}


public class Cuisinier 
{

    public int Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Adresse { get; set; }
    public string Station { get; set; }
    public string MotDePasse { get; set; }

    public Cuisinier(int Id, string Nom, string Prenom, string email, string Adresse, string Station, string mdp){
        this.Id = Id;
        this.Nom = Nom;
        this.Prenom = Prenom;
        this.Email = email;
        this.Adresse = Adresse;
        this.Station = Station;
        this.MotDePasse = mdp;
    }
}

public class Livraison
    {
        public string NomPlat { get; set; }
        public Client Client { get; set; }
        public Cuisinier Cuisinier { get; set; }

        public string ClientNom { get; set; }

        public string StationLivraison {get ; set;}

        public string DateLivraison { get; set; }

        public Livraison(string nomPlat, Client client, Cuisinier cuisinier)
        {
            NomPlat = nomPlat;
            Client = client;
            Cuisinier = cuisinier;
        }


        public Livraison(string nomPlat, Cuisinier cuisinier, string clientNom, string stationLivraison)
        {
            NomPlat = nomPlat;
            Cuisinier = cuisinier;
            ClientNom = clientNom;
            StationLivraison = stationLivraison;
        }
    }


     public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

        public void Execute(object parameter) => _execute((T)parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }


}