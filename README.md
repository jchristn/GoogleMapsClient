# GoogleMapsClient
 
![alt tag](https://raw.githubusercontent.com/jchristn/GoogleMapsClient/master/Assets/icon.ico)

I needed a simple way to parse addresses and resolve coordinates to an address.  Plug in a Google Maps API key and you're all set.

Special thanks to @jorgeaponte and the community for helping improve this library!

## New in v1.1.0

- Retarget to include .NET 6.0, .NET 7.0
- Async APIs
- Minor breaking changes (including ```GoogleMapsAddress```, ```GoogleMapsCoordinates```, ```GoogleMapsTimestamp``` return object)

## Help and Feedback

First things first - do you need help or have feedback?  File an issue here!

## Example 
```
using GoogleMapsClient;

GoogleMaps client = new GoogleMaps("[your API key here]");
GoogleMapsAddress addr;
GoogleMapsTimestamp ts;

// Get details about coordinates
addr = client.QueryCoordinates(37.4220578, -122.0840897);
addr = await client.QueryCoordinatesAsync(37.4220578, -122.0840897);

// Get details about an address
addr = client.QueryAddress("1600 Amphitheatre Pkwy Mountain View CA");
addr = await client.QueryAddressAsync("1600 Amphitheatre Pkwy Mountain View CA");

// Get timestamp for coordinates
ts = client.LocalTimestamp(37.4220578, -122.0840897, DateTime.Now);
ts = await client.LocalTimestampAsync(37.4220578, -122.0840897, DateTime.Now);

// Get timestamp for address
ts = client.LocalTimestamp("1600 Amphitheatre Pkwy Mountain View CA", DateTime.Now);
ts = await client.LocalTimestampAsync("1600 Amphitheatre Pkwy Mountain View CA", DateTime.Now);
```

## Version History

Refer to CHANGELOG.md for version history.
