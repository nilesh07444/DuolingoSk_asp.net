using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class DashboardCountVM
    {
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalDistributors { get; set; }
        public int TotalProductItems { get; set; }
        public int TotalConfirmOrder {get;set;}
        public int TotalNewOrder { get; set; } 
        public int TotalDispatchedOrder { get; set; }        

    }
}