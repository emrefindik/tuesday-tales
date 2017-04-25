using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using System;
using UnityEngine.Analytics;

public class SpatialClient2 : MonoBehaviour
{
    public const string CHECK_YOUR_INTERNET_CONNECTION = "Check your internet connection.";

    // Test Project ID: 588fb546604ae700118697c5
    public const string baseURL = "https://spatial-api-poc.herokuapp.com";
    public const string PROJECT_ID = "58b070d3b4c96e00118b66ee"; // main project ID     "58fec2f29e38830011d2ed05"; // generate random marker test project ID          
    public const string GOOGLE_API_KEY = "AIzaSyCejidwxDYN4APVvtlE7ZPsBtVdhB7JG70";

    // URL to connect to to check whether user has internet connection
    public const string PING_URL = "http://tuesday-tales.etc.cmu.edu/Photos/pingtest.txt";
    // the text that should be returned from pingURL
    public const string PING_TEXT = "a";

    // every PING_TIME seconds, the game checks whether the user has an internet connection
    public const float PING_TIME = 10.0f;

    // If there are no buildings found to destroy within a radius of MAX_BUILDING_MARKER_DISTANCE,
    // create markers by fetching Google Markers data
    public const float MAX_BUILDING_MARKER_DISTANCE = 3000f;

    public static SpatialClient2 single;

    private LoginResponse userSession = null;
    private LocationDatabase _locationDatabase;
    private KaijuFrequencyList _defaultKaiju;

    private Dictionary<string, FriendData> _friends = new Dictionary<string, FriendData>();
    public IEnumerable<FriendData> Friends
    {
        get { return _friends.Values; }
    }

    public IEnumerable<OwnedEgg> EggsOwned
    {
        get { return userSession.User.Metadata.EggsOwned; }
    }

	public string userId{
		get{
			return userSession.User.Id;
		}
	}

    public IEnumerable<Kaiju> Kaiju
    {
        get { return userSession.User.Metadata.Kaiju; }
    }

    private bool StreakNeedsReset
    {
        get {
            return !_isResettingStreak
          && userSession.User.Metadata.StreakTimerStart != UserMetadata.NO_STREAK
          && getTimer() <= 0 && userSession.User.Metadata.StreakMarkers.Count > 0; }
    }

    private bool ready = false;
    private UserList allUser = new UserList();
    private Project project;

    private bool metadataUpdatedSuccessfully = false;
    private bool streakInitialized = false;
    private bool _hasInternetConnection;
    private bool _isResettingStreak;


    void Start()
    {
        _hasInternetConnection = true;
        ready = false;
        _isResettingStreak = false;
        _locationDatabase = new LocationDatabase(new Dictionary<string, SpatialMarker>());
        streakInitialized = false;
        metadataUpdatedSuccessfully = false;
        single = this;
        userSession = null;

        List<ItemWithFrequency<Kaiju>> defaultKaijuFrequencies = new List<ItemWithFrequency<Kaiju>>();
        defaultKaijuFrequencies.Add(new KaijuWithFrequency(new Kaiju(Color.blue, 1, 1, 1, "Defaultzilla"), 1));
        // TODO add other default kaiju

        _defaultKaiju = new KaijuFrequencyList(defaultKaijuFrequencies);

        // for marker setup. delete this
        setUpMarkers();
        StartCoroutine(addFriends());
    }

    public Kaiju randomKaijuFromMarkers(List<SpatialMarker> markersAround)
    {
        Kaiju k = (new KaijuFrequencyList(markersAround)).randomItem();
        if (k == default(Kaiju))
            return _defaultKaiju.randomItem();
        else
            return k;
    }

    public LocationCombinationData randomLocationFromMarkers(List<SpatialMarker> markersAround)
    {
        LocationCombinationData k = (new LocationFrequencyList(markersAround)).randomItem();
        if (k == default(LocationCombinationData))
        {
            int locationCount = (int)(Mathf.Pow(UnityEngine.Random.Range(0.0f, 1.0f), 3) * 5) + 1;
            Dictionary<GenericLocation.GooglePlacesType, int> dict = new Dictionary<GenericLocation.GooglePlacesType, int>();
            while (locationCount > 0)
            {
                GenericLocation.GooglePlacesType type = (GenericLocation.GooglePlacesType)(new System.Random().Next(0, (int)GenericLocation.GooglePlacesType.DEFAULT));
                if (!dict.ContainsKey(type))
                    dict[type] = 1;
                else
                    dict[type]++;
                locationCount--;
            }
            List<LocationTypeCountTuple> ltct = new List<LocationTypeCountTuple>();
            foreach (GenericLocation.GooglePlacesType key in dict.Keys)
                ltct.Add(new LocationTypeCountTuple(key, dict[key]));
            return new LocationCombinationData(ltct, new List<string>());
        }
        else
            return k;
    }

    private IEnumerator addFriends()
    {
        /*yield return LoginUser(new CoroutineResponse(), "Z", "Z");
        yield return AddFriend("58f3a9f42a07420011c19e0f");
        Debug.Log("yo! friend added"); */
        yield return null;
    }

    /** Checks whether the user has internet connection every PING_TIME seconds.
      * If the user previously lost connection but reestablished connection,
      * updates the user metadata. */
    public IEnumerator checkInternetConnection()
    {
        bool connectionSuccessful;
        while (true)
        {
            WWW pingWWW = new WWW(PING_URL);
            yield return pingWWW;
            connectionSuccessful = pingWWW.text == PING_TEXT && String.IsNullOrEmpty(pingWWW.error);
            if (!_hasInternetConnection && connectionSuccessful)
                UpdateMetadata(null, "", false);
            _hasInternetConnection = connectionSuccessful;
            yield return new WaitForSeconds(PING_TIME);
        }
    }

    private void setUpMarkers()
    {
        //StartCoroutine(DeleteMarkerById("58f3f96b4e973b0011f5f6b9"));
        //StartCoroutine(CreateMarker(40.442557, -79.942535, "CMU map overlay", "map overlay for the Carnegie Mellon campus", MarkerMetadata.newMapOverlayMetadata("http://tuesday-tales.etc.cmu.edu/Photos/cmumap.jpg", new ImageBounds(40.445924, 40.439190, -79.936435, -79.948635))));
		//StartCoroutine(CreateMarker(40.442557, -79.942535, "Paris", "", MarkerMetadata.newCheckInLocationMetadata()));
		//StartCoroutine(CreateMarker(40.442557, -79.942535, "Mount Everest", "", MarkerMetadata.newCheckInLocationMetadata()));
		//StartCoroutine(CreateMarker(40.442557, -79.942535, "Hawaii", "", MarkerMetadata.newCheckInLocationMetadata()));
		//StartCoroutine(DeleteMarkerById("58f3eb594e973b0011f5f6b5"));
		//StartCoroutine (CreateMarker (40.44334, -79.943345, "Purnell Center for the Arts", "Home to Carnegie Mellon University's School of Drama and the Philip Chosky Theater.", MarkerMetadata.newBuildingMetadata ("http://tuesday-tales.etc.cmu.edu/Photos/building1.jpg", "", new ImageBounds (40.4435, 40.4431, -79.9431, -79.9435))));
        /* StartCoroutine(CreateMarker(40.432791, -79.964793, "Entertainment Technology Center", "The Entertainment Technology Center at Carnegie Mellon University", MarkerMetadata.newCheckInLocationMetadata()));
        StartCoroutine(DeleteMarkerById("58f00e3f2aac62001128c2c8")); */
        //StartCoroutine(DeleteMarkerById("58f2df8699807700111ae95d"));

        /*List<ItemWithFrequency<Kaiju>> lst = new List<ItemWithFrequency<Kaiju>>();
        lst.Add(new KaijuWithFrequency(new Kaiju(Color.yellow, 1, 3, 3, "Gelb"), 5));
        lst.Add(new KaijuWithFrequency(new Kaiju(Color.red, 4, 5, 1, "Blaze"), 3));
        lst.Add(new KaijuWithFrequency(new Kaiju(Color.green, 2, 2, 2, "Stomper"), 2));

        List<ItemWithFrequency<LocationCombinationData>> lst2 = new List<ItemWithFrequency<LocationCombinationData>>();
        List<LocationTypeCountTuple> ltct = new List<LocationTypeCountTuple>();*/
        /*List<string> str1 = new List<string>();
        str1.Add("58f00e3f2aac62001128c2c1"); // cathedral of learning - right off campus
        List<string> str2 = new List<string>();
        str2.Add("58f00e3f2aac62001128c2c2"); // peace garden - on campus
        List<string> str3 = new List<string>();
        str3.Add("58f00e3f2aac62001128c2c3"); // phipps conservatory - right off campus
        List<string> str4 = new List<string>();
        str4.Add("58f00e3f2aac62001128c2c4"); // mattress factory
        List<string> str5 = new List<string>();
        str5.Add("58f00e3f2aac62001128c2c5"); // carnegie museum of art - right off campus
        List<string> str6 = new List<string>();
        str6.Add("58f00e3f2aac62001128c2c6"); // the fence - on campus
        List<string> str7 = new List<string>();
        str7.Add("58f00e402aac62001128c2c7"); // centerfield gesling stadium - on campus
        List<string> str8 = new List<string>();
        str8.Add("58f12cd1a30d230011f0f123"); // ETC
        List<string> str9 = new List<string>();
        str9.Add("58f00e402aac62001128c2c9"); // Dippy the Dinosaur - right off campus
        List<string> str0 = new List<string>();
        str0.Add("58f00e402aac62001128c2ca"); // Schell Games Studio
        lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str1), 3));
        lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str2), 4));
        lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str3), 3));
        lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str4), 2));
        lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str5), 3));
        lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str6), 4));
        lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str7), 4));
        lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str8), 2));
        lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str9), 3));
        lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str0), 2)); */

		/*List<string> str2 = new List<string>();
		str2.Add("58fd43ac8bd5410011d11150"); // mount everest
		List<string> str3 = new List<string>();
		str3.Add("58fd43ac8bd5410011d11152"); // hawaii
		List<string> str4 = new List<string>();
		str4.Add("58fd43ac8bd5410011d11151"); // paris

		lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str2), 1));
		lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str3), 1));
		lst2.Add(new LocationWithFrequency(new LocationCombinationData(ltct, str4), 1));

        StartCoroutine(CreateMarker(40.442557, -79.942535, "softs kaiju spawn point", "kaiju spawn point", MarkerMetadata.newKaijuSpawnPointMetadata(new KaijuFrequencyList(lst), new LocationFrequencyList(lst2))));
		*/
    }

