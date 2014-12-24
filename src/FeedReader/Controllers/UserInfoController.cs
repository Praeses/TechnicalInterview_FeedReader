using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FeedReader.Models;
using Microsoft.AspNet.Identity;

namespace FeedReader.Controllers
{
    public class UserInfoController : Controller
    {
        private UserContext db = new UserContext();

        // GET: UserInfo
        public ActionResult Index()
        {
            var users = db.AspNetUserInfo.ToList();
            return View(users);
        }

        public ActionResult ManagerView()
        {
            return View(db.AspNetUserInfo.ToList());
        }

        // GET: UserInfo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUserInfo aspNetUserInfo = db.AspNetUserInfo.Find(id);
            if (aspNetUserInfo == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUserInfo);
        }

        // GET: UserInfo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Created,FirstName,LastName")] AspNetUserInfo aspNetUserInfo)
        {
            if (ModelState.IsValid)
            {
                aspNetUserInfo.UserId = User.Identity.GetUserId();
                db.AspNetUserInfo.Add(aspNetUserInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetUserInfo);
        }

        // GET: UserInfo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUserInfo aspNetUserInfo = db.AspNetUserInfo.Find(id);
            if (aspNetUserInfo == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUserInfo);
        }

        // POST: UserInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Created,FirstName,LastName")] AspNetUserInfo aspNetUserInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUserInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUserInfo);
        }

        // GET: UserInfo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUserInfo aspNetUserInfo = db.AspNetUserInfo.Find(id);
            if (aspNetUserInfo == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUserInfo);
        }

        // POST: UserInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetUserInfo aspNetUserInfo = db.AspNetUserInfo.Find(id);
            db.AspNetUserInfo.Remove(aspNetUserInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
