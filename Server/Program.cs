using System.Net;
using System.Net.Sockets;

using ClientServerAPI;

namespace Server {
    internal sealed class Program {
        private static readonly List<Client> _users = [];
        private static readonly string _serverIpAddress = "127.0.0.1";
        private static TcpListener? _listener;
        private static readonly int _port = 6667;
        
        private static void Main() {
            _listener = new(IPAddress.Parse(_serverIpAddress), _port);
            _listener.Start();
            while (true) {
                Client client = new(_listener.AcceptTcpClient());
                _users.Add(client);
                BroadcastConnection();
            }
        }

        private static void BroadcastConnection() {
            foreach (var user in _users)
                foreach (var otherUser in _users) {
                    ResponseWriter packetToBroadcast = new();
                    packetToBroadcast.WriteMessageType((byte)MessageType.ClientConnected);
                    packetToBroadcast.WriteMessage(otherUser.Username);
                    packetToBroadcast.WriteMessage(otherUser.UniqueId.ToString());
                    user.ClientSocket.Client.Send(packetToBroadcast.GetResponseBytes());
                }
        }

        public static void BroadcastMessages(string message) {
            foreach (var user in _users) {
                ResponseWriter packetToBroadcast = new();
                packetToBroadcast.WriteMessageType((byte)MessageType.MessageReceived);
                packetToBroadcast.WriteMessage(message);
                user.ClientSocket.Client.Send(packetToBroadcast.GetResponseBytes());
            }
        }

        public static void BroadcastDisconnect(string uid) {
            Client? disconnected = _users.Where(some => some.UniqueId.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnected!);
            foreach (var user in _users) {
                ResponseWriter packetToBroadcast = new();
                packetToBroadcast.WriteMessageType((byte)MessageType.ClientDisconnected);
                packetToBroadcast.WriteMessage(uid);
                user.ClientSocket.Client.Send(packetToBroadcast.GetResponseBytes());
            }
            BroadcastMessages($"[{disconnected!.Username}] отключился!");
        }
    }
}