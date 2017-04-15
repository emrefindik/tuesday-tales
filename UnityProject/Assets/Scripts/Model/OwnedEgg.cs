using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** The egg class */
[Serializable]
public class OwnedEgg
{
    public const int NUMBER_OF_EGG_IMAGES = 4;
    public const double STARTING_EGG_SIZE = 1.0;

    // do not serialize. this is for internal use in game
    private List<HatchLocationMarker> _checkInnableMarkers;
    public List<HatchLocationMarker> CheckInnableMarkers
    {
        get { return _checkInnableMarkers; }
        private set { _checkInnableMarkers = value; }
    }
    private Dictionary<BasicMarker, HashSet<GenericLocation>> _checkInnableLocs;
    public Dictionary<BasicMarker, HashSet<GenericLocation>> CheckInnableLocs
    {
        get { return _checkInnableLocs; }
        private set { _checkInnableLocs = value; }
    }

    [SerializeField]
    private int _imageIndex;
    public Sprite Sprite
    {
        get { return KaijuDatabase.instance.EggSprites[_imageIndex]; }
    }

    [SerializeField]
    private bool _hatchable;
    public bool Hatchable
    {
        get { return _hatchable; } // NO SET!
    }

    /** The user IDs (NOT FRIEND ID!) of the friend that helped you hatch this egg */
    [SerializeField]
    private IdList _helpers;
    public IEnumerable<string> Helpers
    {
        get
        {
            if (_helpers != null) return _helpers;
            return Enumerable.Empty<string>();
        }
    }

    [SerializeField]
    /** The user IDs (NOT FRIEND ID!) of the friends you sent requests to */
    private IdList _requests;
    public IEnumerable<string> Requests
    {
        get
        {
            if (_requests != null) return _requests;
            return Enumerable.Empty<string>();
        }
    }

    /** The egg ID */
    [SerializeField]
    private string _id;
    public string Id
    {
        get { return _id; }
        private set { _id = value; }
    }

    [SerializeField]
    private string _name;
    public string Name
    {
        get { return _name; }
        private set { _name = value; }
    }

    [SerializeField]
    private Kaiju _kaiju;
    public Kaiju KaijuEmbryo
    {
        get { return _kaiju; }
        private set { _kaiju = value; }
    }

    [SerializeField]
    private LocationCombination _requiredLocations;
    public IEnumerable<HatchLocationMarker> MarkersToTake
    {
        get { return _requiredLocations.MarkersToTake; }
    }
    public IEnumerable<GenericLocation> GenericLocationsToTake
    {
        get { return _requiredLocations.GenericLocationsToTake; }
    }

    // Locations that this egg can hatch.
    // TODO if we can anyhow convert this to "any beach" as opposed to a list of specific locations, that would be great
    public IEnumerable<CheckInPlace> PlacesToTake
    {
        get
        {
            return Enumerable.Concat<CheckInPlace>(MarkersToTake.Cast<CheckInPlace>(), GenericLocationsToTake.Cast<CheckInPlace>());
        }
    }

    // TODO add other relevant fields

    private OwnedEgg(string name, int imageIndex, string id, LocationCombination combo)
    {
        _name = name;
        _id = id;
        _imageIndex = imageIndex;
        _helpers = new IdList();
        _requests = new IdList();
        _requiredLocations = combo;
        //_size = STARTING_EGG_SIZE;
        _checkInnableMarkers = new List<HatchLocationMarker>();
        _checkInnableLocs = new Dictionary<BasicMarker, HashSet<GenericLocation>>();
        _hatchable = !(PlacesToTake.Count() > 0);
        _kaiju = null;
    }

    public IEnumerator initializeSprite(CoroutineResponse response)
    {
        yield return KaijuDatabase.instance.checkAndDownloadEggSprite(_imageIndex, response);
    }

    public IEnumerator initializeKaiju()
    {
        if (_kaiju == null)
        {
            List<SpatialMarker> markersAround = new List<SpatialMarker>();
            CoroutineResponse markerResponse = new CoroutineResponse();
            yield return SpatialClient2.single.GetMarkersByDistance(Input.location.lastData.longitude, Input.location.lastData.latitude, UserMetadata.KAIJU_MARKER_RADIUS, true, markersAround, markerResponse);
            _kaiju = (new KaijuFrequencyList()).randomKaiju(markersAround);
        }
        else yield break;
    }

