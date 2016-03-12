using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ims
{
    public class imsmetadata
    {
        public struct ClientData
        {
            public String ClientId;
            public String Title;
            public String Surname;
            public String Names;
            public String Identity;
            public DateTime DOB;
            public String MaritalStatus;
            public String Gender;
            public String Nationality;
            public String Language;
            public AdditionalClientData AddressAndContact;
        }
        public struct AdditionalClientData
        {
            public String AddressLine1;
            public String AddressLine2;
            public String AddressLine3;
            public string ContactNumber;
            public string AlternativeNumber;
            public String EmailAddress;
        }
        public struct StockItem
        {
            public String StockCode;
            public String Description;
            public String BatchCode;
            public Decimal PackSize;
            public Decimal Quantity;
            public DateTime ExpiryDate;
            public String Notes;
            public String UserId;
            public String AccountNumber;
            public int RecordId;
        }
        public enum ActionType
        {
            Received,WriteOff,Transfer,Issue,Return
        }
        public enum ActionSuccessState
        {
            True,False,Error
        }
        public struct ReturnData
        {
            public ActionSuccessState Success;
            public String Description;
        }
    }
}
