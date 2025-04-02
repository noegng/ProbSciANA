using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ProbSciANA.Interface
{
    public partial class App : Application { }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Interface.StartView());
        }
    }
}