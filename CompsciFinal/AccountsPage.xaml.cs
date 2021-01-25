﻿using System;
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

        string thisFailedValEmail;

        string authuid;

        string thisFailedVal;

        string classCodeFailedVal;

        public AccountsPage(Person person)
        {
            InitializeComponent();

            usernameLabel.Text = person.Name;

            classCodeLabel.Text = "Class Code: " + person.classCode; //set class code label

            this.person = person;

            passResetContainer.IsVisible = false; //hide contains until requested through buttons
            usernameChangeContainer.IsVisible = false;
            passwordChangeContainer.IsVisible = false;
            classCodeContainer.IsVisible = false;
            resetContainer.IsVisible = false;

        }

        public bool emailValidation(string email) //validation for email
        {

            bool hasAtSymbol = false; //ensuring it has the at symbol
            bool onlyOneAt = false; //and checking it only has the @ symbol once
            bool meetsLengthReq = email.Length > 3;

            int atCount = 0; //counts the amount of @ in the email

            foreach (char x in email) //for every character in the email address
            {
                if (x.ToString() == "@") //if it is the @ sign
                {
                    hasAtSymbol = true; //assign to true
                    onlyOneAt = true; //assign to true
                    atCount++; //increase the count of the amount of @
                }

            }

            if (atCount > 1) //if this number is more than 1, @ count is more than 1
                onlyOneAt = false; //so say that the @ count being less than 2 is false


            if (hasAtSymbol && onlyOneAt && meetsLengthReq) //check they are both true
                return true;
            else

                if (!hasAtSymbol)
                thisFailedValEmail = "Email does not conatain @ symbol";
            else if (!onlyOneAt)
                thisFailedValEmail = "Email contains more than one @ symbol";

            return false;


        }

        private void showHideResetPass_Clicked(object sender, EventArgs e) //these control whether to show their respective containers
        {
            passResetContainer.IsVisible = !passResetContainer.IsVisible;
        }

        private async void resetEmail_Clicked(object sender, EventArgs e) //send a reset email
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

            if(emailValidation(emailTextBox.Text))
            {
                await authProvider.SendPasswordResetEmailAsync(emailTextBox.Text); //grab the email from the text box

                await DisplayAlert("Success", "Reset Email Sent", "OK"); //send the reset request
            }
            else
            {
                await DisplayAlert("Error", thisFailedValEmail, "OK");
            }

        }

        private void showHidechangeUsername_Clicked(object sender, EventArgs e)
        {
            usernameChangeContainer.IsVisible = !usernameChangeContainer.IsVisible;
        }

        private async void changeUsername_Clicked(object sender, EventArgs e) //change username
        {
            string username = usernameTextBox.Text;
            string email = usernameEmailTextBox.Text;
            string password = usernamePasswordTextBox.Text;

            if(usernameTextBoxNEW.Text.Length > 2) //validate username length
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password); //log in with firebase auth

                authuid = auth.User.LocalId;

                FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

                firebaseHelper.createClient(authLink.FirebaseToken);

                Person currentPerson = await firebaseHelper.GetPerson(authuid); //get the users current account

                currentPerson.Name = usernameTextBoxNEW.Text; //update the users object

                await firebaseHelper.UpdatePerson(currentPerson, currentPerson.Score, currentPerson.totalAnswered); //reupload to database

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

        private async void changePassword_Clicked(object sender, EventArgs e) //for changing password
        {
            string username = passwordUsernameTextBox.Text;
            string email = passwordEmailTextBox.Text;
            string password = changePassPasswordTextBox.Text;
            string newpassword = changePassPasswordTextBoxNew.Text;

            if(passwordValidation(newpassword)) //validates password to ensure it meets standards
            {
                try
                {
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                    var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

                    authuid = auth.User.LocalId;

                    FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

                    firebaseHelper.createClient(authLink.FirebaseToken);

                    await authProvider.ChangeUserPassword(auth.FirebaseToken, newpassword); //for changing the users password with firebase auth

                    await DisplayAlert("Success!", "Your password has been changed", "OK");
                }
                catch (Firebase.Auth.FirebaseAuthException exception)
                {
                    string errorReason = exception.Reason.ToString();
                    if (errorReason == "UnknownEmailAddress" || errorReason == "InvalidEmailAddress") //invalid email
                    {
                        await DisplayAlert("Error", "Invalid Email", "OK");
                        passwordEmailTextBox.Text = "";
                    }

                    else if (errorReason == "WrongPassword") //Incorrect password
                    {
                        await DisplayAlert("Error", "Incorrect Password", "OK");
                        changePassPasswordTextBox.Text = "";
                    }

                    else if (errorReason == "MissingPassword") //Empty password field
                    {
                        await DisplayAlert("Error", "Missing Password", "OK");
                    }

                    else if (errorReason == "TooManyAttemptsTryLater") //Account is being spammed so cooldown
                    {
                        await DisplayAlert("Error", "You have attempted to log in too many times, try again later", "OK");
                        passwordEmailTextBox.Text = "";
                        changePassPasswordTextBox.Text = "";
                    }
                }

            }
            else
            {
                await DisplayAlert("Error", thisFailedVal, "OK");
            }

            

        }

        public bool passwordValidation(string pass)
        {
            const int minLen = 6;

            bool meetsLengthReq = pass.Length >= minLen;
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

        private void showHideClassCode_Clicked(object sender, EventArgs e)
        {
            classCodeContainer.IsVisible = !classCodeContainer.IsVisible;
        }

        public bool classCodeValidation(string classCode)
        {
            const int minLen = 4;

            bool meetsLengthReq = classCode.Length >= minLen;
            bool doesNotContainSymbols = true;

            foreach(char x in classCode)
            {
                if (char.IsPunctuation(x) || char.IsSymbol(x))
                    doesNotContainSymbols = false;
            }

            if (doesNotContainSymbols && meetsLengthReq)
                return true;
            else
            {
                if (!doesNotContainSymbols)
                    classCodeFailedVal = "Class codes cannot contain symbols";
                else
                    classCodeFailedVal = "Class codes must be longer than 3 characters";

                return false;
            }

        }

        private async void changeClassCode_Clicked(object sender, EventArgs e) //for changing class codes
        {
            bool decision = await DisplayAlert("Set class code", "Are you sure?", "OK", "Cancel");
            if(decision)
            {
                if(classCodeValidation(classCodeClassCodeTextBox.Text))
                {
                    try
                    {
                        var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                        var auth = await authProvider.SignInWithEmailAndPasswordAsync(classCodeEmailTextBox.Text, classCodePasswordTextBox.Text);

                        authuid = auth.User.LocalId;

                        FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

                        firebaseHelper.createClient(authLink.FirebaseToken);

                        person.classCode = classCodeClassCodeTextBox.Text; //change object class code

                        await firebaseHelper.UpdatePerson(person, person.Score, person.totalAnswered); //update with firebase database
                    }
                    catch (Firebase.Auth.FirebaseAuthException exception)
                    {
                        string errorReason = exception.Reason.ToString();
                        if (errorReason == "UnknownEmailAddress" || errorReason == "InvalidEmailAddress") //invalid email
                        {
                            await DisplayAlert("Error", "Invalid Email", "OK");
                            classCodeEmailTextBox.Text = "";
                        }

                        else if (errorReason == "WrongPassword") //Incorrect password
                        {
                            await DisplayAlert("Error", "Incorrect Password", "OK");
                            classCodePasswordTextBox.Text = "";
                        }

                        else if (errorReason == "MissingPassword") //Empty password field
                        {
                            await DisplayAlert("Erorr", "Missing Password", "OK");
                        }

                        else if (errorReason == "TooManyAttemptsTryLater") //Account is being spammed so cooldown
                        {
                            await DisplayAlert("Error", "You have attempted to log in too many times, try again later", "OK");
                            classCodeEmailTextBox.Text = "";
                            classCodePasswordTextBox.Text = "";
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Error", classCodeFailedVal, "Ok");
                    classCodeClassCodeTextBox.Text = string.Empty;
                }
                
                
            }
        }

        private void showHideResetAcc_Clicked(object sender, EventArgs e)
        {
            resetContainer.IsVisible = !resetContainer.IsVisible;
        }

        private async void resetScores_Clicked(object sender, EventArgs e) //for resetting user scores
        {
            bool decision = await DisplayAlert("Reset Scores", "Are you sure, this cannot be reversed", "OK", "Cancel");

            bool fieldCheck = ((emailValidation(resetScoresEmailTextBox.Text) && passwordValidation(resetScoresPasswordTextBox.Text) && resetScoresUsernameTextBox.Text.Length > 2)); 

            if(decision && fieldCheck)
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                var auth = await authProvider.SignInWithEmailAndPasswordAsync(resetScoresEmailTextBox.Text, resetScoresPasswordTextBox.Text);

                authuid = auth.User.LocalId;

                FirebaseAuthLink authLink = await auth.GetFreshAuthAsync();

                firebaseHelper.createClient(authLink.FirebaseToken);

                person.conversionsScore = 0; //set all scores to 0
                person.cyberScore = 0;
                person.hardwareScore = 0;
                person.programmingScore = 0;
                person.softwareScore = 0;

                person.totalConversions = 0;
                person.totalCyber = 0;
                person.totalHardware = 0;
                person.totalProgramming = 0;
                person.totalSoftware = 0;

                await firebaseHelper.UpdatePerson(person, 0, 0); //update with database

                await DisplayAlert("Success", "Your scores have been reset", "OK");

                List<string> tags = new List<string>();
                tags.Add("all");

                await Navigation.PushModalAsync(new MainPage(person, tags, authLink));
            }
            else
            {
                if (!passwordValidation(resetScoresPasswordTextBox.Text))
                    await DisplayAlert("Error", thisFailedVal, "Ok");
                else if (!emailValidation(resetScoresEmailTextBox.Text))
                    await DisplayAlert("Error", thisFailedValEmail, "Ok");
                else
                    await DisplayAlert("Error", "Username invalid", "Ok");
            }
        }

        private async void backBTN_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}