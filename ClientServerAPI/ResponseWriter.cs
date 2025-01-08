using System.Text;

namespace ClientServerAPI {
    public sealed class ResponseWriter {
        private readonly MemoryStream _stream;

        public ResponseWriter()
            => _stream = new MemoryStream();

        public void WriteMessageType(in byte type)
            => _stream.WriteByte(type);

        public void WriteMessage(in string message) {
            int messageLength = message.Length;
            _stream.Write(BitConverter.GetBytes(messageLength));
            _stream.Write(Encoding.UTF8.GetBytes(message));
        }

        public byte[] GetResponseBytes()
            => _stream.ToArray();
    }
}