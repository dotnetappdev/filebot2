using Microsoft.UI.Xaml;
using System;

namespace RenameIt
{
    public partial class App : Application
    {
        public Window? MainWindow { get; private set; }

        public static new App Current => (App)Application.Current;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();
            MainWindow.Activate();
        }
    }
}