    private void Update()
    {
        if (streakInitialized && StreakNeedsReset)
        {
            _isResettingStreak = true;
            StartCoroutine(resetStreak());
        }
    }

    public bool isLoggedIn()
    {
        return userSession != null;
    }    

    public string getStreakPathAsJsonString()
    {
		Debug.Log (userSession.User.Metadata.StreakMarkers.Count());
		Debug.Log(JsonUtility.ToJson(userSession.User.Metadata.StreakMarkers.ToList()));
		Debug.Log (userSession.User.Metadata.StreakMarkers.ToList().Count());
		return JsonUtility.ToJson(userSession.User.Metadata.StreakMarkers);
    }

    public int getScore()
    {
        return userSession.User.Metadata.Score;
    }

    public int getMultiplier()
    {
        return userSession.User.Metadata.ScoreMultiplier;
    }

    public long getTimer()
    {
        return (userSession.User.Metadata.LastRampage +
            userSession.User.Metadata.StreakTimerStart) -
            UserMetadata.currentTimestamp();
    }

    // START OF EMRE'S CODE
    public IEnumerator resetStreak()
    {
		if (userSession.User.Metadata.StreakMarkers.Count > 0) {
			Analytics.CustomEvent ("EndOfStreak", new Dictionary<string,object> {
				{ "PlayerId", userId },
				{ "StreakLength",userSession.User.Metadata.StreakMarkers.Count },
				{ "StreakStart",userSession.User.Metadata.StreakMarkers [0] },
				{ "StreakEnd", userSession.User.Metadata.StreakMarkers [userSession.User.Metadata.StreakMarkers.Count - 1] }
			});
		}
        userSession.User.Metadata.resetStreak();
        yield return UpdateMetadata(null, "Could not update score. " + CHECK_YOUR_INTERNET_CONNECTION, false);
        Debug.Log("reset streak");
        _isResettingStreak = false;
    }


    public IEnumerator updateLastRampage(int scoreIncrement, string markerId)
    {
        userSession.User.Metadata.updateLastRampage(scoreIncrement, markerId);
        yield return UpdateMetadata(null, "Could not update score. " + CHECK_YOUR_INTERNET_CONNECTION, false);
    }

    public IEnumerator updateLastRampageWithMultiplier(int scoreIncrement, string markerId)
    {
        userSession.User.Metadata.updateLastRampage(scoreIncrement * userSession.User.Metadata.ScoreMultiplier, markerId);
        yield return UpdateMetadata(null, "Could not update score. " + CHECK_YOUR_INTERNET_CONNECTION, false);
    }
    // END OF EMRE'S CODE

    public string newEggIdForSelf()
    {
        string eggId = userSession.User.Id + userSession.User.Id + userSession.User.Metadata.EggsCreated.ToString("x");
        userSession.User.Metadata.incrementEggsCreated();
        return eggId;
        // do not update metadata. update only when egg is created
    }

    public string newEggIdForFriend(FriendData friend)
    {
        string eggId = friend.Friend.Id + userSession.User.Id + userSession.User.Metadata.EggsCreated.ToString("x");
        userSession.User.Metadata.incrementEggsCreated();
        return eggId;
        // do not update metadata. update only when egg is created
    }

    public IEnumerator addEggToSelf(OwnedEgg egg)
    {
        userSession.User.Metadata.EggsOwned.checkAndAdd(egg, true);
		yield return UpdateMetadata(MainMenuScript.EggsCanvas, "Could not add egg " + egg.KaijuEmbryo.Name + ". " + CHECK_YOUR_INTERNET_CONNECTION, false);
    }

    public IEnumerator addOrUpdateEggInFriendsEggs(OwnedEgg egg)
    {
        userSession.User.Metadata.FriendEggsCheckedIn.checkAndAdd(egg, false);
        yield return UpdateMetadata(MainMenuScript.EggsCanvas, "Could not add egg " + egg.KaijuEmbryo.Name + " to the list of eggs you are holding onto from your friends. " + CHECK_YOUR_INTERNET_CONNECTION, true);
    }

	public IEnumerator updateMetadataAfterOwnEggCheckedIn(string eggName)
	{
		yield return SpatialClient2.single.UpdateMetadata(MainMenuScript.EggsCanvas, "Could not check in egg " + eggName + ". " + SpatialClient2.CHECK_YOUR_INTERNET_CONNECTION, true);
	}

    public IEnumerator hatchEgg(OwnedEgg egg)
    {
		Analytics.CustomEvent("EggHatched", new Dictionary<string, object> {
			{"PlayerId", userId},
			{"Time", DateTime.UtcNow.ToString()},
			{"Location", Input.location.lastData.latitude.ToString() + ", " + Input.location.lastData.longitude.ToString()},
			{"HatchedEgg", egg.Id}
		});
        userSession.User.Metadata.EggsOwned.remove(egg);
        userSession.User.Metadata.Kaiju.hatchEgg(egg);
        CoroutineResponse spritesResponse = new CoroutineResponse();
        StartCoroutine(egg.KaijuEmbryo.initializeSprites(spritesResponse));
        yield return UpdateMetadata(MainMenuScript.EggsCanvas, "Could not hatch egg " + egg.KaijuEmbryo.Name + ". " + CHECK_YOUR_INTERNET_CONNECTION, true);
        while (spritesResponse.Success == null) yield return null;
    }

    public string getNameOfFriend(string friendUserID)
    {
        if (_friends.ContainsKey(friendUserID))
            return _friends[friendUserID].Friend.getName();
        else
            return "";
    }

	public IEnumerator getLocationsForEgg(LocationCombinationData combo, List<HatchLocationMarker> markersToTake, List<GenericLocation> genericLocationsToTake)
	{
		markersToTake.Clear ();
		genericLocationsToTake.Clear ();
		Debug.Log (_locationDatabase.SpecificMarkers.Values.Count);
        foreach (LocationTypeCountTuple locationIndex in combo.GenericLocations)
            genericLocationsToTake.Add(new GenericLocation(locationIndex));
		Debug.Log ("pirates are we");
        List<MarkerWrapper> wrappers = new List<MarkerWrapper>();
        List<CoroutineResponse> responses = new List<CoroutineResponse>();
        foreach (string markerIndex in combo.SpecificMarkers)
        {
			Debug.Log ("marker: " + markerIndex);
            //SpatialMarker marker = _locationDatabase.getSpecificMarkerWithId(markerIndex); // TODO replace by get marker by ID
            CoroutineResponse response = new CoroutineResponse();
            responses.Add(response);
            MarkerWrapper wrapper = new MarkerWrapper();
            wrappers.Add(wrapper);
            StartCoroutine(GetMarkerByID(wrapper, markerIndex, response));
        }
        yield return waitUntilCoroutinesReturn(responses);
        foreach (MarkerWrapper w in wrappers)
            markersToTake.Add(new HatchLocationMarker(w._marker.Name, w._marker.Loc, w._marker.Id));
        yield return null;
    }

