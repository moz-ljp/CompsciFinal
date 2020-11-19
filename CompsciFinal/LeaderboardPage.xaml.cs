using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompsciFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaderboardPage : ContentPage
    {

        FirebaseHelper firebaseHelper = new FirebaseHelper();

        List<CompsciFinal.Person> Persons = new List<CompsciFinal.Person>();

        List<CompsciFinal.uid> UIDS = new List<CompsciFinal.uid>();


        public LeaderboardPage()
        {
            InitializeComponent();
            getAllPersons();
        }

        public async void getAllPersons()
        {
            base.OnAppearing();
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

            UIDS = await firebaseHelper.GetAllUids();

            await DisplayAlert("OK", UIDS.ToString(), "ok");

            foreach(uid x in UIDS)
            {
                Person z = await firebaseHelper.GetPerson(x.uidString);
                Persons.Add(z);
            }

            //await DisplayAlert("OK", firstList.ToString(), "OK");

            //Persons = firstList.

            leaderBoardList.ItemsSource = Persons;
        }

    }
}