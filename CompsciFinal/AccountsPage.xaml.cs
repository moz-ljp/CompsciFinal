using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Firebase;
using Firebase.Auth;

namespace CompsciFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountsPage : ContentPage
    {

        Person person;

        public AccountsPage(Person person)
        {
            InitializeComponent();

            this.person = person;

            passResetContainer.IsVisible = false;

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




    }
}