    private IEnumerator checkFirstLogin()
    {
        if (userSession.User.Metadata.EggsOwned == null ||
            userSession.User.Metadata.FriendEggsCheckedIn == null ||
            userSession.User.Metadata.StreakMarkers == null ||
            userSession.User.Metadata.ScoreMultiplier == 0 ||
            userSession.User.Metadata.LastRampage == 0 ||
            userSession.User.Metadata.StreakMarkers == null ||
            userSession.User.Metadata.Kaiju == null)
        {
            Debug.Log("first login");
            yield return userSession.User.Metadata.initialize();
            yield return UpdateMetadata(MainMenuScript.LoginCanvas, "Could not create new egg lists on the server. " + CHECK_YOUR_INTERNET_CONNECTION, true);
        }
    }

    private IEnumerator GetMarkerByID(MarkerWrapper wrapper, string markerID, CoroutineResponse response, string projectID = PROJECT_ID)
    {
        response.reset();
        ready = false;

        WWW www = new WWW(string.Format("{0}/v1/marker-by-id?projectId={1}&markerId={2}", baseURL, projectID, markerID));
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error + "\n" + www.text);
            response.setSuccess(false);
        }
        else
        {
            Debug.Log(www.text);
            wrapper._marker = JsonUtility.FromJson<MarkerByIDResponse>(www.text).Marker;
            Debug.Log(wrapper._marker.Name);
            ready = true;
            response.setSuccess(true);
        }
    }

    // May not be used
    public IEnumerator CreateProject(string projName, string projCategory, string email)
    {
        ready = false;

        string url = string.Format("{0}/v1/project", baseURL);

        WWWForm form = new WWWForm();
        form.AddField("name", projName);
        form.AddField("category", projCategory);
        form.AddField("email", email);
        WWW www = new WWW(url, form);
        // yield WWW Continue after a WWW download has completed
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
        }
        else
        {
            ResponseProjectMessage rm = JsonUtility.FromJson<ResponseProjectMessage>(www.text);
            if (rm.Success)
            {
                project = rm.Project;
                ready = true;
                Debug.Log(www.text);
            }
            else
            {
                Debug.Log("Get project info failed.");
            }
        }
    }

    /*
     *  Get all the project information
     */
    public IEnumerator GetProjectInfo(string projectID = PROJECT_ID)
    {
        ready = false;

        string url = string.Format("{0}/v1/project/{1}", baseURL, projectID);
        WWW www = new WWW(url);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
        }
        else
        {
            ResponseProjectMessage rm = JsonUtility.FromJson<ResponseProjectMessage>(www.text);
            if (rm.Success)
            {
                project = rm.Project;
                ready = true;
                Debug.Log(www.text);
            }
            else
            {
                Debug.Log("Get project info failed.");
            }
        }
    }

    // Longitude must be between -180 and 180. latitude must be between -90 and 90.
    public IEnumerator CreateMarker(double latitude, double longitude, string name, string description, MarkerMetadata metadata, CoroutineResponse response, string projectID = PROJECT_ID)
    {
        response.reset();
        ready = false;

        string url = string.Format("{0}/v1/marker", baseURL);

        WWWForm form = new WWWForm();
        form.AddField("longitude", longitude.ToString());
        form.AddField("latitude", latitude.ToString());
        form.AddField("name", name);
        form.AddField("description", description);
        form.AddField("projectId", projectID);
        if (metadata != null)
            form.AddField("metadata", JsonUtility.ToJson(metadata));
        else
            form.AddField("metadata", JsonUtility.ToJson(MarkerMetadata.newGenericMetadata()));

        WWW www = new WWW(url, form);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
            response.setSuccess(false);
        }
        else
        {
            ready = true;
            Debug.Log(www.text);
            response.setSuccess(true);
        }
    }

    /** Emre says: Did not work when I checked. */
    public IEnumerator GetMarkersByMetadataType(Dictionary<string, SpatialMarker> markers, MarkerMetadata.MarkerType type, CoroutineResponse response, string projectID = PROJECT_ID)
    {
        ready = false;

        string url = string.Format("{0}/v1/markers-by-metadata", baseURL);

        WWWForm form = new WWWForm();
        List<MarkersByMetadataRequestTerm> typeList = new List<MarkersByMetadataRequestTerm>();
        typeList.Add(new MarkersByMetadataRequestTerm("type", MarkerMetadata.markerTypeToString(type), false));        
        form.AddField("terms", JsonUtility.ToJson(typeList));
        form.AddField("projectId", projectID);
        
        WWW www = new WWW(url, form);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
            response.setSuccess(false);
        }
        else
        {
            ready = true;
            ResponseMarkersMessage rm = JsonUtility.FromJson<ResponseMarkersMessage>(www.text);
            if (rm.Success)
            {
                markers.Clear();
                foreach (SpatialMarker marker in rm.Markers)
                {
                    markers[marker.Id] = marker;
                }
                ready = true;
                Debug.Log(www.text);
                response.setSuccess(true);
            }
            else
            {
                Debug.Log("Get markers by project failed.");
                response.setSuccess(false);
            }
        }
    }

    public IEnumerator GetMarkersByProject(List<SpatialMarker> markers, CoroutineResponse response, string projectID = PROJECT_ID)
    {
        response.reset();
        ready = false;

        string url = string.Format("{0}/v1/markers-by-project?projectId={1}", baseURL, projectID);
        WWW www = new WWW(url);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
            response.setSuccess(false);
        }
        else
        {
            ResponseMarkersMessage rm = JsonUtility.FromJson<ResponseMarkersMessage>(www.text);
            if (rm.Success)
            {
                markers.Clear();
                markers.AddRange(rm.Markers);
						Debug.Log ("markercount: " + markers.Count.ToString());
                ready = true;
                Debug.Log(www.text);
                response.setSuccess(true);
            }
            else
            {
                Debug.Log("Get markers by project failed.");
                response.setSuccess(false);
            }
        }
    }

    public IEnumerator GetGoogleLocationsByDistance(double latitude, double longitude, double radius, List<BasicMarker> basicMarkers, GenericLocation.GooglePlacesType type, CoroutineResponse response)
    {
        response.reset();
        basicMarkers.Clear();
        string url;
        if (type == GenericLocation.GooglePlacesType.DEFAULT)
            url = string.Format("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius={2}&key={3}", latitude, longitude, radius, GOOGLE_API_KEY);
        else
            url = string.Format("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius={2}&type={3}&rankby=distance&key={4}", latitude, longitude, radius, GenericLocation.googlePlacesTypeToString(type), GOOGLE_API_KEY);
        WWW www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
        }
        else
        {
            NearbySearchResponse rm = JsonUtility.FromJson<NearbySearchResponse>(www.text);
            if (rm.status == "OK")
            {
                foreach (GooglePlaceResult result in rm.results)
                    basicMarkers.Add(new BasicMarker(result.name, new Location(result.geometry.location.lat, result.geometry.location.lng)));
                response.setSuccess(true);
                Debug.Log(www.text);
                Debug.Log(JsonUtility.ToJson(rm.results[0]));
            }
            else
            {
                response.setSuccess(false);
                Debug.Log("Get google places failed. Status: " + rm.status);
            }
        }
    }

    public IEnumerator GetMarkersByDistance(double longitude, double latitude, List<SpatialMarker> markers, CoroutineResponse response, string projectID = PROJECT_ID)
    {
        response.reset();
        ready = false;

        string url = string.Format("{0}/v1/markers-by-distance?projectId={1}&latitude={2}&longitude={3}", baseURL, projectID, latitude, longitude);

        /*WWWForm form = new WWWForm();
        form.AddField("longitude", longitude.ToString());
        form.AddField("latitude", latitude.ToString());
        form.AddField("projectId", projectID);

        WWW www = new WWW(url, form); */
        WWW www = new WWW(url);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
            response.setSuccess(false);
            markers.Clear();
        }
        else
        {
            ResponseMarkersMessage rm = JsonUtility.FromJson<ResponseMarkersMessage>(www.text);
            if (rm.Success)
            {
                markers = rm.Markers;
                ready = true;
                response.setSuccess(true);
                Debug.Log(www.text);
            }
            else
            {
                response.setSuccess(false);
                Debug.Log("Get markers by Distance failed.");
            }
        }

    }

    public IEnumerator GetMarkersByDistance(double longitude, double latitude, double value, bool isMeter, List<SpatialMarker> markers, CoroutineResponse response, string projectID = PROJECT_ID)
    {
        response.reset();
        markers.Clear();
        ready = false;
        string url;

        if (isMeter)
            url = string.Format("{0}/v1/markers-by-distance?projectId={1}&latitude={2}&longitude={3}&meters={4}", baseURL, projectID, latitude, longitude, value);
        else
            url = string.Format("{0}/v1/markers-by-distance?projectId={1}&latitude={2}&longitude={3}&miles={4}", baseURL, projectID, latitude, longitude, value);

        /*WWWForm form = new WWWForm();
        form.AddField("longitude", longitude.ToString());
        form.AddField("latitude", latitude.ToString());
        form.AddField("projectId", projectID);

        if (isMeter)
        {
            form.AddField("meters", value.ToString());
        }
        else
        {
            form.AddField("miles", value.ToString());
        }

        WWW www = new WWW(url, form); */

        WWW www = new WWW(url);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            response.setSuccess(false);
            print(www.error);
        }
        else
        {
            ResponseMarkersMessage rm = JsonUtility.FromJson<ResponseMarkersMessage>(www.text);
            if (rm.Success)
            {
				markers.Clear();
				markers.AddRange(rm.Markers);
                ready = true;
                response.setSuccess(true);
                Debug.Log(www.text);
            }
            else
            {
                response.setSuccess(false);
                Debug.Log("Get markers by distance failed.");
            }
        }
    }

    public IEnumerator DeleteMarkerById(string markerId, string projectID = PROJECT_ID)
    {
        ready = false;

        string url = string.Format("{0}/v1/marker/delete", baseURL);
        Debug.Log(url);

        WWWForm form = new WWWForm();
        form.AddField("markerId", markerId);
        form.AddField("projectId", projectID);

        WWW www = new WWW(url, form);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
        }
        else
        {
            ready = true;
            Debug.Log(www.text);
        }
    }

    //Must login after creation
	public IEnumerator CreateUser(CoroutineResponse response, string userName, string password, string projectID = PROJECT_ID)
    {
		response.reset();
        ready = false;

        string url = baseURL + "/v1/project-user/create-user";
        WWWForm form = new WWWForm();
        form.AddField("username", userName);
        form.AddField("password", password);
        form.AddField("projectId", projectID);
        WWW www = new WWW(url, form);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
			if (www.error.StartsWith("500")) response.setSuccess(false);
        }
        else
        {
            ready = true;
            Debug.Log(www.text);
			response.setSuccess (true);
        }
    }

    public IEnumerator LoginUser(CoroutineResponse response, string userName, string password, string projectID = PROJECT_ID)
    {
        response.reset();
        ready = false;

        string url = baseURL + "/v1/project-user/login-project-user";
        WWWForm form = new WWWForm();
        form.AddField("username", userName);
        form.AddField("password", password);
        form.AddField("projectId", projectID);
        WWW www = new WWW(url, form);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
            if (www.error.StartsWith("400")) response.setSuccess(false);

        }
        else
        {
            Debug.Log(www.text);
            userSession = JsonUtility.FromJson<LoginResponse>(www.text);

            // TODO download the location and kaiju database
            CoroutineResponse markersResponse = new CoroutineResponse();
            //StartCoroutine(GetMarkersByMetadataType(_locationDatabase.SpecificMarkers, MarkerMetadata.MarkerType.CHECK_IN_LOCATION, markersResponse)); TODO uncomment this once this works, or delete it when you can get marker by ID
			StartCoroutine(getCheckInMarkers(markersResponse));

            yield return checkFirstLogin();
            if (StreakNeedsReset) yield return resetStreak();
            streakInitialized = true;
            CoroutineResponse kaijuImageResponse = new CoroutineResponse();
            StartCoroutine(getKaijuImages(kaijuImageResponse));
            CoroutineResponse eggImageResponse = new CoroutineResponse();
            StartCoroutine(getEggImages(eggImageResponse));
            yield return GetFriends();
            List<CoroutineResponse> friendImageResponses = new List<CoroutineResponse>();
            foreach (FriendData fd in _friends.Values)
            {
                CoroutineResponse friendImageResponse = new CoroutineResponse();
                friendImageResponses.Add(friendImageResponse);
                StartCoroutine(getFriendEggImages(friendImageResponse, fd));
            }
            while (kaijuImageResponse.Success == null || eggImageResponse.Success == null || markersResponse.Success == null)
                yield return null;
            yield return waitUntilCoroutinesReturn(friendImageResponses);
            StartCoroutine(checkInternetConnection());
            ready = true;
            response.setSuccess(true);
        }
        // do not call displayError, since that error screen would direct to the main menu instead of the login screen
    }

    public IEnumerator checkIfMarkersExist()
    {
        List<SpatialMarker> allMarkers = new List<SpatialMarker>();

        yield return GetMarkersByDistance(Input.location.lastData.longitude, Input.location.lastData.latitude, MAX_BUILDING_MARKER_DISTANCE, true, allMarkers, new CoroutineResponse());

        foreach (SpatialMarker spatialMarker in allMarkers)
            if (spatialMarker.Metadata.Type == MarkerMetadata.MarkerType.BUILDING_TO_DESTROY) yield break;

        List<BasicMarker> allGoogleMarkers = new List<BasicMarker>();

        yield return GetGoogleLocationsByDistance(Input.location.lastData.latitude, Input.location.lastData.longitude, MAX_BUILDING_MARKER_DISTANCE, allGoogleMarkers, GenericLocation.GooglePlacesType.DEFAULT, new CoroutineResponse());        

        List<CoroutineResponse> responses = new List<CoroutineResponse>();
        foreach (BasicMarker googleMarker in allGoogleMarkers)
        {
            CoroutineResponse markerResponse = new CoroutineResponse();
            StartCoroutine(CreateMarker(googleMarker.Loc.Latitude, googleMarker.Loc.Longitude, googleMarker.Name, "", MarkerMetadata.newBuildingMetadata(), markerResponse));
            responses.Add(markerResponse);
        }
        yield return waitUntilCoroutinesReturn(responses);
    }

	public IEnumerator getCheckInMarkers(CoroutineResponse response)
	{
		List<SpatialMarker> allMarkers = new List<SpatialMarker> ();
		yield return GetMarkersByProject(allMarkers, new CoroutineResponse());
		foreach (SpatialMarker marker in allMarkers) {
					Debug.Log ("markertype: " + marker.Metadata.Type.ToString());
			if (marker.Metadata.Type == MarkerMetadata.MarkerType.CHECK_IN_LOCATION)
				_locationDatabase.SpecificMarkers [marker.Id] = marker;
		}
		Debug.Log ("locdatacount" + _locationDatabase.SpecificMarkers.Count.ToString ());
		response.setSuccess (true);
	}

    public static IEnumerator waitUntilCoroutinesReturn(IEnumerable<CoroutineResponse> responses)
    {
        bool coroutinesGoing = true;
        while (coroutinesGoing)
        {
            yield return null;
            coroutinesGoing = false;
            foreach (CoroutineResponse response in responses)
            {
                if (response.Success == null)
                {
                    coroutinesGoing = true;
                    break;
                }
            }
        }
    }

    public IEnumerator getKaijuImages(CoroutineResponse response)
    {
        response.reset();
        List<CoroutineResponse> responses = new List<CoroutineResponse>();
        foreach (Kaiju k in userSession.User.Metadata.Kaiju)
        {
            CoroutineResponse kaijuImageResponse = new CoroutineResponse();
            responses.Add(kaijuImageResponse);
            StartCoroutine(k.initializeSprites(kaijuImageResponse));
        }
        yield return waitUntilCoroutinesReturn(responses);
        response.setSuccess(true);
    }

    public IEnumerator getEggImages(CoroutineResponse response)
    {
        response.reset();
        List<CoroutineResponse> responses = new List<CoroutineResponse>();
        foreach (OwnedEgg e in userSession.User.Metadata.EggsOwned)
        {
            CoroutineResponse eggImageResponse = new CoroutineResponse();
            responses.Add(eggImageResponse);
            StartCoroutine(e.initializeSprite(eggImageResponse));
        }
        yield return waitUntilCoroutinesReturn(responses);
        response.setSuccess(true);
    }

    public IEnumerator getFriendEggImages(CoroutineResponse response, FriendData friend)
    {
        response.reset();
        List<CoroutineResponse> responses = new List<CoroutineResponse>();
        foreach (OwnedEgg e in friend.Friend.Metadata.EggsOwned)
        {
            CoroutineResponse friendEggImageResponse = new CoroutineResponse();
            responses.Add(friendEggImageResponse);
            StartCoroutine(e.initializeSprite(friendEggImageResponse));
        }
        yield return waitUntilCoroutinesReturn(responses);
        response.setSuccess(true);
    }

    public IEnumerator AddFriend(string friendID, string projectID = PROJECT_ID)
    {
        MessageController.single.displayWaitScreen(MainMenuScript.FriendsCanvas);
        ready = false;

        string url = baseURL + "/v1/project-friend/add-friend";
        WWWForm form = new WWWForm();
        form.AddField("projectId", projectID);
        form.AddField("friendId", friendID);
        Dictionary<string, string> header = new Dictionary<string, string>();
        header["auth-token"] = userSession.Token;
        WWW www = new WWW(url, form.data, header);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
            MessageController.single.displayError(MainMenuScript.FriendsCanvas, "Could not add friend. Check your internet connection.");
        }
        else
        {
            ready = true;
            Debug.Log(www.text);
            MessageController.single.closeWaitScreen(false);
        }
    }

    public IEnumerator RemoveFriend(string friendID, string token, string projectID = PROJECT_ID)
    {
        MessageController.single.displayWaitScreen(MainMenuScript.FriendsCanvas);
        ready = false;

        string url = baseURL + "/v1/project-friend/remove-friend";
        WWWForm form = new WWWForm();
        form.AddField("projectId", projectID);
        form.AddField("friendId", friendID);
        Dictionary<string, string> header = new Dictionary<string, string>();
        header["auth-token"] = token;
        WWW www = new WWW(url, form.data, header);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
            MessageController.single.displayError(MainMenuScript.FriendsCanvas, "Could not remove friend. Check your internet connection.");
        }
        else
        {
            ready = true;
            Debug.Log(www.text);
            MessageController.single.closeWaitScreen(false);
        }
    }

    public IEnumerator GetFriends(string projectID = PROJECT_ID)
    {
        MessageController.single.displayWaitScreen(null);
        ready = false;
        
        /*WWWForm form = new WWWForm();
        string url = baseURL + "/v1/project-friend/get-friends-by-id";
        form.AddField("userId", userSession.user._id);
        form.AddField("projectId", projectID); 
        WWW www = new WWW(url, form);*/
        string url = baseURL + "/v1/project-friend/get-friends-by-id?userId=" + userSession.User.Id + "&projectId=" + projectID;
        WWW www = new WWW(url);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
            print("friend error text maybe: " + www.text);
            MessageController.single.displayError(null, "Could not get friends. Check your internet connection.");
        }
        else
        {
            FriendList friendList = JsonUtility.FromJson<FriendList>(www.text);
            bool metadataUpdated = false;
            bool ownEggsUpdated = false;
            foreach (FriendData fd in friendList.Friends)
            {
                _friends[fd.Friend.Id] = fd;
                if (fd.Friend.Metadata.EggsOwned == null) fd.Friend.Metadata.initializeEggsOwned();
                // TODO request system for accepting eggs

                /*foreach (OwnedEgg egg in fd.Friend.Metadata.EggsOwned)
                {
                    if (egg._friendUserID == userSession.User.Id)
                    {
                        // TODO request system for accepting eggs
                        userSession.User.Metadata.FriendsEggs.Add(egg);
                        metadataUpdated = true;
                    }
                } */
                if (fd.Friend.Metadata.FriendEggsCheckedIn == null) fd.Friend.Metadata.initializeFriendsEggs();
                foreach (OwnedEgg egg in fd.Friend.Metadata.FriendEggsCheckedIn)
                {
                    if (egg.Id.StartsWith(userSession.User.Id))
                    {
                        userSession.User.Metadata.EggsOwned.checkAndAdd(egg, true);
                        ownEggsUpdated = true;
                    }
                }
            }
            if (ownEggsUpdated) yield return UpdateMetadata(MainMenuScript.EggsCanvas, "Could not refresh your egg list. " + CHECK_YOUR_INTERNET_CONNECTION, true);
            else if (metadataUpdated) yield return UpdateMetadata(MainMenuScript.EggsCanvas, "Could not get egg requests from friends. " + CHECK_YOUR_INTERNET_CONNECTION, true);
            ready = true;
            Debug.Log(www.text);
            MessageController.single.closeWaitScreen(false);
        }
    }
    
    public IEnumerator GetUsersByProject(string projectID = PROJECT_ID)
    {
        ready = false;

        string url = baseURL + "/v1/project-user/get-users-by-project/" + projectID;
        WWW www = new WWW(url);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
        }
        else
        {
            allUser = JsonUtility.FromJson<UserList>(www.text);
            ready = true;
            Debug.Log(www.text);
        }
    }

	public void sendRequestToFriend(FriendData friend)
	{
		if (OwnEggMenuItem.eggToSend.addRequest(friend.Friend.Id))
			StartCoroutine(SpatialClient2.single.UpdateMetadata(MainMenuScript.FriendsCanvas, "Could not send request to " + friend.Friend.getName() + ". " + SpatialClient2.CHECK_YOUR_INTERNET_CONNECTION, true));
		else
			MessageController.single.displayError(MainMenuScript.FriendsCanvas, "You already sent a request to " + friend.Friend.getName() + '!');
	}

    /* public IEnumerator UpdateMetadataWithEggs(string errorText)
    {
        // update the actual eggsOwned list on the metadata
        userSession.User.Metadata.initializeEggsOwned(eggsOwned.Values);
        yield return UpdateMetadata(errorText);
    } */

	private IEnumerator UpdateMetadata(Canvas sender, string errorText, bool displayWaitScreen)
	{
		if (displayWaitScreen) MessageController.single.displayWaitScreen(sender);
        metadataUpdatedSuccessfully = false;
        ready = false;
        Debug.Log("updating user metadata");
        string url = baseURL + "/v1/project-user/update-metadata";
        WWWForm form = new WWWForm();
        Debug.Log(JsonUtility.ToJson(userSession.User.Metadata));
        Debug.Log("token: " + userSession.Token);
        form.AddField("metadata", JsonUtility.ToJson(userSession.User.Metadata));
        form.AddField("projectId", PROJECT_ID);
		Dictionary<string, string> header = new Dictionary<string, string>();
		header["auth-token"] = userSession.Token;
		WWW www = new WWW(url, form.data, header);
		yield return www;

		// Post Process
		if (!string.IsNullOrEmpty(www.error))
		{
            print(www.error);
            print("error text maybe: " + www.text);
            MessageController.single.displayError(sender, errorText);
		}
		else
		{
            metadataUpdatedSuccessfully = true;
            ready = true;
			Debug.Log(www.text);
            Debug.Log("user metadata updated");
			if (displayWaitScreen) MessageController.single.closeWaitScreen(false);
		}
	}
}