    public static IEnumerator createEggForSelf(GameObject eggMenuItemPrefab, Transform eggMenuContentPanel, int imageIndex, string eggName = "")
    {
        if (SpatialClient2.single.isLoggedIn())
        {
            List<SpatialMarker> markersAround = new List<SpatialMarker>();
            CoroutineResponse markerResponse = new CoroutineResponse();
            yield return SpatialClient2.single.GetMarkersByDistance(Input.location.lastData.longitude, Input.location.lastData.latitude, UserMetadata.KAIJU_MARKER_RADIUS, true, markersAround, markerResponse);
            // assign the hexadecimal representation of the egg creation number as custom part of id
            OwnedEgg egg = new OwnedEgg(eggName, imageIndex, SpatialClient2.single.newEggIdForSelf(), SpatialClient2.single.getLocationsForEgg((new LocationFrequencyList()).randomKaiju(markersAround)));
            yield return egg.initializeSprite(new CoroutineResponse()); // sprite should already be there since we are coming from the egg screen, but just checking
            yield return egg.initializeKaiju();
            yield return SpatialClient2.single.addEggToSelf(egg);
            GameObject eggMenuItem = GameObject.Instantiate(eggMenuItemPrefab);
            eggMenuItem.transform.SetParent(eggMenuContentPanel, false);
            eggMenuItem.GetComponent<OwnEggMenuItem>().Egg = egg; // also updates the egg menu item's view
			Debug.Log("egg created");
        }
        else
        {
            Debug.Log("tried to create egg while not logged in");
        }
    }

    public static IEnumerator createEggForFriend(FriendData friend, int imageIndex, string eggName = "")
    {
        if (SpatialClient2.single.isLoggedIn())
        {
            List<SpatialMarker> markersAround = new List<SpatialMarker>();
            CoroutineResponse markerResponse = new CoroutineResponse();
            yield return SpatialClient2.single.GetMarkersByDistance(Input.location.lastData.longitude, Input.location.lastData.latitude, UserMetadata.KAIJU_MARKER_RADIUS, true, markersAround, markerResponse);
            OwnedEgg egg = new OwnedEgg(eggName, imageIndex, SpatialClient2.single.newEggIdForFriend(friend), SpatialClient2.single.getLocationsForEgg((new LocationFrequencyList()).randomKaiju(markersAround)));
            yield return egg.initializeSprite(new CoroutineResponse()); // sprite should already be there since we are coming from the egg screen, but just checking
            yield return egg.initializeKaiju();
            yield return SpatialClient2.single.addOrUpdateEggInFriendsEggs(egg);
        }
        else
        {
            Debug.Log("tried to create egg while not logged in");
        }
    }

    public void checkIn()
    {
        foreach (HatchLocationMarker marker in _checkInnableMarkers)
        {
            marker.markVisited();
            MainMenuScript.removeEntryFromIdMarkers(marker.Id, this);
        }
        _checkInnableMarkers.Clear();
        foreach (BasicMarker marker in _checkInnableLocs.Keys)
        {
            bool unvisitedLocsUpdated = false;
            foreach (GenericLocation loc in _checkInnableLocs[marker])
            {
                if (!unvisitedLocsUpdated)
                {
                    MainMenuScript.removeEntryFromPlaceTypes(loc.LocationType, this);
                    unvisitedLocsUpdated = true;
                }
                loc.markVisited(marker);
            }            
        }
        _checkInnableLocs.Clear();
        foreach (CheckInPlace place in PlacesToTake)
        {
            if (place.needToBeVisited()) return;
        }
        _hatchable = true;
    }

    public void initializeCheckInnables()
    {
        _checkInnableMarkers = new List<HatchLocationMarker>();
        _checkInnableLocs = new Dictionary<BasicMarker, HashSet<GenericLocation>>();
    }

    public void updateCheckins(OwnedEgg egg)
    {
        Dictionary<string, HatchLocationMarker> idHlmDict = new Dictionary<string, HatchLocationMarker>();
        foreach (HatchLocationMarker marker in MarkersToTake)
        {
            idHlmDict[marker.Id] = marker;
        }
        foreach (HatchLocationMarker marker in egg.MarkersToTake)
        {
            if (!marker.needToBeVisited())
                idHlmDict[marker.Id].markVisited();
        }

        Dictionary<GenericLocation.GooglePlacesType, GenericLocation> typeGlDict = new Dictionary<GenericLocation.GooglePlacesType, GenericLocation>();
        foreach (GenericLocation location in GenericLocationsToTake)
        {
            typeGlDict[location.LocationType] = location;
        }
        foreach (GenericLocation location in egg.GenericLocationsToTake)
        {
            typeGlDict[location.LocationType].updateVisits(location);
        }
    }

    /** Returns true if request already sent to friend, false otherwise.
      * Adds the friend user ID to the list of requests sent out */
    public bool addRequest(string friendUserId)
    {
        if (_requests.Contains(friendUserId)) return false;
        _requests.add(friendUserId);
        return true;
    }

    /** Adds the friend user ID to the list of friends who helped this egg hatch,
      * if that friend does not already exist in that list. */
    public void addHelper(string friendUserId)
    {
        if (!_requests.Contains(friendUserId)) _requests.add(friendUserId);
    }
}
