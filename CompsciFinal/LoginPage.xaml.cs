﻿using Firebase.Auth;
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

                    authuid = auth.User.LocalId;

                    FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

                    firebaseHelper.createClient(authLink.FirebaseToken);

                    //await DisplayAlert("Debug", "Name:  " + usernameField.Text + ", AuthUID: " + authuid, "Ok");

                    Person user = await firebaseHelper.GetPerson(authuid);

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

            foreach(char x in email)
            {
                if (x.ToString() == "@")
                    hasAtSymbol = true;
            }

            if (hasAtSymbol)
                return true;
            else
                return false;


        }

        public async void create()
        {

            bool passValid = passwordValidation(passwordField.Text);

            bool emailValid = emailValidation(emailField.Text);

            if(passValid && emailValid)

            {

                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(emailField.Text, passwordField.Text);

                FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

                firebaseHelper.createClient(authLink.FirebaseToken);

                await firebaseHelper.AddPerson(usernameField.Text, auth.User.LocalId, 0, 0);

                await DisplayAlert("Success!", "Account has been created, please verify your email", "Ok");

                Person thisperson = new Person();
                thisperson.Name = usernameField.Text;
                thisperson.PersonId = auth.User.LocalId;
                thisperson.Score = 0;

                await authProvider.SendEmailVerificationAsync(auth.FirebaseToken);

                await Navigation.PushModalAsync(new MainPage(thisperson, chosenTags, authLink));

            }

            else
            {
                if(!passValid)
                {
                    await DisplayAlert("Error", thisFailedVal, "Ok");
                    passwordField.Text = "";
                }
                else
                {
                    await DisplayAlert("Error", "Invalid email", "Ok");
                    emailField.Text = "";
                }
                
            }

           
        }

        private void submitCreate_Clicked(object sender, EventArgs e)
        {
            create();
        }
    }
}