using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecurityProj2.Models;
using System.Web.Security;
using System.Text;

namespace SecurityProj2.Controllers
{
    public class PasswordKeyController : Controller
    {
        private PasswordKeyDBContext db = new PasswordKeyDBContext();

        //
        // GET: /PasswordKey/

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(db.Keys.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        //
        // GET: /PasswordKey/Details/5

        public ActionResult Details(Guid id)
        {
            PasswordKey passwordkey = db.Keys.Find(id);
            if (passwordkey == null)
            {
                return HttpNotFound();
            }
            return View(passwordkey);
        }

        //
        // GET: /PasswordKey/Create

        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        //
        // POST: /PasswordKey/Create

        [HttpPost]
        public ActionResult Create(PasswordKey passwordkey)
        {
            if (ModelState.IsValid)
            {
                string temp = User.Identity.Name;
                passwordkey.PasswordId = Guid.NewGuid();
                passwordkey.UserName = temp;
                passwordkey.Password = generatePassword(10, true);
                db.Keys.Add(passwordkey);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(passwordkey);
        }

        public string generatePassword(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        //
        // GET: /PasswordKey/Edit/5

        public ActionResult Edit(Guid id)
        {
            PasswordKey passwordkey = db.Keys.Find(id);
            if (passwordkey == null)
            {
                return HttpNotFound();
            }
            return View(passwordkey);
        }

        //
        // POST: /PasswordKey/Edit/5

        [HttpPost]
        public ActionResult Edit(PasswordKey passwordkey)
        {
            if (ModelState.IsValid)
            {
                db.Entry(passwordkey).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(passwordkey);
        }

        //
        // GET: /PasswordKey/Delete/5

        public ActionResult Delete(Guid id)
        {
            PasswordKey passwordkey = db.Keys.Find(id);
            if (passwordkey == null)
            {
                return HttpNotFound();
            }
            return View(passwordkey);
        }

        //
        // POST: /PasswordKey/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            PasswordKey passwordkey = db.Keys.Find(id);
            db.Keys.Remove(passwordkey);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}