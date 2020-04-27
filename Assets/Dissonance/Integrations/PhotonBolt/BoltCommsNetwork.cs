using System;
using System.Collections.Generic;
using Bolt;
using Dissonance.Networking;
using UdpKit;
using UnityEngine;

namespace Dissonance.Integrations.PhotonBolt
{
    [BoltGlobalBehaviour]
    public class BoltDissonanceChannels
        : GlobalEventListener
    {
        public static UdpChannelName UnreliableChannelToServer;
        public static UdpChannelName UnreliableChannelToClient;

        public override void BoltStartBegin()
        {
            UnreliableChannelToServer = BoltNetwork.CreateStreamChannel("DissonanceVoiceToServer", UdpChannelMode.Unreliable, 1);
            UnreliableChannelToClient = BoltNetwork.CreateStreamChannel("DissonanceVoiceToClient", UdpChannelMode.Unreliable, 1);
        }
    }

    public class BoltDissonanceRelay
        : GlobalEventListener
    {
        #region Nested Types

        public interface IBoltPacketListener
        {
            void PacketReceived(BoltPeer peer, ArraySegment<byte> data);
        }

        public interface IBoltDisconnectListener
        {
            void PeerDisconnected(BoltPeer peer);
        }

        #endregion

        private static readonly Log Log = Logs.Create(LogCategory.Network, typeof(BoltDissonanceRelay).Name);
        private static readonly IEqualityComparer<UdpChannelName> ChannelComparer = UdpChannelName.EqualityComparer.Instance;
        private readonly List<IBoltPacketListener> _clientPacketListeners = new List<IBoltPacketListener>();
        private readonly List<IBoltPacketListener> _serverPacketListeners = new List<IBoltPacketListener>();
        private readonly List<IBoltDisconnectListener> _disconnectListeners = new List<IBoltDisconnectListener>();

        /// <summary>
        ///     Registers a listener which listens to packets addressed to clients.
        /// </summary>
        public void RegisterClientPacketListener(IBoltPacketListener listener)
        {
            _clientPacketListeners.Add(listener);
        }

        /// <summary>
        ///     Registers a listener which listens to packets addressed to the server.
        /// </summary>
        public void RegisterServerPacketListener(IBoltPacketListener listener)
        {
            _serverPacketListeners.Add(listener);
        }

        public void RegisterDisconnectListener(IBoltDisconnectListener listener)
        {
            _disconnectListeners.Add(listener);
        }

        /// <summary>
        ///     Unregisters a listener which listens to packets addressed to clients.
        /// </summary>
        public bool UnregisterClientPacketListener(IBoltPacketListener listener)
        {
            return _clientPacketListeners.Remove(listener);
        }

        /// <summary>
        ///     Unregisters a listener which listens to packets addressed to the server.
        /// </summary>
        public bool UnregisterServerPacketListener(IBoltPacketListener listener)
        {
            return _serverPacketListeners.Remove(listener);
        }

        public bool UnregisterDisconnectListener(IBoltDisconnectListener listener)
        {
            return _disconnectListeners.Remove(listener);
        }

        public override void Disconnected(BoltConnection connection)
        {
            foreach (var listener in _disconnectListeners)
                listener.PeerDisconnected(new BoltPeer(connection));
        }

        public override void StreamDataReceived(BoltConnection connection, UdpStreamData data)
        {
            if (ChannelComparer.Equals(data.Channel, BoltDissonanceChannels.UnreliableChannelToClient))
                SendPacketReceived(new BoltPeer(connection), new ArraySegment<byte>(data.Data), _clientPacketListeners);

            if (ChannelComparer.Equals(data.Channel, BoltDissonanceChannels.UnreliableChannelToServer))
                SendPacketReceived(new BoltPeer(connection), new ArraySegment<byte>(data.Data), _serverPacketListeners);
        }

        public override void OnEvent([NotNull] DissonanceToServer evnt)
        {
            SendPacketReceived(new BoltPeer(evnt.RaisedBy), new ArraySegment<byte>(evnt.BinaryData), _serverPacketListeners);
        }

        public override void OnEvent([NotNull] DissonanceToClient evnt)
        {
            SendPacketReceived(new BoltPeer(evnt.RaisedBy), new ArraySegment<byte>(evnt.BinaryData), _clientPacketListeners);
        }

        private void SendPacketReceived(BoltPeer sender, ArraySegment<byte> data, List<IBoltPacketListener> listeners)
        {
            foreach (var listener in listeners)
                listener.PacketReceived(sender, data);
        }

        public void SendReliableToServer(ArraySegment<byte> data)
        {
            if (BoltNetwork.IsServer)
                SendPacketReceived(BoltPeer.Local, data, _serverPacketListeners);
            else
            {
                var packet = DissonanceToServer.Create(GlobalTargets.OnlyServer);
                packet.BinaryData = ToDirectArray(data);
                packet.Send();
            }
        }

        public void SendUnreliableToServer(ArraySegment<byte> data)
        {
            if (BoltNetwork.IsServer)
                SendPacketReceived(BoltPeer.Local, data, _serverPacketListeners);
            else
                BoltNetwork.Server.StreamBytes(BoltDissonanceChannels.UnreliableChannelToServer, ToDirectArray(data));
        }

        public void SendReliable(BoltPeer destination, ArraySegment<byte> data)
        {
            if (BoltPeer.Local.Equals(destination))
                SendPacketReceived(BoltPeer.Local, data, _clientPacketListeners);
            else
            {
                var packet = DissonanceToClient.Create(destination.Connection);
                packet.BinaryData = ToDirectArray(data);
                packet.Send();
            }
        }

        public void SendUnreliable(BoltPeer destination, ArraySegment<byte> data)
        {
            if (BoltPeer.Local.Equals(destination))
                SendPacketReceived(BoltPeer.Local, data, _clientPacketListeners);
            else
                destination.Connection.StreamBytes(BoltDissonanceChannels.UnreliableChannelToClient, ToDirectArray(data));
        }

        private byte[] ToDirectArray(ArraySegment<byte> segment)
        {
            if (segment.Count == segment.Array.Length)
                return segment.Array;

            var array = new byte[segment.Count];
            Array.Copy(segment.Array, array, segment.Count);
            return array;
        }
    }

