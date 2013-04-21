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



//EmailController.cs manages all email reset functionality like Email reset and Forgot password
namespace SecurityProj2.Controllers
{
    public class EmailController : Controller
    {
        //
        // GET: /Email/
        //Returns Reset View
        public ActionResult Reset()
        {
            EmailModel email = new EmailModel();
            email.Email = User.Identity.Name;
            return View();
        }

        //Returns Email/EmailSent View that displays password forget confirmation
        public ActionResult EmailSent()
        {
            return View();
        }


        //Returns Email/ResetPasswordCondirm View and temporary password is issued if user clicked link in Forgot password email.
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

        //Returns Email/EmailSuccess View. Email successfully sent reset token to user account
        public ActionResult EmailSuccess()
        {
            return View();
        }


        //Returns Email/EmailFail View. Email failed to send reset token to user account
        public ActionResult EmailFail()
        {
            return View();
        }


        //Tries to send email to parameter "email" if the username exists. Inside the email, there is a password reset token
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
                var resetLink = "http://passwordvault.apphb.com/Email/ResetPasswordConfirm?username=" + email + "&rt=" + token;
                string emailBody = "Please click the link to reset your password  "
                    + resetLink; //edit it
                string subject = "Password Reset Token";
                message.Body = emailBody;
                message.Subject = subject;
                message.To.Add(new MailAddress(email));
                message.From = new MailAddress("passVaultSystem@gmail.com");

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
