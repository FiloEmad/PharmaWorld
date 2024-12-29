using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Transactions;

namespace WebApplication3.Pages.Models
{
    public class DB
    {
        private string ConnectionString = "Data Source=LAPTOP-GG7NKS6O; Initial Catalog=PHARMAWORLDT03; Integrated Security=True; Encrypt=False";

        public SqlConnection con { get; set; }

        public DB()
        {
            con = new SqlConnection(ConnectionString);
        }

        public DataTable getEmployee()
        {
            string query = @"
                SELECT 
                    empolyee.FName, empolyee.LName, empolyee.ID, empolyee.age, empolyee.Workinghours,
                    empolyee.Workingdays, empolyee.EAddress, empolyee.phonenumber, empolyee.bonus,
                    Department.DName AS DepartmentName, Department.ID AS DepartmentID
                FROM 
                    empolyee
                LEFT JOIN 
                    Department ON empolyee.DID = Department.ID;";

            DataTable result = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return result;
        }
        public DataTable getProducts()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = "SELECT p.ID, p.pname, p.ptype, p.expdate, s.sellprice, b.buyprice, p.descrep, s.amount " +
                               "FROM products p " +
                               "JOIN sellprice s ON p.ID = s.PID " +
                               "JOIN buyprice b ON p.ID = b.PID";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }
        public DataTable gettransactions()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = @"select empolyee.FName, empolyee.LName,empolyee.ID,transactions.tdate,transactions.Paid_amount,transactions.price,transactions.remainder, transactions.ID as tid,transactions.CID, clients.cname, products.pname as pname, transactions.sell_buy
            from transactions 
            join clients on transactions.CID = clients.ID 
            join products on transactions.PID = products.ID
            join empolyee on transactions.salesID = empolyee.ID";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }
        public Companyinfo getCompanyInfo()
        {
            Companyinfo company = null;
            string query = "select * from company_info;";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            company = new Companyinfo
                            {
                                Cname = reader["Cname"].ToString(),
                                Adress = reader["Adress"].ToString(),
                                C_owner = reader["C_owner"].ToString(),
                                Finance = Convert.ToInt32(reader["Finance"]),
                                Contact = Convert.ToInt32(reader["Contact"]),
                                Email = reader["Email"].ToString(),
                                Cdate = Convert.ToDateTime(reader["Cdate"])
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return company;
        }
        public bool UpdateCompanyInfo(Companyinfo company)
        {
            bool success = false;
            string query = @"
        UPDATE company_info
        SET 
            Adress = @Adress,
            C_owner = @C_owner,
            Finance = @Finance,
            Cdate = @Cdate,
            Contact = @Contact,
            Email = @Email
        WHERE Cname = @Cname";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@Cname", company.Cname);
                        cmd.Parameters.AddWithValue("@Adress", company.Adress);
                        cmd.Parameters.AddWithValue("@C_owner", company.C_owner);
                        cmd.Parameters.AddWithValue("@Finance", company.Finance);
                        cmd.Parameters.AddWithValue("@Cdate", company.Cdate);
                        cmd.Parameters.AddWithValue("@Contact", company.Contact);
                        cmd.Parameters.AddWithValue("@Email", company.Email);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            success = true; // Update was successful
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return success;
        }

