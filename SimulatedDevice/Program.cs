using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using DT = System.Data;            // System.Data.dll  
using QC = System.Data.SqlClient;  // System.Data.dll  
namespace SimulatedDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "dxretailiothub.azure-devices.net";
        static string deviceKey = "SUuM6szwQJKAzDP1tRJueyfpLkcNXd7Wp6Zqlhr/jaI=";
        static List<product> products;



        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("Device1", deviceKey));


            

            GetProductInfo();
            SendDeviceToCloudCheckdataAsync();


            //SendDeviceToCloudTelemetryAsync();
            //SendDeviceToCloudSurveydataAsync();
            //SendDeviceToCloudUXClickdataAsync();

            Console.ReadLine();
        }

        private static async void SendDeviceToCloudCheckdataAsync()
        {
            Random rand = new Random();

            while (true)
            {
                //double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;
                double currentWindSpeed =  rand.Next(5, 12);
                int productnumber = rand.Next(213, 606);
                var checkDataPoint = new
                {
                    //Generate POS data
                    datatype = "checkdata",

                    ProductKey = products[productnumber].productkey,
                    OrderDateKey = 20061130,
                    DueDateKey = 20061130,
                    ShipDateKey = 20061130,
                    CustomerKey = rand.Next(11000, 29484),
                    PromotionKey = 1,
                    CurrencyKey = 98,
                    SalesTerritoryKey = rand.Next(1, 10),
                    SalesOrderNumber = "S" + rand.Next(111000, 199999).ToString(),
                    SalesOrderLineNumber = rand.Next(1, 10),
                    RevisionNumber = 2,
                    OrderQuantity = 1,
                    UnitPrice = products[productnumber].listprice,

                    ExtendedAmount = products[productnumber].listprice,
                    UnitPriceDiscountPct = 0.0f,
                    DiscountAmount = 0.0f,

                    ProductStandardCost = products[productnumber].standardcost,

                    TotalProductCost = products[productnumber].standardcost,
                    SalesAmount = products[productnumber].listprice,
                    TaxAmt = 0.0m,
                    Freight = 0.0m,
                    CarrierTrackingNumber = "",
                    CustomerPONumber = "",
                };
                var messageString = JsonConvert.SerializeObject(checkDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
               // message.Properties["messageType"] = "check";
                message.MessageId = Guid.NewGuid().ToString();

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending Check Data message: {1}", DateTime.Now, checkDataPoint.ProductKey);

                Task.Delay(10000).Wait();
            }
        }
        private static async void SendDeviceToCloudTelemetryAsync()
        {
            while (true)
            {
                var telemetryDataString = "Telemetry Data";
                var telemetryMessage = new Message(Encoding.ASCII.GetBytes(telemetryDataString));
                telemetryMessage.Properties["messageType"] = "telemetry";
                telemetryMessage.MessageId = Guid.NewGuid().ToString();

                await deviceClient.SendEventAsync(telemetryMessage);
                Console.WriteLine("{0} > Sending Telemetry message: {1}", DateTime.Now, telemetryMessage);

                Task.Delay(10000).Wait();
            }
        }
        private static async void SendDeviceToCloudSurveydataAsync()
        {
            while (true)
            {
                var surveyDataString = "Survey Data";
                var surveyMessage = new Message(Encoding.ASCII.GetBytes(surveyDataString));
                surveyMessage.Properties["messageType"] = "survey";
                surveyMessage.MessageId = Guid.NewGuid().ToString();

                await deviceClient.SendEventAsync(surveyMessage);
                Console.WriteLine("{0} > Sending Survey Data: {1}", DateTime.Now, surveyDataString);

                Task.Delay(10000).Wait();
            }
        }

        private static async void SendDeviceToCloudUXClickdataAsync()
        {
            while (true)
            {
                var UXClickdataString = "UX Click Data";
                var UXClickdataMessage = new Message(Encoding.ASCII.GetBytes(UXClickdataString));
                UXClickdataMessage.Properties["messageType"] = "ux";
                UXClickdataMessage.MessageId = Guid.NewGuid().ToString();

                await deviceClient.SendEventAsync(UXClickdataMessage);
                Console.WriteLine("{0} > Sending UX Click Data: {1}", DateTime.Now, UXClickdataString);

                Task.Delay(10000).Wait();
            }
        }

        private static  void GetProductInfo()
        {
            using (var connection = new QC.SqlConnection(
                    //"Server=tcp:dxretaildemosqlserver.database.windows.net,1433;Database=dxretaildemodw;User ID=YOUR_LOGIN_NAME_HERE;Password=YOUR_PASSWORD_HERE;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
                    "Server=tcp:dxretaildemosqlserver.database.windows.net,1433;Database=dxretaildemodw;User ID=tmathew1000;Password=blessing@1;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
                    ))
            {
                connection.Open();
                Console.WriteLine("Retrieved Products successfully.");

                Program.SelectRows(connection);


                connection.Close();
            }
        }

        static public void SelectRows(QC.SqlConnection connection)
        {

            using (var command = new QC.SqlCommand())
            {
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = @"  
    SELECT  
        *  
        FROM [dbo].[DimProduct]
        ORDER BY  
            [ProductKey] ; ";

                QC.SqlDataReader reader = command.ExecuteReader();
                Program.products = new List<product>();
                while (reader.Read())
                {
                    product x = new product();
                    x.productkey = Convert.ToInt32(reader["ProductKey"]);
                    string stdcost = reader["StandardCost"].ToString();
                    if (stdcost=="") x.standardcost = 0.0m;
                    else x.standardcost = Convert.ToDecimal(stdcost);
                    string listprice = reader["ListPrice"].ToString();
                    if (listprice == "") x.listprice = 0.0m;
                    else x.listprice = Convert.ToDecimal(listprice);
                    products.Add(x);                    

                }
            }

        }


    }

    public class product
    {
        public int productkey;
        public decimal standardcost;
        public decimal listprice;
    }
}
