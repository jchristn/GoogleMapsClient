using System;
using System.Threading;
using System.Threading.Tasks;
using GoogleMapsClient;

namespace Test
{
    class Program
    {
        static GoogleMaps _GoogleMaps = null;
        static string _ApiKey = null;
        static Action<string> _Logger = null;
        static bool _RunForever = true;

        static async Task Main(string[] args)
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
                    GoogleMapsTimestamp ts = new GoogleMapsTimestamp();

                    if (timestampStr.Contains(","))
                    {
                        string[] parts = timestampStr.Split(new char[] { ',' }, 2);
                        double latitude = Convert.ToDouble(parts[0]);
                        double longitude = Convert.ToDouble(parts[1]);
                        ts = await _GoogleMaps.LocalTimestampAsync(latitude, longitude, DateTime.Now);
                    }
                    else
                    {
                        ts = await _GoogleMaps.LocalTimestampAsync(timestampStr, DateTime.Now);
                    }

                    Console.WriteLine(ts.TimezoneName + ": " + ts.LocalTime.ToString("s"));
                }
                else
                {
                    GoogleMapsAddress addr = null;

                    if (userInput.Contains(","))
                    {
                        string[] parts = userInput.Split(new char[] { ',' }, 2);
                        double latitude = Convert.ToDouble(parts[0]);
                        double longitude = Convert.ToDouble(parts[1]);
                        addr = await _GoogleMaps.QueryCoordinatesAsync(latitude, longitude);
                    }
                    else
                    {
                        addr = await _GoogleMaps.QueryAddressAsync(userInput);
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
            Console.WriteLine("  [address]      Process address (do not use commas in the address)");
            Console.WriteLine("");
        }
    }
}
