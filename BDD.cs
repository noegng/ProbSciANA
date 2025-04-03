using System;
using System.Collections.Generic;
using System.Windows.Input;


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
}

public class Livraison
    {
        public string NomPlat { get; set; }
        public Client Client { get; set; }
        public Cuisinier Cuisinier { get; set; }

        public string DateLivraison { get; set; }
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