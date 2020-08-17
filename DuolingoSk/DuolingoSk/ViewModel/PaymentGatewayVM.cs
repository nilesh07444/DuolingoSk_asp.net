using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class PaymentGatewayVM
    {
        public string PaymentAPIKey { get; set; }
        public string PaymentSecretKey { get; set; }
        public string PaymentLayerUrl { get; set; }
        public string PaymentPaymentTokenUrl { get; set; }
    }
}