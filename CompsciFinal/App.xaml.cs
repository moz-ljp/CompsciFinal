using Firebase.Auth;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompsciFinal
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            List<String> Tags = new List<String>();
            Tags.Add("all");

            Person empty = new Person();
            empty.Score = 0;
            empty.Name = "";
            empty.PersonId = null;

            MainPage = new MainPage(new Person(), Tags, null);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
