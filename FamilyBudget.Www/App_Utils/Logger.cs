using System;
using log4net;
using log4net.Config;

namespace FamilyBudget.Www.App_Utils
{
    public class Logger
    {
        private static readonly ILog LogManagerLocal;

        static Logger()
        {
            XmlConfigurator.Configure();
            LogManagerLocal = LogManager.GetLogger("IP_Local");
        }

        public static bool IsDebugEnabled
        {
            get { return LogManagerLocal.IsDebugEnabled; }
        }

        public static void Debug(object message)
        {
            Debug(message, null);
        }

        public static void Debug(object message, Exception ex)
        {
            if (!LogManagerLocal.IsDebugEnabled)
            {
                return;
            }
            LogManagerLocal.Debug(message, ex);
        }

        public static void Info(object message)
        {
            Info(message, null);
        }

        public static void Info(object message, Exception ex)
        {
            LogManagerLocal.Info(message, ex);
        }

        public static void Warn(object message)
        {
            Warn(message, null);
        }

        public static void Warn(object message, Exception ex)
        {
            LogManagerLocal.Warn(message, ex);
        }

        public static void Error(object message)
        {
            Error(message, null);
        }

        public static void Error(object message, Exception ex)
        {
            LogManagerLocal.Error(message, ex);
        }

        public static void Fatal(object message)
        {
            Fatal(message, null);
        }

        public static void Fatal(object message, Exception ex)
        {
            LogManagerLocal.Fatal(message, ex);
        }
    }
}