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
    public const string PROJECT_ID = "58b070d3b4c96e00118b66ee"; // new test project ID
    public const string GOOGLE_API_KEY = "AIzaSyCejidwxDYN4APVvtlE7ZPsBtVdhB7JG70";

    public static SpatialClient2 single;

    private LoginResponse userSession = null;
    private LocationDatabase _locationDatabase;

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

    private bool ready = false;
    private UserList allUser = new UserList();
    private Project project;

    private bool metadataUpdatedSuccessfully = false;
    private bool isCheckingStreak = false;
    private bool streakInitialized = false;

    void Start()
    {
        ready = false;
        isCheckingStreak = false;
        _locationDatabase = new LocationDatabase(new Dictionary<string, SpatialMarker>());
        streakInitialized = false;
        metadataUpdatedSuccessfully = false;
        single = this;
        userSession = null;
        //_kaijuDatabase = new KaijuDatabase();
    }

    private void Update()
    {
        if (streakInitialized && !isCheckingStreak
            && userSession.User.Metadata.StreakTimerStart != UserMetadata.NO_STREAK)
            StartCoroutine(checkIfStreakIsOutdated());
    }

    public bool isLoggedIn()
    {
        return userSession != null;
    }    

    public string getStreakPathAsJsonString()
    {
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
        yield return UpdateMetadata(null, "Could not update score. " + CHECK_YOUR_INTERNET_CONNECTION);
        Debug.Log("reset streak");
    }


    public IEnumerator updateLastRampage(int scoreIncrement, string markerId)
    {
        userSession.User.Metadata.updateLastRampage(scoreIncrement, markerId);
        yield return UpdateMetadata(null, "Could not update score. " + CHECK_YOUR_INTERNET_CONNECTION);
    }

    public IEnumerator updateLastRampageWithMultiplier(int scoreIncrement, string markerId)
    {
        userSession.User.Metadata.updateLastRampage(scoreIncrement * userSession.User.Metadata.ScoreMultiplier, markerId);
        yield return UpdateMetadata(null, "Could not update score. " + CHECK_YOUR_INTERNET_CONNECTION);
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
        userSession.User.Metadata.EggsOwned.checkAndAdd(egg);
        yield return UpdateMetadata(null, "Could not add egg " + egg.Name + ". " + CHECK_YOUR_INTERNET_CONNECTION);
    }

    public IEnumerator addOrUpdateEggInFriendsEggs(OwnedEgg egg)
    {
        userSession.User.Metadata.FriendEggsCheckedIn.checkAndAdd(egg);
        yield return UpdateMetadata(MainMenuScript.EggsCanvas, "Could not add egg " + egg.Name + " to the list of eggs you are holding onto from your friends. " + CHECK_YOUR_INTERNET_CONNECTION);
    }

    public IEnumerator hatchEgg(OwnedEgg egg)
    {
		Analytics.CustomEvent("EggHatched", new Dictionary<string, object> {
			{"PlayerId", userId},
			{"Time", DateTime.UtcNow.ToString()},
			{"Location", new List<float>{Input.location.lastData.longitude, Input.location.lastData.latitude}},
			{"HatchedEgg", egg}
		});
        userSession.User.Metadata.EggsOwned.remove(egg);
        userSession.User.Metadata.Kaiju.hatchEgg(egg);
        CoroutineResponse spritesResponse = new CoroutineResponse();
        StartCoroutine(egg.KaijuEmbryo.initializeSprites(spritesResponse));
        yield return UpdateMetadata(MainMenuScript.EggsCanvas, "Could not hatch egg " + egg.Name + ". " + CHECK_YOUR_INTERNET_CONNECTION);
        while (spritesResponse.Success == null) yield return null;
    }

    public string getNameOfFriend(string friendUserID)
    {
        if (_friends.ContainsKey(friendUserID))
            return _friends[friendUserID].Friend.getName();
        else
            return "";
    }

    public LocationCombination getLocationsForEgg(LocationCombinationData combo)
    {
        List<HatchLocationMarker> markersToTake = new List<HatchLocationMarker>();
        List<GenericLocation> genericLocationsToTake = new List<GenericLocation>();
        foreach (LocationTypeCountTuple locationIndex in combo.GenericLocations)
            genericLocationsToTake.Add(new GenericLocation(locationIndex));
        foreach (string markerIndex in combo.SpecificMarkers)
        {
            SpatialMarker marker = _locationDatabase.getSpecificMarkerWithId(markerIndex);
            markersToTake.Add(new HatchLocationMarker(marker.Name, marker.Loc, marker.Id));
        }
        return new LocationCombination(markersToTake, genericLocationsToTake);
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
            yield return UpdateMetadata(MainMenuScript.LoginCanvas, "Could not create new egg lists on the server. " + CHECK_YOUR_INTERNET_CONNECTION);
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
    public IEnumerator CreateMarker(double latitude, double longitude, string name, string description, MarkerMetadata metadata = null, string projectID = PROJECT_ID)
    {
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
        }
        else
        {
            ready = true;
            Debug.Log(www.text);
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
        string url = string.Format("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius={2}&type={3}&key={4}", latitude, longitude, radius, GenericLocation.googlePlacesTypeToString(type), GOOGLE_API_KEY);
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
                markers = rm.Markers;
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
            StartCoroutine(GetMarkersByMetadataType(_locationDatabase.SpecificMarkers, MarkerMetadata.MarkerType.CHECK_IN_LOCATION, markersResponse));
            yield return checkFirstLogin();
            yield return checkIfStreakIsOutdated();
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
            ready = true;
            response.setSuccess(true);
        }
        // do not call displayError, since that error screen would direct to the main menu instead of the login screen
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

    public IEnumerator checkIfStreakIsOutdated()
    {
        isCheckingStreak = true;
        if (getTimer() <= 0)
        {
            yield return resetStreak();
            isCheckingStreak = false;
            yield break;
        }
        // waits for the remaining amount on timer + 10 seconds, then calls this coroutine again
        StartCoroutine(waitBeforeCheckStreak());
    }

    public IEnumerator waitBeforeCheckStreak()
    {
        yield return new WaitForSeconds(getTimer() + 10);
        StartCoroutine(checkIfStreakIsOutdated());
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
            MessageController.single.closeWaitScreen(true);
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
            MessageController.single.closeWaitScreen(true);
        }
    }

    public IEnumerator GetFriends(string projectID = PROJECT_ID)
    {
        MessageController.single.displayWaitScreen(MainMenuScript.EggsCanvas);
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
            MessageController.single.displayError(MainMenuScript.EggsCanvas, "Could not get friends. Check your internet connection.");
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
                        userSession.User.Metadata.EggsOwned.checkAndAdd(egg);
                        ownEggsUpdated = true;
                    }
                }
            }
            if (ownEggsUpdated) yield return UpdateMetadata(MainMenuScript.EggsCanvas, "Could not refresh your egg list. " + CHECK_YOUR_INTERNET_CONNECTION);
            else if (metadataUpdated) yield return UpdateMetadata(MainMenuScript.EggsCanvas, "Could not get egg requests from friends. " + CHECK_YOUR_INTERNET_CONNECTION);
            ready = true;
            Debug.Log(www.text);
            MessageController.single.closeWaitScreen(true);
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

    /* public IEnumerator UpdateMetadataWithEggs(string errorText)
    {
        // update the actual eggsOwned list on the metadata
        userSession.User.Metadata.initializeEggsOwned(eggsOwned.Values);
        yield return UpdateMetadata(errorText);
    } */

	public IEnumerator UpdateMetadata(Canvas sender, string errorText)
	{
        MessageController.single.displayWaitScreen(sender);
        metadataUpdatedSuccessfully = false;
        ready = false;
        Debug.Log("updating user metadata");
        string url = baseURL + "/v1/project-user/update-metadata";
        WWWForm form = new WWWForm();

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
            MessageController.single.displayError(sender, errorText);
		}
		else
		{
            metadataUpdatedSuccessfully = true;
            ready = true;
			Debug.Log(www.text);
            Debug.Log("user metadata updated");
            MessageController.single.closeWaitScreen(true);
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
    public const double KAIJU_MARKER_RADIUS = 1500.0;

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
        kaiju = new KaijuList((new KaijuFrequencyList()).randomKaiju(markersAround));
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
    private double north;
    public double North
    {
        get { return north; }
        private set { north = value; }
    }

    private double south;
    public double South
    {
        get { return south; }
        private set { south = value; }
    }

    private double east;
    public double East
    {
        get { return east; }
        private set { east = value; }
    }

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
    [SerializeField]
    protected List<T> list;

    public ImmutableList()
    {
        list = new List<T>();
    }

    public ImmutableList(IEnumerable<T> items)
    {
        list = new List<T>(items);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public int Count
    {
        get { return list.Count; }
    }

    public T this[int index]
    {
        get { return list[index]; }
        // there should be no set!
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
        list = new List<OwnedEgg>(eggs.Values);
    }

    public void checkAndAdd(OwnedEgg egg)
    {
        if (!eggs.ContainsKey(egg.Id))
        {
            list.Add(egg);
            eggs[egg.Id] = egg;
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
    public KaijuList(Kaiju firstKaiju) : base()
    {
        list.Add(firstKaiju);
    }

    public void hatchEgg(OwnedEgg egg)
    {
        egg.KaijuEmbryo.hatch(egg);
        list.Add(egg.KaijuEmbryo);
    }
}

// the streak path, containing marker ids
[System.Serializable]
public class StreakPath : ImmutableList<string>
{
    public void resetPath()
    {
        list.Clear();
    }

    public void addMarkerId(string markerId)
    {
        list.Add(markerId);
    }
}

[System.Serializable]
public abstract class FrequencyList<T> : ImmutableList<ItemWithFrequency<T>>
{
    public FrequencyList(IEnumerable<ItemWithFrequency<T>> items) : base(items) {}

    protected FrequencyList() : base() {}

    public T randomKaiju(List<SpatialMarker> markers)
    {
        list.Clear();
        Dictionary<SpatialMarker, float> distances = new Dictionary<SpatialMarker, float>();
        foreach (SpatialMarker marker in markers)
        {
            if (marker.Metadata.Type == MarkerMetadata.MarkerType.KAIJU)
                distances[marker] = 1.0f - (float)Math.Sqrt(Geography.getDistanceFromLatLonInM(Input.location.lastData.latitude, Input.location.lastData.longitude, marker.Loc.Latitude, marker.Loc.Longitude) / UserMetadata.KAIJU_MARKER_RADIUS);
        }
        if (distances.Count == 0)
        {
            // no kaiju markers around!
            // return STANDARD_KAIJU; TODO uncomment this once we have a standard kaiju.
        }
        float index = 0.0f;
        int i = 0;
        int j;
        foreach (SpatialMarker marker in distances.Keys)
        {
                list.AddRange(itemsInMarker(marker));
                for (j = i; j < list.Count; j++)
                {
                    list[i].Index = index;
                    index += list[i].Frequency * distances[marker];
                }
                i = j;
        }
        index = UnityEngine.Random.Range(0.0f, index);
        for (i = 1; i < list.Count; i++)
        {
            if (index < list[i].Index) return list[i - 1].Item;
        }
        // return last element in the kaiju list
        return list[i - 1].Item;
    }

    protected abstract IEnumerable<ItemWithFrequency<T>> itemsInMarker(SpatialMarker marker);
}

public class KaijuFrequencyList : FrequencyList<Kaiju>
{
    override protected IEnumerable<ItemWithFrequency<Kaiju>> itemsInMarker(SpatialMarker marker)
    {
        return marker.Metadata.Kaiju;
    }
}

public class LocationFrequencyList : FrequencyList<LocationCombinationData>
{
    override protected IEnumerable<ItemWithFrequency<LocationCombinationData>> itemsInMarker(SpatialMarker marker)
    {
        return marker.Metadata.Locations;
    }
}

//[System.Serializable]
//public class Markers
//{
//    public List<Marker> markers = new List<Marker> { };
//}

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