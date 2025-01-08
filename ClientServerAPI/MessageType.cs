namespace ClientServerAPI {
    public enum MessageType : byte {
        ClientConnected = 1,
        MessageReceived = 2,
        ClientDisconnected = 4
    }
}