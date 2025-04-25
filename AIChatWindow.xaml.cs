using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ChatSystem.Models;
using ChatSystem.Utilities;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ChatSystem
{
    public partial class AIChatWindow : Window
    {
        private readonly int _userId;
        private readonly string _username;
        private Dictionary<string, List<ChatMessage>> _chatSessions;
        private List<ChatListItem> _chatList;
        private string _currentChatId;
        private IAIModel _currentAIModel;
        private bool _isProcessing = false;
        public AIChatWindow(int userId, string username)
        {
            InitializeComponent();
            _userId = userId;
            _username = username;
            _chatSessions = new Dictionary<string, List<ChatMessage>>();
            _chatList = new List<ChatListItem>();
            LoadAIModels();
            _currentAIModel = AIModelFactory.CreateAIModel("TinyLlama");
            LoadChatSessions();
        }
        private void LoadAIModels()
        {
            AIModelComboBox.Items.Clear();
            var availableModels = AIModelFactory.GetAvailableModels();
            foreach (var model in availableModels)
            {
                AIModelComboBox.Items.Add(new ComboBoxItem { Content = model });
            }
            if (AIModelComboBox.Items.Count > 0)
            {
                AIModelComboBox.SelectedIndex = 0;
            }
        }
        private void LoadChatSessions()
        {
            var allMessages = CSVHandler.Instance.LoadChatHistory(_userId);
            _chatSessions = allMessages
                .GroupBy(m => m.ChatId)
                .ToDictionary(g => g.Key, g => g.ToList());
            var sortedChatIds = _chatSessions.Keys
                .OrderByDescending(chatId => _chatSessions[chatId].Max(m => m.Timestamp))
                .ToList();
            _chatList = sortedChatIds.Select(chatId =>
            {
                string title = _chatSessions[chatId].First().Title;
                return new ChatListItem { ChatId = chatId, Title = title };
            }).ToList();
            if (_chatList.Count == 0)
            {
                CreateNewChat();
            }
            else
            {
                ChatSessionsList.ItemsSource = _chatList;
                ChatSessionsList.SelectedIndex = 0;
                _currentChatId = _chatList[0].ChatId;
                UpdateChatDisplay();
            }
        }
        private string GetTitleFromMessage(string message)
        {
            string content = message.Trim();
            var words = content.Split(' ').Where(w => !string.IsNullOrEmpty(w)).ToArray();
            return string.Join(" ", words.Take(5)) + (words.Length > 5 ? "..." : "");
        }
        private void CreateNewChat()
        {
            int chatNumber = _chatList.Count + 1;
            _currentChatId = $"{_userId}_chat{chatNumber}";
            _chatSessions[_currentChatId] = new List<ChatMessage>();
            var newChatItem = new ChatListItem { ChatId = _currentChatId, Title = "New Chat" };
            _chatList.Insert(0, newChatItem);
            ChatSessionsList.ItemsSource = null;
            ChatSessionsList.ItemsSource = _chatList;
            ChatSessionsList.SelectedIndex = 0;
            ChatDisplay.Items.Clear();
        }
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isProcessing || string.IsNullOrWhiteSpace(MessageInput.Text)) return;
            _isProcessing = true;
            SendButton.IsEnabled = false;
            System.Diagnostics.Debug.WriteLine($"SendButton.IsEnabled set to: {SendButton.IsEnabled}");
            string userMessage = MessageInput.Text;
            ChatDisplay.Items.Add($"You: {userMessage}");
            string chatTitle = _chatSessions.ContainsKey(_currentChatId) && _chatSessions[_currentChatId].Any() ?
                              _chatSessions[_currentChatId].First().Title : GetTitleFromMessage(userMessage);
            ChatMessage userChatMessage = new ChatMessage
            {
                ChatId = _currentChatId,
                Timestamp = DateTime.Now,
                Message = userMessage,
                Sender = "You",
                AIModel = _currentAIModel.Name,
                Title = chatTitle
            };
            _chatSessions[_currentChatId].Add(userChatMessage);
            try
            {
                CSVHandler.Instance.SaveChatMessage(_userId, userChatMessage);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving user message: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                string response = await _currentAIModel.GetResponse(userMessage);
                string aiMessagePrefix = $"AI ({_currentAIModel.Name}): ";
                string fullAiMessage = aiMessagePrefix + response;

                int aiMessageIndex = ChatDisplay.Items.Add(aiMessagePrefix);
                ChatDisplay.ScrollIntoView(ChatDisplay.Items[aiMessageIndex]);

                string currentText = aiMessagePrefix;
                foreach (char c in response)
                {
                    currentText += c;
                    ChatDisplay.Items[aiMessageIndex] = currentText;
                    ChatDisplay.ScrollIntoView(ChatDisplay.Items[aiMessageIndex]);
                    await Task.Delay(50);
                }

                ChatMessage aiChatMessage = new ChatMessage
                {
                    ChatId = _currentChatId,
                    Timestamp = DateTime.Now,
                    Message = response,
                    Sender = "AI",
                    AIModel = _currentAIModel.Name,
                    Title = chatTitle
                };
                _chatSessions[_currentChatId].Add(aiChatMessage);
                CSVHandler.Instance.SaveChatMessage(_userId, aiChatMessage);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"AI response error: {ex.Message}\nStack Trace: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            var currentItem = _chatList.First(c => c.ChatId == _currentChatId);
            _chatList.Remove(currentItem);
            _chatList.Insert(0, currentItem);
            ChatSessionsList.ItemsSource = null;
            ChatSessionsList.ItemsSource = _chatList;
            ChatSessionsList.SelectedIndex = 0;
            MessageInput.Clear();
            _isProcessing = false;
            SendButton.IsEnabled = true;
            System.Diagnostics.Debug.WriteLine($"SendButton.IsEnabled set to: {SendButton.IsEnabled}");
        }
        private void NewChatButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNewChat();
        }
        private void DeleteChatButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChatSessionsList.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Please select a chat to delete.", "No Chat Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (System.Windows.MessageBox.Show("Are you sure you want to delete this chat?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            var selected = (ChatListItem)ChatSessionsList.SelectedItem;
            string chatIdToDelete = selected.ChatId;

            if (_chatList.Count == 1)
            {
                System.Windows.MessageBox.Show("Cannot delete the last chat. Create a new one first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _chatSessions.Remove(chatIdToDelete);
            _chatList.Remove(selected);
            CSVHandler.Instance.DeleteChat(_userId, chatIdToDelete);
            ChatSessionsList.ItemsSource = null;
            ChatSessionsList.ItemsSource = _chatList;
            if (chatIdToDelete == _currentChatId)
            {
                ChatSessionsList.SelectedIndex = 0;
                _currentChatId = _chatList[0].ChatId;
                UpdateChatDisplay();
            }
            else
            {
                ChatSessionsList.SelectedItem = _chatList.First(c => c.ChatId == _currentChatId);
            }
        }
        private void RenameChatButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChatSessionsList.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Please select a chat to rename.", "No Chat Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var selected = (ChatListItem)ChatSessionsList.SelectedItem;
            string newTitle = Microsoft.VisualBasic.Interaction.InputBox("Enter new chat title:", "Rename Chat", selected.Title);
            if (!string.IsNullOrEmpty(newTitle))
            {
                selected.Title = newTitle;
                var messages = _chatSessions[_currentChatId];
                foreach (var msg in messages)
                {
                    msg.Title = newTitle;
                    CSVHandler.Instance.UpdateChatMessage(_userId, msg);
                }
                ChatSessionsList.ItemsSource = null;
                ChatSessionsList.ItemsSource = _chatList;
                ChatSessionsList.SelectedItem = selected;
            }
        }
        private void ChatSessionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatSessionsList.SelectedItem == null) return;
            var selected = (ChatListItem)ChatSessionsList.SelectedItem;
            _currentChatId = selected.ChatId;
            UpdateChatDisplay();
        }
        private void UpdateChatDisplay()
        {
            ChatDisplay.Items.Clear();
            if (_chatSessions.ContainsKey(_currentChatId))
            {
                foreach (var msg in _chatSessions[_currentChatId])
                {
                    string displayText = msg.Sender == "You" ? $"You: {msg.Message}" : $"AI ({msg.AIModel}): {msg.Message}";
                    ChatDisplay.Items.Add(displayText);
                }
            }
        }
        private void AIModelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AIModelComboBox.SelectedItem == null) return;
            string selectedModel = ((ComboBoxItem)AIModelComboBox.SelectedItem).Content.ToString();
            if (selectedModel == "Select more")
            {
                var dialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Title = "Select the directory containing your AI models (each model should be in its own subdirectory):",
                    InitialDirectory = ConfigManager.GetModelsDirectory()
                };
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    ConfigManager.SetModelsDirectory(dialog.FileName);
                    LoadAIModels();
                    foreach (ComboBoxItem item in AIModelComboBox.Items)
                    {
                        if (item.Content.ToString() == "TinyLlama")
                        {
                            AIModelComboBox.SelectedItem = item;
                            _currentAIModel = AIModelFactory.CreateAIModel("TinyLlama");
                            return;
                        }
                    }
                    AIModelComboBox.SelectedIndex = 0;
                    _currentAIModel = AIModelFactory.CreateAIModel(((ComboBoxItem)AIModelComboBox.SelectedItem).Content.ToString());
                }
                else
                {
                    foreach (ComboBoxItem item in AIModelComboBox.Items)
                    {
                        if (item.Content.ToString() == "TinyLlama")
                        {
                            AIModelComboBox.SelectedItem = item;
                            _currentAIModel = AIModelFactory.CreateAIModel("TinyLlama");
                            return;
                        }
                    }
                    AIModelComboBox.SelectedIndex = 0;
                    _currentAIModel = AIModelFactory.CreateAIModel(((ComboBoxItem)AIModelComboBox.SelectedItem).Content.ToString());
                }
                return;
            }
            string modelPath = null;
            var modelDirPath = Path.Combine(ConfigManager.GetModelsDirectory(), selectedModel);
            if (Directory.Exists(modelDirPath))
            {
                modelPath = modelDirPath;
            }
            try
            {
                _currentAIModel = AIModelFactory.CreateAIModel(selectedModel, modelPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load model {selectedModel}: {ex.Message}", "Model Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                foreach (ComboBoxItem item in AIModelComboBox.Items)
                {
                    if (item.Content.ToString() == "TinyLlama")
                    {
                        AIModelComboBox.SelectedItem = item;
                        _currentAIModel = AIModelFactory.CreateAIModel("TinyLlama");
                        return;
                    }
                }
                AIModelComboBox.SelectedIndex = 0;
                _currentAIModel = AIModelFactory.CreateAIModel(((ComboBoxItem)AIModelComboBox.SelectedItem).Content.ToString());
            }
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
        private void ChatDisplay_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ChatDisplay.SelectedItem == null) return;
            string selectedMessage = ChatDisplay.SelectedItem.ToString();
            if (selectedMessage.StartsWith("You:"))
            {
                string edit = Microsoft.VisualBasic.Interaction.InputBox("Edit your message:", "Edit Message", selectedMessage.Substring(4));
                if (!string.IsNullOrEmpty(edit))
                {
                    var msgToEdit = _chatSessions[_currentChatId].First(m => m.Message == selectedMessage.Substring(4) && m.Sender == "You");
                    msgToEdit.Message = edit;
                    CSVHandler.Instance.UpdateChatMessage(_userId, msgToEdit);
                    ChatDisplay.Items[ChatDisplay.SelectedIndex] = $"You: {edit}";
                    SendEditedMessage(edit);
                }
            }
        }
        private async void SendEditedMessage(string editedMessage)
        {
            try
            {
                string response = await _currentAIModel.GetResponse(editedMessage);
                ChatDisplay.Items.Add($"AI ({_currentAIModel.Name}): {response}");
                ChatMessage responseMessage = new ChatMessage
                {
                    ChatId = _currentChatId,
                    Timestamp = DateTime.Now,
                    Message = response,
                    Sender = "AI",
                    AIModel = _currentAIModel.Name,
                    Title = _chatSessions[_currentChatId].First().Title
                };
                _chatSessions[_currentChatId].Add(responseMessage);
                CSVHandler.Instance.SaveChatMessage(_userId, responseMessage);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"AI response or save error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MessageInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendButton_Click(sender, e);
            }
        }
    }
    public class ChatListItem
    {
        public string ChatId { get; set; }
        public string Title { get; set; }
    }
}