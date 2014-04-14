﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace SQL
{
    //Static Information for easy changes
    class connection
    {
        public const string sql_username = "OSUSVT";
        public const string sql_password = "ManBearPig";
        public const string sql_database = "TELEMETRY";
    }
    class SQLclass
    {
        private static MySqlConnection svt_telemetry = new MySqlConnection(
                                       "user id="+connection.sql_username+";" +
                                       "password="+connection.sql_password+";"+
                                       "database="+connection.sql_database+";" +
                                       "server=localhost;"
                                       );
        /*
        SqlCommand add_table = new SqlCommand("",svt_telemetry);
         */
        public SQLclass()
        {
            try
            {
                svt_telemetry.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException exep)
            {
                if (exep.Number == 1042) //We Cant establish a connection because the server is not on or installed
                {
                    MessageBox.Show(exep.Message + "\nMySQL is probably not running or not installed");
                } 
                /*
                 * Try to Automatically Create Database and a user
                 */
                if (exep.Number == 0) //Permissions Denied, we can attempt to fix this
                {
                    string sqlRoot = Microsoft.VisualBasic.Interaction.InputBox("In order to configure MySQL, Please Enter MySQL root password: ", "Root Password", "OSUSVT");
                    try
                    {
                        MySqlConnection setupdatabase = new MySqlConnection("uid=root; server=localhost; password=" + sqlRoot + ";");
                        setupdatabase.Open();
                        MySqlCommand adddata = new MySqlCommand("DROP USER '" + connection.sql_username + "'; ", setupdatabase);
                        adddata.ExecuteNonQuery();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException)
                    {
                        ;//If there is a probelm droping the user, we don't care about it, it probably did not exist
                    }
                    try
                    {
                        MySqlConnection setupdatabase = new MySqlConnection("uid=root; server=localhost; password=" + sqlRoot + ";");
                        setupdatabase.Open();
                        string CommandText = "CREATE DATABASE IF NOT EXISTS " + connection.sql_database + "; " +
                            "CREATE USER '" + connection.sql_username + "'@'%' IDENTIFIED BY '" + connection.sql_password + "'; " +
                            "GRANT ALL PRIVILEGES ON " + connection.sql_database + ".* TO '" + connection.sql_username + "'@'%'; ";

                        MySqlCommand adddata = new MySqlCommand(CommandText, setupdatabase);
                        adddata.ExecuteNonQuery();

                        //Last Step, Open it up...
                        svt_telemetry.Open();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        MessageBox.Show("Failed to Configure the Database Automatically, try configuring manually or check the root password.\n" + "ERROR TEXT: " + ex.Message);
                        //Program should quit here?
                    }
                    
                }
            }
            //
            //Create table(s)
            //
            MySqlCommand set_table = new MySqlCommand("CREATE TABLE IF NOT EXISTS telemetrytable (Id INT NOT NULL PRIMARY KEY AUTO_INCREMENT, RecDate TEXT, RecTime TEXT, Longitude TEXT, Latitude TEXT, Altitude DECIMAL, Velocity DECIMAL);", svt_telemetry);
            set_table.ExecuteNonQuery();
        }
        //Gets the last 5 rows from the database and averages them
        public double get_average(string fieldname, int numRecords)
        {
            const string tablename = "telemetrytable";
            string CommandText = "SELECT SUM( " + fieldname + " ) FROM (SELECT " + fieldname + " FROM " + tablename + " ORDER BY Id DESC LIMIT "+numRecords+" ) AS subquery;";
            MySqlCommand select = new MySqlCommand(CommandText, svt_telemetry);
            /*select.Parameters.AddWithValue("@fieldname", fieldname);
            select.Parameters.AddWithValue("@number", numRecords);
            */ //Parameterizing Did not work, because it adds "" around the parameters
            return Convert.ToDouble(select.ExecuteScalar());
        }

        public void update_database(string date, string utc, string longitude, string latitude, string velocity, string altitude)
        {
            MySqlCommand add_to_database = new MySqlCommand();
            add_to_database.Connection = svt_telemetry;
            add_to_database.Parameters.AddWithValue("@date", date); //Stackoverflow insisted this be the way this is done. I think it prevents SQL injections. Not that we should ever have to be concerned about those.
            add_to_database.Parameters.AddWithValue("@utc", utc);
            add_to_database.Parameters.AddWithValue("@longitude", longitude);
            add_to_database.Parameters.AddWithValue("@latitude", latitude);
            add_to_database.Parameters.AddWithValue("@altitude", altitude);
            add_to_database.Parameters.AddWithValue("@velocity", velocity);
            //If Structure Changes remember to change the CREATE TABLE command in the constructer and delete the database...
            add_to_database.CommandText = "INSERT INTO telemetrytable(RecDate,RecTime,Longitude,Latitude,Altitude,Velocity) VALUES (@date,@utc,@longitude,@latitude,@altitude,@velocity)";
            add_to_database.ExecuteNonQuery();
        }
    }
}