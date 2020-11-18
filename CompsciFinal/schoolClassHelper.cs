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
    class schoolClassHelper
    {

        FirebaseClient firebase = new FirebaseClient("https://compsci-c8f5a.firebaseio.com//"); //get our firebase databases

        public async Task<List<schoolClass>> GetAllSchools() //getting all questions in the database
        {

            return (await firebase
              .Child("schoolClasses")
              .OnceAsync<schoolClass>()).Select(item => new schoolClass
              {
                  teacherUsername = item.Object.teacherUsername,
                  schoolName = item.Object.schoolName,
                  Students = item.Object.Students,
                  schoolScore = item.Object.schoolScore,
                  schoolCode = item.Object.schoolCode
                  


              }).ToList();
        }

        public async Task AddschoolClass() //creating new questions
        {

            await firebase
              .Child("schoolClasses")
              .PostAsync(new schoolClass() {  });
        }

        public async Task<schoolClass> GetschoolClass(string schoolName) //downloading a specific question (not in use)
        {
            var allSchools = await GetAllSchools();
            await firebase
              .Child("schoolClasses")
              .OnceAsync<schoolClass>();
            return allSchools.Where(a => a.schoolName == schoolName).FirstOrDefault();
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
