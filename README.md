# GoogleMapsClient
 
![alt tag](https://raw.githubusercontent.com/jchristn/GoogleMapsClient/master/Assets/icon.ico)

I needed a simple way to parse addresses and resolve coordinates to an address.  Plug in a Google Maps API key and you're all set.

## New in v1.0.1

- LocalTimestamp API

## Help and Feedback

First things first - do you need help or have feedback?  File an issue here!

## Example 
```
using GoogleMapsClient;

GoogleMaps client = new GoogleMaps("[your API key here]");
Address addr = null;
DateTime dt = DateTime.Now;
string tz = null;

// Get details about coordinates
addr = client.QueryCoordinates(37.4220578, -122.0840897);

// Get details about an address
addr = client.QueryAddress("1600 Amphitheatre Pkwy Mountain View CA");

// Get timestamp for coordinates
dt = client.LocalTimestamp(37.4220578, -122.0840897, DateTime.Now, out tz);

// Get timestamp for address
dt = client.LocalTimestamp("1600 Amphitheatre Pkwy Mountain View CA", DateTime.Now, out tz);
```

## Version History

Refer to CHANGELOG.md for version history.
