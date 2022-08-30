using System.ComponentModel;
using System.Reflection;

namespace CommonLog
{
    public sealed class LogWriter
    {
        private const long MaxFileSize = 50*1024*1024;
        private static readonly Lazy<LogWriter> Lazy = new Lazy<LogWriter> (() => new LogWriter());

        public static LogWriter Instance { get { return Lazy.Value; } }

        public async void LogAsync(object caller, LogType logType, string message)
        {
            var nameSpace = GetNameSpace(caller);

            var filepath = GetFilePath(nameSpace, logType);
            var formattedMessage = GetFormattedMessage(message);
            if (File.Exists(filepath))
            {
                using var writer = File.AppendText(filepath);
                await writer.WriteLineAsync(formattedMessage);
            }
            else
            {
                using var writer = File.CreateText(filepath);
                await writer.WriteLineAsync(formattedMessage);
            }
        }

        private string GetNameSpace(object caller)
        {
            var nameSpace = caller.GetType().Namespace;
            if (nameSpace.Contains('.'))
            {
                nameSpace = nameSpace.Split('.')[0];
            }

            return nameSpace;
        }
        private string GetFormattedMessage(string message)
        {
            string date = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff zzz]");
            string formattedMessage = string.Join(" ", new string[] { date, message });
            return formattedMessage;
        }

        private string GetFilePath(string nameSpace, LogType logType)
        {
            var iteration = 0;
            string filetype = Enum.GetName(logType);
            var directory = $"/var/log/{nameSpace}";
            Directory.CreateDirectory(directory);

            var filepath = $"{directory}/{filetype}.log.{iteration}";
            var isFileReady = false;

            while (!isFileReady)
            {
                filepath = $"{directory}/{filetype}.log.{iteration}";
                if (File.Exists(filepath))
                {
                    long size = new FileInfo(filepath).Length;
                    if (size > MaxFileSize)
                    {
                        ++iteration;
                        continue;
                    }
                }

                isFileReady = true;
            }

            return filepath;
        }
    }
}