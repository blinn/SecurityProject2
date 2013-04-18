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
using System.Security.Cryptography;
using System.IO;

namespace SecurityProj2.Controllers
{
    public class PasswordKeyController : Controller
    {
        private string aesKeyString = "ng©sâ~ëþ—l+×’.‚èºJB#\n\"OøÎ&\v¯¼\t#¯";
        private string aesRijIV = "’µHß“„ØÃ¼½&ä";
        private PasswordKeyDBContext db = new PasswordKeyDBContext();

        //
        // GET: /PasswordKey/

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                List<PasswordKey> list = new List<PasswordKey>();
                foreach (var item in db.Keys){
                    if (item.UserName == User.Identity.Name)
                    {
                        string decryptedPassword = AES.Main(item.Password, aesKeyString, aesRijIV, false);
                        string decryptedUsername = AES.Main(item.HandleName, aesKeyString, aesRijIV, false);
                        item.Password = decryptedPassword;
                        item.HandleName = decryptedUsername;
                        list.Add(item);
                    }
                }
                return View(list);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
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
        public ActionResult Create(PasswordKey passwordData)
        {
            if (ModelState.IsValid)
            {
                string name = User.Identity.Name;
                string password = generatePassword(passwordData.passwordLength, passwordData.includeUpper, passwordData.includeNumbers, passwordData.includeSpecial);

                string encryptedPassword = AES.Main(password, aesKeyString, aesRijIV, true);
                string encryptedHandlename = AES.Main(passwordData.HandleName, aesKeyString, aesRijIV, true);

                passwordData.UserName = name;
                passwordData.PasswordId = Guid.NewGuid();
                passwordData.Password = encryptedPassword;
                passwordData.HandleName = encryptedHandlename;
                passwordData.passwordLength = 100;

                db.Keys.Add(passwordData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(passwordData);
        }

        public string generatePassword(int size, bool includeUpper, bool includeNumbers, bool includeSpecial)
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder();
            string completeArrString = "abcdefghijklmnopqrstuvwxyz";;
            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numbers = "123456789";
            string special = "@%+!#$?-*";
            if(includeUpper){
                completeArrString = completeArrString + upper;
            }
            if(includeNumbers){
                completeArrString = completeArrString + numbers;
            }
            if(includeSpecial){
                completeArrString = completeArrString + special;
            }
            char[] array = completeArrString.ToCharArray();
            for (int i = 0; i < size; i++)
            {
                int num = random.Next(0, array.Length);
                builder.Append(array[num]);
            }
            return builder.ToString();
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
            if (passwordkey.UserName == User.Identity.Name)
            {
                string decryptedPassword = AES.Main(passwordkey.Password, aesKeyString, aesRijIV, false);
                string decryptedUsername = AES.Main(passwordkey.HandleName, aesKeyString, aesRijIV, false);
                passwordkey.Password = decryptedPassword;
                passwordkey.HandleName = decryptedUsername;
                return View(passwordkey);
            }
            else
            {
                return RedirectToAction("Index");
            }
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

    public class AES
    {
        public static string Main(string password, string key, string rijIV, bool encrypt)
        { 
            byte[] keyByte = Encoding.Default.GetBytes(key);
            byte[] rijIVByte = Encoding.Default.GetBytes(rijIV);
            if (encrypt)
            {
                try
                {
                    // Create a new instance of the Rijndael 
                    // class.  This generates a new key and initialization  
                    // vector (IV). 
                    using (Rijndael myRijndael = Rijndael.Create())
                    {
                        // Encrypt the string to an array of bytes. 
                        byte[] encrypted = EncryptStringToBytes(password, keyByte, rijIVByte);
                        string RijIV = Encoding.Default.GetString(myRijndael.IV);

                        // Decrypt the bytes to a string.
                        string encryptedString = Encoding.Default.GetString(encrypted);
                        return encryptedString;
                    }

                }
                catch (Exception e)
                {

                    Console.WriteLine("Error: {0}", e.Message);
                    return e.Message;
                }
            }
            else
            {
                try
                {
                    // Create a new instance of the Rijndael 
                    // class.  This generates a new key and initialization  
                    // vector (IV). 
                    using (Rijndael myRijndael = Rijndael.Create())
                    {
                        // Decrypt the bytes to a string.
                        byte[] passwordBytes = Encoding.Default.GetBytes(password);
                        string originalPass = DecryptStringFromBytes(passwordBytes, keyByte, rijIVByte);
                        return originalPass;
                    }

                }
                catch (Exception e)
                {

                    Console.WriteLine("Error: {0}", e.Message);
                    return e.Message;
                }
            }
            
        }
        public static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an Rijndael object 
            // with the specified key and IV. 
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted;

        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an Rijndael object 
            // with the specified key and IV. 
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}