[System.Serializable]
public class NearbySearchResponse
{
    //List<string> html_attributions;
    public List<GooglePlaceResult> results;
    public string status;
}

[System.Serializable]
public class GooglePlaceResult
{
    public GooglePlaceGeometry geometry;
    public string name;
}
[System.Serializable]
public class GooglePlaceGeometry
{
    public LocationCoord location;
}

[System.Serializable]
public class LoginResponse
{
    [SerializeField]
	private UserData user;
    public UserData User
    {
        get { return user; }
        private set { user = value; }
    }

    [SerializeField]
	private string token;
    public string Token
    {
        get { return token; }
        private set { token = value; }
    }
}

[System.Serializable]
public class UserData
{
    [SerializeField]
	private string username;
    public string Username
    {
        get { return username; }
        private set
        { username = value; }
    }

    [SerializeField]
    private string _id;
    public string Id
    {
        get
        { return _id; }
        private set
        { _id = value; }
    }

    [SerializeField]
    private string projectId;
	public string ProjectId
    {
        get { return projectId; }
        private set { projectId = value; }
    }
    
    [SerializeField]
    private UserMetadata metadata;
    public UserMetadata Metadata
    {
        get { return metadata; }
        private set { metadata = value; }
    }

    public string getName()
    {
        // TODO change to real name?
        return username;
    }
}

