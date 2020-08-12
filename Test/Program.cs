using System;
using GoogleMapsClient;
using Newtonsoft.Json;

namespace Test
{
    class Program
    {
        static GoogleMaps _GoogleMaps = null;
        static string _ApiKey = null;
        static bool _RunForever = true;

        static void Main(string[] args)
        {
            while (String.IsNullOrEmpty(_ApiKey))
            {
                Console.Write("API key: ");
                _ApiKey = Console.ReadLine();
            }

            _GoogleMaps = new GoogleMaps(_ApiKey);
            _GoogleMaps.Logger = Console.WriteLine;
            
            while (_RunForever)
            {
                Console.Write("Command [? for help]: ");
                string userInput = Console.ReadLine();
                if (String.IsNullOrEmpty(userInput)) continue;

                if (userInput.Equals("q"))
                {
                    _RunForever = false;
                }
                else if (userInput.Equals("cls") || userInput.Equals("c"))
                {
                    Console.Clear();
                }
                else if (userInput.Equals("?"))
                {
                    Menu();
                }
                else
                {
                    Address addr = null;

                    if (userInput.Contains(","))
                    {
                        string[] parts = userInput.Split(new char[] { ',' }, 2);
                        double latitude = Convert.ToDouble(parts[0]);
                        double longitude = Convert.ToDouble(parts[1]);
                        addr = _GoogleMaps.QueryCoordinates(latitude, longitude);
                    }
                    else
                    {
                        addr = _GoogleMaps.QueryAddress(userInput); 
                    }

                    Console.WriteLine(SerializeJson(addr));
                }
            }
        }

        static void Menu()
        {
            Console.WriteLine("--- Available Commands ---");
            Console.WriteLine("  q           Quit");
            Console.WriteLine("  ?           Help, this menu");
            Console.WriteLine("  cls         Clear the screen");
            Console.WriteLine("  [lat,lng]   Process coordinates");
            Console.WriteLine("  [address]   Process address");
            Console.WriteLine("");
        }

        static string SerializeJson(object obj)
        {
            if (obj == null) return null;

            string json = JsonConvert.SerializeObject(
                obj,
                Newtonsoft.Json.Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                }); 

            return json;
        }

    }
}
