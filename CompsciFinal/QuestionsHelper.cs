using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;

namespace CompsciFinal
{
    class QuestionsHelper
    {

        FirebaseClient firebase = new FirebaseClient("https://compsci-c8f5a.firebaseio.com//"); //get our firebase databases

        public async Task<List<Question>> GetAllQuestions() //getting all questions in the database
        {

            return (await firebase
              .Child("Questions")
              .OnceAsync<Question>()).Select(item => new Question
              {
                  QuestionText = item.Object.QuestionText, //get all the info from each question
                  CorrectAnswer = item.Object.CorrectAnswer,
                  IncorrectAnswerOne = item.Object.IncorrectAnswerOne,
                  IncorrectAnswerTwo = item.Object.IncorrectAnswerTwo,
                  IncorrectAnswerThree = item.Object.IncorrectAnswerThree,
                  tag = item.Object.tag,
                  votecount = item.Object.votecount
              }).ToList();
        }

        public async Task AddQuestion(string QuestionTextA, string CorrectAnswerA, string IncorrectAnswerA, string IncorrectAnswerB, string IncorrectAnswerC, string tags) //creating new questions
        {

            await firebase
              .Child("Questions")
              .PostAsync(new Question() { QuestionText = QuestionTextA, CorrectAnswer = CorrectAnswerA, IncorrectAnswerOne = IncorrectAnswerA, IncorrectAnswerTwo = IncorrectAnswerB, IncorrectAnswerThree = IncorrectAnswerC, votecount=20, tag=tags });
        }

        public async Task<Question> GetQuestion(string QuestionTextA) //downloading a specific question (not in use)
        {
            var allPersons = await GetAllQuestions();
            await firebase
              .Child("Questions")
              .OnceAsync<Person>();
            return allPersons.Where(a => a.QuestionText == QuestionTextA).FirstOrDefault();
        }

        public async Task UpdateQuestion(string QuestionTextA, string CorrectAnswerA, string IncorrectAnswerA, string IncorrectAnswerB, string IncorrectAnswerC, string tags, int votecountA) //updating a question
        {
            var toUpdateQuestion = (await firebase
              .Child("Questions")
              .OnceAsync<Question>()).Where(a => a.Object.QuestionText == QuestionTextA).FirstOrDefault();

            await firebase
              .Child("Questions")
              .Child(toUpdateQuestion.Key)
              .PutAsync(new Question() { QuestionText = QuestionTextA, votecount = votecountA, CorrectAnswer = CorrectAnswerA, IncorrectAnswerOne = IncorrectAnswerA, IncorrectAnswerTwo = IncorrectAnswerB, IncorrectAnswerThree = IncorrectAnswerC, tag = tags });
        }

        public async Task DeleteQuestion(string QuestionTextA) //deleting a question (not in use)
        {
            var toDeleteQuestion = (await firebase
              .Child("Questions")
              .OnceAsync<Question>()).Where(a => a.Object.QuestionText == QuestionTextA).FirstOrDefault();
            await firebase.Child("Questions").Child(toDeleteQuestion.Key).DeleteAsync();

        }

    }
}