[System.Serializable]
public class UserMetadata// : ISerializationCallbackReceiver
{

    // start time stamp to offset Spatial times by
    // set to UTC March 30th, 8:00 PM
    // DO NOT CHANGE!!
    public static DateTime startTime = new DateTime(2017, 3, 30, 20, 0, 0);

    // interval between first two destructions to trigger a streak
    public const int INITIAL_RAMPAGE_INTERVAL = 300;
    // value to set streakTimerStart to when there is no streak
    public const int NO_STREAK = -1;
    // value to set lastRampage when the user has not destroyed anything yet
    public const int NO_RAMPAGE = -1;
    // the furthest distance to check for a kaiju spawn point, in meters
    public const double KAIJU_MARKER_RADIUS = 15000000000.0; // TODO make this smaller after you have more spawn locations around

    [SerializeField]
    private KaijuList kaiju;
    public KaijuList Kaiju
    {
        get { return kaiju; }
        private set { kaiju = value; }
    }

    [SerializeField]
    private EggList eggsOwned;
    public EggList EggsOwned
    {
        get { return eggsOwned; }
        private set { eggsOwned = value; }
    }

    [SerializeField]
    private EggList friendEggsCheckedIn;
    public EggList FriendEggsCheckedIn
    {
        get { return friendEggsCheckedIn; }
        private set { friendEggsCheckedIn = value; }
    }

