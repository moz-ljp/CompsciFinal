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
    public partial class ClassStats : ContentPage
    {

        Person person;

        FirebaseAuthLink authLink;

        schoolClass thisClass = new schoolClass();

        FirebaseHelper firebaseHelper = new FirebaseHelper();

        schoolClassHelper classHelper = new schoolClassHelper();

        List<Person> studentList = new List<Person>();

        List<Person> sortedStudentList = new List<Person>();
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

            try
            {
                thisClass = await classHelper.GetSchoolClass(person.classCode);
                schoolNameLabel.Text = thisClass.schoolName;
                schoolCodeLabel.Text = thisClass.schoolCode;

                firebaseHelper.createClient(authLink.FirebaseToken);

                List<string> ids = new List<string>();

                ids = await firebaseHelper.GetAllPersons();

                foreach (string x in ids)
                {
                    studentList.Add(await firebaseHelper.GetPerson(x));
                    //await DisplayAlert("ok", x, "ok");
                }

                foreach(Person x in studentList)
                {
                    if(x.classCode == thisClass.schoolCode)
                    {
                        sortedStudentList.Add(x);
                    }
                }

                int globalScore = 0;

                foreach(Person x in sortedStudentList)
                {
                    globalScore += x.Score;
                }

                studentCountLabel.Text = sortedStudentList.Count().ToString();

                totalClassScoreLabel.Text = globalScore.ToString();

                var personsVar = sortedStudentList;

                var OrderedPersons = personsVar.OrderBy(f => f.Score).Reverse();

                List<Person> sortedAndOrderedStudents = new List<Person>();

                sortedAndOrderedStudents = OrderedPersons.ToList<Person>();

                int currentStudentPos = 0;

                foreach(Person x in sortedAndOrderedStudents)
                {
                    if(x == person)
                    {
                        currentStudentPos = sortedAndOrderedStudents.IndexOf(x);
                    }
                }

                currentStudentPos += 1;

                thisStudentPosInClass.Text = currentStudentPos.ToString();

            }

            catch
            {
                await DisplayAlert("Error", "Class not found", "Ok");
                await Navigation.PopModalAsync();
            }

        }


    }
}