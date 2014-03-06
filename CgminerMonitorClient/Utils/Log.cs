using System;

namespace CgminerMonitorClient.Utils
{
    public class Log
    {
        [Flags]
        public enum Level
        {
            Off = 0,
            Normal = 1,
            Verbose = 2
        }

        private static readonly object Lock = new object();

        private static Log _instance;

        private Level _level;

        private Log(Level level)
        {
            _level = Level.Off;
            _level = _level.Add(level);
        }

        public static Log Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Log(Level.Normal);
                        }
                    }
                }
                return _instance;
            }
        }

        public void ToggleLevel(Level level)
        {
            if (_level.Has(level))
                _level = _level.Remove(level);
            else
                _level = _level.Add(level);
        }

        public void Info(object message, Exception exception)
        {
            if (_level.HasFlag(Level.Normal))
                WriteToConsole(message + Environment.NewLine + exception);
        }

        public void Info(object message)
        {
            if (_level.HasFlag(Level.Normal))
                WriteToConsole(message.ToString());
        }

        public void InfoRaw(object message)
        {
            if (_level.HasFlag(Level.Normal))
                WriteToConsoleRaw(message.ToString());
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (_level.HasFlag(Level.Normal))
                WriteToConsole(string.Format(format, args));
        }

        public void Debug(object message, Exception exception)
        {
            if (_level.HasFlag(Level.Verbose))
                WriteToConsole(message + Environment.NewLine + exception);
        }

        public void Debug(object message)
        {
            if (_level.HasFlag(Level.Verbose))
                WriteToConsole(message.ToString());
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (_level.HasFlag(Level.Verbose))
                WriteToConsole(string.Format(format, args));
        }

        private static void WriteToConsole(string message)
        {
            Console.WriteLine("{0} {1}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.ff"), message);
        }

        private void WriteToConsoleRaw(string message)
        {
            Console.WriteLine(message);
        }
    }
}