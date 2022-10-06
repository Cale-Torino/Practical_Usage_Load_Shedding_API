namespace Fan_Shutdown
{
    internal class HttpResult
    {
        //set or get logsdir variable globally
        internal static string httpResult = "";
        internal static string HttpResultString
        {
            get
            {
                return httpResult;
            }
            set
            {
                httpResult = value;
            }
        }
    }
}
