using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Firebase.Auth;

namespace CompsciFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class testPage : ContentPage
    {

        FirebaseHelper firebaseHelper = new FirebaseHelper();

        public testPage()
        {
            InitializeComponent();
        }

        public async void login()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

            var auth = await authProvider.SignInWithEmailAndPasswordAsync(emailField.Text, passwordField.Text);

            System.Diagnostics.Debug.Write("EMAIL: ", auth.User.Email);

            System.Diagnostics.Debug.Write("UID", auth.User.LocalId);
        }

        public async void create()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCGJx-mKV7Ms8BRkJupNe8wvlHwZDJAXMs"));

            var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(emailField.Text, passwordField.Text);

            Person thisPerson = new Person();
            thisPerson.Name = usernameField.Text;
            thisPerson.PersonId = auth.User.LocalId;
            thisPerson.Score = 0;

            System.Diagnostics.Debug.Write("Account created");
        }

        private void submitBTN_Clicked(object sender, EventArgs e)
        {
            login();
        }

        private void createBTN_Clicked(object sender, EventArgs e)
        {
            create();
        }
    }
}