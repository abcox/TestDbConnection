using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiceBrokerListener.Domain;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using TestDbConnector.models;

namespace TestDbConnector
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = GetListener("PublisherConnection", "PriceBook", OnPriceBookTableChanged);
            listener.Start();
            //DisplayPrompt(args);
            //do
            //{
            //    Thread.Sleep(100);
            //} while (Console.ReadKey().KeyChar != 'x');

            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    // Do something
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            listener.Stop();
        }

        private static SqlDependencyEx GetListener(
            string connectionStringName,
            string tableName,
            EventHandler<SqlDependencyEx.TableChangedEventArgs> callbackMethod)
        {
            var connectionString = Configuration.GetConnectionString(connectionStringName);
            var databaseName = new SqlConnection(connectionString).Database;

            SqlDependencyEx listener = null;
            try
            {
                listener = new SqlDependencyEx(connectionString, databaseName, tableName);
                listener.TableChanged += callbackMethod;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to create a listener on table {tableName} in database {databaseName}", ex.Message);
            }
            return listener;
        }

        private static void OnPriceBookTableChanged(object sender, SqlDependencyEx.TableChangedEventArgs tableChangedEventArgs)
        {
            Console.WriteLine("EventType: {0}", tableChangedEventArgs.NotificationType);
            Console.WriteLine("Data: {0}", tableChangedEventArgs.Data);

            try
            {
                //var result = JsonConvert.DeserializeObject<ListenerResponse_TpPriceBookRecord>(tableChangedEventArgs.Data.ToString());
                XmlSerializer serializer = new XmlSerializer(typeof(ListenerResponse_TpPriceBookRecord));
                // convert string to stream
                byte[] byteArray = Encoding.UTF8.GetBytes(tableChangedEventArgs.Data.ToString());
                //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
                MemoryStream stream = new MemoryStream(byteArray);
                var result = (ListenerResponse_TpPriceBookRecord)serializer.Deserialize(stream);

                //todo:  write record in destination database...
                var cnn2 = Configuration.GetConnectionString("SubsciberConnection");
                using (IDbConnection db = new SqlConnection(cnn2))
                {
                    string insertQuery = @"INSERT INTO [dbo].[PriceBook]([Name], [Price], [RefId]) VALUES (@Name, @Price, @RefId)";

                    var result2 = db.Execute(insertQuery, new TpPriceBookRecord
                    {
                        Name = result.inserted.row.Name,
                        Price = result.inserted.row.Price,
                        RefId = result.inserted.row.Id,
                    });
                }
                Console.WriteLine("result: {0}", result.inserted.row.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static IConfiguration Configuration
        {
            get
            {
                return new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
            }
        }
    }
}
