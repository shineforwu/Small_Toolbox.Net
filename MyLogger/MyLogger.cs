/**
 * @Author: Shine Wu
 * @Date:   2025-02-18 13:25:05
 * @Last Modified by:   Shine Wu
 * @Last Modified time: 2025-02-20 22:10:17
 */

using System;
using System.IO;
using System.Text;

namespace SmallToolBox

{

    // 定义日志级别枚举
    public enum LogLevel
    {
        DEBUG = 10,
        INFO = 20,
        WARNING = 30,
        ERROR = 40,
        CRITICAL = 50,
        MUST = 100
    }

    // 日志记录器类，使用单例模式
    public sealed class MyLogger
    {
        public Action<string> Log_Acation = null;
        // 单例实例
        private static readonly Lazy<MyLogger> lazy = new Lazy<MyLogger>(() => new MyLogger());

        // 获取单例实例的公共属性
        public static MyLogger Instance { get { return lazy.Value; } }

        private string _baseFileName;

        private string _basePath;
        private string _logFile;
        private LogLevel _logLevel;
        private int _isToWrite;
        private string _moduleName;
        private int _num;
        private readonly object _fileLock = new object();

        // 私有构造函数，防止外部实例化
        private MyLogger(int isToWrite = 1, string filePath = "./Logs/", string logBaseName = "MyLog", string moduleName = "Default ", LogLevel logLevel = LogLevel.DEBUG)
        {
            _baseFileName = logBaseName;
            _basePath = filePath;
            _logFile = $"{_basePath}{_baseFileName}.log";
            _logLevel = logLevel;
            _isToWrite = isToWrite;
            _moduleName = moduleName;
            _num = 0;
        }

        public void SetWriteValue(int value)
        {
            _isToWrite = value;
        }


        // 设置模块名称
        public void SetModuleName(string moduleName)
        {
            _moduleName = moduleName;
        }

        // 设置日志基础名称
        public void SetLogBaseName(string logBaseName)
        {
            _baseFileName = logBaseName;
            _logFile = $"{logBaseName}.log";
        }

        // 记录日志的核心方法
        public void Log(LogLevel level, string moduleName, string message)
        {
            if ((int)level >= (int)_logLevel)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string levelName = level.ToString();
                StringBuilder logMessageBuilder = new StringBuilder();
                logMessageBuilder.Append($"{DateTime.Now} [{levelName}] [{moduleName}] {message}");
                Log_Acation?.Invoke(logMessageBuilder.ToString());
                if (_isToWrite == 1)
                {
                    lock (_fileLock)
                    {
                        EnsureLogFileExists();
                        using (StreamWriter file = File.AppendText(_logFile))
                        {
                            file.WriteLine(logMessageBuilder.ToString());
                            // file.Flush();
                            // file.Close();
                        }
                    }
                }
            }
        }

        #region log level
        // 记录 INFO 级别的日志
        public void Info(string message)
        {
            Info(_moduleName, message);
        }

        // 记录 INFO 级别的日志，并包含模块名称
        public void Info(string moduleName, string message)
        {
            Log(LogLevel.INFO, moduleName, message);
        }

        // 记录 DEBUG 级别的日志
        public void Debug(string message)
        {
            Debug(_moduleName, message);
        }

        // 记录 DEBUG 级别的日志，并包含模块名称
        public void Debug(string moduleName, string message)
        {
            Log(LogLevel.DEBUG, moduleName, message);
        }

        // 记录 WARNING 级别的日志
        public void Warning(string message)
        {
            Warning(_moduleName, message);
        }

        // 记录 WARNING 级别的日志，并包含模块名称
        public void Warning(string moduleName, string message)
        {
            Log(LogLevel.WARNING, moduleName, message);
        }

        // 记录 ERROR 级别的日志
        public void Error(string message)
        {
            Error(_moduleName, message);
        }

        // 记录 ERROR 级别的日志，并包含模块名称
        public void Error(string moduleName, string message)
        {
            Log(LogLevel.ERROR, moduleName, message);
        }

        // 记录 CRITICAL 级别的日志
        public void Critical(string message)
        {
            Critical(_moduleName, message);
        }

        // 记录 CRITICAL 级别的日志，并包含模块名称
        public void Critical(string moduleName, string message)
        {
            Log(LogLevel.CRITICAL, moduleName, message);
        }

        #endregion log level
        // 确保日志文件存在
        private void EnsureLogFileExists()
        {
            int num = _num;
            while (true)
            {
                string fileName = $"{_basePath}{_baseFileName}_{num}.log";
                if (!File.Exists(fileName))
                {
                    if (!Directory.Exists(_basePath))
                    {
                        Directory.CreateDirectory(_basePath);
                    }
                    using (StreamWriter file = File.CreateText(fileName))
                    {
                        file.WriteLine($"Log file created on {DateTime.Now}");
                    }
                    _logFile = fileName;
                    return;
                }
                else
                {
                    long fileSize = new FileInfo(fileName).Length;
                    if (fileSize > 1024 * 1024 * 1024)
                    {
                        _num++;
                        num++;
                    }
                    else
                    {
                        _logFile = fileName;
                        return;
                    }
                }
            }
        }
    }
}