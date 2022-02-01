using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace batchMaster
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter batch number...");
            string temp = Console.ReadLine();

            string sql = "select batch_programs.program_no from dbo.batch " +
                "left join dbo.batch_programs on batch.door_id = dbo.batch_programs.door_id " +
                "where batch_id =" + temp + " group by batch_programs.program_no";

            DataTable badNumbers = new DataTable();
            badNumbers.Columns.Add("number", typeof(string));

            using (SqlConnection conn = new SqlConnection(CONNECT.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    string filePath = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        // Console.WriteLine(row[0].ToString());
                        filePath = @"\\designsvr1\subcontracts\yaweiNC\" + row[0].ToString() + ".MPF";
                        //open the file for each one
                        if (File.Exists(filePath))
                        {
                            string test = File.ReadAllText(filePath);
                            bool optimised;
                            optimised = test.Contains(";OPTIMIZED");
                            if (optimised == false)
                            {
                                DataRow drNumber = badNumbers.NewRow();
                                drNumber[0] = row[0].ToString();
                                badNumbers.Rows.Add(drNumber);
                            }
                        }
                    }
                    if (badNumbers.Rows.Count > 0)
                    {
                        Console.WriteLine("The following numbers have errors...");
                        Console.WriteLine("-------------------------------------------------");
                        foreach (DataRow row in badNumbers.Rows)
                        {
                            Console.WriteLine(row[0].ToString());
                        }

                        Console.WriteLine("-------------------------------------------------");
                    }
                    else
                        Console.WriteLine("No errors found...");
                }
                conn.Close();
            }
            Console.ReadLine();

        }
    }
}
