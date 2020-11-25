using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Firebase.Database;
using Firebase;
using Firebase.Database.Query;
using Firebase.Auth;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiteDB;
using Plugin.SimpleAudioPlayer;

namespace CompsciFinal
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        FirebaseHelper firebaseHelper = new FirebaseHelper();

        QuestionsHelper questionsHelper = new QuestionsHelper();

        ISimpleAudioPlayer correctSound = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        ISimpleAudioPlayer incorrectSound = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();

        int count = 0;

        int prev = 0;

        int score;

        int totalAnswered = 0;

        int currentrandom;

        int currentvotecount = 0;

        int total;

        string correctAnswer;

        bool solved = false;

        bool questionsAcquired = false;

        public string thisusername = "";

        List<CompsciFinal.Question> Questions = new List<CompsciFinal.Question>();

        List<CompsciFinal.Question> preQuestions = new List<CompsciFinal.Question>();

        List<string> chosenTags = new List<string>();

        public string authuid;

        public string thisuid;

        string thisTag;

        Person person;

        bool masterLogged = false;

        FirebaseAuthLink thisauthLink;

        FirebaseClient firebase = new FirebaseClient("https://compsci-c8f5a.firebaseio.com//");

        private const string BaseUrl = "https://compsci-c8f5a.firebaseio.com//";

        public List<string> newTagsList = new List<string>();
        public List<int> newTagScore = new List<int>();
        public List<int> newTagTotal = new List<int>();

        //private ChildQuery _query;

        public MainPage(Person user, List<String> Tags, FirebaseAuthLink authLink)
        {
            InitializeComponent();

            Navigation.PopToRootAsync();

            NavigationPage.SetHasNavigationBar(this, false);
            
            thisuid = user.PersonId;

            correctSound.Load("correct.wav");
            incorrectSound.Load("incorrect.wav");


            thisauthLink = authLink;

            person = user;

            totalAnswered = user.totalAnswered;

            thisusername = user.Name;

            chosenTags = Tags;

            usernameLabel.Text = thisusername;

            score = user.Score;

            counterLabel.Text = score.ToString();

            if (thisuid != null)
            {
                masterLogged = true;
                firebaseHelper.createClient(authLink.FirebaseToken);

            }

        }

        public async void answeroneclicked(object sender, EventArgs e)
        {
            string thisanswer = answerone.Text;

            if (solved == false)
            {
                totalAnswered += 1;

                if (thisanswer == correctAnswer)
                {
                    resultLabel.Text = "Correct";
                    resultLabel.TextColor = Color.Lime;
                    score += 1;
                    increaseTagScore(true);
                    playAudio(true);
                    //incScore();
                    counterLabel.Text = score.ToString();
                }
                else
                {
                    counterLabel.Text = score.ToString();
                    resultLabel.Text = "Incorrect";
                    increaseTagScore(false);
                    playAudio(false);
                    resultLabel.TextColor = Color.Red;
                }
                if (masterLogged)
                {
                    await firebaseHelper.UpdatePerson(person, score, totalAnswered);
                }
            }

            solved = true;
        }

        public async void answertwoclicked(object sender, EventArgs e)
        {
            string thisanswer = answertwo.Text;

            if (solved == false)
            {
                totalAnswered += 1;

                if (thisanswer == correctAnswer)
                {
                    resultLabel.Text = "Correct";
                    resultLabel.TextColor = Color.Lime;
                    score += 1;
                    increaseTagScore(true);
                    playAudio(true);
                    //incScore();
                    counterLabel.Text = score.ToString();
                }
                else
                {
                    counterLabel.Text = score.ToString();
                    resultLabel.Text = "Incorrect";
                    increaseTagScore(false);
                    playAudio(false);
                    resultLabel.TextColor = Color.Red;
                }
                if (masterLogged)
                {
                    await firebaseHelper.UpdatePerson(person, score, totalAnswered);
                }
            }

            solved = true;
        }

        public async void answerthreeclicked(object sender, EventArgs e)
        {
            string thisanswer = answerthree.Text;

            if (solved == false)
            {
                totalAnswered += 1;

                if (thisanswer == correctAnswer)
                {
                    resultLabel.Text = "Correct";
                    resultLabel.TextColor = Color.Lime;
                    score += 1;
                    increaseTagScore(true);
                    playAudio(true);
                    //incScore();
                    counterLabel.Text = score.ToString();
                }
                else
                {
                    counterLabel.Text = score.ToString();
                    resultLabel.Text = "Incorrect";
                    increaseTagScore(false);
                    playAudio(false);
                    resultLabel.TextColor = Color.Red;
                }

                if (masterLogged)
                {
                    await firebaseHelper.UpdatePerson(person, score, totalAnswered);
                }
            }

            solved = true;
        }

        public async void answerfourclicked(object sender, EventArgs e)
        {
            string thisanswer = answerfour.Text;

            if (solved == false)
            {
                totalAnswered += 1;

                if (thisanswer == correctAnswer)
                {
                    resultLabel.Text = "Correct";
                    resultLabel.TextColor = Color.Lime;
                    score += 1;
                    playAudio(true);
                    //incScore();
                    counterLabel.Text = score.ToString();
                    increaseTagScore(true);
                    
                }
                else
                {
                    counterLabel.Text = score.ToString();
                    resultLabel.Text = "Incorrect";
                    increaseTagScore(false);
                    playAudio(false);
                    resultLabel.TextColor = Color.Red;
                }

                if (masterLogged)
                {
                    await firebaseHelper.UpdatePerson(person, score, totalAnswered);
                }
            }

            solved = true;
        }

        public async void increaseTagScore(bool correct)
        {
            if(thisTag == "hardware")
            {
                if(correct)
                    person.hardwareScore++;
                person.totalHardware++;
            }
            else if(thisTag == "software")
            {
                if(correct)
                    person.softwareScore++;
                person.totalSoftware++;
            }
            else if(thisTag == "cyber security")
            {
                if(correct)
                    person.cyberScore++;
                person.totalCyber++;
            }
            else if(thisTag == "conversions")
            {
                if(correct)
                    person.conversionsScore++;
                person.totalConversions++;
            }
            else if(thisTag == "programming")
            {
                if(correct)
                    person.programmingScore++;
                person.totalProgramming++;
            }
            else
            {

            }
        }

        public async void playAudio(bool correct)
        {
            if (correct)
                correctSound.Play();
            else
                incorrectSound.Play();

        }



        private async void nextquestionclicked(object sender, EventArgs e)
        {
            /*
            try
            {
                await firebaseHelper.UpdatePerson("moz", authuid, 10);
                await DisplayAlert("Nice", "It worked", "Ok");
            }
            catch
            {
                await DisplayAlert("Fucked", "sorry mate, database fucked", "Ok");
            }
            
            */
            //await Navigation.PushModalAsync(new RootPage());

            if (count == 0)
            {

                try // on first run, download all the questions into a list 
                {
                    var questions = await questionsHelper.GetAllQuestions();

                    preQuestions = questions.ToList();

                    count = 1;

                    //await DisplayAlert("alert", chosenTags[0].ToString(), "ok");

                    if(!chosenTags.Contains("all")) //if chosentags does not contain all
                    {
                        
                        foreach (Question x in preQuestions)
                        {

                            if (chosenTags.Contains(x.tag.ToUpper()))
                                {
                                
                                Questions.Add(x);
                                //Debug.Write("Question added " + x.tag);

                            }
                        }
                    }
                    else
                    {
                        Questions = questions.ToList();
                        Debug.Write("No tags");
                    }
                    //Questions = questions.ToList();
                    total = Questions.Count();
                    questionsAcquired = true;
                }
                catch
                {
                    await DisplayAlert("Failed to load questions", "Check your internet access", "OK"); //if we cant get the questions, assume its a network problem
                }

            }

            if (questionsAcquired)
                {

                //await DisplayAlert("Alert", Questions[0].tag, "ok");

                Random rnd = new Random();
                    int randommessage = rnd.Next(0, total); //choose a random question


                    if (prev == randommessage) //if the random question is the same as the previous one, dont show it. might do this in a stack at some point
                    {
                        if (randommessage < (total - 1))
                        {
                            randommessage += 1;
                        }
                        else
                        {
                            randommessage -= 1;
                        }

                    }

                currentrandom = randommessage;

                

                Console.WriteLine(randommessage);



                    //string question = releases[randommessage]["question"].ToString();

                    string question = Questions[randommessage].QuestionText.ToString(); //set all relevant text to relevant info

                    string answer = Questions[randommessage].CorrectAnswer.ToString();

                    string incorrectA = Questions[randommessage].IncorrectAnswerOne.ToString();

                    string incorrectB = Questions[randommessage].IncorrectAnswerTwo.ToString();

                    string incorrectC = Questions[randommessage].IncorrectAnswerThree.ToString();

                if (masterLogged)
                    thisTag = Questions[randommessage].tag.ToLower();
                else
                    thisTag = null;
                    

                    solved = false; //say that the current question is not solved.

                    int rndPos = rnd.Next(0, 4); //choose a random position for the correct answer to the question, otherwise the correct answer would always be present in one place

                    if (rndPos == 0)
                    {
                        answerone.Text = answer;
                        answertwo.Text = incorrectA;
                        answerthree.Text = incorrectB;
                        answerfour.Text = incorrectC;
                    }
                    else if(rndPos == 1)
                    {
                        answerone.Text = incorrectC;
                        answertwo.Text = answer;
                        answerthree.Text = incorrectA;
                        answerfour.Text = incorrectB;
                    }
                    else if(rndPos == 2)
                    {
                        answerone.Text = incorrectB;
                        answertwo.Text = incorrectC;
                        answerthree.Text = answer;
                        answerfour.Text = incorrectA;
                    }
                    else
                    {
                        answerone.Text = incorrectA;
                        answertwo.Text = incorrectB;
                        answerthree.Text = incorrectC;
                        answerfour.Text = answer;
                    }


                    correctAnswer = answer;

                    questionLabel.Text = question;

                    resultLabel.Text = "";

                    prev = randommessage;
                }


            
        }

        private async void menuButton(object sender, EventArgs e)
        {
            //Person thisperson = await firebaseHelper.GetPerson(person.PersonId);
            person.Score = score;
            person.totalAnswered = totalAnswered;
            await Navigation.PushModalAsync(new MenuPage(person, thisauthLink));
        }

        private async void downVote_Clicked(object sender, EventArgs e)
        {
            //await DisplayAlert("ok", Questions[currentrandom].QuestionText, "ok");
            bool answer = await DisplayAlert("Vote Down Question", "Are you sure?", "Yes", "No");
            if(answer == true)
            {
                await questionsHelper.UpdateQuestion(questionLabel.Text, Questions[currentrandom].CorrectAnswer, Questions[currentrandom].IncorrectAnswerOne, Questions[currentrandom].IncorrectAnswerTwo, Questions[currentrandom].IncorrectAnswerThree, Questions[currentrandom].tag, (Questions[currentrandom].votecount - 1));
                if (Questions[currentrandom].votecount-1 < 1)
                {
                    await questionsHelper.DeleteQuestion(questionLabel.Text);
                }
            }
            
        }
    }
}
