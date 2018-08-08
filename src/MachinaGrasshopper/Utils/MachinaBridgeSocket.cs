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
        public List<string> receivedMessages = new List<string>();
        public int logged = 0;

        public int minCount = 6;
        public int maxCount = 24;

        public MachinaBridgeSocket(string socketName) {
            this.name = socketName;
        }

        public void Log(string msg)
        {
            receivedMessages.Add(msg);
            logged++;

            //ResizeBuffer();
        }

        public void Flush()
        {
            receivedMessages.Clear();
            logged = 0;
        }

        private bool ResizeBuffer()
        {
            if (receivedMessages.Count > maxCount)
            {
                receivedMessages = receivedMessages.GetRange(receivedMessages.Count - minCount - 1, minCount);
                logged = receivedMessages.Count;
                return true;
            }
            return false;
        }




    }
}
