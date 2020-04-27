using System;
using Dissonance.Networking;

namespace Dissonance.Integrations.PhotonBolt
{
    public class BoltClient
        : BaseClient<BoltServer, BoltClient, BoltPeer>, BoltDissonanceRelay.IBoltPacketListener
    {
        private readonly BoltDissonanceRelay _relay;

        public BoltClient([NotNull] BoltCommsNetwork network, BoltDissonanceRelay relay)
            : base(network)
        {
            _relay = relay;
        }

        public void PacketReceived(BoltPeer server, ArraySegment<byte> data)
        {
            NetworkReceivedPacket(data);
        }

        public override void Connect()
        {
            _relay.RegisterClientPacketListener(this);

            Connected();
        }

        public override void Disconnect()
        {
            _relay.UnregisterClientPacketListener(this);

            base.Disconnect();
        }

        protected override void ReadMessages()
        {
            // messages are received in an event handler, so we don't need to do any work to read events
        }

        protected override void SendReliable(ArraySegment<byte> packet)
        {
            _relay.SendReliableToServer(packet);
        }

        protected override void SendUnreliable(ArraySegment<byte> packet)
        {
            _relay.SendUnreliableToServer(packet);
        }
    }
}
