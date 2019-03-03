using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using System.Data.Odbc;
using Test1.Models;

namespace Test1
{
    public class OdbcRepository
    {
        public List<User> Get()
        {
            #region ODBC + Dapper
            //Create a connection   
            string connectionString = @"Dsn=test;dbq=C:\Users\user\Desktop\test.accdb;driverid=25;fil=MS Access;maxbuffersize=2048;pagetimeout=5;uid=admin"; ;
            var connection = new OdbcConnection(connectionString);

            //Construct the query
            //Usually you use @parameter as the syntax for querying SQL Server
            //But for querying to Microsoft Access you must use ?
            var queryText = "SELECT * FROM Users";

            //Use dapper to query with parameter
            //It's also a good idea if you use a string as a parameter that you use DbString instead of sending the variable directly
            //You will also need to specify the Length exactly as the length of the column in the Microsoft Access table
            connection.Open();
            List<User> data = connection.Query<User>(queryText).ToList();
            User kr = data.ElementAt(2);

            //var queryText = "SELECT * FROM table1 WHERE Name = ?";
            //var data = connection.Query<User>(queryText, new { Name = new DbString { Value = name, Length = 10, IsAnsi = true } });

            return data.ToList();
            #endregion

            #region EF Core + EntityFrameworkCore.Jet
            // Niestety nie można użyć EF ponieważ ten provider musi być wykorzystany w środowisku uruchomieniowym .NET Framework pomimo iż można go zaimplementować w programie .NET Core
            #endregion
        }
    }
}
