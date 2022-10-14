using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UDPongServer
{
    public class Pad
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public IPEndPoint IPEndPoint { get => iPEndPoint; set => iPEndPoint = value; }

        private IPEndPoint iPEndPoint;
        public Pad(IPEndPoint senderInfo)
        {
            iPEndPoint = senderInfo;
        }

    }
}
