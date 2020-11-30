﻿using System;
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

        public async void createClient(string auth)
        {
            FirebaseOptions options = new FirebaseOptions()
            {
                AuthTokenAsyncFactory = async () => auth

            };
            firebase = new FirebaseClient("https://compsci-c8f5a.firebaseio.com//", options);
        }

        public async Task<List<schoolClass>> GetAllSchools()
        {

            return (await firebase
              .Child("schoolClasses")
              .OnceAsync<schoolClass>()).Select(item => new schoolClass
              {
                  teacherUsername = item.Object.teacherUsername,
                  schoolName = item.Object.schoolName,
                  schoolScore = item.Object.schoolScore,
                  schoolCode = item.Object.schoolCode
                  


              }).ToList();
        }

        public async Task AddschoolClass(schoolClass thisClass) //creating school class
        {

            await firebase
              .Child("schoolClasses")
              .PostAsync(new schoolClass() { schoolName = thisClass.schoolName, schoolCode = thisClass.schoolCode, schoolScore = thisClass.schoolScore, teacherUsername = thisClass.teacherUsername, totalSchoolAnswered = thisClass.totalSchoolAnswered });
        }

        public async Task<schoolClass> GetschoolClass(string schoolName)
        {
            var allSchools = await GetAllSchools();
            await firebase
              .Child("schoolClasses")
              .OnceAsync<schoolClass>();
            return allSchools.Where(a => a.schoolName == schoolName).FirstOrDefault();
        }

        public async Task UpdateClass(schoolClass thisClass) //updating a question
        {
            var toUpdateSchool = (await firebase
              .Child("schoolClasses")
              .OnceAsync<schoolClass>()).Where(a => a.Object.schoolCode == thisClass.schoolCode).FirstOrDefault();

            await firebase
              .Child("schoolClasses")
              .Child(toUpdateSchool.Key)
              .PutAsync(new schoolClass() { totalSchoolAnswered = thisClass.totalSchoolAnswered, schoolCode = thisClass.schoolCode, teacherUsername = thisClass.teacherUsername, schoolScore = thisClass.schoolScore, schoolName = thisClass.schoolName });
        }

        public async Task DeleteQuestion(string QuestionTextA)
        {
            var toDeleteQuestion = (await firebase
              .Child("Questions")
              .OnceAsync<Question>()).Where(a => a.Object.QuestionText == QuestionTextA).FirstOrDefault();
            await firebase.Child("Questions").Child(toDeleteQuestion.Key).DeleteAsync();

        }

    }
}
