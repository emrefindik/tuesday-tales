using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationCombinationData
{
    [SerializeField]
    private List<LocationTypeCountTuple> _genericLocations;
    public IEnumerable<LocationTypeCountTuple> GenericLocations
    {
        get { return _genericLocations; }
    }

    [SerializeField]
    private List<string> _specificMarkers;
    public IEnumerable<string> SpecificMarkers
    {
        get { return _specificMarkers; }
    }

    public LocationCombinationData(List<LocationTypeCountTuple> genericLocations, List<string> specificMarkers)
    {
        _genericLocations = genericLocations;
        _specificMarkers = specificMarkers;
    }
}

[System.Serializable]
public class LocationCombination
{

	[SerializeField]
    List<HatchLocationMarker> _markersToTake;
    public List<HatchLocationMarker> MarkersToTake
    {
        get { return _markersToTake; }
    }

	[SerializeField]
    List<GenericLocation> _genericLocationsToTake;
    public List<GenericLocation> GenericLocationsToTake
    {
        get { return _genericLocationsToTake; }
    }

    public LocationCombination(List<HatchLocationMarker> markersToTake, List<GenericLocation> genericLocationsToTake)
    {
        _markersToTake = markersToTake;
        _genericLocationsToTake = genericLocationsToTake;
    }
}

[System.Serializable]
public class LocationDatabase
{
    /*[SerializeField]
    private List<GenericLocation> _genericLocations;
    public IEnumerable<GenericLocation> GenericLocations
    {
        get { return _genericLocations; }
    } */

    [SerializeField]
    private Dictionary<string, SpatialMarker> _specificMarkers;
    public Dictionary<string, SpatialMarker> SpecificMarkers
    {
        get { return _specificMarkers; }
    }

    public LocationDatabase(Dictionary<string, SpatialMarker> specificMarkers)
    {
        //_genericLocations = genericLocations;
        _specificMarkers = specificMarkers;
    }

    /*public GenericLocation getGenericLocationAtIndex(int index)
    {
        return _genericLocations[index];
    } */

    public SpatialMarker getSpecificMarkerWithId(string index)
    {
        return _specificMarkers[index];
    }
}


