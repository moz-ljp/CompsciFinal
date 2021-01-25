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

        string classCodeFailedVal;

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

        public bool classCodeValidation(string classCode)
        {
            const int minLen = 4;

            bool meetsLengthReq = classCode.Length >= minLen;
            bool doesNotContainSymbols = true;

            foreach (char x in classCode)
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

        public async void genCode()
        {

            //int code = Convert.ToInt32(idField.Text);

            //string hexCode = code.ToString("X2");

            if(classCodeValidation(idField.Text) && (schoolNameField.Text.Length > 2))
            {
                thisClass.schoolCode = idField.Text; //grabs all the variables into a schoolClass object
                thisClass.teacherUsername = person.Name;
                thisClass.schoolScore = person.Score;
                thisClass.totalSchoolAnswered = person.totalAnswered;
                thisClass.schoolName = schoolNameField.Text;

                classhelper.createClient(thisAuthLink.FirebaseToken); //create a firebase client

                List<schoolClass> allClasses = new List<schoolClass>(); //list for all classes

                allClasses = await classhelper.GetAllSchools(); //get all of the classes into list

                bool uniqueID = true; //this will only set to false if another id the same is found

                foreach (schoolClass x in allClasses) //for every class in the list
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
                    if (uniqueID)
                        await classhelper.addSchoolClass(thisClass);
                }
                catch
                {
                    await DisplayAlert("Error", "Error occured with database", "Ok");
                    await Navigation.PopModalAsync();
                }
            }
            else
            {

                await DisplayAlert("Error", "You must populate every field", "Ok");

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