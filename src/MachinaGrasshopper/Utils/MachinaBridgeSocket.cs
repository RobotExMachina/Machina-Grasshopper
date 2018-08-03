using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;

namespace MachinaGrasshopper.Utils
{
    /// <summary>
    /// A quick wrapper class that incorporates a log of received messages...
    /// </summary>
    public class MachinaBridgeSocket
    {
        public WebSocket socket;
        public string name;
        public List<string> receivedMessage = new List<string>();

        public MachinaBridgeSocket(string socketName) {
            this.name = socketName;
        }

        public void Log(string msg)
        {
            receivedMessage.Add(msg);
        }

        public void Flush()
        {
            receivedMessage.Clear();
        }

    }
}
