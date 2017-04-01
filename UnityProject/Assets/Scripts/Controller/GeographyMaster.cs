using System;

public class GeographyMaster {

    //static GeographyMaster single;

    const double EARTH_RADIUS = 6371000.0;

    /*
	// Use this for initialization
	void Start () {
		single = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	static public bool withinDistance(double lat1, double lon1, double lat2, double lon2, double radius){
		if (radius > getDistanceFromLatLonInM (lat1, lon1, lat2, lon2))
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
		return d * 1000;	// Distance in m
	}

	static double deg2rad(double deg) {
		return deg * (Mathf.PI / 180);
	} */

    /*public static double degreeToRadian(double deg)
    {
        return (Math.PI * deg) / 180.0;
    }

    public static double calculateDistance(Location l1, Location l2)
    {
        return calculateDistance(l1.Coordinates[1], l1.Coordinates[0], l2.Coordinates[1], l2.Coordinates[0]);
    }

    public static double calculateDistance(Location loc, double latitude, double longitude)
    {
        return calculateDistance(loc.Coordinates[1], loc.Coordinates[0], latitude, longitude);
    }

    public static double calculateDistance(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        // script from
        // http://www.movable-type.co.uk/scripts/latlong.html

        var deltaphi = degreeToRadian(latitude2 - latitude1);
        var deltalambda = degreeToRadian(longitude2 - longitude1);

        var a = Math.Sin(deltaphi / 2) * Math.Sin(deltaphi / 2) +
                Math.Cos(degreeToRadian(latitude1)) * Math.Cos(degreeToRadian(latitude2)) *
                Math.Sin(deltalambda / 2) * Math.Sin(deltalambda / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // multiply with earth's radius
        return EARTH_RADIUS * c;
    }*/

}
