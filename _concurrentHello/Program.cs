using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _concurrentHello
{
    using System;
    using System.Timers;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public class Program
    {
        private static Timer aTimer;
        private static DataGrid grid1;
        private static Random r = new Random();
        private static  bool Updated = false;
        private static string USER_IP_ADRESS = "83.249.122.120";

        public static void Main()
	{
		grid1 = new DataGrid();
		  aTimer = new System.Timers.Timer(4000);
        aTimer.Elapsed += OnTimedEvent;
        aTimer.Enabled = true;
		aTimer.AutoReset = true;
        aTimer.Start();
        Console.WriteLine("Initialized...");
		for(;/*Ever and ever*/;)
		{
            if (!Updated) continue;
		    Console.WriteLine("Updated at " + DateTime.Now);
		    var enumerable = grid1.ItemsSource as IEnumerable<String>;
		    if (enumerable == null) continue;
		    foreach (var url in enumerable)
		    {
		        Console.WriteLine(url);
		    }
            Console.WriteLine();
		    Updated = false;
		}
		
	}
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {

            var urlList = new List<String>(Search(r.Next(1, 100)));

            //Woop woop 8 trådar!
            var newList = urlList.AsParallel().Select(PrependHello);
            grid1.ItemsSource = newList;
            Updated = true;
        }
        private static IEnumerable<String> Search(int param)
        {

            using (var webClient = new System.Net.WebClient())
            {

                var json = webClient.DownloadString(String.Format("https://ajax.googleapis.com/ajax/services/search/web?q={0}&v=1.0&rsz=8&userip={1}", param,USER_IP_ADRESS));
                dynamic JsonDe = JsonConvert.DeserializeObject(json);
                if (JsonDe.responseStatus == "403")
                {
                    Console.WriteLine("Google spamblock this turn");
                    yield return "Blocked";
                    yield break;

                }

                foreach (dynamic url in JsonDe.responseData.results)
                {
                    yield return url.url;
                }
            }
        }

        //Hard labor!
        private static String PrependHello(string s)
        {
            return "Hello " + s;
        }


    }
    //Bogusklass
    public class DataGrid
    {
        public object ItemsSource { get; set; }
    }
			
}
