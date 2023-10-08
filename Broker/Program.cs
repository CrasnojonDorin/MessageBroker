using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
namespace Broker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Broker");

            BrokerSocket socket = new BrokerSocket();
            socket.Start(Settings.BROKER_IP, Settings.BROKER_PORT);

            var worker= new Worker();
            Task.Factory.StartNew(worker.DoSendMessageWork,TaskCreationOptions.LongRunning);

            Console.ReadLine();
        }
    }
}
