using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test2.Models;

using System.Data.Odbc;
using System.Data.OleDb;
using Dapper;
using System.Data.Entity;
using Dapper.Contrib.Extensions;

namespace Test2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            #region OLEDB + Dapper
            // OLEDB dobrze działa 
            string connetionString = null;
            OleDbConnection cnn;

            // Tylko dla formatu .mdb
            connetionString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source = C:\Users\user\Desktop\test.mdb";
            // Zarówno .mdb jak i .accdb
            connetionString = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\user\Desktop\test.accdb";

            cnn = new OleDbConnection(connetionString);
            try
            {
                cnn.Open();

                var usr = cnn.Query<User>("SELECT * FROM Users");

                User user = new User
                {
                    FirstName = "InsertDap"
                };


                //cnn.Query("INSERT INTO Users VALUES(@Ob)", new { Ob = user.FirstName });
                //var sr = cnn.Get<User>(1);

                //Dapper Contrib dla Inserta baza Accesa odmawia współpracy, gdyż nie jest w stanie przetworzyc 2 zapytań na raz
                //cnn.Insert(new User {FirstName = "insertContrib" });
                //cnn.Insert<User>(user);

                cnn.Close();
            }
            catch (Exception ex)
            {

            }
            #endregion

            #region ODBC + Dapper
            // Trzeba najpierw utworzyć DSN poprzez narzędzie administracyjne
            string connectionString2 = @"Dsn=test;dbq=C:\Users\user\Desktop\test.accdb;driverid=25;fil=MS Access;maxbuffersize=2048;pagetimeout=5;uid=admin";
            //connectionString2 = @"Driver ={ Microsoft Access Driver(*.mdb, *.accdb)}; Dsn = test; dbq = C:\Users\user\Desktop\test.accdb;";
            var connection = new OdbcConnection(connectionString2);

            //Construct the query
            //Usually you use @parameter as the syntax for querying SQL Server
            //But for querying to Microsoft Access you must use ?
            var queryText = "SELECT * FROM Users";

            //Use dapper to query with parameter
            //It's also a good idea if you use a string as a parameter that you use DbString instead of sending the variable directly
            //You will also need to specify the Length exactly as the length of the column in the Microsoft Access table
            connection.Open();
            var data = connection.Query<User>(queryText);

            //var queryText = "SELECT * FROM table1 WHERE Name = ?";
            //var data = connection.Query<User>(queryText, new { Name = new DbString { Value = name, Length = 10, IsAnsi = true } });

            #endregion

            #region EF z JetEntityFrameworkProvider
            // Entity Framework wraz z JetEntityFrameworkProvider --> dziala bardzo dobrze (nawet z istniejącą bazą danych)
            using (var context = new Context())
            {
                User usr = new User
                {
                    FirstName = "Test"
                };
                context.Users.Add(usr);
                context.Users.ToList();

                context.SaveChanges();
            }
            #endregion

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
             
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public class Context : DbContext
    {
        public Context() : base("DefaultConnection")
        {
            Database.SetInitializer<Context>(null);
        }

        public DbSet<User> Users { get; set; }
    }
}