using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** The egg class */
[Serializable]
public class OwnedEgg// : ISerializationCallbackReceiver
{
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

    /** The user IDs (NOT FRIEND ID!) of the friend that helped you hatch this egg */
    private IdList _helpers;
    public IEnumerable<string> Helpers
    {
        get
        {
            if (_helpers != null) return _helpers;
            return Enumerable.Empty<string>();
        }
    }

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
    private List<HatchLocationMarker> _markersToTake;
    [SerializeField]
    private List<GenericLocation> _genericLocationsToTake;
    public IEnumerable<GenericLocation> GenericLocationsToTake
    {
        get { return _genericLocationsToTake.Cast<GenericLocation>(); }
    }

    // Locations that this egg can hatch.
    // TODO if we can anyhow convert this to "any beach" as opposed to a list of specific locations, that would be great
    public IEnumerable<CheckInPlace> PlacesToTake
    {
        get
        {
            return Enumerable.Concat<CheckInPlace>(_markersToTake.Cast<CheckInPlace>(), _genericLocationsToTake.Cast<CheckInPlace>());
        }
    }

    // TODO add other relevant fields

    private OwnedEgg(string name, string id)
    {
        _name = name;
        _id = id;
        _helpers = new IdList();
        _requests = new IdList();
        _markersToTake = new List<HatchLocationMarker>();
        _genericLocationsToTake = new List<GenericLocation>();
        //_size = STARTING_EGG_SIZE;
        _checkInnableMarkers = new List<HatchLocationMarker>();
        _checkInnableLocs = new Dictionary<BasicMarker, HashSet<GenericLocation>>();
    }

    /**  */
    public static IEnumerator createEggForSelf(GameObject eggMenuItemPrefab, Transform eggMenuContentPanel, string eggName = "")
    {
        if (SpatialClient2.single.isLoggedIn())
        {
            // assign the hexadecimal representation of the egg creation number as custom part of id
            OwnedEgg egg = new OwnedEgg(eggName, SpatialClient2.single.newEggIdForSelf());
            yield return SpatialClient2.single.addEggToSelf(egg);
            GameObject eggMenuItem = GameObject.Instantiate(eggMenuItemPrefab);
            eggMenuItem.transform.SetParent(eggMenuContentPanel, false);
            eggMenuItem.GetComponent<EggMenuItem>().Egg = egg; // also updates the egg menu item's view
        }
        else
        {
            Debug.Log("tried to create egg while not logged in");
        }
    }

    public static IEnumerator createEggForFriend(FriendData friend, string eggName = "")
    {
        if (SpatialClient2.single.isLoggedIn())
        {
            OwnedEgg egg = new OwnedEgg(eggName, SpatialClient2.single.newEggIdForFriend(friend));
            yield return SpatialClient2.single.addEggToFriendsEggs(egg);
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
            foreach (GenericLocation loc in _checkInnableLocs[marker])
            {
                loc.markVisited(marker);
            }
            _checkInnableLocs[marker].RemoveWhere(loc => !loc.needToBeVisited());
        }
        _checkInnableLocs.Clear();
    }

    public void initializeCheckInnables()
    {
        _checkInnableMarkers = new List<HatchLocationMarker>();
        _checkInnableLocs = new Dictionary<BasicMarker, HashSet<GenericLocation>>();
    }

    public void updateCheckins(OwnedEgg egg)
    {
        Dictionary<string, HatchLocationMarker> idHlmDict = new Dictionary<string, HatchLocationMarker>();
        foreach (HatchLocationMarker marker in _markersToTake)
        {
            idHlmDict[marker.Id] = marker;
        }
        foreach (HatchLocationMarker marker in egg._markersToTake)
        {
            if (!marker.needToBeVisited())
                idHlmDict[marker.Id].markVisited();
        }

        Dictionary<GenericLocation.GooglePlacesType, GenericLocation> typeGlDict = new Dictionary<GenericLocation.GooglePlacesType, GenericLocation>();
        foreach (GenericLocation location in _genericLocationsToTake)
        {
            typeGlDict[location.LocationType] = location;
        }
        foreach (GenericLocation location in egg._genericLocationsToTake)
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
