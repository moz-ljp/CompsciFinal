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

        List<Person> sortedOrderedStudents = new List<Person>();
        public ClassStats(Person person, FirebaseAuthLink authLink)
        {
            InitializeComponent();

            this.Content.IsVisible = false;

            this.person = person;
            this.authLink = authLink;

            if (!person.teacher)
            {
                studentPicker.IsVisible = false;
                selectStudent.IsVisible = false;
            }
                

            getClass();

        }

        public async void getClass()
        {

            base.OnAppearing();

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
                    studentList.Add(await firebaseHelper.GetPerson(x)); //get every student into a list
                    //await DisplayAlert("ok", x, "ok");
                }

                foreach(Person x in studentList)
                {
                    if(x.classCode == thisClass.schoolCode) // if the student has this classes class code
                    {
                        sortedStudentList.Add(x); //add them to a different list
                    }
                }

                int globalScore = 0;

                foreach(Person x in sortedStudentList) //for every student
                {
                    globalScore += x.Score; //add their score to the total tally
                }

                studentCountLabel.Text = "Students in class: " + sortedStudentList.Count().ToString();

                totalClassScoreLabel.Text = "Total Class Score: " + globalScore.ToString();

                var personsVar = sortedStudentList;

                var OrderedPersons = personsVar.OrderBy(f => f.Score).Reverse(); //sort the students by score

                List<Person> sortedAndOrderedStudents = new List<Person>();

                sortedAndOrderedStudents = OrderedPersons.ToList<Person>();

                int currentStudentPos = 0;

                foreach(Person x in sortedAndOrderedStudents)
                {
                    if(x == person) //get this users position in the list of students
                    {
                        currentStudentPos = sortedAndOrderedStudents.IndexOf(x);
                    }
                }

                currentStudentPos += 1; // add one (as the list begins at index 0)

                thisStudentPosInClass.Text = "Your position in the class: " +currentStudentPos.ToString(); //display pos

                bestStudentLabel.Text = "Best student in class: "+ sortedAndOrderedStudents.First<Person>().Name; //display best student

                List<string> studentNames = new List<string>(); //list for student names

                foreach (Person x in sortedAndOrderedStudents) //get every student name into list
                { 
                    studentNames.Add(x.Name);

                }

                studentPicker.ItemsSource = studentNames; //set the source for an item picker.

                sortedOrderedStudents = sortedAndOrderedStudents;
                this.Content.IsVisible = true;


            }

            catch
            {
                await DisplayAlert("Error", "Class not found", "Ok");
                await Navigation.PopModalAsync();
            }

        }

        private void selectStudent_Clicked(object sender, EventArgs e)
        {
            string studentName = studentPicker.SelectedItem.ToString();

            if(studentName != null)
            {
                foreach (Person x in sortedOrderedStudents)
                {
                    if (x.Name == studentName)
                    {
                        string rank = calculateRank(x);
                        openStats(x, rank);
                    }
                }
            }

            

        }

        public async void openStats(Person student, string rank)
        {
            await Navigation.PushModalAsync(new studentViewer(student, rank));
        }

        public string calculateRank(Person thisStudent)
        {

            Rank beginner = new Rank();
            beginner.rankName = "Beginner";
            beginner.requiredScore = 0;

            Rank novice = new Rank();
            novice.rankName = "Novice";
            novice.requiredScore = 100;

            Rank adept = new Rank();
            adept.rankName = "Adept";
            adept.requiredScore = 200;

            Rank expert = new Rank();
            expert.rankName = "Expert";
            expert.requiredScore = 400;

            Rank master = new Rank();
            master.rankName = "Master";
            master.requiredScore = 800;

            Rank god = new Rank();
            god.rankName = "God";
            god.requiredScore = 1600;

            Rank genius = new Rank();
            genius.rankName = "Genius";
            genius.requiredScore = 3200;

            List<Rank> ranks = new List<Rank>();

            ranks.Add(beginner);
            ranks.Add(novice);
            ranks.Add(adept);
            ranks.Add(expert);
            ranks.Add(master);
            ranks.Add(god);
            ranks.Add(genius);

            int thisScore = thisStudent.Score;
            double successRate = successRateCalculator(thisScore, thisStudent.totalAnswered);

            string rank = "";

            int element = 0;


            while (thisScore >= ranks[element].requiredScore && element < ranks.Count)
            {
                if (ranks[element + 1].requiredScore > thisScore)
                {
                    rank = ranks[element].rankName;
                    System.Diagnostics.Debug.Write("Set Rank");
                    break;
                }
                else
                {
                    element++;
                    System.Diagnostics.Debug.Write("Increased element");
                }
            }

            if (successRate < 40)
            {
                rank = ranks[element - 1].rankName;
            }
            else if (successRate > 60)
            {
                rank = ranks[element + 1].rankName;
            }

            return rank;

        }

        public double successRateCalculator(int correct, int total)
        {

            return Convert.ToDouble((Convert.ToDecimal(correct) / Convert.ToDecimal(total)) * 100);

        }

        private async void backBTN_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}