using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class GeneralVM
    {
        public string ClientUserId { get; set; }
        public string ItemId { get; set; }
        public string WishListId { get; set; }
        public string RoleId { get; set; }
        public string CategoryId { get; set; }
        public string SortBy { get; set; }
        public string ProductId { get; set; }
        public string SubProductId { get; set; }
        public string SessionUniqueId { get; set; }
        public string StrCartItems { get; set; }
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string RazorPaymentId { get; set; }
        public string RazorOrderid { get; set; }
    }
}