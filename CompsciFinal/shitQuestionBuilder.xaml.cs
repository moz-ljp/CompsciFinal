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

        List<CompsciFinal.Question> Questions = new List<CompsciFinal.Question>();

        List<string> TagList = new List<string>();

        List<string> ChosenTags = new List<string>();

        string tagschosen = "";

        QuestionsHelper questionsHelper = new QuestionsHelper();

        int total = 0;

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
            var questions = await questionsHelper.GetAllQuestions();

            Questions = questions.ToList();

            total = Questions.Count();

            //await DisplayAlert("Total", Questions[0].tag.ToString(), "ok");

            //tagPicker.ItemsSource = new List<string>();

            foreach(var a in Questions)
            {
                //TagList.Add(a.tag);

                TagList.Add(a.tag.ToString().ToUpper());

                //tagPicker.Items.Add(a.tag.ToString());

            }



            //await DisplayAlert("a", tagPicker.ItemsSource[0].ToString(), "ok");

            TagList = TagList.Distinct().ToList();

            tagPicker.ItemsSource = TagList;
            tagPickerCreation.ItemsSource = TagList;




        }

        public async void addTagClicked(object sender, EventArgs e)
        {
            if(tagPicker.SelectedItem != null)
            {
                ChosenTags.Add(tagPicker.SelectedItem.ToString());

                tagschosen += tagPicker.SelectedItem.ToString() + ", ";

                taglist.Text = tagschosen;

            }
            else
            {
                await DisplayAlert("Error", "You need to choose a tag first", "OK");
            }

        }

        public async void btnCreateQ_Clicked(object sender, EventArgs e)
        {
            if (Question.Text.Length != 0 && Question.Text.Length < 40)
            {
                if (Correct.Text.Length != 0 && Correct.Text.Length < 20)
                {

                    if (IncorrectA.Text.Length != 0 && IncorrectA.Text.Length < 20)

                    {
                        await questionsHelper.AddQuestion(Question.Text, Correct.Text, IncorrectA.Text, IncorrectB.Text, IncorrectC.Text, tagPickerCreation.SelectedItem.ToString());//Tags.Text);
                        //txtId.Text = string.Empty;
                        Question.Text = String.Empty;
                        Correct.Text = String.Empty;
                        IncorrectA.Text = String.Empty;
                        IncorrectB.Text = String.Empty;
                        IncorrectC.Text = String.Empty;
                        //Tags.Text = String.Empty;
                        await DisplayAlert("Success", "Question Created", "OK");
                        //var allPersons = await firebaseHelper.GetAllPersons();
                        //lstPersons.ItemsSource = allPersons;
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
                    await Navigation.PushModalAsync(new MainPage(thisPerson, ChosenTags, thisAuthLink));
                }
                else //otherwise give 'all' tag so that mainpage knows to accept all questions
                {
                    ChosenTags.Add("all");
                    await Navigation.PushModalAsync(new MainPage(thisPerson, ChosenTags, thisAuthLink));
                }
            }
            else
            {
                if (thisPerson.PersonId != null) //if user account is logged in
                {
                    if (ChosenTags != null) //if chosentags list actually has tasks in, submit them
                    {
                        await Navigation.PushModalAsync(new MainPage(thisPerson, ChosenTags, thisAuthLink));
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
}