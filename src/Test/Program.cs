using System;
using GoogleMapsClient;

namespace Test
{
    class Program
    {
        static GoogleMaps _GoogleMaps = null;
        static string _ApiKey = null;
        static Action<string> _Logger = null;
        static bool _RunForever = true;

        static void Main(string[] args)
        {
            while (String.IsNullOrEmpty(_ApiKey))
            {
                Console.Write("API key: ");
                _ApiKey = Console.ReadLine();
            }

            _GoogleMaps = new GoogleMaps(_ApiKey);
            _GoogleMaps.Logger = _Logger;
            
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
                else if (userInput.StartsWith("ts "))
                {
                    string timestampStr = userInput.Substring(2);
                    DateTime ts = DateTime.Now;
                    string tz = null;

                    if (timestampStr.Contains(","))
                    {
                        string[] parts = timestampStr.Split(new char[] { ',' }, 2);
                        double latitude = Convert.ToDouble(parts[0]);
                        double longitude = Convert.ToDouble(parts[1]);
                        ts = _GoogleMaps.LocalTimestamp(latitude, longitude, DateTime.Now, out tz);
                    }
                    else
                    {
                        ts = _GoogleMaps.LocalTimestamp(timestampStr, DateTime.Now, out tz);
                    }

                    Console.WriteLine(tz + ": " + ts.ToString("s"));
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

                    Console.WriteLine(SerializationHelper.SerializeJson(addr, true));
                }
            }
        }

        static void Menu()
        {
            Console.WriteLine("--- Available Commands ---");
            Console.WriteLine("  q              Quit");
            Console.WriteLine("  ?              Help, this menu");
            Console.WriteLine("  cls            Clear the screen");
            Console.WriteLine("  ts [address]   Generate timestamp for a specific address");
            Console.WriteLine("  ts [lat,lng]   Generate timestamp for specific coordinates");
            Console.WriteLine("  [lat,lng]      Process coordinates");
            Console.WriteLine("  [address]      Process address");
            Console.WriteLine("");
        }
    }
}
