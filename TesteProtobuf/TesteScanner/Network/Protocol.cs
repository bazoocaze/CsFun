using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteScanner.Bytes;
using TesteScanner.Protos;

namespace TesteScanner.Network
{
    public class Protocol
    {
        public static void ServerPacketReceiver(Client client, ByteStream packetStream)
        {
            ScannerMsgPB msg = new ScannerMsgPB();
            msg.MergeFrom(new CodedInputStream(packetStream));

            if (msg.TipoCase != ScannerMsgPB.TipoOneofCase.Req) return;
            switch(msg.Req.TipoCase)
            {
                case ScannerRequestPB.TipoOneofCase.Ping:
                    client.Send(NewPongMsg());
                    break;
            }
        }

        public static void ClientPacketReceiver(Client client, ByteStream packetStream)
        {

        }

        public static ScannerMsgPB NewPingMsg()
        {
            ScannerMsgPB ret = new ScannerMsgPB();
            ret.Req = new ScannerRequestPB();
            ret.Req.Ping = new ScannerRequestPingPB();
            return ret;
        }

        public static ScannerMsgPB NewPongMsg()
        {
            ScannerMsgPB ret = new ScannerMsgPB();
            ret.Resp = new ScannerResponsePB();
            ret.Resp.Pong = new ScannerResponsePongPB();
            return ret;
        }
    }
}
