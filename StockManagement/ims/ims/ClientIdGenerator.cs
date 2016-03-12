using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ims
{
    public class ClientIdGenerator
    {
        private String conString;
        public String SDAP
        {
            set
            {
                this.conString = value;
            }
        }
        public ClientIdGenerator(){}
        public String getNewClientId()
        {
            if (this.conString == null)
            {
                return null;
            }
            String clientId = string.Empty;
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                clientId = this.newClientId();
            }
            catch (Exception e)
            {
                Console.WriteLine(" Error Generating Client Id : " + e.Message);
            }
            finally
            {
                if (connection != null) connection.Dispose();
                if (cmd != null) cmd.Dispose();
                if (reader != null) reader.Dispose();
            }
            return clientId;
        }
        private String newClientId()
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

                            tagId.Append(t1);
                            tagId.Append(t2);
                            tagId.Append(t3);
                            lastNum = this.getLastClientOf(tagId.ToString()) + 1;
                            i = 16;
                            x = i;
                            newId.Append(tagId.ToString());
                            if (lastNum.ToString().Length == 1)
                            {
                                newId.Append(String.Format("00{0}",lastNum));
                            }
                            else if (lastNum.ToString().Length == 2)
                            {
                                newId.Append(String.Format("0{0}", lastNum));
                            }
                            else
                            {
                                newId.Append(lastNum);
                            }
                            break;
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
    }
}