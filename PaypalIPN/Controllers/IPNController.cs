using PaypalIPN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PaypalIPN.Controllers
{
    public class IPNController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public EmptyResult PayPalPaymentNotification(PayPalCheckoutInfo payPalCheckoutInfo)
        {
            PayPalListenerModel model = new PayPalListenerModel();
            model._PayPalCheckoutInfo = payPalCheckoutInfo;
            byte[] parameters = Request.BinaryRead(Request.ContentLength);

            if (parameters != null)
            {
                model.GetStatus(parameters);
            }
            return new EmptyResult();
        }


        [HttpPost]
        public async Task<ActionResult> ItemSold()
        {
            var ipn = Request.Form.AllKeys.ToDictionary(k => k, k => Request[k]);
            ipn.Add("cmd", "_notify-validate");

            var isIpnValid = await ValidateIpnAsync(ipn);
            if (isIpnValid)
            {
                for (int i = 0; i < ipn.Keys.Count; i++)
                {
                    Console.WriteLine(ipn.Keys.Take(i).First());

                }
                // process the IPN
            }

            return new EmptyResult();
        }

        private static async Task<bool> ValidateIpnAsync(IEnumerable<KeyValuePair<string, string>> ipn)
        {
            using (var client = new HttpClient())
            {
                const string PayPalUrl = "https://www.paypal.com/cgi-bin/webscr";

                // This is necessary in order for PayPal to not resend the IPN.
                await client.PostAsync(PayPalUrl, new StringContent(string.Empty));

                var response = await client.PostAsync(PayPalUrl, new FormUrlEncodedContent(ipn));

                var responseString = await response.Content.ReadAsStringAsync();
                return (responseString == "VERIFIED");
            }
        }
    }
}