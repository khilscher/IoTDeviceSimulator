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

        //Obtain the device's connection string from Device Explorer and paste it here.
        //Sample:
        //static string deviceConnString = $"HostName=yourIoThub.azure-devices.net;DeviceId={deviceId};SharedAccessKey=0mNwS92zxxUOSP/gPgan7Sjnbkjh879875Ce9371jG0=";
        static string deviceConnString = "";

        static void Main(string[] args)
        {

            deviceClient = DeviceClient.CreateFromConnectionString(deviceConnString, TransportType.Amqp); //Can also use MQTT or HTTP

            SendD2CMessageAsync("NGTK2111_vol", "500001", 2200, 2205, "m3");
            SendD2CMessageAsync("NGTK2111_temp", "500002", 64.4, 65.3, "f");
            SendD2CMessageAsync("NGTK2111_pressure", "500003", 2800, 2801, "psi");

            SendD2CMessageAsync("NGTK2112_vol", "500004", 1803.4, 1803.6, "m3");
            SendD2CMessageAsync("NGTK2112_temp", "500005", 65.1, 65.3, "f");
            SendD2CMessageAsync("NGTK2111_pressure", "500006", 2830, 2831, "psi");

            SendD2CMessageAsync("CRTK8113_vol", "600001", 1603.1, 1603.6, "m3");
            SendD2CMessageAsync("CRTK8114_vol", "600002", 1107.1, 1107.7, "m3");


            Console.WriteLine("Press any key to stop...");

            Console.ReadLine();

            deviceClient.CloseAsync();

            deviceClient.Dispose();

        }

        private static async Task SendD2CMessageAsync(string displayName, string address, double low, double high, string engUnit)
        {

            while (true)
            {

                Random rnd = new Random();

                double value = rnd.NextDouble() * (high-low) + low;

                PayloadModel messagePayload = new PayloadModel();

                messagePayload.DisplayName = displayName;

                messagePayload.Address = address;

                messagePayload.Value = Math.Round(value, 2);

                messagePayload.EngUnit = engUnit;

                messagePayload.SourceTimestamp = DateTime.UtcNow;

                var messagePayloadInJson = JsonConvert.SerializeObject(messagePayload);

                var encodedMessage = new Message(Encoding.ASCII.GetBytes(messagePayloadInJson));

                await deviceClient.SendEventAsync(encodedMessage);

                Console.WriteLine($"Message sent: {messagePayloadInJson}");

                Task.Delay(60000).Wait(); //Send message every 5 seconds

            }

        }

        private class PayloadModel
        {

            public string DisplayName { get; set; }

            public string Address { get; set; }

            public double Value { get; set; }

            public string EngUnit { get; set; }

            public DateTime SourceTimestamp { get; set; }

        }

    }
}
