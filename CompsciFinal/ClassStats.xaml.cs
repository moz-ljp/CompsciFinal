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
    public partial class ClassStats : ContentPage
    {

        Person person;

        FirebaseAuthLink authLink;

        schoolClass thisClass = new schoolClass();

        schoolClassHelper classHelper = new schoolClassHelper();
        public ClassStats(Person person, FirebaseAuthLink authLink)
        {
            InitializeComponent();

            this.person = person;
            this.authLink = authLink;

            getClass();

        }

        public async void getClass()
        {

            classHelper.createClient(authLink.FirebaseToken);

            thisClass = await classHelper.GetSchoolClass(person.classCode);

            await DisplayAlert("OK", thisClass.schoolName, "ok");

        }


    }
}