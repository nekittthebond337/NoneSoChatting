using System.Net;
using System.Net.Sockets;

using ClientServerAPI;

namespace ClientApp.Net {
    internal sealed class Server {
        private readonly TcpClient _tcpClient;
        private readonly string _serverIpAddress = "127.0.0.1";
        private readonly int _port = 6667;

        internal Server()
            => _tcpClient = new TcpClient();

        public event Action? Connected;
        public event Action? MessageReceived;
        public event Action? Disconnected;

        public RequestReader? Reader { get; private set; }

        public void ConnectToServer(in string username) {
            if (!_tcpClient.Connected) {
                _tcpClient.Connect(IPAddress.Parse(_serverIpAddress), _port);
                Reader = new RequestReader(_tcpClient.GetStream());
                if (!string.IsNullOrEmpty(username)) {
                    ResponseWriter responseToSend = new();
                    responseToSend.WriteMessageType((byte)MessageType.ClientConnected);
                    responseToSend.WriteMessage(username);
                    _tcpClient.Client.Send(responseToSend.GetResponseBytes());
                }
                ReadRequests();
            }
        }

        public void SendMessageToServer(in string message) {
            ResponseWriter responseMessage = new();
            responseMessage.WriteMessageType((byte)MessageType.MessageReceived);
            responseMessage.WriteMessage(message);
            _tcpClient.Client.Send(responseMessage.GetResponseBytes());
        }

        private void ReadRequests() {
            Task.Run(() => {
                while (true) {
                    MessageType messageType = (MessageType)Reader!.ReadByte();
                    switch (messageType) {
                        case MessageType.ClientConnected:
                            Connected?.Invoke();
                            break;
                        case MessageType.MessageReceived:
                            MessageReceived?.Invoke();
                            break;
                        case MessageType.ClientDisconnected:
                            Disconnected?.Invoke();
                            break;
                    }
                }
            });
        }
    }
}