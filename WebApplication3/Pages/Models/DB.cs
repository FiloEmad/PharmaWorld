using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication3.Pages.Models
{
    public class DB
    {
        //private string ConnectionString = "Data Source=LAPTOP-GG7NKS6O;Initial Catalog=PHARMAWORLDT03;Integrated Security=True;Encrypt=False";
        private string ConnectionString = "Data Source=FELOEMAD; Initial Catalog=PHARMAWORLDT03; Integrated Security=True; Encrypt=False";

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

        public DataTable AddEmployee(User emp)
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
                string query = @"SELECT p.ID, p.pname, p.ptype, s.expdate, s.sellprice, s.buyprice, p.descrep, s.amount 
                                 FROM products p 
                                 JOIN Stock s ON p.ID = s.PID ";

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
                string query = @"SELECT empolyee.FName, empolyee.LName,empolyee.ID,FORMAT(transactions.tdate, 'dd/MM/yyyy') AS tdate,transactions.Paid_amount,transactions.price,transactions.remainder, transactions.ID AS tid,transactions.CID, clients.cname, products.pname AS pname, transactions.sell_buy
                                FROM transactions 
                                JOIN clients ON transactions.CID = clients.ID 
                                JOIN products ON transactions.PID = products.ID
                                JOIN empolyee ON transactions.salesID = empolyee.ID";
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
            string query = "select empolyee.*, Department.DName , Department.salary+bonus as salary\r\n  from empolyee join Department on DID=Department.ID\r\n  where empolyee.ID = @id";
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
                                salary = Convert.ToInt32(reader["salary"]),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = Convert.ToInt32(reader["phonenumber"]),
                                EAddress = reader["EAddress"].ToString(),
                                DID = Convert.ToInt32(reader["DID"]),
                                Dname = reader["DName"].ToString()
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

        public User EditEmpFname(string First, int id)
        {
            string query = " update empolyee \r\n set FName = @First \r\n where ID = @id ";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameter to prevent SQL injection
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@First", First);

                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return getEmpInfo(id);
        }

        public User EditEmpLname(string Last, int id)
        {
            string query = " update empolyee \r\n set LName = @Last \r\n where ID = @id ";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameter to prevent SQL injection
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Last", Last);

                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return getEmpInfo(id);
        }

        public User EditEmpPhone(string Phone, int id)
        {
            string query = " update empolyee \r\n set phonenumber = @Phone \r\n where ID = @id ";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameter to prevent SQL injection
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Phone", Phone);

                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return getEmpInfo(id);
        }

        public User EditEmpEmail(string Email, int id)
        {
            string query = " update empolyee \r\n set Email = @Email \r\n where ID = @id ";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameter to prevent SQL injection
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Email", Email);

                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return getEmpInfo(id);
        }

        public User EditEmpAddress(string Address, int id)
        {
            string query = " update empolyee \r\n set EAddress = @Address \r\n where ID = @id ";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameter to prevent SQL injection
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Address", Address);

                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return getEmpInfo(id);
        }

        public void SendDayoffReq(int eid, string reason, DateOnly start, DateOnly end)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                // Retrieve the maximum ID from the table
                int ID = 1; // Default to 1 if the table is empty
                string query = " SELECT ISNULL(MAX(ID), 0) AS MaxID FROM Daysoff"; // Ensure a default of 0

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
                string insertQuery = " INSERT INTO Daysoff (ID, EID, REASON, Ddate, Dstate, enddate) VALUES (@id, @eid, @reason, @start, N'طلب', @end)";


                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", ID);
                    cmd.Parameters.AddWithValue("@eid", eid);
                    cmd.Parameters.AddWithValue("@reason", reason);
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", end);

                    cmd.ExecuteNonQuery();
                }
            }
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
                string query = @"SELECT MONTH(tdate) AS monthprofit, YEAR(tdate) AS yearprofit , SUM(netprice) AS net_profit
                                FROM(
                                SELECT tdate, transactions.sell_buy,
                                CASE transactions.sell_buy
                                WHEN N'بيع' THEN price
                                ELSE -price
                                END AS netprice
                                FROM transactions
                                JOIN clients ON transactions.CID = clients.ID 
                                JOIN products ON transactions.PID = products.ID
                                JOIN empolyee ON transactions.salesID = empolyee.ID) AS netprofit
                                GROUP BY MONTH(tdate), YEAR(tdate)
                                ORDER BY monthprofit";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;

        }
        public bool deleteprod(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT MONTH(tdate) AS monthprofit, YEAR(tdate) AS yearprofit , SUM(netprice) AS net_profit
                                FROM(
                                SELECT tdate, transactions.sell_buy,
                                CASE transactions.sell_buy
                                WHEN N'بيع' THEN price
                                ELSE -price
                                END AS netprice
                                FROM transactions
                                JOIN clients ON transactions.CID = clients.ID 
                                JOIN products ON transactions.PID = products.ID
                                JOIN empolyee ON transactions.salesID = empolyee.ID) AS netprofit
                                GROUP BY MONTH(tdate), YEAR(tdate)
                                ORDER BY monthprofit";
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
                string insertQuery = @"INSERT INTO products (ID, pname, ptype, descrep) VALUES (@id, @pname, @ptype,@desc);
                                       INSERT INTO Stock (PID, sellprice, buyprice, expdate, amount) VALUES(@id, @sellprice, @buyprice, @edate, @amount)";


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
        public DataTable offers()
        {
            DataTable res = new DataTable();
            string query = "SELECT * FROM Offers1";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(res);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return res;


        }
        public DataTable offers2()
        {
            DataTable res = new DataTable();
            string query = "SELECT * FROM Offers2";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(res);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return res;


        }
        public DataTable offers3()
        {
            DataTable res = new DataTable();
            string query = "SELECT * FROM Offers3";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(res);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return res;


        }

        public DataTable transSort(int i)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT empolyee.FName, empolyee.LName,empolyee.ID,FORMAT(transactions.tdate, 'dd/MM/yyyy') AS tdate,transactions.Paid_amount,transactions.price,transactions.remainder, transactions.ID AS tid,transactions.CID, clients.cname, products.pname AS pname, transactions.sell_buy
                             FROM transactions 
                             JOIN clients ON transactions.CID = clients.ID 
                             JOIN products ON transactions.PID = products.ID
                             JOIN empolyee ON transactions.salesID = empolyee.ID ORDER BY transactions.price DESC;";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                if (i == 0)
                {


                    query = @"SELECT empolyee.FName, empolyee.LName,empolyee.ID,FORMAT(transactions.tdate, 'dd/MM/yyyy') AS tdate,transactions.Paid_amount,transactions.price,transactions.remainder, transactions.ID AS tid,transactions.CID, clients.cname, products.pname AS pname, transactions.sell_buy
                            FROM transactions 
                            JOIN clients ON transactions.CID = clients.ID 
                            JOIN products ON transactions.PID = products.ID
                            JOIN empolyee ON transactions.salesID = empolyee.ID order BY transactions.price DESC; ";
                }
                if (i == 1)
                {
                    query = @"SELECT empolyee.FName, empolyee.LName,empolyee.ID,FORMAT(transactions.tdate, 'dd/MM/yyyy') AS tdate,transactions.Paid_amount,transactions.price,transactions.remainder, transactions.ID AS tid,transactions.CID, clients.cname, products.pname AS pname, transactions.sell_buy
                                FROM transactions 
                                JOIN clients ON transactions.CID = clients.ID 
                                JOIN products ON transactions.PID = products.ID
                                JOIN empolyee ON transactions.salesID = empolyee.ID order BY transactions.price ASC; ";
                }
                if (i == 2)
                {
                    query = @"SELECT empolyee.FName, empolyee.LName,empolyee.ID,FORMAT(transactions.tdate, 'dd/MM/yyyy') AS tdate,transactions.Paid_amount,transactions.price,transactions.remainder, transactions.ID AS tid,transactions.CID, clients.cname, products.pname AS pname, transactions.sell_buy
                                FROM transactions 
                                JOIN clients ON transactions.CID = clients.ID 
                                JOIN products ON transactions.PID = products.ID
                                JOIN empolyee ON transactions.salesID = empolyee.ID order BY  empolyee.FName; ";

                }

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }
        public DataTable Filter(int i)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT empolyee.FName, empolyee.LName,empolyee.ID,FORMAT(transactions.tdate, 'dd/MM/yyyy') AS tdate,transactions.Paid_amount,transactions.price,transactions.remainder, transactions.ID AS tid,transactions.CID, clients.cname, products.pname AS pname, transactions.sell_buy
                            FROM transactions 
                            JOIN clients ON transactions.CID = clients.ID 
                            JOIN products ON transactions.PID = products.ID
                            JOIN empolyee ON transactions.salesID = empolyee.ID WHERE transactions.sell_buy = N'بيع'; ";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                if (i == 0)
                {


                    query = @"SELECT empolyee.FName, empolyee.LName,empolyee.ID,FORMAT(transactions.tdate, 'dd/MM/yyyy') AS tdate,transactions.Paid_amount,transactions.price,transactions.remainder, transactions.ID AS tid,transactions.CID, clients.cname, products.pname AS pname, transactions.sell_buy
                                FROM transactions 
                                JOIN clients ON transactions.CID = clients.ID 
                                JOIN products ON transactions.PID = products.ID
                                JOIN empolyee ON transactions.salesID = empolyee.ID WHERE transactions.sell_buy = N'بيع'; ";
                }
                if (i == 1)
                {
                    query = @"SELECT empolyee.FName, empolyee.LName,empolyee.ID,FORMAT(transactions.tdate, 'dd/MM/yyyy') AS tdate,transactions.Paid_amount,transactions.price,transactions.remainder, transactions.ID AS tid,transactions.CID, clients.cname, products.pname AS pname, transactions.sell_buy
                                FROM transactions 
                                JOIN clients ON transactions.CID = clients.ID 
                                JOIN products ON transactions.PID = products.ID
                                JOIN empolyee ON transactions.salesID = empolyee.ID WHERE transactions.sell_buy = N'شراء'; ";
                }


                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable mostpaidproduct()
        {
            DataTable st = new DataTable();
            string query = "with count_pro as\r\n(select PID,count(*) as c from transactions where transactions.sell_buy=N'بيع ' group by transactions.PID ),\r\nmax_pro as(\r\nselect max(c) as f from count_pro)\r\nselect products.pname,c from (max_pro join count_pro on f=c) join products on products.ID= PID";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        SqlDataAdapter ss = new SqlDataAdapter(query, con);
                        ss.Fill(st);
                    }
                    catch { }
                    finally { con.Close(); }
                }
            }
            return st;
        }

        public DataTable mostcomapny()
        {
            DataTable st = new DataTable();
            string query = "with count_pro as\r\n(select CID,count(*) as c from transactions group by transactions.CID ),\r\nmax_pro as(\r\nselect max(c) as f from count_pro)\r\nselect clients.cname,c from (max_pro join count_pro on f=c) join clients on clients.ID= CID";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        SqlDataAdapter ss = new SqlDataAdapter(query, con);
                        ss.Fill(st);
                    }
                    catch { }
                    finally { con.Close(); }
                }
            }
            return st;
        }

        public DataTable mostysalesagent()
        {
            DataTable dt = new DataTable();
            string query = "with count_pro as\r\n(select salesID,count(*) as c from transactions group by transactions.salesID ),\r\nmax_pro as(\r\nselect max(c) as f from count_pro)\r\nselect empolyee.FName, empolyee.LName,c from (max_pro join count_pro on f=c) join empolyee on empolyee.ID= salesID";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        SqlDataAdapter dto = new SqlDataAdapter(query, con);
                        dto.Fill(dt);

                    }
                    catch { }
                    finally { con.Close(); }
                }
            }
            return dt;
        }

        public DataTable inbetweenamount(int amount1)
        {
            DataTable dt = new DataTable();
            if (amount1 == 0)
            {

                string query = @"SELECT products.pname, sum(amount) AS amount FROM products JOIN Stock ON products.ID = Stock.PID
                                GROUP BY products.pname
                                ORDER BY amount DESC";
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        try
                        {
                            con.Open();
                            SqlDataAdapter dto = new SqlDataAdapter(query, con);
                            dto.Fill(dt);

                        }
                        catch { }
                        finally { con.Close(); }
                    }
                }
            }
            else
            {

                string query = @"SELECT products.pname, sum(amount) AS amount FROM products JOIN Stock ON products.ID = Stock.PID
                                    WHERE amount< @amount1
                                    GROUP BY products.pname
                                    ORDER BY amount DESC";
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        try
                        {
                            con.Open();
                            SqlDataAdapter dto = new SqlDataAdapter(query, con);
                            dto.Fill(dt);

                        }
                        catch { }
                        finally { con.Close(); }
                    }
                }
            }
            return dt;
        }
        public Dictionary<string, int> productamount(int range)
        {
            if (range == 0)
            {
                Dictionary<string, int> labelsAndCounts = new Dictionary<string, int>();
                try
                {
                    con.Open();
                    string query = @"SELECT products.pname, sum(amount) AS amount 
                                    FROM products JOIN Stock ON products.ID = Stock.PID 
                                    GROUP BY products.pname 
                                    ORDER BY amount DESC";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@range", range);
                    SqlDataReader sdr = cmd.ExecuteReader();
                    // store labels and counts inside the dictionary 
                    while (sdr.Read())
                    {
                        labelsAndCounts.Add(sdr["pname"].ToString(), (int)sdr["amount"]);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally { con.Close(); }
                return labelsAndCounts;
            }
            else
            {
                Dictionary<string, int> labelsAndCounts = new Dictionary<string, int>();
                try
                {
                    con.Open();
                    string query = "select products.pname, sum(amount) as amount from products join sellprice on products.ID = sellprice.PID\r\nwhere amount< @range\r\ngroup by products.pname \r\norder by amount desc";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@range", range);
                    SqlDataReader sdr = cmd.ExecuteReader();
                    // store labels and counts inside the dictionary 
                    while (sdr.Read())
                    {
                        labelsAndCounts.Add(sdr["pname"].ToString(), (int)sdr["amount"]);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally { con.Close(); }
                return labelsAndCounts;

            }

        }
        public Dictionary<string, int> finchart(int range)
        {
            if (range == 0)
            {
                Dictionary<string, int> labelsAndCounts = new Dictionary<string, int>();
                try
                {
                    con.Open();
                    string query = "select cname,sum(Amount) as sum from FINANCIAL_INFO join clients on FINANCIAL_INFO.CID= clients.ID\r\n  where STATUSf= N'دين'\r\n  group by cname";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@range", range);
                    SqlDataReader sdr = cmd.ExecuteReader();
                    // store labels and counts inside the dictionary 
                    while (sdr.Read())
                    {
                        labelsAndCounts.Add(sdr["cname"].ToString(), (int)sdr["sum"]);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally { con.Close(); }
                return labelsAndCounts;
            }
            else
            {
                Dictionary<string, int> labelsAndCounts = new Dictionary<string, int>();
                try
                {
                    con.Open();
                    string query = "select cname,sum(Amount) as sum from FINANCIAL_INFO join clients on FINANCIAL_INFO.CID= clients.ID\r\n  where STATUSf= N'ائتمان'\r\n  group by cname";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@range", range);
                    SqlDataReader sdr = cmd.ExecuteReader();
                    // store labels and counts inside the dictionary 
                    while (sdr.Read())
                    {
                        labelsAndCounts.Add(sdr["cname"].ToString(), (int)sdr["sum"]);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally { con.Close(); }
                return labelsAndCounts;

            }


        }
        public DataTable finSort(int i)
        {
            DataTable dt = new DataTable();
            string query = @" SELECT FINANCIAL_INFO.ID AS id, FINANCIAL_INFO.CID AS cid, FINANCIAL_INFO.Amount, FINANCIAL_INFO.STATUSf, clients.cname
  FROM  FINANCIAL_INFO JOIN clients ON  FINANCIAL_INFO.CID = clients.ID
  order by amount desc; ";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                if (i == 0)
                {


                    query = @" SELECT FINANCIAL_INFO.ID AS id, FINANCIAL_INFO.CID AS cid, FINANCIAL_INFO.Amount, FINANCIAL_INFO.STATUSf, clients.cname
  FROM  FINANCIAL_INFO JOIN clients ON  FINANCIAL_INFO.CID = clients.ID
  order by amount desc;";
                }
                if (i == 1)
                {
                    query = @"SELECT FINANCIAL_INFO.ID AS id, FINANCIAL_INFO.CID AS cid, FINANCIAL_INFO.Amount, FINANCIAL_INFO.STATUSf, clients.cname
  FROM  FINANCIAL_INFO JOIN clients ON  FINANCIAL_INFO.CID = clients.ID
  order by amount asc; ";
                }
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }
        public DataTable Filterfin(int i)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT FINANCIAL_INFO.ID AS id, FINANCIAL_INFO.CID AS cid, FINANCIAL_INFO.Amount, FINANCIAL_INFO.STATUSf, clients.cname
  FROM  FINANCIAL_INFO JOIN clients ON  FINANCIAL_INFO.CID = clients.ID
   ";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                if (i == 0)
                {


                    query = @"  SELECT FINANCIAL_INFO.ID AS id, FINANCIAL_INFO.CID AS cid, FINANCIAL_INFO.Amount, FINANCIAL_INFO.STATUSf, clients.cname
  FROM  FINANCIAL_INFO JOIN clients ON  FINANCIAL_INFO.CID = clients.ID where STATUSf=N'دين' ";
                }
                if (i == 1)
                {
                    query = @"SELECT FINANCIAL_INFO.ID AS id, FINANCIAL_INFO.CID AS cid, FINANCIAL_INFO.Amount, FINANCIAL_INFO.STATUSf, clients.cname
  FROM  FINANCIAL_INFO JOIN clients ON  FINANCIAL_INFO.CID = clients.ID where STATUSf=N'ائتمان'";
                }


                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }
        public DataTable Searchfin(string search)
        {

            string input = search;
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            string reversed = new string(charArray);


            DataTable dataTable = new DataTable();
            string query = @"
    SELECT FINANCIAL_INFO.ID AS id, 
           FINANCIAL_INFO.CID AS cid, 
           FINANCIAL_INFO.Amount, 
           FINANCIAL_INFO.STATUSf, 
           clients.cname
    FROM FINANCIAL_INFO
    JOIN clients ON FINANCIAL_INFO.CID = clients.ID 
    WHERE clients.cname = @reversed;";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        cmd.Parameters.AddWithValue("@reversed", reversed);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dataTable);
                        Console.WriteLine(reversed);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    finally { con.Close(); }
                }
            }
            return dataTable;
        }
        public DataTable daysoffsort()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    string query = "Select[Daysoff].EID as eid,[Daysoff].[ID] AS ddid,[FName],[LName],[DName],REASON,Dstate from \r\nDaysoff Join [dbo].[empolyee] on [dbo].[empolyee].ID=Daysoff.EID join [dbo].[Department] \r\non  [empolyee].DID =[Department].ID \r\norder by FName";

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
        public DataTable daysofffilter(int filter)
        {
            DataTable dt = new DataTable();
            if (filter == 1)
            {
                string query = "Select[Daysoff].EID as eid,[Daysoff].[ID] AS ddid,[FName],[LName],[DName],REASON,Dstate from \r\nDaysoff Join [dbo].[empolyee] on [dbo].[empolyee].ID=Daysoff.EID join [dbo].[Department] \r\non  [empolyee].DID =[Department].ID  where Dstate=N'رفض'";
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter(query, con);
                        da.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                string query = "Select[Daysoff].EID as eid,[Daysoff].[ID] AS ddid,[FName],[LName],[DName],REASON,Dstate from \r\nDaysoff Join [dbo].[empolyee] on [dbo].[empolyee].ID=Daysoff.EID join [dbo].[Department] \r\non  [empolyee].DID =[Department].ID  where Dstate=N'موافقة'";
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter(query, con);
                        da.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
            return dt;
        }
        public DataTable searchdays(string search)
        {
            string input = search;
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            string reversed = new string(charArray);
            DataTable dataTable = new DataTable();
            string query = "Select[Daysoff].EID as eid,[Daysoff].[ID] AS ddid,[FName],[LName],[DName],REASON,Dstate from \r\nDaysoff left Join [dbo].[empolyee] on [dbo].[empolyee].ID=Daysoff.EID left join [dbo].[Department] \r\non  [empolyee].DID =[Department].ID  where Fname= @reversed";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        cmd.Parameters.AddWithValue("@reversed", reversed);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dataTable);
                        Console.WriteLine(reversed);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    finally { con.Close(); }
                }
            }

            return dataTable;

        }
        public Dictionary<string, int> depdays(string range)
        {
          
            string input = range;
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
           string  reversed = new string(charArray);
            Console.WriteLine(reversed);
            Dictionary<string, int> labelsAndCounts = new Dictionary<string, int>();
            try
            {
                con.Open();
                string query = "with countdays as (\r\nselect EID, Count(*) as f from Daysoff group by EID)\r\nselect empolyee.FName, empolyee.LName, countdays.f from (empolyee join countdays on empolyee.ID= EID) join Department \r\non empolyee.DID= Department.ID where Department.DName=@reversed";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@reversed", reversed);
                SqlDataReader sdr = cmd.ExecuteReader();
                // store labels and counts inside the dictionary 
                while (sdr.Read())
                {
                    string fullName = $"{sdr["FName"]} {sdr["LName"]}";
                    int sum = (int)sdr["f"];

                    labelsAndCounts.Add(fullName, sum);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally { con.Close(); }
            return labelsAndCounts;
        }
        public Dictionary<string, int> depdays2(string range)
        {
            Dictionary<string, int> labelsAndCounts = new Dictionary<string, int>();
            try
            {
                con.Open();
                string query = "with countdays as (\r\nselect EID, Count(*) as f from Daysoff group by EID)\r\nselect Department.DName, countdays.f from ((empolyee join countdays on empolyee.ID= EID) join Department \r\non empolyee.DID= Department.ID) join Daysoff on Daysoff.EID= empolyee.ID where @range between Ddate and enddate;";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@range", range);
                SqlDataReader sdr = cmd.ExecuteReader();
                // store labels and counts inside the dictionary 
                while (sdr.Read())
                {
                    labelsAndCounts.Add(sdr["DName"].ToString(), (int)sdr["f"]);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally { con.Close(); }
            return labelsAndCounts;
        }

        public Dictionary<string, int> deptt()
        {
            Dictionary<string, int> labelsAndCounts = new Dictionary<string, int>();
            try
            {
                con.Open();
                string query = "select STATUSf as stat, sum(Amount) as sum from FINANCIAL_INFO group by STATUSf";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader sdr = cmd.ExecuteReader();
                // store labels and counts inside the dictionary 
                while (sdr.Read())
                {
                    labelsAndCounts.Add(sdr["stat"].ToString(), (int)sdr["sum"]);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally { con.Close(); }
            return labelsAndCounts;
        }
    }
            
    
   

}
