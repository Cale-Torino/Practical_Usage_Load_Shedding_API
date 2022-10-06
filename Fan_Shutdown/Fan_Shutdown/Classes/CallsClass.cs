//using LoadShedding.NET.Clients;
//using LoadShedding.NET.Objects;
using System.Text.RegularExpressions;

namespace Fan_Shutdown.Classes
{
    internal class CallsClass
    {
        /*        internal async Task Check(EskomLoadSheddingClient ELSC)
                {
                    try
                    {
                        bool ils = await ELSC.IsLoadShedding();
                        Console.Write("Is there Load-Shedding?");
                        if (ils == true)
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine(" Yes");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(" No");
                            Console.ResetColor();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ALERT! Could not call Check() method \n {ex.Message}");
                        LoggerClass.WriteLine(" *** Error:" + ex.Message + " [App] ***");
                        return;
                    }
                }*/

        /*        internal async Task Getstats(EskomLoadSheddingClient ELSC)
                {
                    try
                    {
                        LoadShedding.NET.LoadSheddingStage ils = await ELSC.GetLoadSheddingStage();
                        Console.WriteLine($"We are currently on: {ils} ");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ALERT! Could not call Getstats() method \n {ex.Message}");
                        LoggerClass.WriteLine(" *** Error:" + ex.Message + " [App] ***");
                        return;
                    }
                }*/

        /*        internal async void GetMunicipalities(EskomLoadSheddingClient ELSC)
                {
                    try
                    {
                        List<Municipality> i = await ELSC.GetMunicipalities(LoadShedding.NET.Provinces.KwaZuluNatal);
                        foreach (var item in i)
                        {
                            Console.WriteLine($"{item.Name}, {item.ID}\n");
                        }

                        //Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ALERT! Could not call GetMunicipalities() method \n {ex.Message}");
                        LoggerClass.WriteLine(" *** Error:" + ex.Message + " [App] ***");
                        return;
                    }
                }*/

        /*        internal async void Get(EskomLoadSheddingClient ELSC)
                {
                    try
                    {
                        HttpResponseMessage i = await ELSC.Get("https://github.com/IsaTippens/LoadShedding.NET");
                        Console.WriteLine($"{i.Headers}\n");
                        //Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ALERT! Could not call Get() method \n {ex.Message}");
                        LoggerClass.WriteLine(" *** Error:" + ex.Message + " [App] ***");
                        return;
                    }
                }*/

        /*        internal async Task GetScheduleData(EskomLoadSheddingClient ELSC)
                {
                    try
                    {
                        string i = await ELSC.GetScheduleData(1061287, 4, 4);
                        Console.WriteLine($"{i}\n");
                        //Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ALERT! Could not call GetScheduleData() method \n {ex.Message}");
                        LoggerClass.WriteLine(" *** Error:" + ex.Message + " [App] ***");
                        return;
                    }
                }*/

        /*        internal async void GetSuburbsData(EskomLoadSheddingClient ELSC)
                {
                    try
                    {
                        Municipality m = new Municipality();
                        string i = await ELSC.GetSuburbsData(m);
                        Console.WriteLine($"{i}\n");
                        //Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ALERT! Could not call GetSuburbsData() method \n {ex.Message}");
                        LoggerClass.WriteLine(" *** Error:" + ex.Message + " [App] ***");
                        return;
                    }
                }*/

        public IEnumerable<string> Regex(string HTML)
        {
            List<string> hrefTags = new List<string>();
            //string linkpattern = @"(?inx)<a \s [^>]*href \s* = \s*(?<q> ['""] )(?<url> [^""]+ )\k<q>[^>]* >";
            string timepattern = @"<a.*?onclick=(""|')(?<onclick>.*?)(""|').*?>(?<url>.*?)</a>";

            Regex reHref = new Regex(timepattern);

            foreach (Match match in reHref.Matches(HTML))
            {
                //hrefTags.Add(match.Groups["url"].ToString());
                //hrefTags.Add(match.Groups["var2"].ToString());
                hrefTags.Add(match.Groups[0].ToString());
            }

            return hrefTags;
        }
    }
}
