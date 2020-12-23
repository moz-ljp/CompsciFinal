using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompsciFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class shitQuestionBuilder : ContentPage
    {

        List<CompsciFinal.Question> Questions = new List<CompsciFinal.Question>(); //a list for all questions to be acquired into

        List<string> TagList = new List<string>(); //stores all of the tags available

        List<string> ChosenTags = new List<string>(); //stores the tags the user chooses

        string tagschosen = ""; //used for storing and displaying current chosen tag

        QuestionsHelper questionsHelper = new QuestionsHelper(); //new instance of question helper

        int totalQuestionCount = 0;

        Person thisPerson;

        FirebaseAuthLink thisAuthLink;

        public shitQuestionBuilder(Person person, FirebaseAuthLink authLink)
        {
            InitializeComponent();

            thisAuthLink = authLink;

            thisPerson = person;

            getTags();
        }

        public async void getTags()
        {
            var questions = await questionsHelper.GetAllQuestions(); //grab all of the questions

            Questions = questions.ToList(); //put the questions into a list of Question objects

            totalQuestionCount = Questions.Count();//count the amount of questions we have

            //await DisplayAlert("Total", Questions[0].tag.ToString(), "ok");

            //tagPicker.ItemsSource = new List<string>();

            foreach(var a in Questions) //for every question available in questions
            {
                //TagList.Add(a.tag);

                TagList.Add(a.tag.ToString().ToUpper()); //add that tag to our tag list

                //tagPicker.Items.Add(a.tag.ToString());

            }



            //await DisplayAlert("a", tagPicker.ItemsSource[0].ToString(), "ok");

            TagList = TagList.Distinct().ToList(); //distinct ensures elements can only occur once (no repeats)

            tagPicker.ItemsSource = TagList; //sets the source for the content of the pickers
            tagPickerCreation.ItemsSource = TagList;




        }

        public async void addTagClicked(object sender, EventArgs e)
        {
            if(tagPicker.SelectedItem != null) //checks there is a tag in the picker
            {
                ChosenTags.Add(tagPicker.SelectedItem.ToString()); //add the tag to our chosen tags list

                tagschosen += tagPicker.SelectedItem.ToString() + ", "; //adds to our variable

                taglist.Text = tagschosen; //takes variable into list

            }
            else
            {
                await DisplayAlert("Error", "You need to choose a tag first", "OK");
            }

        }

        public async void btnCreateQ_Clicked(object sender, EventArgs e) //for creating questions
        {
            if (Question.Text.Length != 0 && Question.Text.Length < 40 && Question.Text.Length > 3) //checks the question length is not 0 and is not more than 40 char
            {
                if (Correct.Text.Length != 0 && Correct.Text.Length < 20) //checks the correct answer is not 0 and is not more than 20 char
                {

                    if (IncorrectA.Text.Length != 0 && IncorrectA.Text.Length < 20 && IncorrectB.Text.Length != 0 && IncorrectB.Text.Length < 20 && IncorrectC.Text.Length != 0 && IncorrectC.Text.Length < 20) //checks the incorrect answers length

                    {
                        await questionsHelper.AddQuestion(Question.Text, Correct.Text, IncorrectA.Text, IncorrectB.Text, IncorrectC.Text, tagPickerCreation.SelectedItem.ToString());//Tags.Text); adds a this new question to database
                        //txtId.Text = string.Empty;
                        Question.Text = String.Empty; //clears all of the strings
                        Correct.Text = String.Empty;
                        IncorrectA.Text = String.Empty;
                        IncorrectB.Text = String.Empty;
                        IncorrectC.Text = String.Empty;
                        //Tags.Text = String.Empty;
                        await DisplayAlert("Success", "Question Created", "OK");
                        //var allPersons = await firebaseHelper.GetAllPersons();
                    }
                    else
                    {
                        if (IncorrectA.Text.Length == 0)
                        {
                            await DisplayAlert("Error", "Your incorrect answers cannot be empty", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Error", "Your incorrect answers cannot be too long", "OK");
                        }

                    }


                }
                else
                {
                    if (Correct.Text.Length == 0)
                    {
                        await DisplayAlert("Error", "Your answer cannot be empty", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Your answer cannot be too long", "OK");
                    }

                }


            }
            else
            {
                if(Question.Text.Length == 0)
                {
                    await DisplayAlert("Error", "Your question cannot be empty", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Your question cannot be too long", "OK");
                }
                
            }
        }

        private async void submitbtn_Clicked(object sender, EventArgs e)
        {

            if (thisPerson.PersonId != null) //if user account is logged in
            {
                if (ChosenTags != null) //if chosentags list actually has tasks in, submit them
                {
                    await Navigation.PushModalAsync(new MainPage(thisPerson, ChosenTags, thisAuthLink)); //push to new page with user account and tags
                }
                else //otherwise give 'all' tag so that mainpage knows to accept all questions
                {
                    ChosenTags.Add("all");
                    await Navigation.PushModalAsync(new MainPage(thisPerson, ChosenTags, thisAuthLink));
                }
            }


            
            
            
        }

       

    }
}