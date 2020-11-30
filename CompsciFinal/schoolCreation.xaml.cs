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

        schoolClass thisClass;

        schoolClassHelper classhelper;

        FirebaseAuthLink thisAuthLink;

        public schoolCreation(Person person, FirebaseAuthLink authlink)
        {
            InitializeComponent();

            this.person = person;

            thisAuthLink = authlink;

        }

        public async void genCode()
        {

            int code = Convert.ToInt32(idField.Text);

            string hexCode = code.ToString("X2");

            thisClass.schoolCode = hexCode;
            thisClass.teacherUsername = person.Name;
            thisClass.schoolScore = person.Score;
            thisClass.totalSchoolAnswered = person.totalAnswered;
            thisClass.schoolName = schoolNameField.Text;

            classhelper.createClient(thisAuthLink.FirebaseToken);

            await classhelper.AddschoolClass(thisClass);
            
        }

        private void btnCreateQ_Clicked(object sender, EventArgs e)
        {

        }

        private void btnSolveID_Clicked(object sender, EventArgs e)
        {
            genCode();
        }
    }
}