using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

namespace CompsciFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountsPage : ContentPage
    {

        Person person;

        FirebaseHelper firebaseHelper = new FirebaseHelper();

        FirebaseClient firebase = new FirebaseClient("https://compsci-c8f5a.firebaseio.com//");

        private const string BaseUrl = "https://compsci-c8f5a.firebaseio.com//";

        string authuid;

        string thisFailedVal;

        public AccountsPage(Person person)
        {
            InitializeComponent();

            usernameLabel.Text = person.Name;

            this.person = person;

            passResetContainer.IsVisible = false;
            usernameChangeContainer.IsVisible = false;
            passwordChangeContainer.IsVisible = false;

        }

        private void showHideResetPass_Clicked(object sender, EventArgs e)
        {
            passResetContainer.IsVisible = !passResetContainer.IsVisible;
        }

        private async void resetEmail_Clicked(object sender, EventArgs e)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

            await authProvider.SendPasswordResetEmailAsync(emailTextBox.Text);

            await DisplayAlert("Success", "Reset Email Sent", "OK");
        }

        private void showHidechangeUsername_Clicked(object sender, EventArgs e)
        {
            usernameChangeContainer.IsVisible = !usernameChangeContainer.IsVisible;
        }

        private async void changeUsername_Clicked(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text;
            string email = usernameEmailTextBox.Text;
            string password = usernamePasswordTextBox.Text;

            if(usernameTextBoxNEW.Text.Length > 2)
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

                authuid = auth.User.LocalId;

                FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

                firebaseHelper.createClient(authLink.FirebaseToken);

                Person currentPerson = await firebaseHelper.GetPerson(authuid);

                currentPerson.Name = usernameTextBoxNEW.Text;

                await firebaseHelper.UpdatePerson(currentPerson, currentPerson.Score, currentPerson.totalAnswered);

                List<string> chosenTags = new List<string>();
                chosenTags.Add("all");

                await DisplayAlert("Success!", "Your username has been changed", "OK");

                await Navigation.PushModalAsync(new MainPage(currentPerson, chosenTags, authLink));
            }

            else
            {
                await DisplayAlert("Error", "Your username must be longer than 2 characters", "OK");
            }

            
        }

        private void showHidechangePassword_Clicked(object sender, EventArgs e)
        {
            passwordChangeContainer.IsVisible = !passwordChangeContainer.IsVisible;
        }

        private async void changePassword_Clicked(object sender, EventArgs e)
        {
            string username = passwordUsernameTextBox.Text;
            string email = passwordEmailTextBox.Text;
            string password = changePassPasswordTextBox.Text;
            string newpassword = changePassPasswordTextBoxNew.Text;

            if(passwordValidation(newpassword))
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

                authuid = auth.User.LocalId;

                FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

                firebaseHelper.createClient(authLink.FirebaseToken);

                await authProvider.ChangeUserPassword(auth.FirebaseToken, newpassword);

                await DisplayAlert("Success!", "Your password has been changed", "OK");
            }
            else
            {
                await DisplayAlert("Error", thisFailedVal, "OK");
            }

            

        }

        public bool passwordValidation(string pass)
        {
            const int minLen = 6;

            bool meetsLengthReq = pass.Length >= 6;
            bool hasUpperCase = false;
            bool hasLowerCase = false;
            bool hasDecimals = false;
            bool hasSymbols = false;

            foreach (char x in pass)
            {
                if (char.IsUpper(x))
                    hasUpperCase = true;
                else if (char.IsLower(x))
                    hasLowerCase = true;
                else if (char.IsDigit(x))
                    hasDecimals = true;
                else if (char.IsPunctuation(x) || char.IsSymbol(x))
                    hasSymbols = true;
            }

            bool finalValidation = meetsLengthReq && hasUpperCase && hasLowerCase && hasDecimals && hasSymbols;
            if (finalValidation)
                return finalValidation;
            else
            {
                if (!hasUpperCase)
                    thisFailedVal = "No upper case letters";
                else if (!hasLowerCase)
                    thisFailedVal = "No lower case letters";
                else if (!hasDecimals)
                    thisFailedVal = "No numbers";
                else if (!meetsLengthReq)
                    thisFailedVal = "Does not meet length requirement of 6 or more characters";
                else if (!hasSymbols)
                    thisFailedVal = "Does not contain symbols";

                return false;
            }

        }
    }
}