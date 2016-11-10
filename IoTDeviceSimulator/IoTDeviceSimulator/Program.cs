using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client; //Add NuGet Pkg
using Newtonsoft.Json; //Add NuGet Pkg

//Tutorial https://azure.microsoft.com/en-us/documentation/articles/iot-hub-csharp-csharp-getstarted/

namespace IoTDeviceSimulator
{
    class Program
    {
        static DeviceClient deviceClient;

        //Simulated device name. Register this name in IoT Hub using Device Explorer.
        static string deviceId = "Device1";

        //Obtain the device's connection string from Device Explorer and paste it here.
        //Sample:
        //static string deviceConnString = $"HostName=yourIoThub.azure-devices.net;DeviceId={deviceId};SharedAccessKey=0mNwS92zxxUOSP/gPgan7Sjnbkjh879875Ce9371jG0=";
        static string deviceConnString = "<insert device connection string from Device Explorer>";

        static void Main(string[] args)
        {

            deviceClient = DeviceClient.CreateFromConnectionString(deviceConnString, TransportType.Amqp); //Can also use MQTT or HTTP

            SendD2CMessageAsync();

            Console.WriteLine("Press any key to stop...");

            Console.ReadLine();

            deviceClient.CloseAsync();

            deviceClient.Dispose();

        }

        private static async void SendD2CMessageAsync()
        {

            while (true)
            {

                Random rnd = new Random();

                double temp = rnd.Next(8, 40);

                double speed = rnd.Next(80, 120);

                PayloadModel messagePayload = new PayloadModel();

                messagePayload.deviceId = deviceId;

                messagePayload.sampleDateTimeUtc = DateTime.UtcNow;

                messagePayload.speed = speed;

                messagePayload.temp = temp;

                var messagePayloadInJson = JsonConvert.SerializeObject(messagePayload);

                var encodedMessage = new Message(Encoding.ASCII.GetBytes(messagePayloadInJson));

                await deviceClient.SendEventAsync(encodedMessage);

                Console.WriteLine($"Message sent: {messagePayloadInJson}");

                Task.Delay(5000).Wait(); //Send message every 5 seconds

            }

        }

        private class PayloadModel
        {

            public string deviceId { get; set; }

            public DateTime sampleDateTimeUtc { get; set; }

            public double temp { get; set; }

            public double speed { get; set; }

        }

    }
}
