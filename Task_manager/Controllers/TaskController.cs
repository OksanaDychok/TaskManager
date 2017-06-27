using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_manager.Hubs;
using Task_manager.Models;

namespace Task_manager.Controllers
{
    public class TaskController : Controller
    {
        // GET: Task
        UsersContext context = new UsersContext();

        [HttpGet]
        public ActionResult Create()
        {
            TaskManager t = new TaskManager();
            return View(t);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Create(TaskManager t)
        {
            if(ModelState.IsValid)
            {
                t.UserProfiles = context.UserProfiles.Where(s => s.UserName == User.Identity.Name).Select(s => s).SingleOrDefault();
                t.WhoSharedTask = User.Identity.Name;
                context.TaskManagers.Add(t);
                context.SaveChanges();
                TasksHub.Show();
                return RedirectToAction("ViewTasks");
            }
            return View("Create");

        }
        [HttpGet]
        public ActionResult ViewTasks()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetViewTasks()
        {
            var TaskList = context.TaskManagers.Where(s=>s.UserProfiles.UserName==User.Identity.Name).Select(s=>s);
            return PartialView("GetViewTasks", TaskList);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            TaskManager t = context.TaskManagers.Where(el => el.ID == id).FirstOrDefault();
            if (t == null)
                return HttpNotFound();

            return View(t);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditConfirmed(TaskManager t)
        {
            t.WhoSharedTask = User.Identity.Name;
            context.Entry(t).State = EntityState.Modified;
            context.SaveChanges();
            TasksHub.Show();
            return RedirectToAction("ViewTasks");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            TaskManager t = context.TaskManagers.Where(el => el.ID == id).FirstOrDefault();
            return View(t);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            TaskManager t = context.TaskManagers.Where(el => el.ID == id).FirstOrDefault();
            context.TaskManagers.Remove(t);
            context.SaveChanges();
            TasksHub.Show();
            return RedirectToAction("ViewTasks");
        }

        [HttpGet]
        public ActionResult Share(int id)
        {
            List<UserProfile> allUsers = new List<UserProfile>(context.UserProfiles.Where(n => n.UserName.ToString() != User.Identity.Name).Select(n => n).ToList());
            ViewBag.allUsers = new SelectList(allUsers, "UserId", "Email");
            TaskManager t = context.TaskManagers.Where(el => el.ID == id).FirstOrDefault();
            return View(t);
        }

        [HttpPost, ActionName("Share")]
        public ActionResult SharedConfirmed(TaskManager t, int userId)
        {
            t.UserProfiles = context.UserProfiles.Where(u=>u.UserId==userId).Select(u=>u).First();
            t.WhoSharedTask = User.Identity.Name;
            context.TaskManagers.Add(t);
            context.SaveChanges();
            TasksHub.Show();
            return RedirectToAction("ViewTasks");
        }
    }
}