    [SerializeField]
    private int eggsCreated;
    public int EggsCreated
    {
        get { return eggsCreated; }
        private set { eggsCreated = value; }
    }

    // START OF EMRE'S CODE
    [SerializeField]
    private int score;
    public int Score
    {
        get { return score; }
        private set { score = value; }
    }

    [SerializeField]
    private long lastRampage;
    public long LastRampage
    {
        get { return lastRampage; }
        private set { lastRampage = value; }
    }

    [SerializeField]
    // the value of the streak timer at the start in seconds
    // should be updated with each rampage reset
    private int streakTimerStart;
    public int StreakTimerStart
    {
        get { return streakTimerStart; }
        private set { streakTimerStart = value; }
    }

    [SerializeField]
    // the streak combo multiplier
    private int scoreMultiplier;
    public int ScoreMultiplier
    {
        get { return scoreMultiplier; }
        private set { scoreMultiplier = value; }
    }

    [SerializeField]
    // the streak path, containing marker ids
    private StreakPath streakMarkers;
    public StreakPath StreakMarkers
    {
        get { return streakMarkers; }
        private set { streakMarkers = value; }
    }

    public static long currentTimestamp()
    {
        return Convert.ToInt64(DateTime.UtcNow.Subtract(startTime).TotalSeconds);
    }

    public void updateLastRampage(int scoreIncrement, string newMarkerId)
    {
        lastRampage = currentTimestamp();
        score += scoreIncrement;
        if (streakTimerStart <= 0 || streakTimerStart == NO_STREAK)
            streakTimerStart = INITIAL_RAMPAGE_INTERVAL;
        else
            streakTimerStart *= 2;
        scoreMultiplier++;
        streakMarkers.addMarkerId(newMarkerId);
    }

    public void resetStreak()
    {
        streakTimerStart = NO_STREAK;
        streakMarkers.resetPath();
        score = 0;
        scoreMultiplier = 1;
    }
    // END OF EMRE'S CODE

    /*public void initializeEggsOwned(IEnumerable<OwnedEgg> eggs)
    {
        eggsOwned = new EggList(eggs);
    } */

    public IEnumerator initialize()
    {
        eggsOwned = new EggList();
        friendEggsCheckedIn = new EggList();
        eggsCreated = 0;
        lastRampage = NO_RAMPAGE;
        score = 0;
        scoreMultiplier = 1;
        streakMarkers = new StreakPath();
        streakTimerStart = NO_STREAK;

        // initialize kaiju list
        List<SpatialMarker> markersAround = new List<SpatialMarker>();
        CoroutineResponse response = new CoroutineResponse();
        yield return SpatialClient2.single.GetMarkersByDistance(Input.location.lastData.longitude, Input.location.lastData.latitude, KAIJU_MARKER_RADIUS, true, markersAround, response);
		Debug.Log (markersAround.Count);
        kaiju = new KaijuList(SpatialClient2.single.randomKaijuFromMarkers(markersAround));
    }

    public void initializeEggsOwned()
    {
        eggsOwned = new EggList();
    } 

    public void initializeFriendsEggs()
    {
        friendEggsCheckedIn = new EggList();
    }

    public void incrementEggsCreated()
    {
        eggsCreated++;
    }

    /*public void OnBeforeSerialize() {}

    public void OnAfterDeserialize()
    {
        if (eggsOwned == null) eggsOwned = new EggList();
        if (friendEggsCheckedIn == null) friendEggsCheckedIn = new EggList();
        if (lastRampage == 0) lastRampage = NO_RAMPAGE;
        if (streakMarkers == null) streakMarkers = new StreakPath();
        if (kaiju == null) kaiju = new KaijuList();
        if (scoreMultiplier == 0) scoreMultiplier = 1;
        if (streakTimerStart == 0) streakTimerStart = NO_STREAK;
    } */
}

[System.Serializable]
public class ResponseMarkersMessage
{
    [SerializeField]
    private bool success;
    public bool Success
    {
        get { return success; }
        private set { success = value; }
    }

    [SerializeField]
    private List<SpatialMarker> markers = new List<SpatialMarker> { };
    public List<SpatialMarker> Markers
    {
        get { return markers; }
        private set { markers = value; }
    }
}

[System.Serializable]
public class BasicMarker
{
    // maximum distance that two markers that are supposed to be the same place can be, in meters
    public const double MINIMUM_MARKER_SEPARATION = 50.0;

    [SerializeField]
    protected string name;
    public string Name
    {
        get { return name; }
        private set { name = value; }
    }

    [SerializeField]
    protected Location loc;
    public Location Loc
    {
        get { return loc; }
        private set { loc = value; }
    }

    public BasicMarker(string nm, Location location)
    {
        loc = location;
        name = nm;
    }

    public bool isSame(BasicMarker bm)
    {
        return (bm.name.ToLower() == name.ToLower()) || Geography.withinDistance(bm.loc.Latitude, bm.loc.Longitude, loc.Latitude, loc.Longitude, MINIMUM_MARKER_SEPARATION);
    }
}

[System.Serializable]
public class MarkerWithSpatialId : BasicMarker
{
    [SerializeField]
    private string _id;
    public string Id
    {
        get { return _id; }
        private set { _id = value; }
    }

    [SerializeField]
    protected string description;
    public string Description
    {
        get { return description; }
        private set { description = value; }
    }

    public MarkerWithSpatialId(string nm, Location location, string id) : base(nm, location) {
        _id = id;
    }
}

[System.Serializable]
public class HatchLocationMarker : MarkerWithSpatialId, CheckInPlace
{
    [SerializeField]
    private bool visited;

    public HatchLocationMarker(string nm, Location location, string id) : base(nm, location, id)
    {
        visited = false;
    }

    public string getDescriptor()
    {
        return name;
    }

    public bool needToBeVisited()
    {
        return !visited;
    }

    public void markVisited()
    {
        visited = true;
    }
}


[System.Serializable]
public class SpatialMarker : MarkerWithSpatialId
{
    [SerializeField]
    private string projectId;
    public string ProjectId
    {
        get { return projectId; }
        private set { projectId = value; }
    }

    [SerializeField]
    private MarkerMetadata metadata;

    private SpatialMarker(string nm, Location location, string id) : base(nm, location, id) { }

    public MarkerMetadata Metadata
    {
        get { return metadata; }
        private set { metadata = value; }
    }
}

[System.Serializable]
public class Location
{
    public const int LATITUDE_INDEX = 1;
    public const int LONGITUDE_INDEX = 0;

    [SerializeField]
    private string type;
    public string Type
    {
        get { return type; }
        private set { type = value; }
    }

    [SerializeField]
    private List<double> coordinates;
    public double Latitude
    {
        get { return coordinates[LATITUDE_INDEX]; }
        private set { coordinates[LATITUDE_INDEX] = value; }
    }
    public double Longitude
    {
        get { return coordinates[LONGITUDE_INDEX]; }
        private set { coordinates[LONGITUDE_INDEX] = value; }
    }

    public Location(double lat, double lng, string t = "")
    {
        type = t;
        coordinates = new List<double>();
        coordinates.Add(lng);
        coordinates.Add(lat);
    }
}


[System.Serializable]
public class ResponseProjectMessage
{
    [SerializeField]
    private bool success;
    public bool Success
    {
        get { return success; }
        private set { success = value; }
    }

