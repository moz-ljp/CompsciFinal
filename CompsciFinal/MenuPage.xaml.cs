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
    public partial class MenuPage : ContentPage
    {

        Person thisPerson;
        FirebaseAuthLink thisAuthLink;


        public MenuPage(Person person, FirebaseAuthLink authlink)
        {
            InitializeComponent();
            thisPerson = person;
            thisAuthLink = authlink;
            if (person.PersonId != null)
                userUsername.Text = person.Name;
            else
                userUsername.Text = "Guest User";

            if (!person.teacher)
                schoolCreator.IsVisible = false;
            else
                schoolCreator.IsVisible = true;

            if(person.classCode == null)
            {
                classStatsPage.IsVisible = false;
            }
        }


        private async void questionBuilder_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new shitQuestionBuilder(thisPerson, thisAuthLink));
        }

        private async void schoolCreator_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new schoolCreation(thisPerson, thisAuthLink));
        }

        private async void loginPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LoginPage());
        }

        private async void statsPage_Clicked(object sender, EventArgs e)
        {
            if (thisPerson.PersonId != null)
                await Navigation.PushModalAsync(new StatsPage(thisPerson));
            else
                await DisplayAlert("Error", "You must be logged in to view statistics", "OK");
        }

        private async void accountsPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AccountsPage(thisPerson));
        }

        private async void leaderBoardPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LeaderboardPage(thisPerson, thisAuthLink));
        }

        private async void backBTN_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void classStatsPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ClassStats(thisPerson, thisAuthLink));
        }
    }
}