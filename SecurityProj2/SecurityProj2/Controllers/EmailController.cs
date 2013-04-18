using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Web.Security;
using SecurityProj2.Models;
using WebMatrix.WebData;
using SecurityProj2.Controllers;

namespace SecurityProj2.Controllers
{
    public class EmailController : Controller
    {
        //
        // GET: /Email/

        public ActionResult Reset()
        {
            EmailModel email = new EmailModel();
            email.Email = User.Identity.Name;
            return View();
        }

        public ActionResult EmailSent()
        {
            return View();
        }


        public ActionResult ResetPasswordConfirm(string username, string rt)
        {
            Random random = new Random();
            int r1 = random.Next(0, 50);
            int r2 = random.Next(0, 50);
            int r3 = random.Next(0, 50);
            //generate random password
            string newpassword = "temp"+r1+r2+r3;
            //reset password
            bool response = WebSecurity.ResetPassword(rt, newpassword);
            if (response == true)
            {
                WebSecurity.Login(username, newpassword);     
            }
            PasswordRecovery pr = new PasswordRecovery();
            pr.TempPassword = newpassword;
            return View(pr);
        }

        public ActionResult EmailSuccess()
        {
            return View();
        }

        public ActionResult EmailFail()
        {
            return View();
        }

        public string Send(String email)
        {
            MailMessage message = new MailMessage();
            MembershipUser mu = Membership.GetUser(email);

            if (mu == null)
            {
                return "error";
            }
            else
            {

                var token = WebSecurity.GeneratePasswordResetToken(mu.UserName, 10);
                var resetLink = "href='" + Url.Action("ResetPasswordConfirm", "Email", new { username = email, rt = token }, "http") + "'>Reset Password";
                string emailBody = "Please click the link to reset your password  "
                    + resetLink; //edit it
                string subject = "Password Reset Token";
                message.Body = emailBody;
                message.Subject = subject;
                message.To.Add(new MailAddress(email));
                message.From = new MailAddress("depauwbookexchange@gmail.com");

                try
                {
                    //SendEMail(email, subject, emailBody);
                    SmtpClient client = new SmtpClient();
                    client.EnableSsl = true;
                    client.Port = 587;
                    client.Host = "smtp.gmail.com";
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    return "error";
                }
                return "success";

            }
        }


        
    }
}
