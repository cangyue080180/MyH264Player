using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyH264Player
{
    public class MyUdpServer : UDPServer
    {
        public event Action<object,MyCustomEventArgs> DataReceived;
        public MyUdpServer(int port) : base(port)
        {
        }

        protected override void PacketReceived(UDPPacketBuffer buffer)
        {
            byte[] receData = new byte[buffer.DataLength];
            Array.Copy(buffer.Data,receData,receData.Length);
            MyCustomEventArgs e = new MyCustomEventArgs() {Data=receData };
            if(DataReceived!=null)
            {
                DataReceived(this,e);
            }
        }

        protected override void PacketSent(UDPPacketBuffer buffer, int bytesSent)
        {

        }
        public void StartServer()
        {
            Start();
        }
        public void StopServer()
        {
            Stop();
        }
    }
    public class MyCustomEventArgs:EventArgs
    {
        public byte[] Data { get; set; }
    } 
}
