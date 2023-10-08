using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broker
{
    class PayloadHandler
    {
        public static void Handle(byte[] payloadBytes,ConnectionInfo connectionInfo)
        {
            var payloadString = Encoding.UTF8.GetString(payloadBytes);

            if (payloadString.StartsWith("subscribe#"))
            {
                connectionInfo.Topic = payloadString.Split(new string[] { "subscribe#" }, StringSplitOptions.None).LastOrDefault();
                //adaugam conectiunea in storage
                ConnectionsStorage.Add(connectionInfo);
            }
            else
            {
                Payload payload= JsonConvert.DeserializeObject<Payload>(payloadString);
                //adaugam in storage
                PayloadStorage.Add(payload);
                

            }

        }
    }
}
