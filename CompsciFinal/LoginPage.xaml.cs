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

        FirebaseHelper firebaseHelper = new FirebaseHelper(); //firebase helper object for the users 

        FirebaseClient firebase = new FirebaseClient("https://compsci-c8f5a.firebaseio.com//"); //firebase link

        private const string BaseUrl = "https://compsci-c8f5a.firebaseio.com//";

        public string authuid; //the users authuid

        List<string> chosenTags = new List<string>(); //tags list

        string thisFailedVal = ""; //stores error from validation

        string thisFailedValEmail = ""; //stores error from failed email validation

        public LoginPage()
        {
            InitializeComponent();
            chosenTags.Add("all"); //add 'all' to the chosen tags list for when we pass back to the main page
        }

        private void submitLogin_Clicked(object sender, EventArgs e)
        {
            login();
        }

        public async void login()
        {
            if(emailValidation(emailField.Text) && passwordValidation(passwordField.Text) && usernameField.Text.Length > 2) //validate everything
            {
                try
                {
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs")); //authenticate with database

                    var auth = await authProvider.SignInWithEmailAndPasswordAsync(emailField.Text, passwordField.Text); //attempt firebase auth login
                    await loadingBar.ProgressTo(.4, 250, Easing.Linear); //increase loading bar

                    authuid = auth.User.LocalId; //assign authuid to variable

                    FirebaseAuthLink authLink = await auth.GetFreshAuthAsync(); //grab a new auth link for that user

                    firebaseHelper.createClient(authLink.FirebaseToken); //and make a client so we can interract with database
                    await loadingBar.ProgressTo(.8, 250, Easing.Linear);

                    //await DisplayAlert("Debug", "Name:  " + usernameField.Text + ", AuthUID: " + authuid, "Ok");

                    Person user = await firebaseHelper.GetPerson(authuid); //get our user and store all of their details in an object

                    await loadingBar.ProgressTo(1, 250, Easing.Linear);

                    await DisplayAlert("Success!", "You have logged in, " + user.Name, "Ok"); //report success to user

                    await Navigation.PushModalAsync(new MainPage(user, chosenTags, authLink)); //go to main page with acquired info


                }

                catch (Firebase.Auth.FirebaseAuthException exception) //get any firebase exceptions
                {

                    //await DisplayAlert("OK", exception.Reason.ToString(), "OK");

                    string errorReason = exception.Reason.ToString(); //get the reason from the obj

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
                        await DisplayAlert("Error", "Missing Password", "OK");
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
                if(thisFailedVal.Length > 1)
                    await DisplayAlert("Error", thisFailedVal, "Ok");
                if (thisFailedValEmail.Length > 1)
                    await DisplayAlert("Error", thisFailedValEmail, "Ok");
            }
            
            


            
        }

        public bool passwordValidation(string pass) //for validating passwords when creating and logging in
        {
            const int minLen = 6; //choose a minimum length (set to 6)

            bool meetsLengthReq = pass.Length >= 6; //check if the password meets minimum length
            bool hasUpperCase = false; //set all to false before checks
            bool hasLowerCase = false;
            bool hasDecimals = false;
            bool hasSymbols = false;

            foreach(char x in pass) //for every character in the given password
            {
                if (char.IsUpper(x)) //if its upper case
                    hasUpperCase = true;
                else if (char.IsLower(x)) //if its lower case
                    hasLowerCase = true;
                else if (char.IsDigit(x)) //if it is a number
                    hasDecimals = true;
                else if (char.IsPunctuation(x) || char.IsSymbol(x)) //if it is punctuation or a symbol
                    hasSymbols = true;
            }

            bool finalValidation = meetsLengthReq && hasUpperCase && hasLowerCase && hasDecimals && hasSymbols; //check they are all true using and
            if(finalValidation) //if all are true
                return finalValidation; //return it
            else //otherwise, find out why
            {
                if (!hasUpperCase)
                    thisFailedVal = "No upper case letters password";
                else if (!hasLowerCase)
                    thisFailedVal = "No lower case letters in password";
                else if (!hasDecimals)
                    thisFailedVal = "No numbers in password";
                else if (!meetsLengthReq)
                    thisFailedVal = "Password not meet length requirement of 6 or more characters";
                else if (!hasSymbols)
                    thisFailedVal = "Password not contain symbols";

                return false; //return false and the reason is now stored
            }

        }

        public bool emailValidation(string email) //validation for email
        {

            bool hasAtSymbol = false; //ensuring it has the at symbol
            bool onlyOneAt = false; //and checking it only has the @ symbol once
            bool meetsLengthReq = email.Length > 3;

            int atCount = 0; //counts the amount of @ in the email

            foreach(char x in email) //for every character in the email address
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

        public bool usernameValidation(string username) //validating username
        {
            bool lengthCheck = false; //for checking length of username
            bool doesNotContainSymbol = true; //for checking the username does not have symbols in it

            if (username.Length > 2) //if the username length is longer than 2, its ok
                lengthCheck = true; //set to true

            foreach(char x in username) //for every character in the username
            {
                if (char.IsPunctuation(x) || char.IsSymbol(x)) //if the character is punctuation or a symbol
                    doesNotContainSymbol = false; //set this to false
            }

            if (lengthCheck && doesNotContainSymbol) //if these both pass as true
                return true; //allow
            else
                return false;
        }

        public async void create() //for creating an account
        {

            bool passValid = passwordValidation(passwordField.Text); //validate password

            bool emailValid = emailValidation(emailField.Text); //validate email

            bool usernameValid = usernameValidation(usernameField.Text); //validate username

            await loadingBar.ProgressTo(.2, 250, Easing.Linear);

            if (passValid && emailValid && usernameValid) //if all the fields are valid

            {

                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(emailField.Text, passwordField.Text); //login to firebase auth
                await loadingBar.ProgressTo(.4, 250, Easing.Linear);

                FirebaseAuthLink authLink = await auth.GetFreshAuthAsync(); //create an auth link

                firebaseHelper.createClient(authLink.FirebaseToken); //create a client using that auth link

                await firebaseHelper.AddPerson(usernameField.Text, auth.User.LocalId, 0, 0); //add that person to the database
                await loadingBar.ProgressTo(.8, 250, Easing.Linear);

                await DisplayAlert("Success!", "Account has been created, please verify your email", "Ok");

                Person thisPerson = new Person(); //create a new person object for all of their information
                thisPerson.Name = usernameField.Text;
                thisPerson.PersonId = auth.User.LocalId;
                thisPerson.Score = 0;
                thisPerson.classCode = "";

                await authProvider.SendEmailVerificationAsync(auth.FirebaseToken); //send firebase email verification
                await loadingBar.ProgressTo(1, 250, Easing.Linear);

                await Navigation.PushModalAsync(new MainPage(thisPerson, chosenTags, authLink)); //go to main page

            }

            else
            {
                if(!passValid) //checks if pass is returned invalid
                {
                    await DisplayAlert("Error", thisFailedVal, "Ok");
                    passwordField.Text = "";
                }
                else if(!emailValid) //checks if email returned invalid
                {
                    await DisplayAlert("Error", "Invalid email", "Ok");
                    emailField.Text = "";
                }
                else //checks if username invalid
                {
                    await DisplayAlert("Error", "Username is not valid", "Ok");
                    usernameField.Text = "";
                }
                
            }

            submitCreate.IsEnabled = true;

        }

        private void submitCreate_Clicked(object sender, EventArgs e)
        {
            create();
            submitCreate.IsEnabled = false; //disables the create button to prevent error
        }

        private async void forgotPass_Clicked(object sender, EventArgs e) //allows user to request a password reset email
        {

            bool decision = await DisplayAlert("Send reset email", "Are you sure?", "OK", "Cancel");
            if(decision)
            {
                if ((emailField.Text != "" || emailField.Text != null) && emailValidation(emailField.Text)) //ensures an email is present and the email is valid
                {
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

                    await authProvider.SendPasswordResetEmailAsync(emailField.Text); //send the email using the email address

                    await DisplayAlert("Success", "Reset email sent", "OK");
                }

                else
                {
                    await DisplayAlert("Error", "Email field cannot be empty!", "OK"); //if something is missing, report
                }
            }
            
        }
    }
}