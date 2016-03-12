using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ims
{
    public class StockManagement
    {
        private String conString;
        public String SDAP
        {
            set
            {
                this.conString = value;
            }
        }
        public imsmetadata.ReturnData issueProductSupply(imsmetadata.StockItem data)
        {
            imsmetadata.ReturnData rState = new imsmetadata.ReturnData();

            if (this.conString != null)
            {
                #region Validate Inputs
                if (data.Quantity <= 0)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = String.Format("Quantity of {0} not acceptable", data.Quantity);
                }
                else if (data.AccountNumber == null || data.AccountNumber == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "No account number provided";
                }
                else if (data.UserId == null || data.UserId == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "User id not provided";
                }
                else if (data.Description == null || data.Description == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Stock Code's description unspecified";
                }
                else if (data.StockCode == null || data.StockCode == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Stock Code unspecified";
                }
                else
                {
                    if (data.BatchCode == null) data.BatchCode = string.Empty;
                    if (data.Notes == null) data.Notes = string.Empty;

                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Failed To Issue product supply";
                    decimal TotalToSubtract = data.Quantity;
                    List<imsmetadata.StockItem> items = this.getAllAvailableItems(data.StockCode, data.AccountNumber);
                    if (items.Count > 0)
                    {
                        if (items[0].Quantity > 0)
                        {
                            imsmetadata.StockItem item;
                            decimal temp = 0, issued = 0;
                            for (int i = 0; i < items.Count; i++)
                            {
                                item = items[i];
                                if (TotalToSubtract > 0)
                                {
                                    if (item.Quantity >= TotalToSubtract)
                                    {
                                        item.Quantity -= TotalToSubtract;
                                        item.Notes = data.Notes;
                                        if (this.refreshedItem(item, imsmetadata.ActionType.Issue, TotalToSubtract))
                                        {
                                            issued += TotalToSubtract;
                                            rState.Description = String.Format("{0} units issued", issued);
                                            rState.Success = imsmetadata.ActionSuccessState.True;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (item.Quantity > 0)
                                        {
                                            temp = item.Quantity;
                                            item.Quantity = 0;
                                            item.Notes = data.Notes;
                                            if (this.refreshedItem(item, imsmetadata.ActionType.Issue, temp))
                                            {
                                                issued += temp;
                                                TotalToSubtract -= temp;
                                                rState.Description = String.Format("{0} units issued", issued);
                                            }
                                            else
                                            {
                                                item.Quantity = temp;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    rState.Success = imsmetadata.ActionSuccessState.True;
                                    rState.Description = string.Empty;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            rState.Description = "This product is out of stock";
                        }
                    }
                    else
                    {
                        rState.Success = imsmetadata.ActionSuccessState.False;
                        rState.Description = "Stock code does not exist";
                    }
                }
                #endregion
            }
            else
            {
                rState.Success = imsmetadata.ActionSuccessState.Error;
                rState.Description = "No database connections specified!!!";
            }
            return rState;
        }

        public imsmetadata.ReturnData writeProductOff(imsmetadata.StockItem data)
        {
            imsmetadata.ReturnData rState = new imsmetadata.ReturnData();

            if (this.conString != null)
            {
                #region Validate Inputs
                if (data.Quantity <= 0)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = String.Format("Quantity of {0} not acceptable", data.Quantity);
                }
                else if (data.ExpiryDate.Date <= DateTime.Now.Date)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Item product has reached it's expiry date!!!";
                }
                else if (data.AccountNumber == null || data.AccountNumber == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "No account number provided";
                }
                else if (data.UserId == null || data.UserId == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "User id not provided";
                }
                else if (data.Description == null || data.Description == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Stock Code's description unspecified";
                }
                else if (data.StockCode == null || data.StockCode == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Stock Code unspecified";
                }
                else
                {
                    if (data.BatchCode == null) data.BatchCode = string.Empty;
                    if (data.Notes == null) data.Notes = string.Empty;

                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Failed To Issue product supply";
                    decimal TotalToSubtract = data.Quantity;
                    List<imsmetadata.StockItem> items = this.getAllAvailableItems(data.StockCode, data.AccountNumber);
                    if (items.Count > 0)
                    {
                        if (items[0].Quantity > 0)
                        {
                            imsmetadata.StockItem item;
                            decimal temp = 0, issued = 0;
                            for (int i = 0; i < items.Count; i++)
                            {
                                item = items[i];
                                if (item.ExpiryDate.Date == data.ExpiryDate.Date)
                                {
                                    if (TotalToSubtract > 0)
                                    {
                                        if (item.Quantity >= TotalToSubtract)
                                        {
                                            item.Quantity -= TotalToSubtract;
                                            item.Notes = data.Notes;
                                            if (this.refreshedItem(item, imsmetadata.ActionType.WriteOff, TotalToSubtract))
                                            {
                                                issued += TotalToSubtract;
                                                rState.Description = String.Format("{0} units written off", issued);
                                                rState.Success = imsmetadata.ActionSuccessState.True;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (item.Quantity > 0)
                                            {
                                                temp = item.Quantity;
                                                item.Quantity = 0;
                                                item.Notes = data.Notes;
                                                if (this.refreshedItem(item, imsmetadata.ActionType.WriteOff, temp))
                                                {
                                                    issued += temp;
                                                    TotalToSubtract -= temp;
                                                    rState.Description = String.Format("{0} units written off", issued);
                                                }
                                                else
                                                {
                                                    item.Quantity = temp;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        rState.Success = imsmetadata.ActionSuccessState.True;
                                        rState.Description = string.Empty;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            rState.Description = "This product is out of stock";
                        }
                    }
                    else
                    {
                        rState.Success = imsmetadata.ActionSuccessState.False;
                        rState.Description = "Stock code does not exist";
                    }
                }
                #endregion
            }
            else
            {
                rState.Success = imsmetadata.ActionSuccessState.Error;
                rState.Description = "No database connections specified!!!";
            }
            return rState;
        }

        public imsmetadata.ReturnData transferProduct(imsmetadata.StockItem data)
        {
            imsmetadata.ReturnData rState = new imsmetadata.ReturnData();

            if (this.conString != null)
            {
                #region Validate Inputs
                if (data.Quantity <= 0)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = String.Format("Quantity of {0} not acceptable", data.Quantity);
                }
                else if (data.ExpiryDate.Date <= DateTime.Now.Date)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Item product has reached it's expiry date!!!";
                }
                else if (data.AccountNumber == null || data.AccountNumber == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "No account number provided";
                }
                else if (data.UserId == null || data.UserId == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "User id not provided";
                }
                else if (data.Description == null || data.Description == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Stock Code's description unspecified";
                }
                else if (data.StockCode == null || data.StockCode == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Stock Code unspecified";
                }
                else
                {
                    if (data.BatchCode == null) data.BatchCode = string.Empty;
                    if (data.Notes == null) data.Notes = string.Empty;

                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Failed To Issue product supply";
                    decimal TotalToSubtract = data.Quantity;
                    List<imsmetadata.StockItem> items = this.getAllAvailableItems(data.StockCode, data.AccountNumber);
                    if (items.Count > 0)
                    {
                        if (items[0].Quantity > 0)
                        {
                            imsmetadata.StockItem item;
                            decimal temp = 0, issued = 0;
                            for (int i = 0; i < items.Count; i++)
                            {
                                item = items[i];
                                if (item.ExpiryDate.Date == data.ExpiryDate.Date)
                                {
                                    if (TotalToSubtract > 0)
                                    {
                                        if (item.Quantity >= TotalToSubtract)
                                        {
                                            item.Quantity -= TotalToSubtract;
                                            item.Notes = data.Notes;
                                            if (this.refreshedItem(item, imsmetadata.ActionType.Transfer, TotalToSubtract))
                                            {
                                                issued += TotalToSubtract;
                                                rState.Description = String.Format("{0} units transferred", issued);
                                                rState.Success = imsmetadata.ActionSuccessState.True;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (item.Quantity > 0)
                                            {
                                                temp = item.Quantity;
                                                item.Quantity = 0;
                                                item.Notes = data.Notes;
                                                if (this.refreshedItem(item, imsmetadata.ActionType.Transfer, temp))
                                                {
                                                    issued += temp;
                                                    TotalToSubtract -= temp;
                                                    rState.Description = String.Format("{0} units transferred", issued);
                                                }
                                                else
                                                {
                                                    item.Quantity = temp;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        rState.Success = imsmetadata.ActionSuccessState.True;
                                        rState.Description = string.Empty;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            rState.Description = "This product is out of stock";
                        }
                    }
                    else
                    {
                        rState.Success = imsmetadata.ActionSuccessState.False;
                        rState.Description = "Stock code does not exist";
                    }
                }
                #endregion
            }
            else
            {
                rState.Success = imsmetadata.ActionSuccessState.Error;
                rState.Description = "No database connections specified!!!";
            }
            return rState;
        }

        public imsmetadata.ReturnData returnProduct(imsmetadata.StockItem data,DateTime TransactionDate)
        {
            imsmetadata.ReturnData rState = new imsmetadata.ReturnData();

            if (this.conString != null)
            {
                #region Validate Inputs
                if (data.Quantity <= 0)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = String.Format("Quantity of {0} not acceptable", data.Quantity);
                }
                else if (data.ExpiryDate.Date <= DateTime.Now.Date)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Item product has reached it's expiry date!!!";
                }
                else if (data.AccountNumber == null || data.AccountNumber == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "No account number provided";
                }
                else if (data.UserId == null || data.UserId == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "User id not provided";
                }
                else if (data.Description == null || data.Description == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Stock Code's description unspecified";
                }
                else if (data.StockCode == null || data.StockCode == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Stock Code unspecified";
                }
                else
                {
                    if (data.BatchCode == null) data.BatchCode = string.Empty;
                    if (data.Notes == null) data.Notes = string.Empty;

                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Failed to return data";
                    decimal TotalToReturn = data.Quantity;
                    //get only items which cann be returned
                    List<imsmetadata.StockItem> items = this.getAllAvailableItems(data.StockCode, data.AccountNumber, data.ExpiryDate);
                    if (items.Count > 0)
                    {
                        imsmetadata.StockItem item;
                        decimal temp = 0, returned = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            item = items[i];
                            if (TotalToReturn > 0)
                            {
                                if (item.Quantity >= TotalToReturn)
                                {
                                    item.Quantity += TotalToReturn;
                                    item.Notes = data.Notes;
                                    if (this.refreshedItem(item, imsmetadata.ActionType.Return, TotalToReturn))
                                    {
                                        returned += TotalToReturn;
                                        rState.Description = String.Format("{0} units returned", returned);
                                        rState.Success = imsmetadata.ActionSuccessState.True;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (item.Quantity > 0)
                                    {
                                        temp = item.Quantity;
                                        item.Quantity = 0;
                                        item.Notes = data.Notes;
                                        if (this.refreshedItem(item, imsmetadata.ActionType.Return, temp))
                                        {
                                            returned += temp;
                                            TotalToReturn -= temp;
                                            rState.Description = String.Format("{0} units returned", returned);
                                        }
                                        else
                                        {
                                            item.Quantity = temp;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                rState.Success = imsmetadata.ActionSuccessState.True;
                                rState.Description = string.Empty;
                                break;
                            }
                        }
                    }
                    else
                    {
                        rState.Success = imsmetadata.ActionSuccessState.False;
                        rState.Description = String.Format("There were no transactions for this product {0} on {1}", data.StockCode, TransactionDate);
                    }
                }
                #endregion
            }
            else
            {
                rState.Success = imsmetadata.ActionSuccessState.Error;
                rState.Description = "No database connections specified!!!";
            }
            return rState;
        }

        public imsmetadata.ReturnData addNewStock(imsmetadata.StockItem data)
        {
            imsmetadata.ReturnData rState = new imsmetadata.ReturnData();
            if (this.conString != null)
            {
                #region Validate Inputs
                if (data.PackSize <= 0)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = String.Format("Pack size of {0} not acceptable", data.PackSize);
                }
                else if (data.Quantity <= 0)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = String.Format("Quantity of {0} not acceptable", data.Quantity);
                }
                else if (data.ExpiryDate.Date <= DateTime.Now.Date)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Item product has reached it's expiry date!!!";
                }
                else if (data.AccountNumber == null || data.AccountNumber == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "No account number provided";
                }
                else if (data.UserId == null || data.UserId == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "User id not provided";
                }
                else if (data.Description == null || data.Description == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Stock Code's description unspecified";
                }
                else if (data.StockCode == null || data.StockCode == string.Empty)
                {
                    rState.Success = imsmetadata.ActionSuccessState.False;
                    rState.Description = "Stock Code unspecified";
                }
                else
                {
                    if (data.BatchCode == null) data.BatchCode = string.Empty;
                    if (data.Notes == null) data.Notes = string.Empty;

                    try
                    {
                        int success = 0;
                        decimal existing = this.getExistingTotals(data.StockCode, data.AccountNumber, data.ExpiryDate);
                        String sql;
                        if (this.isNewProduct(data.StockCode, data.AccountNumber, data.ExpiryDate))
                        {
                            sql = string.Format(
                                "Insert Into [StockManagement].[dbo].[CurrentStock] values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',getdate())",
                                data.StockCode,
                                data.Description.Replace("'", "`"),
                                data.BatchCode,
                                data.PackSize,
                                data.Quantity,
                                data.ExpiryDate.Date,
                                data.AccountNumber,
                                data.UserId
                            );
                        }
                        else
                        {
                            sql = string.Format(
                                "Update [StockManagement].[dbo].[CurrentStock] Set Quantity+='{3}' Where [StockCode]='{0}' and [AccountNumber]='{1}' and [ExpiryDate]='{2}'",
                                data.StockCode,
                                data.AccountNumber,
                                data.ExpiryDate.Date,
                                data.Quantity
                            );
                        }
                        using (SqlConnection connection = new SqlConnection(this.conString))
                        {
                            connection.Open();
                            using (SqlCommand cmd = new SqlCommand(sql, connection))
                            {
                                success = cmd.ExecuteNonQuery();
                            }
                            connection.Close();
                        }
                        sql = string.Format(
                            "Insert Into [StockManagement].[dbo].[TransactionsLog] values(getdate(),'Received','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                            data.Notes.Replace("'", "`"),
                            data.StockCode,
                            data.Quantity,
                            (existing <= 0 && data.Quantity < 0) ? 0 : (data.Quantity + (existing < 0 ? 0 : existing)),
                            data.ExpiryDate,
                            data.AccountNumber,
                            data.UserId,
                            data.BatchCode
                        );
                        using (SqlConnection connection = new SqlConnection(this.conString))
                        {
                            connection.Open();
                            using (SqlCommand cmd = new SqlCommand(sql, connection))
                            {
                                success += cmd.ExecuteNonQuery();
                            }
                            connection.Close();
                        }
                        if (success < 1)
                        {
                            rState.Success = imsmetadata.ActionSuccessState.False;
                            rState.Description = "Update unsuccessful";
                        }
                    }
                    catch (Exception e)
                    {
                        rState.Success = imsmetadata.ActionSuccessState.Error;
                        rState.Description = string.Format("Error Loading Stock : {0}.\\nPlease try again", e.Message);
                    }
                }
                #endregion
                return rState;
            }
            else
            {
                rState.Success = imsmetadata.ActionSuccessState.Error;
                rState.Description = "No database connections specified!!!";
                return rState;
            }
        }
        private bool isNewProduct(String stockCode, String accountnumber, DateTime ExpiryDate)
        {
            decimal matches = 0;
            try
            {
                String sql = string.Format(
                    "SELECT count(1) Total " +
                    "FROM [StockManagement].[dbo].[CurrentStock] " +
                    "Where [StockCode]='{0}' " +
                    "and [AccountNumber]='{1}' " +
                    "and [ExpiryDate]='{2}'",
                    stockCode,
                    accountnumber,
                    ExpiryDate.Date
                );
                using (SqlConnection connection = new SqlConnection(this.conString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal.TryParse(reader.GetValue(0).ToString().Trim(), out matches);
                            }
                            reader.Close();
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Checking Existence : " + e.Message);
            }
            return matches == 0;
        }
        private decimal getExistingTotals(String stockCode, String accountnumber, DateTime ExpiryDate)
        {
            decimal matches = 0;
            try
            {
                String sql = string.Format(
                    "SELECT sum(quantity) Total " +
                    "FROM [StockManagement].[dbo].[CurrentStock] " +
                    "Where [StockCode]='{0}' " +
                    "and [AccountNumber]='{1}' " +
                    "and [ExpiryDate]='{2}'",
                    stockCode,
                    accountnumber,
                    ExpiryDate.Date
                );
                using (SqlConnection connection = new SqlConnection(this.conString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal.TryParse(reader.GetValue(0).ToString().Trim(), out matches);
                            }
                            else
                            {
                                matches = -1;
                            }
                            reader.Close();
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Checking Existence : " + e.Message);
            }
            return matches;
        }
        private bool refreshedItem(imsmetadata.StockItem data, ims.imsmetadata.ActionType action, decimal actionQuantity)
        {
            int success = 0;
            try
            {
                if (action != imsmetadata.ActionType.Received &&
                    action != imsmetadata.ActionType.Return) actionQuantity = -actionQuantity;
                String sql = string.Format(
                    "Update [StockManagement].[dbo].[CurrentStock] Set Quantity='{3}' Where [StockCode]='{0}' and [AccountNumber]='{1}' and [ExpiryDate]='{2}'",
                    data.StockCode,
                    data.AccountNumber,
                    data.ExpiryDate.Date,
                    data.Quantity
                );
                using (SqlConnection connection = new SqlConnection(this.conString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        success += cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                decimal existing = this.getExistingTotals(data.StockCode, data.AccountNumber, data.ExpiryDate);
                sql = string.Format(
                            "Insert Into [StockManagement].[dbo].[TransactionsLog] values(getdate(),'{8}','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                            data.Notes.Replace("'", "`"),
                            data.StockCode,
                            actionQuantity,
                            (existing <= 0 && actionQuantity < 0) ? 0 : existing,
                            data.ExpiryDate,
                            data.AccountNumber,
                            data.UserId,
                            data.BatchCode,
                            action.ToString()
                        );
                using (SqlConnection connection = new SqlConnection(this.conString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        success += cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Checking Existence : " + e.Message);
            }
            return success > 0;
        }
        private List<imsmetadata.StockItem> getAllAvailableItems(String stockCode, String accountnumber, DateTime transactionDate)
        {
            List<imsmetadata.StockItem> allItems = new List<imsmetadata.StockItem>();
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {

                connection = new SqlConnection(this.conString);
                connection.Open();
                cmd = new SqlCommand("ReturnableTransactions", connection);
                cmd.Parameters.Add("@TransactionDate", System.Data.SqlDbType.DateTime).Value = transactionDate;
                cmd.Parameters.Add("@AccountNumber", System.Data.SqlDbType.DateTime).Value = accountnumber;
                cmd.Parameters.Add("@StockCode", System.Data.SqlDbType.DateTime).Value = stockCode;
                reader = cmd.ExecuteReader();
                imsmetadata.StockItem item;
                int i = -1;
                while (reader.Read())
                {
                    i = -1;
                    item = new imsmetadata.StockItem();
                    {
                        Int32.TryParse(reader.GetValue(++i).ToString().Trim(), out item.RecordId);
                        item.StockCode = reader.GetValue(++i).ToString().Trim();
                        item.Description = reader.GetValue(++i).ToString().Trim();
                        item.BatchCode = reader.GetValue(++i).ToString().Trim();
                        decimal.TryParse(reader.GetValue(++i).ToString().Trim(), out item.PackSize);
                        decimal.TryParse(reader.GetValue(++i).ToString().Trim(), out item.Quantity);
                        DateTime.TryParse(reader.GetValue(++i).ToString().Trim(), out item.ExpiryDate);
                        item.AccountNumber = reader.GetValue(++i).ToString().Trim();
                        item.UserId = reader.GetValue(++i).ToString().Trim();
                        item.Notes = reader.GetValue(++i).ToString().Trim();
                    }
                    allItems.Add(item);
                }
                reader.Close();
                reader.Dispose();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Checking Existence : " + e.Message);
            }
            finally
            {
                if (connection != null) connection.Dispose();
                if (cmd != null) cmd.Dispose();
                if (reader != null) reader.Dispose();

            }
            return allItems;
        }
        private List<imsmetadata.StockItem> getAllAvailableItems(String stockCode, String accountnumber)
        {
            List<imsmetadata.StockItem> allItems = new List<imsmetadata.StockItem>();
            try
            {
                String sql = string.Format(
                    "SELECT distinct c.[RecordId]  " +
                    "      ,c.[StockCode]  " +
                    "      ,c.[StockCodeDescription] " +
                    "      ,c.[BatchCode] " +
                    "	  ,c.[PackSize]  " +
                    "	  ,c.[Quantity] " +
                    "	  ,c.[ExpiryDate]  " +
                    "	  ,c.[AccountNumber]  " +
                    "	  ,c.userid " +
                    "	  ,'' notes " +
                    "FROM [StockManagement].[dbo].[CurrentStock] c " +
                    "join StockManagement.dbo.TransactionsLog t " +
                    "on c.stockcode=t.stockcode " +
                    "Where c.[StockCode]='{0}' " +
                    "and c.[AccountNumber]='{1}' " +
                    "order by [ExpiryDate] asc",
                    stockCode,
                    accountnumber
                );
                using (SqlConnection connection = new SqlConnection(this.conString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            imsmetadata.StockItem item;
                            int i = -1;
                            while (reader.Read())
                            {
                                i = -1;
                                item = new imsmetadata.StockItem();
                                {
                                    Int32.TryParse(reader.GetValue(++i).ToString().Trim(), out item.RecordId);
                                    item.StockCode = reader.GetValue(++i).ToString().Trim();
                                    item.Description = reader.GetValue(++i).ToString().Trim();
                                    item.BatchCode = reader.GetValue(++i).ToString().Trim();
                                    decimal.TryParse(reader.GetValue(++i).ToString().Trim(), out item.PackSize);
                                    decimal.TryParse(reader.GetValue(++i).ToString().Trim(), out item.Quantity);
                                    DateTime.TryParse(reader.GetValue(++i).ToString().Trim(), out item.ExpiryDate);
                                    item.AccountNumber = reader.GetValue(++i).ToString().Trim();
                                    item.UserId = reader.GetValue(++i).ToString().Trim();
                                    item.Notes = reader.GetValue(++i).ToString().Trim();
                                }
                                allItems.Add(item);
                            }
                            reader.Close();
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Checking Existence : " + e.Message);
            }
            return allItems;
        }
    }
}