        public User getEmpInfo(int id)
        {
            User emp = null;
            string query = "SELECT * FROM empolyee WHERE ID = @id;";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameter to prevent SQL injection
                        cmd.Parameters.AddWithValue("@id", id);

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            // Create new Employee object and populate it with data
                            emp = new User
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                FName = reader["FName"].ToString(),
                                LName = reader["LName"].ToString(),
                                Age = Convert.ToInt32(reader["age"]),
                                Gender = reader["gender"].ToString(),
                                Workingdays = Convert.ToInt32(reader["Workingdays"]),
                                Workinghours = Convert.ToInt32(reader["Workinghours"]),
                                Bonus = Convert.ToInt32(reader["bonus"]),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = Convert.ToInt32(reader["phonenumber"]),
                                EAddress = reader["EAddress"].ToString(),
                                DID = Convert.ToInt32(reader["DID"])
                            };
                        }
                        else
                        {
                            return null; // No record found
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return emp;
        }
        //public DataTable absence()
        //{
        //    string query = @"Select [DName] , COUNT([dbo].[Daysoff].[EID]) AS COUNTT FROM [empolyee] JOIN [dbo].[Department] ON [dbo].[Department].[ID]= [DID] JOIN[dbo].[Daysoff] 
        //    ON [EID]=[dbo].[empolyee].ID GROUP BY [DName];";

        //    DataTable result = new DataTable();
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(ConnectionString))
        //        {
        //            con.Open();
        //            using (SqlCommand cmd = new SqlCommand(query, con))
        //            {
        //                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //                adapter.Fill(result);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error: " + ex.Message);
        //    }
        //    return result;
        //}
        public DataTable absence()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = "Select [DName] AS a , COUNT([dbo].[Daysoff].[EID]) AS COUNTT FROM [empolyee] JOIN [dbo].[Department] ON [dbo].[Department].[ID]= [DID] JOIN[dbo].[Daysoff]  ON [EID]=[dbo].[empolyee].ID  where Daysoff.Dstate = N'موافقة'  GROUP BY [DName]";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }
        public DataTable daysoff()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    string query = "Select[Daysoff].EID as eid,[Daysoff].[ID] AS ddid,[FName],[LName],[DName],REASON,Dstate from Daysoff Join [dbo].[empolyee] on [dbo].[empolyee].ID=Daysoff.EID join [dbo].[Department] on  [empolyee].DID =[Department].ID";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return dt;
        }
        public DataTable profit()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = "SELECT MONTH(tdate) AS monthprofit, YEAR(tdate) AS yearprofit , SUM(netprice) AS net_profit  FROM(\r\n\tSELECT tdate, sell_buy,\r\n\tCASE sell_buy\r\n\tWHEN N'بيع' THEN price\r\n\tELSE -price\r\n\tEND AS netprice\r\n\tFROM transactions \r\n\tJOIN clients ON transactions.CID = clients.ID \r\n\tJOIN products ON transactions.PID = products.ID\r\n\tJOIN empolyee ON transactions.salesID = empolyee.ID\r\n) AS netprofit\r\nGROUP BY MONTH(tdate), YEAR(tdate)\r\nORDER BY monthprofit\r\n";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;

        }
        public bool deleteprod(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = "\r\nSELECT MONTH(tdate) AS monthprofit, YEAR(tdate) AS yearprofit , SUM(netprice) AS net_profit\r\nFROM(\r\n\tSELECT tdate, sell_buy,\r\n\tCASE sell_buy\r\n\tWHEN N'بيع' THEN price\r\n\tELSE -price\r\n\tEND AS netprice\r\n\tFROM transactions \r\n\tJOIN clients ON transactions.CID = clients.ID \r\n\tJOIN products ON transactions.PID = products.ID\r\n\tJOIN empolyee ON transactions.salesID = empolyee.ID\r\n) AS netprofit\r\nGROUP BY MONTH(tdate), YEAR(tdate)\r\nORDER BY monthprofit\r\n";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                return true;
            }
        }
        public void accept(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string acceptQuery = "UPDATE Daysoff SET Dstate = N'موافقة' WHERE [ID]=@id";
                using (SqlCommand cmd = new SqlCommand(acceptQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery(); // Execute the query
                }
            }
        }

        public void decline(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string declineQuery = "UPDATE Daysoff SET Dstate = N'رفض' WHERE [ID]=@id";
                using (SqlCommand cmd = new SqlCommand(declineQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }


        }
        public void AddProduct(string pname, string ptype, DateTime expdate, string descrep, int buyprice, int sellprice, int amount)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                // Retrieve the maximum ID from the table
                int ID = 1; // Default to 1 if the table is empty
                string query = "SELECT ISNULL(MAX(ID), 0) AS MaxID FROM products"; // Ensure a default of 0

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ID = reader.GetInt32(reader.GetOrdinal("MaxID")) + 1; // Increment the ID
                        }
                    }
                }

                // Insert the new product into the database
                string insertQuery = "INSERT INTO products (ID, pname, ptype, expdate, descrep) VALUES (@id, @pname, @ptype, @edate,@desc) INSERT INTO buyprice (PID, buyprice, amount) VALUES (@id, @buyprice, @amount)INSERT INTO sellprice (PID, sellprice, amount) VALUES (@id, @sellprice, @amount)";


                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", ID);
                    cmd.Parameters.AddWithValue("@pname", pname);
                    cmd.Parameters.AddWithValue("@ptype", ptype);
                    cmd.Parameters.AddWithValue("@edate", expdate);
                    cmd.Parameters.AddWithValue("@desc", descrep);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@buyprice", buyprice);
                    cmd.Parameters.AddWithValue("@sellprice", sellprice);



                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProduct(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                // Correct DELETE query
                string deleteQuery = "DELETE FROM products WHERE ID = @id";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable fin_info()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT FINANCIAL_INFO.ID AS id, FINANCIAL_INFO.CID AS cid, FINANCIAL_INFO.Amount, FINANCIAL_INFO.STATUSf, clients.cname FROM  FINANCIAL_INFO JOIN clients ON  FINANCIAL_INFO.CID = clients.ID";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return dt;
        }
        public FinancialRecord GetRecordById(int id)
        {
            FinancialRecord df = new FinancialRecord();
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT FINANCIAL_INFO.ID AS id, FINANCIAL_INFO.CID AS cid, FINANCIAL_INFO.Amount, FINANCIAL_INFO.STATUSf, clients.cname FROM FINANCIAL_INFO JOIN clients ON FINANCIAL_INFO.CID = clients.ID WHERE FINANCIAL_INFO.ID = @id";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@id", id);  // Add parameter for query
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    // Optionally log or rethrow the error depending on your needs.
                }
            }

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                df.Cid = (int)row["cid"];  // Correct assignment for Cid
                df.Cname = (string)row["cname"];
                df.Amount = (int)row["Amount"];
                df.STATUSf = (string)row["STATUSf"];
            }

            return df;
        }

        public void UpdateRecord(FinancialRecord record)
        {
            int id = record.Id;
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string updateQuery = "UPDATE FINANCIAL_INFO SET Amount = @Amount, STATUSf = @STATUSf WHERE ID = @id";
                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateRecordd(int Amount, int id)
        {
           
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string updateQuery = "UPDATE FINANCIAL_INFO SET Amount = @Amount WHERE ID = @id";
                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@Amount", Amount);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteRecord(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                
                string deleteQuery = "DELETE FROM FINANCIAL_INFO WHERE ID = @id";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        //public void AddTransaction(DateTime TDate, int Amount,int SalesID, int PID,int  CID,String sell_buy, string Payment,int Price, int PaidAmount, int Remainder)
        //{
        //    using (SqlConnection con = new SqlConnection(ConnectionString))
        //    {
        //        try
        //        {
        //            con.Open();
        //            string status;
        //            if (sell_buy == "sell")
        //            {
        //                status = "دين";
        //            }
        //            else
        //            {
        //                status = "ائتمان";
        //            }

        //            int ID1 = 1;
        //            string query1 = "SELECT ISNULL(MAX(ID), 0) AS MaxID FROM Transactions";

        //            using (SqlCommand cmd1 = new SqlCommand(query1, con))
        //            {
        //                using (SqlDataReader reader = cmd1.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        ID1 = reader.GetInt32(reader.GetOrdinal("MaxID")) + 1; // Increment the ID
        //                    }
        //                }
        //            }

        //            string query = "INSERT INTO transactions (ID, tdate, amount, salesID, PID, CID,sell_buy, payment, price, Paid_amount, remainder) VALUES (@ID1, @TDate, @Amount, @SalesID, @PID, @CID, N@sell_buy, N@Payment, @Price, @PaidAmount, @Remainder)";


        //            using (SqlCommand cmd2 = new SqlCommand(query, con))
        //            {
        //                cmd2.Parameters.AddWithValue("@ID", ID1);
        //                cmd2.Parameters.AddWithValue("@TDate", TDate);
        //                cmd2.Parameters.AddWithValue("@Amount", Amount);
        //                cmd2.Parameters.AddWithValue("@SalesID", SalesID);
        //                cmd2.Parameters.AddWithValue("@PID", PID);
        //                cmd2.Parameters.AddWithValue("@CID", CID);
        //                cmd2.Parameters.AddWithValue("@sell_buy", sell_buy);
        //                cmd2.Parameters.AddWithValue("@Payment", Payment);
        //                cmd2.Parameters.AddWithValue("@Price", Price);
        //                cmd2.Parameters.AddWithValue("@PaidAmount", PaidAmount);
        //                cmd2.Parameters.AddWithValue("@Remainder", Remainder);

        //                con.Open();
        //                cmd2.ExecuteNonQuery();
        //            }
        //            int ID2 = 1;
        //            string query3 = "SELECT ISNULL(MAX(ID), 0) AS MaxID FROM FINANCIAL_INFO";

        //            using (SqlCommand cmd3 = new SqlCommand(query1, con))
        //            {
        //                using (SqlDataReader reader = cmd3.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        ID2 = reader.GetInt32(reader.GetOrdinal("MaxID")) + 1;
        //                    }
        //                }
        //            }

        //            string query4 = "INSERT INTO FINANCIAL_INFO(ID, CID, Amount,STATUSf) values(@ID2, @CID,@Remainder, @status) ";


        //            using (SqlCommand cmd4 = new SqlCommand(query, con))
        //            {
        //                cmd4.Parameters.AddWithValue("@ID", ID2);
        //                cmd4.Parameters.AddWithValue("@Amount", Remainder);
        //                cmd4.Parameters.AddWithValue("@CID", CID);
        //                cmd4.Parameters.AddWithValue("@STATUSf", status);
        //                con.Open();
        //                cmd4.ExecuteNonQuery();
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        finally
        //        { con.Close(); }
        //    }
        //}
        public void AddTransaction(DateTime TDate, int Amount, int SalesID, int PID, int CID, string sell_buy, string Payment, int Price, int PaidAmount, int Remainder)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                using (var scope = new TransactionScope())
                {
                    try
                    {
                        string status;
                        if (sell_buy == "sell")
                        {
                            status = "دين";
                        }
                        else
                        {
                            status = "ائتمان";
                        }

                        // Fetch MaxID for transactions
                        int ID1 = 1;
                        string query1 = "SELECT ISNULL(MAX(ID), 0) AS MaxID FROM Transactions";
                        using (SqlCommand cmd1 = new SqlCommand(query1, con))
                        using (SqlDataReader reader = cmd1.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID1 = reader.GetInt32(0) + 1;
                            }
                        }

                        // Insert into transactions
                        string query = "INSERT INTO transactions (ID, tdate, amount, salesID, PID, CID, sell_buy, payment, price, Paid_amount, remainder) VALUES (@ID, @TDate, @Amount, @SalesID, @PID, @CID, @sell_buy, @Payment, @Price, @PaidAmount, @Remainder)";
                        using (SqlCommand cmd2 = new SqlCommand(query, con))
                        {
                            cmd2.Parameters.AddWithValue("@ID", ID1);
                            cmd2.Parameters.AddWithValue("@TDate", TDate);
                            cmd2.Parameters.AddWithValue("@Amount", Amount);
                            cmd2.Parameters.AddWithValue("@SalesID", SalesID);
                            cmd2.Parameters.AddWithValue("@PID", PID);
                            cmd2.Parameters.AddWithValue("@CID", CID);
                            cmd2.Parameters.AddWithValue("@sell_buy", sell_buy);
                            cmd2.Parameters.AddWithValue("@Payment", Payment);
                            cmd2.Parameters.AddWithValue("@Price", Price);
                            cmd2.Parameters.AddWithValue("@PaidAmount", PaidAmount);
                            cmd2.Parameters.AddWithValue("@Remainder", Remainder);
                            cmd2.ExecuteNonQuery();
                        }

                        // Fetch MaxID for FINANCIAL_INFO
                        int ID2 = 1;
                        string query3 = "SELECT ISNULL(MAX(ID), 0) AS MaxID FROM FINANCIAL_INFO";
                        using (SqlCommand cmd3 = new SqlCommand(query3, con))
                        using (SqlDataReader reader = cmd3.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID2 = reader.GetInt32(0) + 1;
                            }
                        }

                        // Insert into FINANCIAL_INFO
                        string query4 = "INSERT INTO FINANCIAL_INFO (ID, CID, Amount, STATUSf) VALUES (@ID, @CID, @Amount, @STATUSf)";
                        using (SqlCommand cmd4 = new SqlCommand(query4, con))
                        {
                            cmd4.Parameters.AddWithValue("@ID", ID2);               // New ID for FINANCIAL_INFO
                            cmd4.Parameters.AddWithValue("@CID", CID);             // Client ID
                            cmd4.Parameters.AddWithValue("@Amount", Remainder);    // Using the 'Remainder' parameter as 'Amount'
                            cmd4.Parameters.AddWithValue("@STATUSf", status);      // Status (دين or ائتمان)
                            cmd4.ExecuteNonQuery();
                        }

                        scope.Complete(); // Commit transaction
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        throw;
                    }
                }
            }
        }

    }
}
