using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_comm_Utility
{
    public static class SD
    {
        //store procedure
        public const string SP_CreateCoverType = "Create_CoverType";
        public const string SP_UpdateCoverType = "Update_CoverType";
        public const string SP_DeleteCoverType = "Delete_CoverType";
        public const string SP_GetCoverTypes = "Get_CoverTypes";
        public const string SP_GetCoverType = "Get_CoverType";
    //Roles
    public const string Role_Admin = "Admin";
    public const string Role_Employee = "Employee User";
    public const string Role_Company = "Company User";
    public const string Role_Individual = "Individual User";
        //Session
        public const string SS_SessionCartCount = "Session Cart Count";

        public static double GetPriceBasedOnQuantity(double quality,
            double price,double price50,double price100)
        {
            if (quality < 50)
                return price;
            else if (quality < 100)
                return price50;
            else return price100;
        }

        //Order State
        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusInProgress = "Progress";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusCancelled = "Cancelled";
        public const string OrderStatusRefunded = "Refunded";

        //Payment Status
        public const string PaymentStatusPending = "Pending";
        public const string PymentstatusApproved = "Approved";
        public const string PaymentstatusDelayPayment = "PaymentStatusDelay";
        public const string PymentStatusRejected = "Rejected";


    }
}
