namespace Fan_Shutdown.Classes.logs
{
    internal class LoggerClass
    {
        //Create logfile log. file
        internal static string LogFile { get; set; } = $@"{AppDomain.CurrentDomain.BaseDirectory}Logs\{AppDomain.CurrentDomain.FriendlyName}_{DateTime.Now:yyyy-dd-M--HH-mm-ss}.log";

        internal static void WriteLine(string txt)
        {
            try
            {
                //Write to the logfile
                File.AppendAllText(LogFile, $"[{DateTime.Now}] : {txt}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ALERT! Could Not Append Text To Log File \n {ex.Message}");
                return;
            }
        }

        internal static void DeleteLog()
        {
            try
            {
                //Delete the log file
                File.Delete(LogFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ALERT! Could Not Delete Log File \n {ex.Message}");
                return;
            }
        }

    }
}
