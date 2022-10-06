// See https://aka.ms/new-console-template for more information
using Fan_Shutdown;
using Fan_Shutdown.Classes;
using Fan_Shutdown.Classes.logs;
using System.Text.RegularExpressions;
using System.Timers;
using Timer = System.Timers.Timer;

string Workingdir = System.Reflection.Assembly.GetExecutingAssembly().Location;
string Logsdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
LogsDirClass.logsdir = Logsdir;
Console.Beep();
Console.WriteLine("----------------------------------------------");
Console.WriteLine(Workingdir);
Console.WriteLine("----------------------------------------------");
Console.WriteLine("*** APPLICATION START ***");
Console.WriteLine("----------------------------------------------");
AsciiClass.GetLogo();
Timer Timer = new(300000); //one hour in milliseconds 60 * 60 * 1000 //10 seconds 10000 //10 min 600000// 5 min 300000
Timer.Elapsed += new ElapsedEventHandler(LoopApp);
Timer.Start();//start at 09:00AM on the dot for accurate results.
AppMain(LogsDirClass.logsdir);
Console.ReadLine();

#region Timer loops ever x amount of time

static void LoopApp(object? source, ElapsedEventArgs e)
{
    if (LogsDirClass.logsdir is not null)
    {
        AppMain(LogsDirClass.logsdir);
    }
}
#endregion

#region Main application method

static void AppMain(string Logsdir)
{
    Console.WriteLine("----------------------------------------------");
    Console.WriteLine($"*** START CHECK TIME: {DateTime.Now}");
    Console.WriteLine("----------------------------------------------");
    CreateFolder(Logsdir);
    Task req = HTTP("https://loadshedding.eskom.co.za/LoadShedding/GetStatus");
    req.Wait();
    GetTimes(HttpResult.HttpResultString);
    Console.WriteLine("----------------------------------------------");
    Console.WriteLine($"*** END CHECK TIME: {DateTime.Now}");
    Console.WriteLine("----------------------------------------------");
}

#endregion

#region Creates the log folder

static void CreateFolder(string Logsdir)
{
    try
    {
        Directory.CreateDirectory($@"{Logsdir}");
        LoggerClass.WriteLine(" *** Application Start [MainForm] ***");
        LoggerClass.WriteLine(" *** CreateDirectory Success [App] ***");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ALERT! Create Folder Error \n {ex.Message}");
        LoggerClass.WriteLine(" *** Error:" + ex.Message + " [App] ***");
        return;
    }
}

#endregion

#region Main loop gets the load-shedding times

static void GetTimes(string Stage)
{
    try
    {
        int s = int.Parse(Stage) -1;
        if (s>1)
        {
            Console.Write($"Is There Load-shedding?:");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($" Yes");
            Console.ResetColor();
        }
        else
        {
            Console.Write($"Is There Load-shedding?:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" No");
            Console.ResetColor();
        }
        Console.Write($"What Stage Are We On?:");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"{s}");
        Console.ResetColor();
        Task req = HTTP($"https://loadshedding.eskom.co.za/LoadShedding/GetScheduleM/1036849/{s}/4/1");
        req.Wait();
        string[] lines = GetPlainTextFromHtml(HttpResult.HttpResultString).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        int n = 0;
        DateTime startTime = Convert.ToDateTime(DateTime.Now.ToString("HH:mm"));
        foreach (var i in lines.Skip(1))
        {
            if (!i.Any(x => char.IsLetter(x)))
            {
                if (n > 1)
                {
                    //do nothing because times belong to other days
                }
                else
                {
                    string[] time = RemoveWhitespace(i).Split('-', StringSplitOptions.RemoveEmptyEntries);
                    DateTime endTime = Convert.ToDateTime(time[0]);
                    TimeSpan span = endTime.Subtract(startTime);
                    TimeSpan toWarning = new TimeSpan(0, 5, 0);//set time here//5 min example
                    if (span >= TimeSpan.Zero && span <= toWarning)
                    {
                        //send shutdown command
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Shutdown Event: Sending shutdown call");
                        Console.ResetColor();
                        LoggerClass.WriteLine($"Shutdown Event: Sending shutdown call");

                    }
                    else
                    {
                        //do nothing because we are not within the time to start shutdown
                    }
                    Console.WriteLine($"Time To Warning: {toWarning} Before Load-shedding time");
                    Console.WriteLine($"Time To Load-shedding: {span}");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Time: {RemoveWhitespace(i)}");
                    Console.ResetColor();
                    LoggerClass.WriteLine($"Time: {RemoveWhitespace(i)}");
                }
            }
            else
            {
                n++;
                string[] strlist = RemoveWhitespace(i).Split(',', StringSplitOptions.RemoveEmptyEntries);
                string[] capsplit = SplitCamelCase(strlist[1]);
                string pcdate = DateTime.Now.ToString("dd MMM");
                string eskomdate = $"{capsplit[0]} {capsplit[1]}";

                if (pcdate == eskomdate)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"Today: {capsplit[0]}, {capsplit[1]}");
                    Console.ResetColor();
                    LoggerClass.WriteLine($"Today: {capsplit[0]}, {capsplit[1]}");
                }
                else
                {
                    //do nothing because date is not today
                }             
            }
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"ALERT! ParseHtml Error \n You can mostly ignore this as it triggers due to unused garbage text on the eskom HTML page, However check the 'ParseHtml Error' line in the log file for more details.");
        Console.ResetColor();
        LoggerClass.WriteLine(" *** ParseHtml Error:" + ex.Message + " [App] ***");
        return;
    }
}

#endregion

#region Https Task call

static async Task HTTP(string url)
{
    using HttpClient client = new();
    //Add Default Request Headers
    //client.DefaultRequestHeaders.Add("Authorization", "Bearer token");
    try
    {
        //http://loadshedding.eskom.co.za/LoadShedding/GetScheduleM/<suburb_id>/<stage>/<province_id>/1
        using HttpResponseMessage response = await client.GetAsync(new Uri($"{url}"));
        using HttpContent content = response.Content;
        string result = await content.ReadAsStringAsync();
        HttpResult.HttpResultString = result;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ALERT! HTTP Error \n {ex.Message}");
        LoggerClass.WriteLine(" *** Error:" + ex.Message + " [App] ***");
        return;
    }
}

#endregion

#region Gets plain text from html string via regex

static string GetPlainTextFromHtml(string htmlString)
{
    string htmlTagPattern = "<.*?>";
    var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
    htmlString = regexCss.Replace(htmlString, string.Empty);
    htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
    htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
    htmlString = htmlString.Replace("&nbsp;", string.Empty);

    return htmlString;
}

#endregion

#region Removes whitespace from a string

static string RemoveWhitespace(string str)
{
    return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
}

#endregion

#region Splits by capital letters

static string[] SplitCamelCase(string source)
{
    return Regex.Split(source, @"(?<!^)(?=[A-Z])");
}
#endregion

#region Parse html via the agility framework [not used for my example]

/*static void ParseHtml(string regx, CallsClass cc)
{
    try
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(regx);
        //var text = doc.DocumentNode.SelectSingleNode("//div[@class=\"scheduleDay\"]//div");
        var text = doc.DocumentNode.SelectNodes("//div[@class=\"scheduleDay\"]//div");
        foreach (HtmlNode node in text)
        {
            Console.WriteLine(RemoveWhitespace(node.InnerText));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ALERT! ParseHtml Error \n {ex.Message}");
        LoggerClass.WriteLine(" *** Error:" + ex.Message + " [App] ***");
        return;
    }
}*/
#endregion
