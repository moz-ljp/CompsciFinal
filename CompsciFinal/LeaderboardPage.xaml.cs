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

            await loadingBar.ProgressTo(.2, 250, Easing.Linear); //increase loading bar

            //authuid = auth.User.LocalId;

            //FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

            try
            {
                firebaseHelper.createClient(authLink.FirebaseToken);

                List<string> ids = new List<string>();

                ids = await firebaseHelper.GetAllPersons(); //acquires all of the ID's found on the database

                await loadingBar.ProgressTo(.6, 250, Easing.Linear);

                System.Diagnostics.Debug.Write(ids[0].ToString());

                foreach (string x in ids)
                {
                    Persons.Add(await firebaseHelper.GetPerson(x)); //gets a user for every ID in the id list
                    //await DisplayAlert("ok", x, "ok");
                }

                await loadingBar.ProgressTo(.8, 250, Easing.Linear);

                if(Persons.Count < 4) //if there are less than 4 people in the datastore
                {
                    await DisplayAlert("Error", "Not enough users for leaderboard to function", "OK"); //display error
                    await Navigation.PopModalAsync(); //and return to menu
                }

                var Personsvar = Persons;

                var OrderedPersons = Personsvar.OrderBy(f => f.Score); //sorting the list 

                var OrderedPersonsReversed = OrderedPersons.Reverse(); //flipping the list because that sorting method goes low to high

                List<Person> OrderedPersonsReversedPersonList = OrderedPersonsReversed.ToList();

                topUserLabel.Text = OrderedPersonsReversedPersonList[0].Name + " " + OrderedPersonsReversedPersonList[0].Score; //assigning top users into lables
                OrderedPersonsReversedPersonList.RemoveAt(0);
                secondUserLabel.Text = OrderedPersonsReversedPersonList[0].Name + " " + OrderedPersonsReversedPersonList[0].Score;
                OrderedPersonsReversedPersonList.RemoveAt(0);
                thirdUserLabel.Text = OrderedPersonsReversedPersonList[0].Name + " " + OrderedPersonsReversedPersonList[0].Score;
                OrderedPersonsReversedPersonList.RemoveAt(0);

                leaderBoardList.ItemsSource = OrderedPersonsReversedPersonList; //sets item source for list

                await loadingBar.ProgressTo(1, 250, Easing.Linear);
            }

            catch(Firebase.Auth.FirebaseAuthException exception)
            {
                string errorReason = exception.Reason.ToString(); //get the reason from the obj

                await DisplayAlert("Error", errorReason, "Ok"); //displays error
                await Navigation.PopModalAsync(); //returns user to menu
            }

            
        }

        

        private async void backBTN_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}