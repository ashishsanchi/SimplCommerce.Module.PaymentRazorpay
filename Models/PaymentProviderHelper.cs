
using System;
using System.Security.Cryptography;
using System.Text;

namespace SimplCommerce.Module.PaymentRazorpay.Models
{
    public static class PaymentProviderHelper
    {
        public static readonly string RazorpayProviderId = "Razorpay";

        internal static object GetToken(string message, string data)
        {
            // throw new NotImplementedException();
            byte[] hash;
            // string postData = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
            // dynamic data = JsonConvert.DeserializeObject(postData);
            string d = data; // data.key + "|" + data.txnid + "|" + data.amount + "|" + data.pinfo + "|" + data.fname + "|" + data.email + "|||||" + data.udf5 + "||||||" + data.salt;
            var datab = Encoding.UTF8.GetBytes(d);
            using (SHA512 shaM = new SHA512Managed())
            {
                hash = shaM.ComputeHash(datab);
            }


            //string json = "{\"success\":\"" + GetStringFromHash(hash) + "\"}";
          
           return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2").ToLower());
            }
            return result.ToString();
        }
    }
}
