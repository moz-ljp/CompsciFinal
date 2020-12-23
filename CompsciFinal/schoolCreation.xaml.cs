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
    public partial class schoolCreation : ContentPage
    {

        List<string> chars = new List<string>();

        string[] characters = { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "1", "2", "3", "4", "5", "6", "7", "8", "9"
        
        
        };

        Person person;

        schoolClass thisClass = new schoolClass();

        schoolClassHelper classhelper = new schoolClassHelper();

        FirebaseAuthLink thisAuthLink;

        public schoolCreation(Person person, FirebaseAuthLink authlink)
        {
            InitializeComponent();

            this.person = person;

            thisAuthLink = authlink;

        }

        public async void genCode()
        {

            //int code = Convert.ToInt32(idField.Text);

            //string hexCode = code.ToString("X2");

            thisClass.schoolCode = idField.Text; //grabs all the variables into a schoolClass object
            thisClass.teacherUsername = person.Name;
            thisClass.schoolScore = person.Score;
            thisClass.totalSchoolAnswered = person.totalAnswered;
            thisClass.schoolName = schoolNameField.Text;

            classhelper.createClient(thisAuthLink.FirebaseToken); //create a firebase client

            List<schoolClass> allClasses = new List<schoolClass>(); //list for all classes

            allClasses = await classhelper.GetAllSchools(); //get all of the classes into list

            bool uniqueID = true; //this will only set to false if another id the same is found

            foreach(schoolClass x in allClasses) //for every class in the list
            {
                if (x.schoolCode == idField.Text) //if the id fields are the same
                {
                    await DisplayAlert("Error", "ID Code taken", "Ok"); //that id is taken
                    idField.Text = String.Empty;
                    uniqueID = false;
                }
                    
                    
            }

            try
            {
                if(uniqueID)
                    await classhelper.AddschoolClass(thisClass);
            }
            catch
            {
                
            }

            
            
        }

        private void btnCreateQ_Clicked(object sender, EventArgs e)
        {

        }

        private void btnSolveID_Clicked(object sender, EventArgs e)
        {
            genCode();
        }

        private async void backBTN_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}