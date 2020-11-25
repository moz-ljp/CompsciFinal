using Firebase.Auth;
using Firebase.Database;
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
    public partial class LoginPage : ContentPage
    {

        FirebaseHelper firebaseHelper = new FirebaseHelper();

        FirebaseClient firebase = new FirebaseClient("https://compsci-c8f5a.firebaseio.com//");

        private const string BaseUrl = "https://compsci-c8f5a.firebaseio.com//";

        public string authuid;

        List<string> chosenTags = new List<string>();

        string thisFailedVal = "";

        public LoginPage()
        {
            InitializeComponent();
            chosenTags.Add("all");
        }

        private void submitLogin_Clicked(object sender, EventArgs e)
        {
            login();
        }

        public async void login()
        {
            if(emailValidation(emailField.Text) && passwordValidation(passwordField.Text) && usernameField.Text.Length > 2)
            {
                try
                {
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                    var auth = await authProvider.SignInWithEmailAndPasswordAsync(emailField.Text, passwordField.Text);
                    await loadingBar.ProgressTo(.4, 250, Easing.Linear);

                    authuid = auth.User.LocalId;

                    FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

                    firebaseHelper.createClient(authLink.FirebaseToken);
                    await loadingBar.ProgressTo(.8, 250, Easing.Linear);

                    //await DisplayAlert("Debug", "Name:  " + usernameField.Text + ", AuthUID: " + authuid, "Ok");

                    Person user = await firebaseHelper.GetPerson(authuid);

                    await loadingBar.ProgressTo(1, 250, Easing.Linear);

                    await DisplayAlert("Success!", "You have logged in, " + user.Name, "Ok");

                    await Navigation.PushModalAsync(new MainPage(user, chosenTags, authLink));


                }

                catch (Firebase.Auth.FirebaseAuthException exception)
                {

                    await DisplayAlert("OK", exception.Reason.ToString(), "OK");

                    string errorReason = exception.Reason.ToString();

                    if (errorReason == "UnknownEmailAddress" || errorReason == "InvalidEmailAddress") //invalid email
                    {
                        await DisplayAlert("Error", "Invalid Email", "OK");
                        emailField.Text = "";
                    }

                    else if (errorReason == "WrongPassword") //Incorrect password
                    {
                        await DisplayAlert("Error", "Incorrect Password", "OK");
                        passwordField.Text = "";
                    }

                    else if (errorReason == "MissingPassword") //Empty password field
                    {
                        await DisplayAlert("Erorr", "Missing Password", "OK");
                    }

                    else if (errorReason == "TooManyAttemptsTryLater") //Account is being spammed so cooldown
                    {
                        await DisplayAlert("Error", "You have attempted to log in too many times, try again later", "OK");
                        usernameField.Text = "";
                        emailField.Text = "";
                        passwordField.Text = "";
                    }

                }
            }
            else
            {
                await DisplayAlert("Error", thisFailedVal, "Ok");
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

            foreach(char x in pass)
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
            if(finalValidation)
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

        public bool emailValidation(string email)
        {

            bool hasAtSymbol = false;
            bool onlyOneAt = false;

            int atCount = 0;

            foreach(char x in email)
            {
                if (x.ToString() == "@")
                {
                    hasAtSymbol = true;
                    onlyOneAt = true;
                    atCount++;
                }
                    
            }

            if (atCount > 1)
                onlyOneAt = false;


            if (hasAtSymbol && onlyOneAt)
                return true;
            else
                return false;


        }

        public bool usernameValidation(string username)
        {
            bool lengthCheck = false;
            bool doesNotContainSymbol = true;

            if (username.Length > 2)
                lengthCheck = true;

            foreach(char x in username)
            {
                if (char.IsPunctuation(x) || char.IsSymbol(x))
                    doesNotContainSymbol = false;
            }

            if (lengthCheck)
                return true;
            else
                return false;
        }

        public async void create()
        {

            bool passValid = passwordValidation(passwordField.Text);

            bool emailValid = emailValidation(emailField.Text);

            bool usernameValid = usernameValidation(usernameField.Text);

            await loadingBar.ProgressTo(.2, 250, Easing.Linear);

            if (passValid && emailValid && usernameValid)

            {

                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(emailField.Text, passwordField.Text);
                await loadingBar.ProgressTo(.4, 250, Easing.Linear);

                FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

                firebaseHelper.createClient(authLink.FirebaseToken);

                await firebaseHelper.AddPerson(usernameField.Text, auth.User.LocalId, 0, 0);
                await loadingBar.ProgressTo(.8, 250, Easing.Linear);

                await DisplayAlert("Success!", "Account has been created, please verify your email", "Ok");

                Person thisperson = new Person();
                thisperson.Name = usernameField.Text;
                thisperson.PersonId = auth.User.LocalId;
                thisperson.Score = 0;

                await authProvider.SendEmailVerificationAsync(auth.FirebaseToken);
                await loadingBar.ProgressTo(1, 250, Easing.Linear);

                await Navigation.PushModalAsync(new MainPage(thisperson, chosenTags, authLink));

            }

            else
            {
                if(!passValid)
                {
                    await DisplayAlert("Error", thisFailedVal, "Ok");
                    passwordField.Text = "";
                }
                else if(!emailValid)
                {
                    await DisplayAlert("Error", "Invalid email", "Ok");
                    emailField.Text = "";
                }
                else
                {
                    await DisplayAlert("Error", "Username is not valid", "Ok");
                    usernameField.Text = "";
                }
                
            }

           
        }

        private void submitCreate_Clicked(object sender, EventArgs e)
        {
            create();
        }

        private async void forgotPass_Clicked(object sender, EventArgs e)
        {

            bool decision = await DisplayAlert("Send reset email", "Are you sure?", "OK", "Cancel");
            if(decision)
            {
                if (emailField.Text != "" || emailField.Text != null)
                {
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                    await authProvider.SendPasswordResetEmailAsync(emailField.Text);

                    await DisplayAlert("Success", "Reset email sent", "OK");
                }

                else
                {
                    await DisplayAlert("Error", "Email field cannot be empty!", "OK");
                }
            }
            
        }
    }
}