    [SerializeField]
    private Project project;
    public Project Project
    {
        get { return project; }
        private set { project = value; }
    }
}

[System.Serializable]
public class Project
{
    [SerializeField]
    private string name;
    public string Name
    {
        get { return name; }
        private set { name = value; }
    }

    [SerializeField]
    private string category;
    public string Category
    {
        get { return category; }
        private set { category = value; }
    }

    [SerializeField]
    private string email;
    public string Email
    {
        get { return email; }
        private set { email = value; }
    }

    [SerializeField]
    private string _id;
    public string Id
    {
        get { return _id; }
        private set { _id = value; }
    }

    public bool __v;   // not sure int or float or bool
}

[System.Serializable]
public class UserList
{
    [SerializeField]
    private List<UserData> users;
    public List<UserData> Users
    {
        get { return users; }
        private set { users = value; }
    }
}

[System.Serializable]
public class FriendData
{
    [SerializeField]
    private string _id;
    public string Id
    {
        get { return _id; }
        private set { _id = value; }
    }

    [SerializeField]
    private UserData friend;
    public UserData Friend
    {
        get { return friend; }
        private set { friend = value; }
    }
}

[System.Serializable]
public class FriendList
{
    [SerializeField]
    private List<FriendData> friends;
    public List<FriendData> Friends
    {
        get { return friends; }
        private set { friends = value; }
    }
}

[System.Serializable]
public class ImageBounds
{
    [SerializeField]
    private double north;
    public double North
    {
        get { return north; }
        private set { north = value; }
    }

    [SerializeField]
    private double south;
    public double South
    {
        get { return south; }
        private set { south = value; }
    }

    [SerializeField]
    private double east;
    public double East
    {
        get { return east; }
        private set { east = value; }
    }

    [SerializeField]
    private double west;
    public double West
    {
        get { return west; }
        private set { west = value; }
    }

    public ImageBounds(double n, double s, double e, double w)
    {
        north = n;
        south = s;
        east = e;
        west = w;
    }
}

[System.Serializable]
public class MarkerMetadata
{
    private const string MAP_OVERLAY = "overlay";
    private const string BUILDING_TO_DESTROY = "building";
    private const string CHECK_IN_LOCATION = "checkin";
    private const string KAIJU = "kaiju";

    public enum MarkerType
    {
        GENERIC,
        MAP_OVERLAY,
        BUILDING_TO_DESTROY,
        KAIJU,  // both for kaiju spawn points and for generic locations
        CHECK_IN_LOCATION
    }

    [SerializeField]
    private string type;

    public MarkerType Type
    {
        get
        {
            return stringToMarkerType(type);
        }
        private set
        {
            type = markerTypeToString(value);
        }
    }

    [SerializeField]
    private string destroyedImagePath;
    [SerializeField]
    private string intactImagePath;
    [SerializeField]
    private ImageBounds imageBounds;
    [SerializeField]
    private KaijuFrequencyList kaiju;
    public KaijuFrequencyList Kaiju
    {
        get { return kaiju; }
        private set { kaiju = value; }
    }

    [SerializeField]
    private LocationFrequencyList locations;
    public LocationFrequencyList Locations
    {
        get { return locations; }
        private set { locations = value; }
    }

    private MarkerMetadata(MarkerType t)
    {
        Type = t;
    }

    // TODO figure the fields out

    public static MarkerMetadata newMapOverlayMetadata(string imagePath, ImageBounds bounds)
    {
        MarkerMetadata mm = new MarkerMetadata(MarkerType.MAP_OVERLAY);
        mm.intactImagePath = imagePath;
        mm.imageBounds = bounds;
        return mm;
    }

    public static MarkerMetadata newBuildingMetadata(string intactImage, string destroyedImage, ImageBounds bounds)
    {
        MarkerMetadata mm = new MarkerMetadata(MarkerType.BUILDING_TO_DESTROY);
        mm.intactImagePath = intactImage;
        mm.imageBounds = bounds;
        return mm;
    }

    public static MarkerMetadata newBuildingMetadata()
    {
        return new MarkerMetadata(MarkerType.BUILDING_TO_DESTROY);
    }

    public static MarkerMetadata newCheckInLocationMetadata()
    {
        return new MarkerMetadata(MarkerType.CHECK_IN_LOCATION);
    }

    public static MarkerMetadata newGenericMetadata()
    {
        return new MarkerMetadata(MarkerType.GENERIC);
    }

    public static MarkerMetadata newKaijuSpawnPointMetadata(KaijuFrequencyList kaijuList, LocationFrequencyList locationList)
    {
        MarkerMetadata mm = new MarkerMetadata(MarkerType.KAIJU);
        mm.kaiju = kaijuList;
        mm.locations = locationList;
        return mm;
    }

    public static MarkerType stringToMarkerType(string t)
    {
        switch (t)
        {
            case MAP_OVERLAY:
                return MarkerType.MAP_OVERLAY;
            case BUILDING_TO_DESTROY:
                return MarkerType.BUILDING_TO_DESTROY;
            case CHECK_IN_LOCATION:
                return MarkerType.CHECK_IN_LOCATION;
            case KAIJU:
                return MarkerType.KAIJU;
            default:
                return MarkerType.GENERIC;
        }
    }

    public static string markerTypeToString(MarkerType t)
    {
        switch (t)
        {
            case MarkerType.MAP_OVERLAY:
                return MAP_OVERLAY;
            case MarkerType.BUILDING_TO_DESTROY:
                return BUILDING_TO_DESTROY;
            case MarkerType.CHECK_IN_LOCATION:
                return CHECK_IN_LOCATION;
            case MarkerType.KAIJU:
                return KAIJU;
            default:
                return "";
        }
    }
}

[System.Serializable]
public abstract class ImmutableList<T> : IEnumerable<T>
{
    protected abstract IEnumerable<T> getList();
    protected abstract void createEmptyList();
    protected abstract void createList(IEnumerable<T> items);

    public ImmutableList()
    {
        //list = new List<T>();
        createEmptyList();
    }

    public ImmutableList(IEnumerable<T> items)
    {
        //list = new List<T>(items);
        createList(items);
    }

	abstract public IEnumerator<T> GetEnumerator ();

	IEnumerator IEnumerable.GetEnumerator ()
	{
        return GetEnumerator();
    }

    public int Count
    {
		get { return getList().Count(); }
    }
}

[System.Serializable]
public class MarkersByMetadataRequestTerm
{
    [SerializeField]
    private string key;
    public string Key
    {
        get { return key; }
        private set { key = value; }
    }

    [SerializeField]
    private string value;
    public string Value
    {
        get { return value; }
        private set { this.value = value; }
    }

    [SerializeField]
    private bool strict;
    public bool Strict
    {
        get { return strict; }
        private set { strict = value; }
    }

    public MarkersByMetadataRequestTerm(string k, string v, bool s)
    {
        key = k;
        value = v;
        strict = s;
    }
}

[System.Serializable]
public class IdList : ImmutableList<string>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<string> list;
	protected override IEnumerable<string> getList()
    {
        return list;
    }

    protected override void createEmptyList()
    {
        list = new List<string>();
    }

    protected override void createList(IEnumerable<string> items)
    {
        list = new List<string>(items);
    }

	override public IEnumerator<string> GetEnumerator()
	{
		return list.GetEnumerator();
	}

    private HashSet<string> ids;

    public IdList() : base()
    {
        ids = new HashSet<string>();
    }

    public IdList(IEnumerable<string> items) : base(items)
    {
        ids = new HashSet<string>(list);
    }

    public void OnAfterDeserialize()
    {
        ids = new HashSet<string>(list);
    }

    public void OnBeforeSerialize()
    {
        list = new List<string>(ids);
    }

    public bool containsId(string id)
    {
        return ids.Contains(id);
    }

    public void add(string id)
    {
        ids.Add(id);
        list.Add(id);
    }

}

