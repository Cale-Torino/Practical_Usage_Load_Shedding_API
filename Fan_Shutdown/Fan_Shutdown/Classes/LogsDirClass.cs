namespace Fan_Shutdown.Classes
{
    internal class LogsDirClass
    {
        //set or get logsdir variable globally
        internal static string logsdir = "";
        internal static string LogsDir
        {
            get
            {
                return logsdir;
            }
            set
            {
                logsdir = value;
            }
        }
    }
}
