using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ProcessDeviceToCloudMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            string iotHubConnectionString = "HostName=dxretailiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=oxRpAjXmMKnm4IpOmTa3yd7Es9i8lpBbuVZWdT5hb7Q=";
            string iotHubEndpoint = "messages/events";
            StoreEventProcessor.StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=dxretailiotblobstorage;AccountKey=AU33H+vHQ9esYfED5e6BkCQ027a1IvbHfTJJYH8sLeNeOKY39Ey48M0COZfe3z0FSHUnlB0HnxZq1W7iD/Dh1Q==";
        //    StoreEventProcessor.ServiceBusConnectionString = "Endpoint=sb://tmd2ctutorial.servicebus.windows.net/;SharedAccessKeyName=send;SharedAccessKey=Ox3KSEZxdgtgdjOMqME2EbRVFqDHRU36Zs9wX9Jt7H8=;EntityPath=d2ctutorial"; //send connection string

            string eventProcessorHostName = Guid.NewGuid().ToString();
            EventProcessorHost eventProcessorHost = new EventProcessorHost(eventProcessorHostName, iotHubEndpoint, EventHubConsumerGroup.DefaultGroupName, iotHubConnectionString, StoreEventProcessor.StorageConnectionString, "checkdatacontainer");

            Console.WriteLine("Registering EventProcessor...");
            eventProcessorHost.RegisterEventProcessorAsync<StoreEventProcessor>().Wait();

            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();

        }
    }
}
