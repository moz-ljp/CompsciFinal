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

        public async Task<List<Person>> GetAllPersons() //get all people with all the info
        {
            return (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Select(item => new Person
              {
                  Name = item.Object.Name,
                  Score = item.Object.Score,
                  PersonId = item.Object.PersonId,
                  totalAnswered = item.Object.totalAnswered

              }).ToList();
        }

        public async Task<List<uid>> GetAllUids()
        {
            return (await firebase
                .Child("Persons")
                .OnceAsync<uid>()).Select(item => new uid
                {
                    uidString = item.Object.ToString()
                }).ToList();
        }

        public async Task AddPerson(string name, string uid, int score, int totalAnsweredB) //add an account (creating account in login page)
        {

            await firebase
              .Child("Persons").Child(uid)
              .PostAsync(new Person() { Name = name , Score = 0, PersonId = uid, totalAnswered = totalAnsweredB});
        }

        public async Task<Person> GetPerson(string uid) //getting an account (logging in on logging page)
        {

            return (await firebase.Child("Persons").Child(uid).OnceAsync<Person>()).Select(item => new Person
            {
                Name = item.Object.Name,
                Score = item.Object.Score,
                PersonId = item.Object.PersonId,
                totalAnswered = item.Object.totalAnswered

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
              .PutAsync(new Person() { Name = person.Name, Score = newScore, PersonId = person.PersonId, totalAnswered = newTotal});

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
