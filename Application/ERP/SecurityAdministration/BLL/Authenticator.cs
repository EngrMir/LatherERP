using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SecurityAdministration.BLL
{
    public class Authenticator
    {
        public static string GetHashPassword(string password)
        {
            var salt = GetRandomSalt();
            var saltWithPassword = salt + password;
            var hash = Sha256Hex(saltWithPassword);
            var saltHash = salt + hash;
            return saltHash;
        }

        public static bool ValidatePassword(string password, string correctHash)
        {
            if (correctHash.Length < 128)
            {
                throw new ArgumentException("Correct hash must be 128 hex characters!");
            }
            string salt = correctHash.Substring(0, 64);
            string passHash = Sha256Hex(salt + password);
            string newHash = salt + passHash;
            return String.CompareOrdinal(correctHash, newHash) == 0;
        }

        public static bool CheckPasswordFormat(string password)
        {
            const string validRegularExpression = "(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*+?]).*$";

            return Regex.IsMatch(password, validRegularExpression);
        }

        #region private methods

        //Returns a random 64 character hex string (256 bits)
        private static string GetRandomSalt()
        {
            var random = new RNGCryptoServiceProvider();
            var salt = new byte[32]; //256 bits
            random.GetBytes(salt);
            var bytesToHex = BytesToHex(salt);
            return bytesToHex;
        }

        //Convert a byte array to a hex string
        private static string BytesToHex(byte[] toConvert)
        {
            var s = new StringBuilder(toConvert.Length * 2);
            foreach (var b in toConvert)
            {
                s.Append(b.ToString("x2"));
            }
            return s.ToString();
        }

        //returns the SHA256 hash of a string, formatted in hex
        private static string Sha256Hex(string toHash)
        {
            var hash = new SHA256Managed();
            var utf8 = UTF8Encoding.UTF8.GetBytes(toHash);
            var bytesToHex = BytesToHex(hash.ComputeHash(utf8));
            return bytesToHex;
        }

        #endregion
    }
}