using System;
using System.Collections.Generic;
using System.Text;

namespace GamingClub
{
    public class FileLogger : IClubLogger
    {
        private readonly string _filePath;

        public FileLogger(string filePath = "club_history.log")
        {
            _filePath = filePath;
        }

        public void LogInfo(string message) => Write("INFO", message);
        public void LogWarning(string message) => Write("WARN", message);
        public void LogError(string message) => Write("ERROR", message);

        private void Write(string level, string message)
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            File.AppendAllText(_filePath, entry + Environment.NewLine);
        }
    }
}
