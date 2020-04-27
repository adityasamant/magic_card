using System;
using Dissonance.Networking;

namespace Dissonance.Integrations.PhotonBolt
{
    public class BoltServer
        : BaseServer<BoltServer, BoltClient, BoltPeer>, BoltDissonanceRelay.IBoltPacketListener, BoltDissonanceRelay.IBoltDisconnectListener
    {
        private readonly BoltDissonanceRelay _relay;

        public BoltServer(BoltDissonanceRelay relay)
        {
            _relay = relay;
        }

        public void PeerDisconnected(BoltPeer peer)
        {
            ClientDisconnected(peer);
        }

        public void PacketReceived(BoltPeer peer, ArraySegment<byte> data)
        {
            NetworkReceivedPacket(peer, data);
        }

        public override void Connect()
        {
            _relay.RegisterServerPacketListener(this);
            _relay.RegisterDisconnectListener(this);

            base.Connect();
        }

        public override void Disconnect()
        {
            _relay.UnregisterServerPacketListener(this);
            _relay.UnregisterDisconnectListener(this);

            base.Disconnect();
        }

        protected override void ReadMessages()
        {
            // messages are received in an event handler, so we don't need to do any work to read events
        }

        protected override void SendReliable(BoltPeer peer, ArraySegment<byte> packet)
        {
            _relay.SendReliable(peer, packet);
        }

        protected override void SendUnreliable(BoltPeer peer, ArraySegment<byte> packet)
        {
            _relay.SendUnreliable(peer, packet);
        }
    }
}
