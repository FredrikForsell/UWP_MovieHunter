﻿using MovieHunter.DataAccess.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MovieHunter.DataAccess.Client.ApiCalls
{
    public static class PersonCalls
    {
        public static async Task<string> postPerson(Person newPerson)
        {
            //HttpPost
            string jsonInput = JsonConvert.SerializeObject(newPerson);

            var client = new HttpClient();
            try
            {
                string uri = "http://localhost:59713/api/People/add";
                var httpResponse = await client.PostAsync(uri, new StringContent(jsonInput, Encoding.UTF8, "application/json"));

                if (httpResponse.Content != null)
                {
                    var json = await httpResponse.Content.ReadAsStringAsync();
                    return json;
                }
            }
            catch
            {
                return "Failed to add person to database";
            }
            return "";
        }



        //HTTPGet for retrieving a list of all the people in the database
        public static async Task<ObservableCollection<Person>> GetPeople()
        {

            ObservableCollection<Person> returnList = new ObservableCollection<Person>();

            var client = new HttpClient();

            try
            {
                string uri = "http://localhost:59713/api/People/";

                //Sends the post request
                var httpResponse = await client.GetAsync(uri);

                //If there is a response from the server
                if (httpResponse.Content != null)
                {
                    var json = await httpResponse.Content.ReadAsStringAsync();

                    //Reading the response so that it will only return the string Name
                    JsonTextReader reader = new JsonTextReader(new StringReader(json));

                    //Loops through all elements in the Person HttpResponse
                    while (reader.Read())
                    {
                        if ((reader.TokenType == JsonToken.StartObject))
                        {
                            // Load object from the stream and do something with it
                            JObject obj = JObject.Load(reader);

                            //Adding the object to the returnList
                            returnList.Add(new Person
                            {
                                PersonId = Convert.ToInt32(obj["personId"]),
                                FirstName = Convert.ToString(obj["firstName"]),
                                LastName = Convert.ToString(obj["lastName"]),
                            });
                        }
                    }
                }
                else
                {
                    return returnList;
                }
            }

            catch
            {
                return returnList;
            }

            //Returning the People List
            return returnList;
        }

        //Uses a list of People to find the name using the personId
        public static string GetPersonNameFromList(ObservableCollection<Person> list, int personId)
        {
            //Looking at all person objects
            foreach (Person person in list)
            {
                //If the person object matches the id
                if (person.PersonId == personId)
                {
                    //Return the name
                    return person.FirstName + " " + person.LastName;
                }
            }
            //Could not find that personId in the list
            return "Could not find the persons name in the list";

        }
    }
}
