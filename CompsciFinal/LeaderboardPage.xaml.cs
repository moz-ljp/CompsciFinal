using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        List<CompsciFinal.Person> OrderedPersonsList = new List<CompsciFinal.Person>();

        string authuid;

        Person person;

        FirebaseAuthLink authLink;

        public LeaderboardPage(Person person, FirebaseAuthLink authLink)
        {
            
            InitializeComponent();

            this.person = person;
            this.authLink = authLink;

            getAllPersons();
        }

        public async void getAllPersons()
        {
            base.OnAppearing();
            //var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

            //var auth = await authProvider.SignInAnonymouslyAsync();

            await loadingBar.ProgressTo(.2, 250, Easing.Linear);

            //authuid = auth.User.LocalId;

            //FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

            firebaseHelper.createClient(authLink.FirebaseToken);

            //Persons = await firebaseHelper.GetAllPersons();

            List<string> ids = new List<string>();

            ids = await firebaseHelper.GetAllPersons();

            await loadingBar.ProgressTo(.6, 250, Easing.Linear);

            System.Diagnostics.Debug.Write(ids[0].ToString());

            //var OrderedPersons = Persons.OrderBy(f => f.Score);

            foreach (string x in ids)
            {
                Persons.Add(await firebaseHelper.GetPerson(x));
                //await DisplayAlert("ok", x, "ok");
            }

            await loadingBar.ProgressTo(.8, 250, Easing.Linear);

            var Personsvar = Persons;

            var OrderedPersons = Personsvar.OrderBy(f => f.Score);

            var OrderedPersonsReversed = OrderedPersons.Reverse();

            List<Person> OrderedPersonsReversedPersonList = OrderedPersonsReversed.ToList();

            topUserLabel.Text = OrderedPersonsReversedPersonList[0].Name + " " + OrderedPersonsReversedPersonList[0].Score;
            OrderedPersonsReversedPersonList.RemoveAt(0);
            secondUserLabel.Text = OrderedPersonsReversedPersonList[0].Name + " " + OrderedPersonsReversedPersonList[0].Score;
            OrderedPersonsReversedPersonList.RemoveAt(0);
            thirdUserLabel.Text = OrderedPersonsReversedPersonList[0].Name + " " + OrderedPersonsReversedPersonList[0].Score;
            OrderedPersonsReversedPersonList.RemoveAt(0);

            leaderBoardList.ItemsSource = OrderedPersonsReversedPersonList;

            await loadingBar.ProgressTo(1, 250, Easing.Linear);
        }

        

        private async void backBTN_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}