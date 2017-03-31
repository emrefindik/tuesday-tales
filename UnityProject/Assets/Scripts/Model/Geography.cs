using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationCoord
{
	public double lat;
	public double lon;
	public LocationCoord(double _lat, double _lon)
	{
		lat = _lat;
		lon = _lon;
	}
}

[System.Serializable]
public class Path
{
	List<LocationCoord> locations;
}

public class Geography {
	
	const double EARTH_RADIUS = 6371d;
	static public bool withinDistance(double lat1, double lon1, double lat2, double lon2, double radius){
		if (radius > getDistanceFromLatLonInM (lat1, lon1, lat2, lon2))
			return true;

		return false;
	}
	static public bool withinDistance(LocationCoord loc1, LocationCoord loc2, double radius){
		if (radius > getDistanceFromLatLonInM (loc1.lat, loc1.lon, loc2.lat, loc2.lon))
			return true;

		return false;
	}

	static double getDistanceFromLatLonInM(double lat1, double lon1, double lat2, double lon2){
		double dLat = deg2rad(lat2-lat1);  // deg2rad below
		double dLon = deg2rad(lon2-lon1); 
		double a = 
			System.Math.Sin(dLat/2) * System.Math.Sin(dLat/2) +
			System.Math.Cos(deg2rad(lat1)) * System.Math.Cos(deg2rad(lat2)) * 
			System.Math.Sin(dLon/2) * System.Math.Sin(dLon/2)
			; 
		double c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1-a)); 
		double d = EARTH_RADIUS * c; // Distance in km
		Debug.Log("Distance between 2 points:" + d*1000);
		return d * 1000;	// Distance in m
	}

	static double deg2rad(double deg) {
		return deg * (Mathf.PI / 180);
	}
}
