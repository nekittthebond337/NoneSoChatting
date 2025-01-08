using System.Net.Sockets;
using System.Text;

namespace ClientServerAPI {
    public sealed class RequestReader(NetworkStream stream) : BinaryReader(stream, Encoding.UTF8) {
        private readonly NetworkStream _stream = stream;

        public string ReadMessage() {
            byte[] messageBuffer;
            int length = ReadInt32() * 2;
            messageBuffer = new byte[length];
            _stream.Read(messageBuffer, 0, length);
            string message = Encoding.UTF8.GetString(messageBuffer);
            return message;
        }
    }
}