# GoogleMapsClient
 
![alt tag](https://github.com/jchristn/GoogleMapsClient/blob/master/assets/icon.png)

I needed a simple way to parse addresses and resolve coordinates to an address.  Plug in a Google Maps API key and you're all set.

## New in v1.0.0

- Initial release

## Help and Feedback

First things first - do you need help or have feedback?  File an issue here!

## Example 
```
using GoogleMapsClient;

GoogleMaps client = new GoogleMaps("[your API key here]");
Address addr = null;

// Get details about coordinates
addr = client.QueryCoordinates(37.4220578, -122.0840897);

// Get details about an address
addr = client.QueryAddress("1600 Amphitheatre Pkway Mountain View CA");
```

## Version History

Refer to CHANGELOG.md for version history.
