using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly List<Info> Infos;
        private static readonly string OutputPath;

        static Program()
        {
            Infos = new List<Info>();
            var ouputDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            OutputPath = Path.Combine(ouputDir, "ouput.txt");
        }

        private static async Task Main()
        {
            //设置Cookies
            var cookies = new Dictionary<string, string>
            {
                ["JSESSIONID"] = "0BB08F5C35FCC330F3B3A749B3ACA2D1",
                ["XSRF-TOKEN"] = "f32d485b-6d39-4fa0-a2f9-dd450c6a9b69"
            };

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            var httpClient = new HttpClient(handler);
            foreach (var cookie in cookies)
            {
                var c = new Cookie(cookie.Key, cookie.Value, "/", "tm.bnuz.edu.cn");
                handler.CookieContainer.Add(c);
            }

            var places = new List<string>
            {
                "http://tm.bnuz.edu.cn/api/place/buildings/%E4%B8%BD%E6%B3%BD%E6%A5%BC/places",
                "http://tm.bnuz.edu.cn/api/place/buildings/%E4%B9%90%E8%82%B2%E6%A5%BC/places",
                "http://tm.bnuz.edu.cn/api/place/buildings/%E5%8A%B1%E8%80%98%E6%A5%BC/places",
                "http://tm.bnuz.edu.cn/api/place/buildings/%E5%BC%98%E6%96%87%E6%A5%BC/places",
                "http://tm.bnuz.edu.cn/api/place/buildings/%E6%9C%A8%E9%93%8E%E6%A5%BC/places"
            };

            WriteLine("---------Start---------");
            WriteLine("Working...");
            try
            {
                await DoWork(httpClient, places);
            }
            catch (Exception ex)
            {
                WriteLine(ex);
            }
            WriteLine("---------End---------");

            ReadKey();
        }

        private static async Task DoWork(HttpClient httpClient, IEnumerable<string> places)
        {
            foreach (var place in places)
            {
                var response = await httpClient.GetAsync(place);
                if (response.StatusCode == HttpStatusCode.Redirect)
                {
                    WriteLine("Cookies 已过期");
                    return;
                }
                var data = await response.Content.ReadAsStringAsync();

                var jsonArr = JArray.Parse(data);
                foreach (var room in jsonArr)
                {
                    var classroom = JsonConvert.DeserializeObject<ClassRoom>(room.ToString());

                    var url = $"{place}/{classroom.Id}/usages";
                    response = await httpClient.GetAsync(url);
                    data = await response.Content.ReadAsStringAsync();
                    var infoArr = JArray.Parse(data);
                    foreach (var message in infoArr)
                    {
                        var info = JsonConvert.DeserializeObject<Info>(message.ToString());
                        info.Room = classroom;
                        Infos.Add(info);
                    }
                }
            }
            Output();
        }

        private static void Output()
        {
            using (var sw = new StreamWriter(OutputPath))
            {
                foreach (var info in Infos)
                {
                    //只查询考试教室
                    //if (info.Type != "ks") continue;
                    sw.WriteLine(info);
                }
            }
        }
    }
}
