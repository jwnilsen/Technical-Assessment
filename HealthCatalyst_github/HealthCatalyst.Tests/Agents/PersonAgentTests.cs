

using System.Text;
using HealthCatalyst.Agents;
using HealthCatalyst.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Data.Entity.Migrations;

namespace HealthCatalyst.Tests.Agents
{
    /// <summary>
    /// Test all services for the Person Entity
    /// </summary>
    /// 
    [TestClass]
    public class PersonAgentTests
    {

        private static string _connectionString = string.Empty;
        private static DalContext _dbContext = new DalContext();
        private AgentFactory agentFactory = new AgentFactory();

        public PersonAgentTests()
        {
        }

        public string connectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        public DalContext dbContext
        {
            get
            {
                return _dbContext;
            }
            set
            {
                _dbContext = value;
            }
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            Debug.WriteLine("ClassSetup");
            PersonAgentTests thisClass = new PersonAgentTests();
            thisClass.ClearAndLoadDatabase();
        }

        [TestMethod]
        public void Person_GetListAll()
        {
            //Arrange
            string caller = "TestMethod Person_GetListAll";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            List<Person> results = null;
            string msg = string.Empty;

            //Act
            try
            {
                // an * indicates a request for all persons
                results = agentFactory.personAgent.GetList("*", caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count == 4);

        }

        [TestMethod]
        public void Person_AddThenVerifyWasAdded()
        {
            //Arrange
            string caller = "TestMethod Person_AddThenVerifyWasAdded";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            PersonViewModel viewModel = new PersonViewModel();
            Person result1 = null;
            Person result2 = null;
            string msg = string.Empty;
            int? id = null;

            //Act
            try
            {
                viewModel.ID = 0;
                viewModel.Name = "New Person";
                viewModel.Address = "New Address";
                viewModel.Age = 99;
                viewModel.Interests = "New Interests";
                viewModel.PictureID = null;

                // returns a copy og the person added
                result1 = agentFactory.personAgent.Add(viewModel, caller, dbCtx);
                id = result1.ID;
            
                // make sure they were added
                result2 = agentFactory.personAgent.Find(id, caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsTrue(result1.Name == viewModel.Name);
            Assert.IsTrue(result1.Address == viewModel.Address);
            Assert.IsTrue(result1.Age == viewModel.Age);
            Assert.IsTrue(result1.Age == 99);
            Assert.IsTrue(result1.Interests == viewModel.Interests);
            Assert.IsTrue(result2.Name == result1.Name);
            Assert.IsTrue(result2.Address == result1.Address);
            Assert.IsTrue(result2.Age == result1.Age);
            Assert.IsTrue(result2.Interests == result1.Interests);
        }

        [TestMethod]
        public void Person_GetForAdd()
        {
            //Arrange
            string caller = "TestMethod Person_GetForAdd";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            PersonViewModel viewModel = null;
            string msg = string.Empty;

            //Act
            try
            {
                viewModel = agentFactory.personAgent.GetForAdd(caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNotNull(viewModel);
            Assert.IsTrue(viewModel.PictureList.Count() > 0);
        }

        [TestMethod]
        public void Person_GetForUpdate()
        {
            //Arrange
            string caller = "TestMethod Person_GetForUpdate";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            Person result = null;
            PersonViewModel viewModel = null;
            string msg = string.Empty;
            int? id = null;

            //Act
            try
            {
                // find a person to process
                result = dbCtx.Persons.ToList().OrderBy(p => p.ID).LastOrDefault();
                id = result.ID;
             
                viewModel = agentFactory.personAgent.GetForUpdate(id, caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(viewModel);
            Assert.IsTrue(viewModel.ID == result.ID);
            Assert.IsTrue(viewModel.Name == result.Name);
            Assert.IsTrue(viewModel.Address == result.Address);
            Assert.IsTrue(viewModel.Age == result.Age);
            Assert.IsTrue(viewModel.Age > 0);
            Assert.IsTrue(viewModel.PictureList.Count() > 0);
        }

        [TestMethod]
        public void Person_FindThrowsErrorNullID()
        {
            //Arrange
            string caller = "TestMethod Person_FindThrowsErrorNullID";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            Person result = null;
            string msg = string.Empty;
            int? id = null;

            //Act
            try
            {
                result = agentFactory.personAgent.Find(id, caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNull(result);
            Assert.IsTrue(msg.ToUpper().Contains("THE ID FOR PERSON TO FIND IS NULL"));
        }

        [TestMethod]
        public void Person_FindThrowsErrorNotFound()
        {
            //Arrange
            string caller = "TestMethod Person_FindThrowsErrorNotFound";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            Person result = null;
            string msg = string.Empty;
            int id = 0;

            //Act
            try
            {
                result = agentFactory.personAgent.Find(id, caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNull(result);
            Assert.IsTrue(msg.ToUpper().Contains("UNABLE TO FIND THE PERSON WITH ID"));
        }

        [TestMethod]
        public void Person_Find()
        {
            //Arrange
            string caller = "TestMethod Person_Find";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            Person result1 = null;
            Person result2 = null;
            string msg = string.Empty;
            int? id = null;

            //Act
            try
            {
                // find a person to verify
                result1 = dbCtx.Persons.ToList().OrderBy(p => p.ID).LastOrDefault();
                id = result1.ID;
             
                result2 = agentFactory.personAgent.Find(id, caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsTrue(result2.ID == result1.ID);
            Assert.IsTrue(result2.Name == result1.Name);
            Assert.IsTrue(result2.Address == result1.Address);
            Assert.IsTrue(result2.Age == result1.Age);
            Assert.IsTrue(result2.Age > 0);
        }

        [TestMethod]
        public void Person_DeleteThenVerifyWasRemoved()
        {
            //Arrange
            string caller = "TestMethod Person_DeleteVerifyWasRemoved";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            Person result1 = null;
            Person result2 = null;
            string msg = string.Empty;
            string resultMsg = string.Empty;
            int? id = null;

            //Act
            try
            {
                // find a person to delete
                result1 = dbCtx.Persons.ToList().OrderBy(p => p.ID).LastOrDefault();
                id = result1.ID;
             
                // delete person
                resultMsg = agentFactory.personAgent.Delete(id, caller, dbCtx);
             
                // verify that the person no longer exists
                result2 = agentFactory.personAgent.Find(id, caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNotNull(result1);
            Assert.IsNull(result2);
            Assert.IsTrue(resultMsg.ToUpper() == "PERSON SUCCESSFULLY DELETED");
            Assert.IsTrue(msg.ToUpper().Contains("UNABLE TO FIND THE PERSON WITH ID="));
        }

        [TestMethod]
        public void Person_SearchArgsAreNull()
        {
            //Arrange
            string caller = "TestMethod Person_SearchArgsAreNull";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            SearchArgs searchArgs = null;
            string result = null;
            string msg = string.Empty;

            //Act
            try
            {
                result = agentFactory.personAgent.SearchPersons(searchArgs, caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ToUpper().Contains("THE JSON SEARCH ARGUMENTS"));
            Assert.IsTrue(result.ToUpper().Contains("ARE NULL"));
        }

        [TestMethod]
        public void Person_SearchArgsAreEmpty()
        {
            //Arrange
            string caller = "TestMethod Person_SearchArgsAreEmpty";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            SearchArgs searchArgs = new SearchArgs();
            string result = null;
            string msg = string.Empty;

            //Act
            try
            {
                result = agentFactory.personAgent.SearchPersons(searchArgs, caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ToUpper().Contains("THE JSON SEARCH ARGUMENTS"));
            Assert.IsTrue(result.ToUpper().Contains("DO NOT HAVE A VALUE REQUIRED"));
        }

        [TestMethod]
        public void Person_SearchReturnsNoResults()
        {
            //Arrange
            string caller = "TestMethod Person_SearchReturnsNoResults";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            string searchArg = string.Empty;
            SearchArgs searchArgs = new SearchArgs();
            string jsonResult = null;
            string msg = string.Empty;

            //Act
            try
            {
                // create a GUID to use for search argument
                searchArg = System.Guid.NewGuid().ToString();

                // search for this person
                searchArgs.Name = searchArg;
                jsonResult = agentFactory.personAgent.SearchPersons(searchArgs, caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNotNull(searchArg);
            Assert.IsNotNull(jsonResult);
            Assert.IsTrue(jsonResult.ToUpper().Contains("NOTFOUND"));
            Assert.IsTrue(jsonResult.ToUpper().Contains("TRUE"));
        }

        [TestMethod]
        public void Person_SearchReturnsAll()
        {
            //Arrange
            string caller = "TestMethod Person_SearchReturnsAll";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            List<Person> results = null;
            SearchArgs searchArgs = new SearchArgs();
            string jsonResult = null;
            string msg = string.Empty;

            //Act
            try
            { 
                // an * indicates a request for all persons in the database
                results = agentFactory.personAgent.GetList("*", caller, dbCtx);

                searchArgs.Name = "*";
                jsonResult = agentFactory.personAgent.SearchPersons(searchArgs, caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNotNull(results);
            Assert.IsNotNull(jsonResult);
            foreach (var entity in results)
            {
                Assert.IsTrue(jsonResult.ToUpper().Contains("<TD>" + entity.Name.ToUpper() + "</TD>"));
            }
        }

        [TestMethod]
        public void Person_SearchReturnsSomeResults()
        {
            //Arrange
            string caller = "TestMethod Person_SearchReturnsSomeResults";
            Debug.WriteLine(caller);
            DalContext dbCtx = dbContext;
            Person result = null;
            List<Person> results = null;
            SearchArgs searchArgs = new SearchArgs();
            string jsonResult = null;
            string msg = string.Empty;

            //Act
            try
            {
                // get the first person in the database - lowest person ID
                result = dbCtx.Persons.ToList().OrderBy(p => p.ID).FirstOrDefault();

                // create a partial search argument from the result name
                Int16 nameLen = (Int16)result.Name.Length;
                Int16 searchArgLen = 0;
                var searchArg = result.Name;
                if (nameLen > 1)
                {
                    searchArgLen = (Int16)(nameLen / 2);
                    searchArg = result.Name.Substring(0, searchArgLen);
                }

                // use the partial search argument to acquire a list of all matching persons in the database
                results = agentFactory.personAgent.GetList(searchArg, caller, dbCtx);

                // get html for the same persons returned above
                searchArgs.Name = searchArg;
                jsonResult = agentFactory.personAgent.SearchPersons(searchArgs, caller, dbCtx);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Debug.WriteLine(msg);
            }

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(results);
            Assert.IsNotNull(jsonResult);
            // verify that the JSON string returned contains the name for every person retrieved with partial search argument
            foreach (var entity in results)
            {
                Assert.IsTrue(jsonResult.ToUpper().Contains("<TD>" + entity.Name.ToUpper() + "</TD>"));
            }
        }


        // -------------------------------
        // local method
        // -------------------------------
        public void ClearAndLoadDatabase()
        {
            using (HealthCatalyst.DalContext dbCtx = new HealthCatalyst.DalContext())
            {
                try
                {
                    // remove all picture entities
                    List<Picture> pictures = dbCtx.Pictures.ToList();
                    Debug.WriteLine("PicturesToDelete:" + pictures.Count);
                    dbCtx.Pictures.RemoveRange(pictures);

                    // remove all person entities
                    List<Person> persons = dbCtx.Persons.ToList();
                    Debug.WriteLine("PersonsToDelete:" + persons.Count);
                    foreach (var entity in persons)
                    {
                        entity.PictureID = null;
                    }
                    dbCtx.Persons.RemoveRange(persons);

                    dbCtx.SaveChanges();

                    // this will add picture and person entities to database for testing
                    //dbCtx.Database.Initialize(true);

                    // add picture entities back to database
                    dbCtx.Pictures.AddOrUpdate(
                        new Picture { ID = 1, FileName = "Bobby-Ruth.jpg" },
                        new Picture { ID = 2, FileName = "Cindy-Moore.jpg" },
                        new Picture { ID = 3, FileName = "Dave-Jones.jpg" },
                        new Picture { ID = 4, FileName = "Susan-Smith.jpg" }
                    );

                    // add picture entities back to database
                    dbCtx.Persons.AddOrUpdate(
                        new Person { ID = 1, Name = "Bobby Ruth", Address = "335 S 2270 E, SLC, UT", Age = 7, Interests = "baseball", PictureID = 1 },
                        new Person { ID = 2, Name = "Cindy Moore", Address = "2256 S 1377 E, SLC UT", Age = 34, Interests = "running, golf", PictureID = 2 },
                        new Person { ID = 3, Name = "Dave Jones", Address = "6993 S 2270 E, Midvale UT", Age = 44, Interests = "golf, tennis, skiing, boating, flying", PictureID = 3 },
                        new Person { ID = 4, Name = "Susan Smith", Address = "9775 S 3535 E, Sandy UT", Age = 31, Interests = "skiing, riding, running", PictureID = 4 }
                    );

                    dbCtx.SaveChanges();
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                    Debug.WriteLine(msg);
                }

            }
        }
    }
}
