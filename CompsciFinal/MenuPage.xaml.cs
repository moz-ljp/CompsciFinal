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
        }


        private async void questionBuilder_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new shitQuestionBuilder(thisPerson, thisAuthLink));
        }

        private async void schoolCreator_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new schoolCreation());
        }

        private async void loginPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LoginPage());
        }

        private async void statsPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new StatsPage(thisPerson));
        }

        private async void accountsPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AccountsPage(thisPerson));
        }
    }
}