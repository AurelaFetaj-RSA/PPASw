using MySql.Data.EntityFramework;
using PPAUtils;
using RSACommon.Configuration;
using RSACommon.DatabasesUtils;
using RSACommon.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace MySqlDB
{
    internal class Program
    {
        public static string USERNAME = "PlasticAuchoUser";
        public static string PWD = "Robots_RSA";
        public static string HOST = "192.168.0.242";
        public static string DBNAME = "PlasticauchoDB";

        public static RSACommon.Configuration.MySqlConfiguration CreateConfiguration(string user, string pwd, string host, string DB)
        {
            string encryptedString = RSACommon.PasswordSecurity.EncryptString(pwd);

            RSACommon.Configuration.MySqlConfiguration configuration = new RSACommon.Configuration.MySqlConfiguration()
            {
                Host = host,
                User = new SqlUser()
                {
                    UserName = user,
                    Password = encryptedString
                },
                DBName = DB
            };

            return configuration;
        }

        static void Main(string[] args)
        {
            DbConfiguration.SetConfiguration(new MySqlEFConfiguration());
            var config = CreateConfiguration(USERNAME, PWD, HOST, DBNAME);
            MySQLService service = new MySQLService(config);

            service.AddTable<model>("models");
            service.AddTable<padlaserprogram>("padlaserprograms");

            service.StartNoAsync();

            Console.WriteLine("Insert MODELS");

            Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    object[] value = new object[]
                    {
                    $"TES{i}",
                    1,2,3,4,5,6,7,8,9,10,11,12
                    };

                    var testResult = await service.DBTable[0].InsertAutomaticAsync(value);

                    Console.WriteLine($"{testResult.Error} {testResult.Message}");
                }

                MySqlResult<model> result = await service.DBTable[0].SelectAllAsync<model>();

                result.Result.ForEach(x => Console.WriteLine(x.model_name));

                //foreach (var key in result.Result)
                //{
                //    service.DBTable[0].DeleteRowPrimaryKey(key.model_name);
                //}

                Console.WriteLine("Clean");
                result.Result.ForEach(x => Console.WriteLine(x.model_name));

            });

            Console.WriteLine("Insert PADLASERS");

            Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    object[] value = new object[]
                    {
                    $"TES{i}",
                    1,2,3,4,5,6,7,8,
                    };

                    var testResult = await service.DBTable[1].InsertAutomaticAsync(value);

                    Console.WriteLine($"{testResult.Error} {testResult.Message}");
                }

                MySqlResult<padlaserprogram> result = await service.DBTable[1].SelectAllAsync<padlaserprogram>();

                result.Result.ForEach(x => Console.WriteLine(x.program_name));

                //foreach (var key in result.Result)
                //{
                //    service.DBTable[1].DeleteRowPrimaryKey(key.program_name);
                //}

                Console.WriteLine("Clean");
                result.Result.ForEach(x => Console.WriteLine(x.program_name));

            });


            //MySqlTransaction transaction = service.SqlConnection.BeginTransaction();

            //try
            //{
            //    using (var context = new PadLaserProgramsContext(service.SqlConnection))
            //    {
            //        // Interception/SQL logging
            //        context.Database.Log = (string message) => { Console.WriteLine(message); };

            //        // Passing an existing transaction to the context
            //        context.Database.UseTransaction(transaction);

            //        List<padlaserprogram> test = new List<padlaserprogram>()
            //        {
            //            new padlaserprogram(){ program_name = "pipp", Q1 = 1, Q2 = 1, Q3 = 3, Q4 = 4, S1 = 0, S2 = 2, S3= 3, S4 = 3},
            //            new padlaserprogram(){ program_name = "pip2", Q1 = 1, Q2 = 1, Q3 = 3, Q4 = 4, S1 = 0, S2 = 2, S3= 3, S4 = 3 },
            //        };

            //        context.PadLaserPrograms.Add(test[0]);
            //        context.SaveChanges();
            //    }

            //    transaction.Commit();
            //}
            //catch (Exception ex)
            //{
            //    transaction.Rollback();
            //    Console.WriteLine(ex.ToString());
            //}

            Console.ReadLine();

        }



    }
}