/** An immutable list of eggs. Use in the eggsOwned field. */
[System.Serializable]
public class EggList : ImmutableList<OwnedEgg>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<OwnedEgg> list;
	protected override IEnumerable<OwnedEgg> getList()
    {
        return list;
    }

    protected override void createEmptyList()
    {
        list = new List<OwnedEgg>();
    }

    protected override void createList(IEnumerable<OwnedEgg> items)
    {
        list = new List<OwnedEgg>(items);
    }

	override public IEnumerator<OwnedEgg> GetEnumerator()
	{
		return list.GetEnumerator();
	}

    protected Dictionary<string, OwnedEgg> eggs;

    public EggList() : base()
    {
        eggs = new Dictionary<string, OwnedEgg>();
    }

    public void OnAfterDeserialize()
    {
        eggs = new Dictionary<string, OwnedEgg>();
        foreach (OwnedEgg egg in list)
        {
            Debug.Log(egg.Id);
            eggs[egg.Id] = egg;
        }
        Debug.Log(eggs);
    }

    public void OnBeforeSerialize()
    {
		Debug.Log ("serializing eggs");
        list = new List<OwnedEgg>(eggs.Values);
    }

    public void checkAndAdd(OwnedEgg egg, bool ownEgg)
    {
        if (!eggs.ContainsKey(egg.Id))
        {
            if (!(egg.Hatchable && ownEgg)) // if the egg is hatchable and the user doesn't have it, that means the user already hatched the egg themselves
            {
                list.Add(egg);
                eggs[egg.Id] = egg;
            }
        }
        else
            eggs[egg.Id].updateCheckins(egg);
    }

    public void remove(OwnedEgg egg)
    {
        eggs.Remove(egg.Id);
        list.Remove(egg);
    }
}

/** An immutable list of kaiju. */
[System.Serializable]
public class KaijuList : ImmutableList<Kaiju>
{
    [SerializeField]
    private List<Kaiju> list;
	protected override IEnumerable<Kaiju> getList()
    {
        return list;
    }

    protected override void createEmptyList()
    {
        list = new List<Kaiju>();
    }

    protected override void createList(IEnumerable<Kaiju> items)
    {
        list = new List<Kaiju>(items);
    }

	override public IEnumerator<Kaiju> GetEnumerator()
	{
		return list.GetEnumerator();
	}

    public KaijuList(Kaiju firstKaiju) : base()
    {
        list.Add(firstKaiju);
    }

    public void hatchEgg(OwnedEgg egg)
    {
        //egg.KaijuEmbryo.hatch(egg);
        list.Add(egg.KaijuEmbryo);
    }

    // TODO DELETE THIS AFTER SETUP
    /*public void addToKaijuListForMarkerSetup(Kaiju k)
    {
        list.Add(k);
    } */
}

// the streak path, containing marker ids
[System.Serializable]
public class StreakPath : ImmutableList<string>
{
    [SerializeField]
    private List<string> list;
	protected override IEnumerable<string> getList()
    {
        return list;
    }

    protected override void createEmptyList()
    {
        list = new List<string>();
    }

    protected override void createList(IEnumerable<string> items)
    {
        list = new List<string>(items);
    }

	override public IEnumerator<string> GetEnumerator()
	{
		return list.GetEnumerator();
	}

    public void resetPath()
    {
        list.Clear();
    }

    public void addMarkerId(string markerId)
    {
        list.Add(markerId);
    }

	public string this[int index]
	{
		get { return list[index]; }
	}
}

[System.Serializable]
public abstract class FrequencyList<T> : ImmutableList<ItemWithFrequency<T>>
{
    private float totalIndex;

    public FrequencyList(IEnumerable<ItemWithFrequency<T>> items) : base(items)
    {
        totalIndex = 0.0f;
        if (items.Count() == 0)
        {
            Debug.Log("gsdcfg");
            // no kaiju markers around!
            return;
        }
        for (int i = 0; i < getList().Count(); i++)
        {
            getElementAtIndex(i).Index = totalIndex;
            totalIndex += getElementAtIndex(i).Frequency;
        }
    }

    public T randomItem()
    {
        if (getList().Count() == 0) return default(T);
        float index = UnityEngine.Random.Range(0.0f, totalIndex);
        int i;
        for (i = 1; i < getList().Count(); i++)
        {
            if (index < getElementAtIndex(i).Index) return getElementAtIndex(i - 1).getItem();
        }
        // return last element in the kaiju list
        return getElementAtIndex(i - 1).getItem();
    }

    public FrequencyList(List<SpatialMarker> markers) : base()
    {
        Dictionary<SpatialMarker, float> distances = new Dictionary<SpatialMarker, float>();
        foreach (SpatialMarker marker in markers)
        {
            if (marker.Metadata.Type == MarkerMetadata.MarkerType.KAIJU)
                distances[marker] = 1.0f - (float)Math.Sqrt(Geography.getDistanceFromLatLonInM(Input.location.lastData.latitude, Input.location.lastData.longitude, marker.Loc.Latitude, marker.Loc.Longitude) / UserMetadata.KAIJU_MARKER_RADIUS);
        }
        totalIndex = 0.0f;
        if (distances.Count == 0)
        {
			Debug.Log ("gsdfg");
            // no kaiju markers around!
            return;
        }
        int i = 0;
        int j;
        foreach (SpatialMarker marker in distances.Keys)
        {
            addItemsInMarker(marker);
			for (j = i; j < getList().Count(); j++)
                {
					getElementAtIndex(j).Index = totalIndex;
					totalIndex += getElementAtIndex(j).Frequency * distances[marker];
                }
                i = j;
        }
        
    }

    protected abstract void addItemsInMarker(SpatialMarker marker);
	protected abstract ItemWithFrequency<T> getElementAtIndex (int index);
}

[System.Serializable]
public class KaijuFrequencyList : FrequencyList<Kaiju>
{
    [SerializeField]
    private List<KaijuWithFrequency> list;
	protected override IEnumerable<ItemWithFrequency<Kaiju>> getList()
    {
        return list.Cast<ItemWithFrequency<Kaiju>>();
    }

    protected override void createEmptyList()
    {
        list = new List<KaijuWithFrequency>();
    }

    protected override void createList(IEnumerable<ItemWithFrequency<Kaiju>> items)
    {
        list = items.Cast<KaijuWithFrequency>().ToList();
    }

	override public IEnumerator<ItemWithFrequency<Kaiju>> GetEnumerator()
	{
		return list.Cast<ItemWithFrequency<Kaiju>>().GetEnumerator();
	}

	/*override public IEnumerator<KaijuWithFrequency> GetEnumerator()
	{
		return list.GetEnumerator();
	}

	override IEnumerator IEnumerable.GetEnumerator()
	{
		return list.GetEnumerator();
	} */

    public KaijuFrequencyList(IEnumerable<ItemWithFrequency<Kaiju>> items) : base(items) { }
    public KaijuFrequencyList(List<SpatialMarker> markers) : base(markers) { }

    override protected void addItemsInMarker(SpatialMarker marker)
    {
		list.AddRange(marker.Metadata.Kaiju.list);
    }

	override protected ItemWithFrequency<Kaiju> getElementAtIndex(int index)
	{
		return list[index];
	}
}

[System.Serializable]
public class LocationFrequencyList : FrequencyList<LocationCombinationData>
{
    [SerializeField]
    private List<LocationWithFrequency> list;
	protected override IEnumerable<ItemWithFrequency<LocationCombinationData>> getList()
    {
        return list.Cast<ItemWithFrequency<LocationCombinationData>>();
    }

    protected override void createEmptyList()
    {
        list = new List<LocationWithFrequency>();
    }

    protected override void createList(IEnumerable<ItemWithFrequency<LocationCombinationData>> items)
    {
        list = items.Cast<LocationWithFrequency>().ToList();
    }

	override public IEnumerator<ItemWithFrequency<LocationCombinationData>> GetEnumerator()
	{
		return list.Cast<ItemWithFrequency<LocationCombinationData>>().GetEnumerator();
	}

	/*override public IEnumerator<LocationWithFrequency> GetEnumerator()
	{
		return list.GetEnumerator();
	}

	override IEnumerator IEnumerable.GetEnumerator()
	{
		return list.GetEnumerator();
	} */

    public LocationFrequencyList(IEnumerable<ItemWithFrequency<LocationCombinationData>> items) : base(items) { }
    public LocationFrequencyList(List<SpatialMarker> markers) : base(markers) { }

    override protected void addItemsInMarker(SpatialMarker marker)
    {
		list.AddRange(marker.Metadata.Locations.list);
    }

	override protected ItemWithFrequency<LocationCombinationData> getElementAtIndex(int index)
	{
		return list[index];
	}
}

public class MarkerWrapper
{
    public SpatialMarker _marker;
}

public class MarkerByIDResponse
{
    [SerializeField]
    private bool success;
    public bool Success
    {
        get { return success; }
    }

    [SerializeField]
    private List<SpatialMarker> marker;
    public SpatialMarker Marker
    {
        get
        {
            if (marker.Count > 0) return marker[0];
            return null;
        }
    }

}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}