    public struct BoltPeer
        : IEquatable<BoltPeer>
    {
        public static readonly BoltPeer Local = new BoltPeer(null);

        public BoltConnection Connection { get; set; }

        public bool RepresentsLocalConnection
        {
            get { return Connection == null; }
        }

        public BoltPeer(BoltConnection connection) : this()
        {
            Connection = connection;
        }

        public bool Equals(BoltPeer other)
        {
            if (Equals(Connection, other.Connection))
                return true;

            if (Connection == null || other.Connection == null)
                return false;

            return Connection.ConnectionId == other.Connection.ConnectionId;
        }
    }

    [RequireComponent(typeof(BoltDissonanceRelay))]
    public class BoltCommsNetwork
        : BaseCommsNetwork<BoltServer, BoltClient, BoltPeer, Unit, Unit>
    {
        private BoltDissonanceRelay _relay;

        public void Awake()
        {
            _relay = GetComponent<BoltDissonanceRelay>() ?? gameObject.AddComponent<BoltDissonanceRelay>();
        }

        protected override BoltServer CreateServer(Unit serverParameters)
        {
            return new BoltServer(_relay);
        }

        protected override BoltClient CreateClient(Unit clientParameters)
        {
            return new BoltClient(this, _relay);
        }

        protected override void Update()
        {
            if (IsInitialized)
            {
                if (BoltNetwork.IsConnected)
                {
                    if (BoltNetwork.IsServer)
                    {
                        if (Mode != NetworkMode.Host)
                            RunAsHost(Unit.None, Unit.None);
                    }
                    else
                    {
                        if (Mode != NetworkMode.Client)
                            RunAsClient(Unit.None);
                    }
                }
                else
                {
                    if (Mode != NetworkMode.None)
                        Stop();
                }
            }


            base.Update();
        }
    }
}
