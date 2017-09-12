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
        static string deviceConnString = "HostName=KHIoTHub.azure-devices.net;DeviceId=Device1;SharedAccessKey=0mNwS92zxxUOSP/gPgan7S1bh0gn88umm5Ce9371jG0=";

        static void Main(string[] args)
        {

            deviceClient = DeviceClient.CreateFromConnectionString(deviceConnString, TransportType.Amqp); //Can also use MQTT or HTTP

            SendD2CMessageAsync();

            Console.WriteLine("Press any key to stop...");

            Console.ReadLine();

            deviceClient.CloseAsync();

            deviceClient.Dispose();

        }

        private static async Task SendD2CMessageAsync()
        {

            while (true)
            {

                Random rnd = new Random();

                double temp = rnd.Next(28, 30); //Celcius

                double flow = rnd.Next(200, 210); //Cubic meters per second

                double pressure = rnd.Next(110, 120); //PSI

                PayloadModel messagePayload = new PayloadModel();

                messagePayload.deviceId = deviceId;

                messagePayload.sampleDateTimeUtc = DateTime.UtcNow;

                messagePayload.flow = flow;

                messagePayload.temp = temp;

                messagePayload.pressure = pressure;

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

            public double flow { get; set; }

            public double pressure { get; set; }

        }

    }
}
