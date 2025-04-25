using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ChatSystem.Models;

namespace ChatSystem
{
    public partial class LoginWindow : Window
    {
        private List<User> _users = new List<User>();
        private string _generatedPassKey;
        private DateTime _passKeyTimestamp;
        private DispatcherTimer _cooldownTimer;
        private int _cooldownSeconds = 30;
        private const string BotToken = "7122088475:AAGB61sykz1wnmTfxxC1vSJ83VrDCQEtE7k";
        private const string ChatId = "5462002868";

        public LoginWindow()
        {
            InitializeComponent();
            InitializeCooldownTimer();
            LoadUsers();
        }
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                ResetTextInputs(this);
            }
        }
        private void ResetTextInputs(DependencyObject parent)
        {
            foreach (object child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is TextBox textbox)
                {
                    textbox.Text = string.Empty;
                }
                else if (child is PasswordBox passwordBox)
                {
                    passwordBox.Password = string.Empty;
                }
                else if (child is DependencyObject depObj)
                {
                    ResetTextInputs(depObj);
                }
            }
        }
        private void InitializeCooldownTimer()
        {
            _cooldownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _cooldownTimer.Tick += CooldownTimer_Tick;
        }

        private void LoadUsers() => _users = Utilities.CSVHandler.Instance.LoadUsers();

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordBox.Password;
            User user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                AIChatWindow chatWindow = new AIChatWindow(user.UserID, user.Username);
                chatWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignUpConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string username = signUpUsernameTextBox.Text;
            string password = signUpPasswordBox.Password;
            string enteredPassKey = passKeyTextBox.Text;

            if (enteredPassKey == _generatedPassKey && DateTime.Now.Subtract(_passKeyTimestamp).TotalSeconds <= 30)
            {
                SaveNewUser(username, password);
                MessageBox.Show("Sign up successful. You can now log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ResetTextInputs(this);
            }
            else
            {
                MessageBox.Show("Incorrect or expired passkey.", "Sign-up Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveNewUser(string username, string password)
        {
            int newUserId = _users.Any() ? _users.Max(u => u.UserID) + 1 : 1;
            User newUser = new User
            {
                UserID = newUserId,
                Username = username,
                PasswordHash = HashPassword(password)
            };
            _users.Add(newUser);
            Utilities.CSVHandler.Instance.SaveUsers(_users);
        }

        private void SendKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cooldownTimer.IsEnabled) return;
            if (string.IsNullOrWhiteSpace(signUpUsernameTextBox.Text) || string.IsNullOrWhiteSpace(signUpPasswordBox.Password))
            {
                MessageBox.Show("Please fill both username and password fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            GeneratePassKey();
            SendGeneratedPassKeyToTelegramBot(signUpUsernameTextBox.Text, _generatedPassKey);
            _cooldownTimer.Start();
        }

        private void GeneratePassKey()
        {
            _generatedPassKey = new Random().Next(1000, 9999).ToString();
            _passKeyTimestamp = DateTime.Now;
        }

        private void SendGeneratedPassKeyToTelegramBot(string username, string passkey)
        {
            string message = $"Registrant Name:{Environment.NewLine}<code>{username}</code>{Environment.NewLine}Generated key:{Environment.NewLine}<code>{passkey}</code>";
            string url = $"https://api.telegram.org/bot{BotToken}/sendMessage?chat_id={ChatId}&text={Uri.EscapeDataString(message)}&parse_mode=HTML";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.DownloadString(url);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Telegram error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CooldownTimer_Tick(object sender, EventArgs e)
        {
            _cooldownSeconds--;
            if (_cooldownSeconds <= 0)
            {
                _cooldownTimer.Stop();
                _cooldownSeconds = 30;
                SendKeyButton.Content = "Send Key";
            }
            else
            {
                SendKeyButton.Content = _cooldownSeconds.ToString();
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string password, string storedHash) => HashPassword(password) == storedHash;
    }
}