using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChatSystem.Models;

namespace ChatSystem.Utilities
{
    public class CSVHandler
    {
        private static readonly CSVHandler _instance = new CSVHandler();
        private const string UserFilePath = "users.csv";
        private CSVHandler() { }
        public static CSVHandler Instance => _instance;
        private string GetChatFilePath(int userId) => $"chats_{userId}.csv";
        public List<User> LoadUsers()
        {
            List<User> users = new List<User>();
            if (File.Exists(UserFilePath))
            {
                var lines = File.ReadAllLines(UserFilePath).Skip(1);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 3)
                        users.Add(new User
                        {
                            UserID = int.Parse(parts[0].Trim()),
                            Username = parts[1].Trim(),
                            PasswordHash = parts[2].Trim()
                        });
                }
            }
            return users;
        }
        public void SaveUsers(List<User> users)
        {
            var lines = new List<string> { "UserID,Username,PasswordHash" };
            lines.AddRange(users.Select(u => $"{u.UserID},{u.Username},{u.PasswordHash}"));
            File.WriteAllLines(UserFilePath, lines);
        }
        public List<ChatMessage> LoadChatHistory(int userId)
        {
            string filePath = GetChatFilePath(userId);
            List<ChatMessage> history = new List<ChatMessage>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath).Skip(1);
                foreach (var line in lines)
                {
                    var parts = ParseCsvLine(line);
                    if (parts.Length == 6) 
                    {
                        history.Add(new ChatMessage
                        {
                            ChatId = parts[0].Trim(),
                            Timestamp = DateTime.Parse(parts[1].Trim()),
                            Message = parts[2].Replace("\\n", "\n").Trim('"'),
                            Sender = parts[3].Trim(),
                            AIModel = parts[4].Trim(),
                            Title = parts[5].Trim()
                        });
                    }
                }
            }
            return history;
        }
        public void SaveChatMessage(int userId, ChatMessage message)
        {
            string filePath = GetChatFilePath(userId);
            try
            {
                if (!File.Exists(filePath))
                {
                    File.WriteAllLines(filePath, new[] { "ChatID,Timestamp,Message,Sender,AIModel,Title" });
                }
                string escapedMessage = $"\"{message.Message.Replace("\"", "\"\"").Replace("\n", "\\n")}\"";
                string line = $"{message.ChatId},{message.Timestamp},{escapedMessage},{message.Sender},{message.AIModel},{message.Title}";
                File.AppendAllLines(filePath, new[] { line });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save chat message: {ex.Message}");
            }
        }
        public void UpdateChatMessage(int userId, ChatMessage updatedMessage)
        {
            string filePath = GetChatFilePath(userId);
            var allMessages = LoadChatHistory(userId);
            var updatedLines = new List<string> { "ChatID,Timestamp,Message,Sender,AIModel,Title" };
            foreach (var msg in allMessages)
            {
                if (msg.Timestamp == updatedMessage.Timestamp && msg.ChatId == updatedMessage.ChatId)
                {
                    string escapedMessage = $"\"{updatedMessage.Message.Replace("\"", "\"\"").Replace("\n", "\\n")}\"";
                    updatedLines.Add($"{updatedMessage.ChatId},{updatedMessage.Timestamp},{escapedMessage},{updatedMessage.Sender},{updatedMessage.AIModel},{updatedMessage.Title}");
                }
                else
                {
                    string escapedMessage = $"\"{msg.Message.Replace("\"", "\"\"").Replace("\n", "\\n")}\"";
                    updatedLines.Add($"{msg.ChatId},{msg.Timestamp},{escapedMessage},{msg.Sender},{msg.AIModel},{msg.Title}");
                }
            }
            File.WriteAllLines(filePath, updatedLines);
        }
        public void DeleteChat(int userId, string chatId)
        {
            string filePath = GetChatFilePath(userId);
            if (File.Exists(filePath))
            {
                var allMessages = LoadChatHistory(userId);
                var remainingMessages = allMessages.Where(m => m.ChatId != chatId).ToList();
                var updatedLines = new List<string> { "ChatID,Timestamp,Message,Sender,AIModel,Title" };
                updatedLines.AddRange(remainingMessages.Select(m =>
                {
                    string escapedMessage = $"\"{m.Message.Replace("\"", "\"\"").Replace("\n", "\\n")}\"";
                    return $"{m.ChatId},{m.Timestamp},{escapedMessage},{m.Sender},{m.AIModel},{m.Title}";
                }));
                File.WriteAllLines(filePath, updatedLines);
            }
        }
        private string[] ParseCsvLine(string line)
        {
            List<string> parts = new List<string>();
            bool inQuotes = false;
            string field = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"' && (i == 0 || line[i - 1] != '\\'))
                {
                    inQuotes = !inQuotes;
                    continue;
                }
                if (c == ',' && !inQuotes)
                {
                    parts.Add(field);
                    field = "";
                }
                else
                {
                    field += c;
                }
            }
            parts.Add(field);
            return parts.ToArray();
        }
    }
}