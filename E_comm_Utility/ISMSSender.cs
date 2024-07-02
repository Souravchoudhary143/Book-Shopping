using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_comm_Utility
{
    public interface ISMSSender
    {
        Task SendMessageAsync(string toPhone, string message);
        public void SendSms (string toPhone, string message);
    }
}
