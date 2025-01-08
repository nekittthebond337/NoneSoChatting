using System.Collections.ObjectModel;
using System.Windows;

using ClientApp.MVVM.Core;
using ClientApp.MVVM.Model;
using ClientApp.Net;

namespace ClientApp.MVVM.ViewModel {
    public class MainViewModel {
        private readonly Server _server;

        public MainViewModel() {
            (Users, Messages) = ([], []);
            _server = new();
            _server.Connected += UserConnected;
            _server.MessageReceived += UserMessageReceived;
            _server.Disconnected += UserDisconnected;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username!), req => !string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message!), req => !string.IsNullOrEmpty(Message));
        }

        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public string? Username { get; set; }
        public string? Message { get; set; }

        private void UserConnected() {
            UserModel user = new() {
                Username = _server.Reader!.ReadMessage(),
                UniqueId = _server.Reader.ReadMessage()
            };
            if (!Users.Any(some => some.UniqueId == user.UniqueId))
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
        }

        private void UserMessageReceived() {
            string message = _server.Reader!.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }

        private void UserDisconnected() {
            string uid = _server.Reader!.ReadMessage();
            UserModel? user = Users.Where(x => x.UniqueId == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user!));
        }
    }
}