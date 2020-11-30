using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;

namespace CompsciFinal
{
    class FirebaseHelper
    {

        FirebaseClient firebase;


        public async void createClient(string auth)
        {
            FirebaseOptions options = new FirebaseOptions()
            {
                AuthTokenAsyncFactory = async () => auth
                
            };
            firebase = new FirebaseClient("https://compsci-c8f5a.firebaseio.com//", options);
        }

        class PersonObject
        {
            public Person person;
        }

        public async Task<List<string>> GetAllPersons() //get all people with all the info
        {

            //System.Diagnostics.Debug.Write("------------------------------------------");

            List<string> ids = new List<string>();

            var people = await firebase
                .Child("Persons")
                .OnceAsync<object>();

            foreach(var person in people)
            {
                //System.Diagnostics.Debug.Write("------------------------------------------", person.Key);
                ids.Add(person.Key);
            }

            return ids;

            /*return (await firebase
              .Child("Persons").Child()
              .OnceAsync<Person>()).Select(item => new Person
              {
                  Name = item.Object.Name,
                  Score = item.Object.Score,
                  PersonId = item.Object.PersonId,
                  totalAnswered = item.Object.totalAnswered

              }).ToList();*/

        }


        public async Task AddPerson(string name, string uid, int score, int totalAnsweredB) //add an account (creating account in login page)
        {

            await firebase
              .Child("Persons").Child(uid)
              .PostAsync(new Person() { Name = name , Score = 0, PersonId = uid, totalAnswered = totalAnsweredB, cyberScore = 0, totalCyber = 0, totalProgramming = 0, totalConversions = 0, totalHardware = 0, totalSoftware = 0, softwareScore = 0, conversionsScore = 0, hardwareScore = 0, programmingScore=0, classCode = "", teacher=false});
        }

        public async Task<Person> GetPerson(string uid) //getting an account (logging in on logging page)
        {

            return (await firebase.Child("Persons").Child(uid).OnceAsync<Person>()).Select(item => new Person
            {
                Name = item.Object.Name,
                Score = item.Object.Score,
                PersonId = item.Object.PersonId,
                totalAnswered = item.Object.totalAnswered,
                cyberScore = item.Object.cyberScore,
                totalCyber = item.Object.totalCyber,
                softwareScore = item.Object.softwareScore,
                totalSoftware = item.Object.totalSoftware,
                hardwareScore = item.Object.totalHardware,
                totalHardware = item.Object.totalHardware,
                conversionsScore = item.Object.conversionsScore,
                totalConversions = item.Object.totalConversions,
                programmingScore = item.Object.programmingScore,
                totalProgramming = item.Object.totalProgramming,
                classCode = item.Object.classCode,
                teacher = item.Object.teacher,

            }).Where(a => a.PersonId == uid).FirstOrDefault();
        }

        public async Task UpdatePerson(Person person, int newScore, int newTotal) //update account (updating password in account page)
        {

            System.Diagnostics.Debug.Write("-------- Point 0 -------");

            System.Diagnostics.Debug.Write("Person ID:", person.PersonId);

            /*
            var toUpdatePerson = (await firebase.Child("Persons").Child(person.PersonId).OnceAsync<Person>()).Select(item => new Person
            {
                Name = item.Object.Name,
                Score = item.Object.Score,
                PersonId = item.Object.PersonId

            }).Where(a => a.PersonId == person.PersonId).FirstOrDefault();
            */

            var toUpdatePerson = (await firebase
              .Child("Persons").Child(person.PersonId)
              .OnceAsync<Person>()).Where(a => a.Object.PersonId == person.PersonId).FirstOrDefault();

            System.Diagnostics.Debug.Write("-------- Point 1 -------");

            System.Diagnostics.Debug.Write("newTotal:"+ newTotal, "Old total:"+ person.totalAnswered.ToString());

            await firebase
              .Child("Persons")
              .Child(person.PersonId)
              .Child(toUpdatePerson.Key)
              .PutAsync(new Person() { Name = person.Name, Score = newScore, PersonId = person.PersonId, totalAnswered = newTotal, softwareScore=person.softwareScore, programmingScore=person.programmingScore, totalProgramming=person.totalProgramming, totalConversions=person.totalConversions, conversionsScore = person.conversionsScore, cyberScore = person.cyberScore, hardwareScore = person.hardwareScore, totalCyber = person.totalCyber, totalHardware = person.totalHardware, totalSoftware = person.totalSoftware, classCode = person.classCode, teacher = false});

            System.Diagnostics.Debug.Write("-------- Point 2 -------");
        }

        public async Task UpdatePersonAdmin(string name, int score, bool tritA, bool deuterA, bool protA, string email, bool isAdmin) //update account (updating password in account page)
        {
            var toUpdatePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.Name == name).FirstOrDefault();

            await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .PutAsync(new Person() { Name = name });
        }

        public async Task DeletePerson(string name) //deleting account (deleting account in accounts page)
        {
            var toDeletePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.Name == name).FirstOrDefault();
            await firebase.Child("Persons").Child(toDeletePerson.Key).DeleteAsync();

        }

    }
}
