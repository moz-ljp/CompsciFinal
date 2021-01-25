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

        bool votedDown = false;

        bool questionsDownloaded = false; //used to determine whether questions have been downloaded

        int prevQuestion = 0; //stores the index of the last question used

        int score; //stores the current score

        int totalAnswered = 0; //stores the total questions answered

        int currentrandom; //stores the current random number used to choose a question

        int currentvotecount = 0; //stores the current vote count of a question

        int totalQuestionAmount; //stores the total amount of questions

        string correctAnswer; //stores the string of the correct answer for comparison

        bool solved = false; //determines whether the question has been solved

        bool questionsAcquired = false; //determiens whether questions are ready for use

        public string thisusername = ""; //stores the users current username

        List<CompsciFinal.Question> Questions = new List<CompsciFinal.Question>(); //stores the list of questions for use

        List<CompsciFinal.Question> preQuestions = new List<CompsciFinal.Question>(); //stores the unsorted list of questions

        List<string> chosenTags = new List<string>(); //stores the tags chosen by the user

        public string authuid; //stores the authuid provided by firebase

        public string thisuid; //stores the uid of the user

        string thisTag; // stores the tag of the current question

        Person person; //stores the person object for the user

        bool masterLogged = false; //stores whether the user is logged in or not

        FirebaseAuthLink thisauthLink; //stores the authentication link delivered from firebase

        FirebaseClient firebase = new FirebaseClient("https://compsci-c8f5a.firebaseio.com//"); //stores the link to firebase database

        private const string BaseUrl = "https://compsci-c8f5a.firebaseio.com//"; //stores the link to firebase database

        public MainPage(Person user, List<String> Tags, FirebaseAuthLink authLink)
        {
            InitializeComponent();

            if(Tags.Count < 1) //if the list is somehow empty
                Tags.Add("all"); //add the all flag

            System.Diagnostics.Debug.Write("Hardware:", user.hardwareScore.ToString());


            Navigation.PopToRootAsync(); //clears any open pages

            NavigationPage.SetHasNavigationBar(this, false); //removes the top nav bar on android for appearances
            
            thisuid = user.PersonId; //sets up the uid of the person into a variable

            correctSound.Load("correct.wav"); //assigns sounds for the audio players
            incorrectSound.Load("incorrect.wav");


            thisauthLink = authLink; //pulls authLink from local to global variable

            person = user; //pulls user from local to script global

            totalAnswered = user.totalAnswered; //gets total answered question count

            thisusername = user.Name; //gets username

            chosenTags = Tags; //gets chosen tags

            usernameLabel.Text = thisusername; //sets the username label to the username

            score = user.Score; //gets the users score from obj

            counterLabel.Text = score.ToString(); //sets the score label

            if (thisuid != null) //check if logged in by seeing if the user has an ID
            {
                masterLogged = true; //if they are, set to true.
                firebaseHelper.createClient(authLink.FirebaseToken); //and create a client using the provided auth link

            }

        }

        public async void updateScore()
        {
            await firebaseHelper.UpdatePerson(person, score, totalAnswered); //updates the score using the firebase helper script
        }

        public async void answeroneclicked(object sender, EventArgs e) //all four are the same so only one will be commented
        {
            string thisAnswer = answerone.Text; //gets the users chosen answer from the buttons text

            if (solved == false) //checks to make sure the user hasnt already answered
            {
                totalAnswered += 1; //increases the total amount of questions the user has answered locally

                if (thisAnswer == correctAnswer) //checks if the answer provided is correct
                {
                    resultLabel.Text = "Correct"; //changes colours and words to give user feedback
                    resultLabel.TextColor = Color.Lime;
                    score += 1; //increase score
                    increaseTagScore(true); //actually where score is increased per tag
                    playAudio(true); //plays audio from another method
                    counterLabel.Text = score.ToString(); //reset label to reflect new score
                }
                else
                {
                    counterLabel.Text = score.ToString(); //reset score just in case it changes
                    resultLabel.Text = "Incorrect"; //change visuals to feed back to user
                    increaseTagScore(false); //do not increase the scores
                    playAudio(false); //play a negative sound
                    resultLabel.TextColor = Color.Red;
                }
                if (masterLogged) //if the user is logged in
                {
                    updateScore(); //update the users score with firebase
                }
            }

            solved = true;
        }

        public async void answertwoclicked(object sender, EventArgs e)
        {
            string thisAnswer = answertwo.Text;

            if (solved == false)
            {
                totalAnswered += 1;

                if (thisAnswer == correctAnswer)
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
                    updateScore();
                }
            }

            solved = true;
        }

        public async void answerthreeclicked(object sender, EventArgs e)
        {
            string thisAnswer = answerthree.Text;

            if (solved == false)
            {
                totalAnswered += 1;

                if (thisAnswer == correctAnswer)
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
                    updateScore();
                }
            }

            solved = true;
        }

        public async void answerfourclicked(object sender, EventArgs e)
        {
            string thisAnswer = answerfour.Text;

            if (solved == false)
            {
                totalAnswered += 1;

                if (thisAnswer == correctAnswer)
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
                    updateScore();
                }
            }

            solved = true;
        }

        public async void increaseTagScore(bool correct) //for increasing individual tags score
        {
            if(thisTag == "hardware") //check which tag it is
            {
                if(correct) //if the answer is correct
                    person.hardwareScore++; //increase the score
                person.totalHardware++; //always increase the total
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

        public async void playAudio(bool correct) //plays audio players
        {
            if (correct) //if the answer was correct
                correctSound.Play(); //play a positive sound
            else //otherwise
                incorrectSound.Play(); //play a negative sound

        }



        private async void nextquestionclicked(object sender, EventArgs e) //occurs when user requests a new question
        {

            if (questionsDownloaded == false) //if questions have not already been downloaded
            {

                try // on first run, download all the questions into a list 
                {
                    var questions = await questionsHelper.GetAllQuestions(); //get all of the questions into a list

                    preQuestions = questions.ToList();

                    questionsDownloaded = true; //state that all questions have been acquired

                    //await DisplayAlert("alert", chosenTags[0].ToString(), "ok");

                    if(!chosenTags.Contains("all")) //if chosentags does not contain all
                    {
                        
                        foreach (Question x in preQuestions) //for every question 
                        {

                            if (chosenTags.Contains(x.tag.ToUpper())) //if the tag is in the users desired tag list
                                {
                                
                                Questions.Add(x); //add that question to our list
                                //Debug.Write("Question added " + x.tag);

                            }
                        }
                    }
                    else //otherwise
                    {
                        Questions = questions.ToList(); //assign the full questions list to our list for use
                        Debug.Write("No tags");
                    }
                    totalQuestionAmount = Questions.Count(); //count the questions through
                    questionsAcquired = true; //state again that they are acquired for other purpose
                }
                catch //if an error occurs, report it as inability to access database due to internet connection - this should be the only error that can feasibly occur.
                {
                    await DisplayAlert("Failed to load questions", "Check your internet access", "OK"); //if we cant get the questions, assume its a network problem
                }

            }

            if (questionsAcquired) //if the questions are ready
                {

                //await DisplayAlert("Alert", Questions[0].tag, "ok");

                Random rnd = new Random(); //create a new random class obj
                int randomQ= rnd.Next(0, totalQuestionAmount); //choose a random question


                    if (prevQuestion == randomQ) //if the random question is the same as the previous one, dont show it. might do this in a stack at some point
                    {
                        if (randomQ < (totalQuestionAmount - 1)) //if the current message is going to cause an error
                        {
                            randomQ += 1; //increase it by 1 so it is in the list
                        }
                        else
                        {
                            randomQ -= 1; //otherwise, decrease by one
                        }

                    }

                currentrandom = randomQ; //assign to appropriate variable

                    //string question = releases[randommessage]["question"].ToString();

                    string question = Questions[randomQ].QuestionText.ToString(); //set all relevant text to relevant info - question

                    string answer = Questions[randomQ].CorrectAnswer.ToString(); //correct answer

                    string incorrectA = Questions[randomQ].IncorrectAnswerOne.ToString(); //first incorrect

                    string incorrectB = Questions[randomQ].IncorrectAnswerTwo.ToString(); //second incorrect

                    string incorrectC = Questions[randomQ].IncorrectAnswerThree.ToString(); //third incorret

                if (masterLogged) //if they are logged in
                    thisTag = Questions[randomQ].tag.ToLower(); //get the question tag
                else //otherwise
                    thisTag = null; //there is no need
                    

                    solved = false; //say that the current question is not solved.

                    int rndPos = rnd.Next(0, 4); //choose a random position for the correct answer to the question, otherwise the correct answer would always be present in one place

                    if (rndPos == 0) //jumble the answers by shifting them based upon the random
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

                    votedDown = false;

                    correctAnswer = answer; //assign the correct answer

                    questionLabel.Text = question; //set the question

                    resultLabel.Text = ""; //clear the question feedback

                    prevQuestion = randomQ; //assign prev question value for checks to ensure there are no repeating questions
                }


            
        }

        private async void menuButton(object sender, EventArgs e) //button for the menu
        {
            person.Score = score; //gets the current score
            person.totalAnswered = totalAnswered; //and total answered
            await Navigation.PushModalAsync(new MenuPage(person, thisauthLink)); //passes to menu page with person object and the auth link
        }

        private async void downVote_Clicked(object sender, EventArgs e) //down vote button (thumbs down)
        {
            //await DisplayAlert("ok", Questions[currentrandom].QuestionText, "ok");
            bool answer = false;

            if (votedDown)
                await DisplayAlert("Error", "You cannot vote down a question more than once", "Ok");
            else if(!votedDown)
                answer = await DisplayAlert("Vote Down Question", "Are you sure?", "Yes", "No"); //if they want to (check)

            if (!questionsAcquired)
                await DisplayAlert("Error", "No question loaded into application", "Ok");


            if (answer && masterLogged && !votedDown && questionsAcquired)
            { //reduce the questions vote count
                votedDown = true;
                await questionsHelper.UpdateQuestion(questionLabel.Text, Questions[currentrandom].CorrectAnswer, Questions[currentrandom].IncorrectAnswerOne, Questions[currentrandom].IncorrectAnswerTwo, Questions[currentrandom].IncorrectAnswerThree, Questions[currentrandom].tag, (Questions[currentrandom].votecount - 1));
                if (Questions[currentrandom].votecount-1 < 1) //if the questions vote count falls below 1
                {
                    await questionsHelper.DeleteQuestion(questionLabel.Text); //delete it
                }
            }
            
            
        }
    }
}
