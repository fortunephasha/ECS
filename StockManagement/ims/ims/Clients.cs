using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ims
{
    public class Clients
    {
        private String conString;
        public String SDAP
        {
            set
            {
                this.conString = value;
            }
        }
        public Clients(){}

        #region New Registrations
        public imsmetadata.ReturnData clientRegistered(imsmetadata.ClientData newClient,String userId,String accountNumber)
        {
            imsmetadata.ReturnData outcome = new imsmetadata.ReturnData();
            if (this.conString == null)
            {
                outcome.Success = imsmetadata.ActionSuccessState.Error;
                outcome.Description = "Cannot establish connection to the server!!!";
                return outcome;
            }
            else
            {
                if (this.isNewClient(newClient.Identity,accountNumber))
                {
                    String clienid = this.getClientId();
                    if (clienid != null && clienid != string.Empty)
                    {
                        SqlConnection connection = null;
                        SqlCommand cmd = null;
                        try
                        {
                            connection = new SqlConnection(this.conString);
                            connection.Open();

                            cmd = new SqlCommand("RegClient", connection);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            #region parameters
                            cmd.Parameters.Add("@ClientId", System.Data.SqlDbType.VarChar).Value = clienid;
                            cmd.Parameters.Add("@Title", System.Data.SqlDbType.VarChar).Value = newClient.Title;
                            cmd.Parameters.Add("@Surname", System.Data.SqlDbType.VarChar).Value = newClient.Surname;
                            cmd.Parameters.Add("@Full_Names", System.Data.SqlDbType.VarChar).Value = newClient.Names;
                            cmd.Parameters.Add("@Identity", System.Data.SqlDbType.VarChar).Value = newClient.Identity;
                            cmd.Parameters.Add("@Date_Of_Birth", System.Data.SqlDbType.Date).Value = newClient.DOB;
                            cmd.Parameters.Add("@Marital_status", System.Data.SqlDbType.VarChar).Value = newClient.MaritalStatus;
                            cmd.Parameters.Add("@Gender", System.Data.SqlDbType.VarChar).Value = newClient.Gender;
                            cmd.Parameters.Add("@Nationality", System.Data.SqlDbType.VarChar).Value = newClient.Nationality;
                            cmd.Parameters.Add("@Home_Language", System.Data.SqlDbType.VarChar).Value = newClient.Language;
                            cmd.Parameters.Add("@UserId", System.Data.SqlDbType.VarChar).Value = userId;
                            cmd.Parameters.Add("@AccountNumber", System.Data.SqlDbType.VarChar).Value = accountNumber;
                            cmd.Parameters.Add("@AddressLine1", System.Data.SqlDbType.VarChar).Value = newClient.AddressAndContact.AddressLine1;
                            cmd.Parameters.Add("@AddressLine2", System.Data.SqlDbType.VarChar).Value = newClient.AddressAndContact.AddressLine2;
                            cmd.Parameters.Add("@AddressLine3", System.Data.SqlDbType.VarChar).Value = newClient.AddressAndContact.AddressLine3;
                            cmd.Parameters.Add("@ContactNumber", System.Data.SqlDbType.VarChar).Value = newClient.AddressAndContact.ContactNumber;
                            cmd.Parameters.Add("@AlternateContactNumber", System.Data.SqlDbType.VarChar).Value = newClient.AddressAndContact.AlternativeNumber;
                            cmd.Parameters.Add("@EmailAddress", System.Data.SqlDbType.VarChar).Value = newClient.AddressAndContact.EmailAddress;
                            #endregion
                            int rs = cmd.ExecuteNonQuery();
                            outcome.Success = imsmetadata.ActionSuccessState.True;
                            outcome.Description = "Registration Successful";
                            connection.Close();
                        }
                        catch (Exception e)
                        {
                            outcome.Success = imsmetadata.ActionSuccessState.Error;
                            if (e.Message.ToLower().Contains("primary key"))
                            {
                                outcome.Description = "Registration Failed : Error Generating New Client ID";
                            }
                            else
                            {
                                outcome.Description = String.Format(
                                    "Registration Failed : (unknown error) ~~ {0}",
                                    e.Message
                                );
                            }
                            return outcome;
                        }
                        finally
                        {
                            if (connection != null) connection.Dispose();
                            if (cmd != null) cmd.Dispose();
                        }
                    }
                    else
                    {
                        outcome.Success = imsmetadata.ActionSuccessState.Error;
                        outcome.Description = "Registration Failed : Unable to generate new client id";
                    }
                }
                else
                {
                    outcome.Success = imsmetadata.ActionSuccessState.False;
                    outcome.Description = "This client is already registered";
                    return outcome;
                }
            }
            return outcome;
        }
        private bool isNewClient(String identity, String account)
        {
            int matched = 0;
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                connection = new SqlConnection(this.conString);
                connection.Open();
                cmd = new SqlCommand("CheckClientExistence", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@ClientIdentity", System.Data.SqlDbType.VarChar).Value = identity;
                cmd.Parameters.Add("@AccountNumber", System.Data.SqlDbType.VarChar).Value = account;
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Int32.TryParse(reader.GetValue(0).ToString().Trim(), out matched);
                }
                reader.Close();
                connection.Close();
                return matched == 0;
            }
            catch (Exception e)
            {
                Debug.WriteLine(" Error Checking Client Existence : "+e.Message);
            }
            finally
            {
                if (connection != null) connection.Dispose();
                if (cmd != null) cmd.Dispose();
                if (reader != null) reader.Dispose();
            }
            return matched == 0;
        }
        #endregion

        #region Client Number
        private String getClientId()
        {            
                StringBuilder newId =new StringBuilder();
                StringBuilder tagId=new StringBuilder();
                int lastNum;
                String t1, t2, t3;
                for (decimal i = 1; i < 16;i++ )
                {
                    for (decimal x = 1; x < 16; x++)
                    {
                        for (decimal k = 1; k < 16; k++)
                        {
                            t1 = i.ToHexString();
                            t2 = x.ToHexString();
                            t3 = k.ToHexString();
                            tagId.Clear();
                            tagId.Append(t1);
                            tagId.Append(t2);
                            tagId.Append(t3);
                            lastNum = this.getLastClientOf(tagId.ToString()) + 1;
                            if (lastNum <= 9999)
                            {
                                i = 16;
                                x = i;
                                newId.Append(tagId.ToString());
                                switch (lastNum.ToString().Length)
                                {
                                    case 1: { newId.Append(String.Format("000{0}", lastNum)); }; break;
                                    case 2: { newId.Append(String.Format("00{0}", lastNum)); }; break;
                                    case 3: { newId.Append(String.Format("0{0}", lastNum)); }; break;
                                    default:
                                        {
                                            if (lastNum > 9999)
                                            {
                                                newId.Append(lastNum);
                                            }
                                            else
                                            {
                                                newId.Append(String.Format("{0}", lastNum));
                                            }
                                            newId.Append(lastNum);
                                        }; break;
                                }
                                break;
                            }
                            else
                            {
                                tagId.Clear();
                            }
                        }
                    }
                }
                return newId.ToString();
        }
        private Int32 getLastClientOf(String tagId)
        {
            int matched = 0;
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                connection = new SqlConnection(this.conString);
                connection.Open();
                cmd = new SqlCommand("LastClientNumber", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@TagId", System.Data.SqlDbType.VarChar).Value = tagId;
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Int32.TryParse(reader.GetValue(0).ToString().Trim(), out matched);
                }
                reader.Close();
                connection.Close();
                return matched;
            }
            catch (Exception e)
            {
                Console.WriteLine(" Error Generating Client Id : " + e.Message);
                return matched;
            }
            finally
            {
                if (connection != null) connection.Dispose();
                if (cmd != null) cmd.Dispose();
                if (reader != null) reader.Dispose();
            }
        }
        #endregion
    }
}