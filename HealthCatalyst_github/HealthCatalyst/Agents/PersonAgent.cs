
using HealthCatalyst.Models;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace HealthCatalyst.Agents
{
    public class PersonAgent
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public PersonAgent()
        {
        }


        /// <summary>
        /// create a new person entity
        /// </summary>
        /// <param name="viewModel, caller, dbCtx"></param>
        /// <returns>new person entity with next identity ID</returns>
        public Person Add(PersonViewModel viewModel, string caller, DalContext dbCtx)
        {
            if (viewModel == null)
            {
                throw new Exception(string.Format("The viewModel for the person entity from the '{0}' Controller Action to be updated is null", caller));
            }

            Person person = new Person();

            // update entity with data from viewModel
            person.ID = viewModel.ID;
            person.Name = viewModel.Name;
            person.Address = viewModel.Address;
            person.Age = viewModel.Age.HasValue ? viewModel.Age.Value : 0;
            person.Interests = viewModel.Interests;
            person.PictureID = null;
            if (viewModel.PictureID.HasValue)
            {
                if (viewModel.PictureID > 0)
                {
                    person.PictureID = viewModel.PictureID;
                }
            }

            // if not adding person entiity with a picture - set picture entity to NULL
            //if (!(viewModel.PictureID.HasValue && viewModel.PictureID.Value > 0))
            //{
            //    person.Picture = null;
            //}

            try
            {
                // add new person to db context
                dbCtx.Persons.Add(person);

                // save to the database
                dbCtx.SaveChanges();
            }
            catch (Exception ex)
            {
                var errMsg = GetDbExceptionMsg(ex, caller);
                throw new Exception(errMsg);
            }
            return person;
        }


    /// <summary>
    /// remove a specific Person Entity
    /// </summary>
    /// <param name="id, caller, dbCtx"></param>
    /// <returns>person entity</returns>
    public string Delete(int? id, string caller, DalContext dbCtx)
        {
            Person person = null;

            try
            {
                // find person to delete
                person = Find(id, caller, dbCtx);

                // make sure not to delete the Picture entity, just the person
                person.PictureID = null;
                dbCtx.Persons.Remove(person);
                dbCtx.SaveChanges();
            }
            catch (Exception ex)
            {
                var errMsg = GetDbExceptionMsg(ex, caller);
                throw new Exception(errMsg);
            }
            return "Person Successfully Deleted";
        }


        /// <summary>
        /// find a specific Person Entity
        /// </summary>
        /// <param name="id, caller, dbCtx"></param>
        /// <returns>person entity</returns>
        public Person Find(int? id, string caller, DalContext dbCtx)
        {
            if (id == null)
            {
                throw new Exception(string.Format("The ID for person to find is null from '{0}' Controller Action", caller));
            }

            Person person = null;
            try
            {
                person = dbCtx.Persons.Include(p => p.Picture).SingleOrDefault(p => p.ID == id);
            }
            catch (Exception ex)
            {
                var errMsg = GetDbExceptionMsg(ex, caller);
                throw new Exception(errMsg);
            }

            // check if the requested entity was found - this should never be the case
            if (person == null)
            {
                throw new Exception(string.Format("Unable to find the person with id={0} in the '{1}' Controller Action", id, caller));
            }

            return person;
        }


        /// <summary>
        /// load view model to create a new person entity
        /// </summary>
        /// <param name="person"></param>
        /// <returns>new person view model with select list for Picture</returns>
        public PersonViewModel GetForAdd(string caller, DalContext dbCtx)
        {
            PersonViewModel viewModel = new PersonViewModel();

            try
            {
                // create a dropdown list of all pictures available for selection
                viewModel.PictureList = new SelectList(dbCtx.Pictures.AsNoTracking().OrderBy(p => p.FileName), "ID", "FileName");
            }
            catch (Exception ex)
            {
                var errMsg = GetDbExceptionMsg(ex, caller);
                throw new Exception(errMsg);
            }

            return viewModel;
        }


        /// <summary>
        /// get exising person entity for editing
        /// </summary>
        /// <param name="id, caller, dbCtx"></param>
        /// <returns>view model with person data</returns>
        public PersonViewModel GetForUpdate(int? id, string caller, DalContext dbCtx)
        {
            Person person = null;
            PersonViewModel viewModel = new PersonViewModel();

            try
            {
                // if there is an error - will be thrown back to caller and handled there
                person = Find(id, caller, dbCtx);

                viewModel.ID = person.ID;
                viewModel.Name = person.Name;
                viewModel.Address = person.Address;
                viewModel.Age = person.Age;
                viewModel.Interests = person.Interests;
                viewModel.PictureID = person.PictureID;

                // create a dropdown list of all pictures available for selection and set selected to the one found for person
                viewModel.PictureList = new SelectList(dbCtx.Pictures.AsNoTracking().OrderBy(p => p.FileName), "ID", "FileName", person.PictureID);
            }
            catch (Exception ex)
            {
                var errMsg = GetDbExceptionMsg(ex, caller);
                throw new Exception(errMsg);
            }

            return viewModel;
        }


        /// <summary>
        /// Return all Person Entities requested
        /// </summary>
        /// <param name="queryArg, caller, dbCtx"></param>
        /// <returns>list of person entities</returns>
        public List<Person> GetList(string queryArg, string caller, DalContext dbCtx)
        {
            List<Person> persons = null;

            try
            {
                // check if this is a request for all persons
                if (queryArg.Equals("*"))
                {
                    persons = dbCtx.Persons.Include(p => p.Picture).AsNoTracking().OrderBy(p => p.Name).ToList();
                }
                else
                {
                    persons = dbCtx.Persons.Include(p => p.Picture).AsNoTracking().OrderBy(p => p.Name).Where(p => p.Name.Contains(queryArg)).ToList();
                }
            }
            catch (Exception ex)
            {
                var errMsg = GetDbExceptionMsg(ex, caller);
                throw new Exception(errMsg);
            }

            return persons;
         }


        /// <summary>
        /// search for person entities
        /// </summary>
        /// <param name="queryArg, caller, dbCtx"></param>
        /// <returns>list of person entities</returns>
        public string SearchPersons(SearchArgs searchArgs, string caller, DalContext dbCtx)
        {
            string jsonResult = string.Empty;

            // verify that JSON search arguments passed from caller are present
            if (searchArgs == null)
            {
                var errMsg = "The JSON search arguments passed to host method SearchPersons are null";
                Console.WriteLine(errMsg);
                jsonResult = JsonConvert.SerializeObject(new { Error = errMsg });
                return jsonResult;
            }

            // must have search argument value to continue
            if (searchArgs.Name == null)
            {
                searchArgs.Name = string.Empty;
            }
            string searchName = searchArgs.Name.Trim();
            if (String.IsNullOrEmpty(searchName))
            {
                var errMsg = "The JSON search arguments passed to host method SearchPersons do not have a value required to search the database";
                Console.WriteLine(errMsg);
                jsonResult = JsonConvert.SerializeObject(new { Error = errMsg });
                return jsonResult;
            }

            List<Person> persons = null;

            // find all matches to the Search argument
            try
            {
                // check if requesting all persons
                if (searchName.Equals("*"))
                {
                    persons = dbCtx.Persons.Include(p => p.Picture).AsNoTracking().OrderBy(p => p.Name).ToList();
                }
                else
                {
                    persons = dbCtx.Persons.Include(p => p.Picture).AsNoTracking().OrderBy(p => p.Name).Where(p => p.Name.Contains(searchName)).ToList();
                }
            }
            catch (Exception ex)
            {
                var errMsg = GetDbExceptionMsg(ex, caller);
                jsonResult = JsonConvert.SerializeObject(new { Error = errMsg });
                return jsonResult;
            }

            // if nothing was found - return notification
            if (!persons.Any())
            {
                jsonResult = JsonConvert.SerializeObject(new { NotFound = "true" });
                return jsonResult;
            }

            // generate HTML for headers 
            StringBuilder htmlData = new StringBuilder();
            htmlData.Append("<table class='table'>");
            htmlData.Append(" <tr><th></th>");
            htmlData.Append("     <th>Name</th>");
            htmlData.Append("     <th>Address</th>");
            htmlData.Append("     <th>Age</th>");
            htmlData.Append("     <th>Interests</th>");
            htmlData.Append(" </tr>");

            // generate HTML for all results found
            string pictureUrl = Constants.PersonImagePath.Replace("~", string.Empty);
            var toggle = "evenRow";
            foreach (var person in persons)
            {
                if (toggle.Equals("oddRow"))
                {
                    toggle = "evenRow";
                }
                else
                {
                    toggle = "oddRow";
                }
                htmlData.Append(" <tr class='" + toggle + "'>");
                htmlData.Append("     <td><img src = " + pictureUrl + person.Picture.FileName + "></td>");
                htmlData.Append("     <td>" + person.Name + "</td>");
                htmlData.Append("     <td>" + person.Address + "</td>");
                htmlData.Append("     <td>" + person.Age + "</td>");
                htmlData.Append("     <td>" + person.Interests + "</td>");
                htmlData.Append(" </tr>");
            }

            htmlData.Append("</table>");
            var jsonObj = new
            {
                Html = htmlData.ToString()
            };

            jsonResult = JsonConvert.SerializeObject(jsonObj);
            return jsonResult;
        }


        /// <summary>
        /// update a modified person entity
        /// </summary>
        /// <param name="viewModel, caller, dbCtx"></param>
        /// <returns>updated person entity</returns>
        public Person Update(PersonViewModel viewModel, string caller, DalContext dbCtx)
        {
            if (viewModel == null)
            {
                throw new Exception(string.Format("The viewModel for the person entity from the '{0}' Controller Action to be updated is null", caller));
            }

            Person personToUpdate = null;
            try 
            {
                // get a fresh copy of this person
                personToUpdate = dbCtx.Persons.SingleOrDefault(p => p.ID == viewModel.ID);
            }
            catch (Exception ex)
            {
                var errMsg = GetDbExceptionMsg(ex, caller);
                throw new Exception(errMsg);
            }

            if (personToUpdate == null)
            {
                throw new Exception(string.Format("Unable to find the person for update with id={0} in the '{1}' Controller Action", viewModel.ID, caller));
            }

            // update entity with data from viewModel
            personToUpdate.ID = viewModel.ID;
            personToUpdate.Name = viewModel.Name;
            personToUpdate.Address = viewModel.Address;
            personToUpdate.Age = viewModel.Age.HasValue ? viewModel.Age.Value : 0;
            personToUpdate.Interests = viewModel.Interests;
            personToUpdate.PictureID = viewModel.PictureID;

            try
            {
                // save these changes to the database
                dbCtx.SaveChanges();
            }
            catch (ArgumentNullException ex)
            {
                var errMsg = GetDbExceptionMsg(ex, caller);
                throw new Exception(errMsg);
            }
            return personToUpdate;
        }


        // ---------------------------------------------
        // Local Methods
        // ---------------------------------------------

        /// <summary>
        /// get exception info
        /// </summary>
        /// <param name="exception, caller"></param>
        /// <returns>error message</returns>
        private string GetDbExceptionMsg(Exception exception, string caller)
        {
            // will log the error to web log file (using Debug.console for this temporarily)

            var errorMsg = string.Empty;
            var exType = exception.GetType().ToString();

            if (exType.Contains("SqlException") || exType.Contains("DbUpdateException"))
            {
                errorMsg = "A database error was encountered during processing in " + caller + " Controller Action";
                SqlException innerException = exception.InnerException.InnerException as SqlException;
              
                if (innerException != null)
                {
                    // check for duplicate insert error 
                    if (innerException.Number == 2601)
                    {
                        // check for duplicate insert error 
                        errorMsg = errorMsg + 
                        ". This person already exists in the database - please delete it and try again if you wish to add it";
                    }
                    else
                    {
                        Debug.WriteLine(errorMsg);
                        Debug.WriteLine("Exception Number: " + innerException.Number.ToString() + ", MESSAGE: "
                             + innerException.Message + ", ERRORS: " + innerException.Errors.ToString());
                    }
                }
                else
                {
                    innerException = exception.InnerException as SqlException;
                    if (innerException != null)
                    {
                        Debug.WriteLine(errorMsg);
                        Debug.WriteLine("Exception Number: " + innerException.Number.ToString() + ", MESSAGE: "
                             + innerException.Message + ", ERRORS: " + innerException.Errors.ToString());
                    }
                    else
                    {
                        Debug.WriteLine(errorMsg);
                        Debug.WriteLine("MESSAGE: " + exception.Message);
                    }
                }
            }
            else
            {
                errorMsg = "An exception was encountered during processing in " + caller + " Controller Action. " + exception.Message;
                Debug.WriteLine(errorMsg);
            }

            return errorMsg;
        }

    }
}
