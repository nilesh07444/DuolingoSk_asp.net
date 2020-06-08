using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class ClientUserVM
    {
        public long ClientUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long RoleId { get; set; }
        public string CompanyName { get; set; }
        public string ProfilePic { get; set; }
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string AddharCardNo { get; set; }        
        public string PanCardNo { get; set; }
        public string GSTNo { get; set; }             
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipPostalCode { get; set; } 
        public string Prefix { get; set; }
        public string AlternateMobileNo { get; set; }
        public string ShopPhoto { get; set; }
        public string AddharPhoto { get; set; }
        public string PancardPhoto { get; set; }
        public string GSTPhoto { get; set; }
        public string ProfilePhoto { get; set; }
        public string ShopName { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime BirthDate { get; set; }
        public string SessionUniqueId { get; set; }
        public int CartCount { get; set; }
        public decimal TotalAmountOfOrderPlaced { get; set; }
        public decimal PointRemaining { get; set; }
    }
}