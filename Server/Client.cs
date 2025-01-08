using System.Net.Sockets;

using ClientServerAPI;

namespace Server {
    public sealed class Client {
        private readonly RequestReader _reader;

        public Client(TcpClient client) {
            (UniqueId, ClientSocket) = (Guid.NewGuid(), client);
            _reader = new(ClientSocket.GetStream());
            MessageType opcode = (MessageType)_reader.ReadByte();
            Username = _reader.ReadMessage();
            Console.WriteLine($"[{DateTime.Now}]: Клиент {Username} подключился!");
            Task.Run(Process);
        }

        public string Username { get; set; }
        public Guid UniqueId { get; set; }
        public TcpClient ClientSocket { get; set; }

        private void Process() {
            while (true) {
                try {
                    var type = (MessageType)_reader.ReadByte();
                    switch (type) {
                        case MessageType.MessageReceived:
                            string message = _reader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Получено сообщение от клиента {Username}.");
                            Program.BroadcastMessages($"{Username}: {message}");
                            break;
                    }
                }
                catch (Exception) {
                    Console.WriteLine($"[{DateTime.Now}]: Клиент {Username} отключился!");
                    Program.BroadcastDisconnect(UniqueId.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}