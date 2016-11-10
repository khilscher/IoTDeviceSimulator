using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Azure.Devices.Client; //Add NuGet Pkg
using Newtonsoft.Json; //Add NuGet Pkg


namespace IoTDeviceSimulator
{
    class Program
    {
        static DeviceClient deviceClient;

        static string deviceConnString = "<insert device connection string from Device Explorer";

        static void Main(string[] args)
        {

            deviceClient = DeviceClient.CreateFromConnectionString(deviceConnString, TransportType.Amqp);

            while (true)
            {

                SendD2CMessageAsync();

                Thread.Sleep(10000);

            }

        }

        private static async void SendD2CMessageAsync()
        {
            Random rnd = new Random();

            decimal temp = rnd.Next(8, 40);

            decimal speed = rnd.Next(80, 120);

            Telemetry messagePayload = new Telemetry();

            messagePayload.sampleDateTimeUtc = DateTime.UtcNow;

            messagePayload.speed = speed;

            messagePayload.temp = temp;

            var messagePayloadInJson = JsonConvert.SerializeObject(messagePayload);

            var encodedMessage = new Message(Encoding.ASCII.GetBytes(messagePayloadInJson));

            await deviceClient.SendEventAsync(encodedMessage);

            Console.WriteLine("Message sent");
        }

        private class Telemetry
        {
            public DateTime sampleDateTimeUtc { get; set; }

            public decimal temp { get; set; }

            public decimal speed { get; set; }
        }

    }
}
