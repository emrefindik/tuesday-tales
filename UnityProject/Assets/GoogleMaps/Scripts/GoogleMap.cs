using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GoogleMap : MonoBehaviour
{

	public enum MapType
	{
		RoadMap,
		Satellite,
		Terrain,
		Hybrid
	}
	public bool loadOnStart = true;
	public bool autoLocateCenter = true;
	public GoogleMapLocation centerLocation;
	public int zoom = 13;
	public MapType mapType;
	public int size = 512;
	public bool doubleResolution = false;
	public GoogleMapMarker[] markers;
	public GoogleMapPath[] paths;

    // Emre's addition
    public Image mapImage;
	
	void Start() {
		if(loadOnStart) Refresh();	
	}

    public void swapMarkersAndRefresh(List<Marker> spatialMarkers)
    {
        Debug.Log("swaping markers");
        if (spatialMarkers == null)
        {
            Debug.Log("no markers");
            return;
        }
        markers = new GoogleMapMarker[spatialMarkers.Count];
        // Convert from one marker type to another
        int count = 0;
        foreach (Marker marker in spatialMarkers)
        {
            GoogleMapLocation[] gml_list = new GoogleMapLocation[]
            {
                new GoogleMapLocation("", (float)marker.loc.coordinates[0],(float)marker.loc.coordinates[1])
            };
            markers[count] = new GoogleMapMarker(GoogleMapMarker.GoogleMapMarkerSize.Mid, GoogleMapColor.gray, 'a', gml_list);
            count++;
        }
        // Refresh after finish everything
        Refresh();
    }

    public void Refresh() {
		if(autoLocateCenter && (markers.Length == 0 && paths.Length == 0)) {
			Debug.LogError("Auto Center will only work if paths or markers are used.");	
		}
		StartCoroutine(_Refresh());
	}

    IEnumerator _Refresh ()
	{
		var url = "http://maps.googleapis.com/maps/api/staticmap";
		var qs = "";
		if (!autoLocateCenter) {
			if (centerLocation.address != "")
				qs += "center=" + WWW.UnEscapeURL(centerLocation.address);
			else {
				qs += "center=" + WWW.UnEscapeURL(string.Format ("{0},{1}", centerLocation.latitude, centerLocation.longitude));
			}
		
			qs += "&zoom=" + zoom.ToString ();
		}
		qs += "&size=" + WWW.UnEscapeURL(string.Format ("{0}x{0}", size));
		qs += "&scale=" + (doubleResolution ? "2" : "1");
		qs += "&maptype=" + mapType.ToString ().ToLower ();
		var usingSensor = false;
#if UNITY_IPHONE
		usingSensor = Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running;
#endif
		qs += "&sensor=" + (usingSensor ? "true" : "false");
        //qs += "&key=" + "AIzaSyCir4JM4zZaFhPTvwEk7J29PJq07DqQE7A";

       foreach (var i in markers) {
            qs += "&markers=";
            bool addVerticalBar = false;
            if (i.size != GoogleMapMarker.GoogleMapMarkerSize.None)
            {
                qs += "size:" + i.size.ToString().ToLower();
                addVerticalBar = true;
            }
            if (i.color != GoogleMapColor.none)
            {
                if (addVerticalBar) qs += "|";
                qs += "color:" + i.color;
                addVerticalBar = true;
            }
            if (char.IsLetterOrDigit(i.Label))
            {
                if (addVerticalBar) qs += "|";
                qs += "label:" + i.Label.ToString().ToUpper();
            }
                //qs += string.Format ("size:{0}|color:{1}|label:{2}", i.size.ToString ().ToLower (), i.color, i.label.ToUpper());
            foreach (var loc in i.locations) {
				if (loc.address != "")
					qs += "|" + WWW.UnEscapeURL(loc.address);
				else
					qs += "|" + WWW.UnEscapeURL(string.Format ("{0},{1}", loc.latitude, loc.longitude));
			}
		}
		
		foreach (var i in paths) {
			qs += "&path=" + string.Format ("weight:{0}|color:{1}", i.weight, i.color);
			if(i.fill) qs += "|fillcolor:" + i.fillColor;
			foreach (var loc in i.locations) {
				if (loc.address != "")
					qs += "|" + WWW.UnEscapeURL(loc.address);
				else
					qs += "|" + WWW.UnEscapeURL(string.Format ("{0},{1}", loc.latitude, loc.longitude));
			}
		}


        var req = new WWW(url + "?" + qs);
        yield return req;

        // changed by Emre
        if (mapImage != null) mapImage.canvasRenderer.SetTexture(req.texture);
    }
	
	
}

public enum GoogleMapColor
{
    black,
    brown,
    green,
    purple,
    yellow,
    blue,
    gray,
    orange,
    red,
    white,
    none
}

[System.Serializable]
public class GoogleMapLocation
{
    public string address;
    public float latitude;
    public float longitude;
    public GoogleMapLocation(string a, float la, float lo)
    {
        address = a;
        latitude = la;
        longitude = lo;
    }
}

[System.Serializable]
public class GoogleMapMarker
{
    public const char NO_LABEL_CHAR = '!';

    public enum GoogleMapMarkerSize
    {
        Tiny,
        Small,
        Mid,
        None
    }
    public GoogleMapMarkerSize size;
    public GoogleMapColor color;

    private char label;
    public char Label
    {
        get
        {
            return label;
        }
        set
        {
            if (char.IsLetterOrDigit(value))
            {
                label = value;
            }
            else
            {
                Debug.Log("label must be a digit or a letter");
                label = NO_LABEL_CHAR;
            }
        }
    }

    public GoogleMapLocation[] locations;
    public GoogleMapMarker(GoogleMapMarkerSize sz, GoogleMapColor co, char la, GoogleMapLocation[] lo)
    {
        size = sz;
        color = co;
        Label = la;
        locations = lo;
    }
    public GoogleMapMarker() {
        size = GoogleMapMarker.GoogleMapMarkerSize.None;
        color = GoogleMapColor.none;
        label = NO_LABEL_CHAR;
    }
}

[System.Serializable]
public class GoogleMapPath
{
    public int weight = 5;
    public GoogleMapColor color;
    public bool fill = false;
    public GoogleMapColor fillColor;
    public GoogleMapLocation[] locations;
}