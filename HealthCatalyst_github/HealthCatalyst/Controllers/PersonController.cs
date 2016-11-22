
using HealthCatalyst.Agents;
using HealthCatalyst.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace HealthCatalyst.Controllers
{
    public class PersonController : Controller
    {
        private DalContext dbCtx = new DalContext();
        private AgentFactory agentFactory = new AgentFactory();

        // GET: Person/Create
        public ActionResult Create()
        {
            string caller = "HttpGet Create";

            PersonViewModel viewModel = null;
            try
            {
                viewModel = agentFactory.personAgent.GetForAdd(caller, dbCtx);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("FatalError");
            }

            return View(viewModel);
        }


        // POST: Person/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PersonViewModel viewModel)
        {
            string caller = "HttpPost Create";
            if (ModelState.IsValid)
            {
                Person person = null;
                try
                {
                    person = agentFactory.personAgent.Add(viewModel, caller, dbCtx);
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    return RedirectToAction("FatalError");
                }
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }


        // GET: Person/Delete/id
        public ActionResult Delete(int? id)
        {
            string caller = "HttpGet Delete";

            Person person = null;
            try
            {
                person = agentFactory.personAgent.Find(id, caller, dbCtx);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("FatalError");
            }

            return View(person);
        }


        // POST: Person/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string caller = "HttpPost Delete";

            try
            {
                string resultMsg = agentFactory.personAgent.Delete(id, caller, dbCtx);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("FatalError");
            }

            return RedirectToAction("Index");
        }


        // GET: Person/Details/id
        public ActionResult Details(int? id)
        {
            string caller = "HttpGet Details";

            Person person = null;
            try
            {
                person = agentFactory.personAgent.Find(id, caller, dbCtx);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("FatalError");
            }
            return View(person);
        }


        // GET: Person/Edit/id
        public ActionResult Edit(int? id)
        {
            string caller = "HttpGet Edit";
            PersonViewModel viewModel = null;
            try
            {
                viewModel = agentFactory.personAgent.GetForUpdate(id, caller, dbCtx);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("FatalError");
            }

            return View(viewModel);
        }


        // POST: Person/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonViewModel viewModel)
        {
            string caller = "HttpPost Edit";
            if (ModelState.IsValid)
            {
                Person person = null;
                try
                {
                    person = agentFactory.personAgent.Update(viewModel, caller, dbCtx);
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    return RedirectToAction("FatalError");
                }

                return RedirectToAction("Index");
            }
            return View(viewModel);
        }


        // Display View for Fatal Error
        public ActionResult FatalError()
        {
            return View();
        }


        // GET: Person List
        public ActionResult Index()
        {
            string caller = "HttpGet Index";
            List<Person> persons = null;
            try
            {
                persons = agentFactory.personAgent.GetList("*", caller, dbCtx);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("FatalError");
            }

            return View(persons);
        }


        [HttpGet]
        // GET: Person Search View
        public ActionResult Search()
        {
            return View();
        }


        [HttpPost]
        public string SearchPersons(SearchArgs searchArgs)
        // replace this to trigger searchArgs error - public JsonResult SearchPersons(string searchArgs)
        {
            string caller = "HttpPost SearchPersons";
            string jsonResult = string.Empty;

            try
            {
                jsonResult = agentFactory.personAgent.SearchPersons(searchArgs, caller, dbCtx);
            }
            catch (Exception ex)
            {
                // All exceptions should be caught in called method and returned in jsonResult above - but!!
                jsonResult = JsonConvert.SerializeObject(new { Error = ex.Message });
                return jsonResult;
            }

            return jsonResult;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbCtx.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
