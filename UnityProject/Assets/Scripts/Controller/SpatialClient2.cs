﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

public class SpatialClient2 : MonoBehaviour
{
    public const string CHECK_YOUR_INTERNET_CONNECTION = "Check your internet connection.";

    // Test Project ID: 588fb546604ae700118697c5
    public const string baseURL = "https://spatial-api-poc.herokuapp.com";
    public const string PROJECT_ID = "58b070d3b4c96e00118b66ee"; // new test project ID
    public const string GOOGLE_API_KEY = "AIzaSyCejidwxDYN4APVvtlE7ZPsBtVdhB7JG70";

    public static SpatialClient2 single;

    Dictionary<string, OwnedEgg> eggsOwned = new Dictionary<string, OwnedEgg>();
    public IEnumerable<OwnedEgg> EggsOwned
    {
        get
        {
            return eggsOwned.Values;
        }
    }

    private LoginResponse userSession = null;

    private Dictionary<string, FriendData> friends = new Dictionary<string, FriendData>();
    public IEnumerable<FriendData> Friends
    {
        get
        {
            return friends.Values;
        }
    }

    public bool ready = false;
    public UserList allUser = new UserList();
    public Project project;

    public bool metadataUpdatedSuccessfully = false;
    private bool isCheckingStreak = false;
    private bool streakInitialized = false;

    void Start()
    {
        ready = false;
        isCheckingStreak = false;
        streakInitialized = false;
        metadataUpdatedSuccessfully = false;
        single = this;
        userSession = null;

        // delete this. this is for test
        List<BasicMarker> bms = new List<BasicMarker>();
        StartCoroutine(GetGoogleLocationsByDistance(43.44, -79.94, 3000, bms, GenericLocation.GooglePlacesType.STORE, new CoroutineResponse()));
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
        userSession.User.Metadata.resetStreak();
        yield return UpdateMetadata("Could not update score. " + CHECK_YOUR_INTERNET_CONNECTION);
        Debug.Log("reset streak");
    }


    public IEnumerator updateLastRampage(int scoreIncrement, string markerId)
    {
        userSession.User.Metadata.updateLastRampage(scoreIncrement, markerId);
        yield return UpdateMetadata("Could not update score. " + CHECK_YOUR_INTERNET_CONNECTION);
    }

    public IEnumerator updateLastRampageWithMultiplier(int scoreIncrement, string markerId)
    {
        userSession.User.Metadata.updateLastRampage(scoreIncrement * userSession.User.Metadata.ScoreMultiplier, markerId);
        yield return UpdateMetadata("Could not update score. " + CHECK_YOUR_INTERNET_CONNECTION);
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
        eggsOwned[egg.Id] = egg;
        yield return UpdateMetadataWithEggs("Could not add egg " + egg.Name + ". " + CHECK_YOUR_INTERNET_CONNECTION);
    }

    public IEnumerator addEggToFriendsEggs(OwnedEgg egg)
    {
        userSession.User.Metadata.FriendsEggs.Add(egg);
        yield return UpdateMetadata("Could not add egg " + egg.Name + " to the list of eggs you are holding onto from your friends. " + CHECK_YOUR_INTERNET_CONNECTION);
    }

    public string getNameOfFriend(string friendUserID)
    {
        if (friends.ContainsKey(friendUserID))
            return friends[friendUserID].Friend.getName();
        else
            return "";
    }

    private IEnumerator checkFirstLogin()
    {
        if (userSession.User.Metadata.EggsOwned == null ||
            userSession.User.Metadata.FriendsEggs == null ||
            userSession.User.Metadata.StreakMarkers == null ||
            userSession.User.Metadata.ScoreMultiplier == 0)
        {
            Debug.Log("first login");
            userSession.User.Metadata.initialize();
            yield return UpdateMetadata("Could not create new egg lists on the server. " + CHECK_YOUR_INTERNET_CONNECTION);
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
            form.AddField("metadata", JsonUtility.ToJson(new MarkerMetadata(MarkerMetadata.MarkerType.GENERIC)));

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

    public IEnumerator GetMarkersByProject(List<SpatialMarker> markers, string projectID = PROJECT_ID)
    {
        ready = false;

        string url = string.Format("{0}/v1/markers-by-project?projectId={1}", baseURL, projectID);
        WWW www = new WWW(url);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
        }
        else
        {
            ResponseMarkersMessage rm = JsonUtility.FromJson<ResponseMarkersMessage>(www.text);
            if (rm.Success)
            {
                markers = rm.Markers;
                ready = true;
                Debug.Log(www.text);
            }
            else
            {
                Debug.Log("Get markers by project failed.");
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

        string url = string.Format("{0}/v1/markers-by-distance", baseURL);

        WWWForm form = new WWWForm();
        form.AddField("longitude", longitude.ToString());
        form.AddField("latitude", latitude.ToString());
        form.AddField("projectId", projectID);

        WWW www = new WWW(url, form);
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

        string url = string.Format("{0}/v1/markers-by-distance", baseURL);

        WWWForm form = new WWWForm();
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

        WWW www = new WWW(url, form);
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
    public IEnumerator CreateUser(string userName, string password, string projectID = PROJECT_ID)
    {
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
        }
        else
        {
            ready = true;
            Debug.Log(www.text);
        }
    }

    public IEnumerator LoginUser(CoroutineResponse response, string userName, string password, string projectID = PROJECT_ID)
    {
        MainMenuScript.displayWaitScreen();
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
            userSession = JsonUtility.FromJson<LoginResponse>(www.text);
            yield return checkFirstLogin();
            yield return checkIfStreakIsOutdated();
            streakInitialized = true;
            eggsOwned = userSession.User.Metadata.EggsOwned.makeDictionary();
            yield return GetFriends();
            ready = true;
            Debug.Log(www.text);
            response.setSuccess(true);
        }
        // do not call displayError, since that error screen would direct to the main menu instead of the login screen
        MainMenuScript.closeWaitScreen();
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
        MainMenuScript.displayWaitScreen();
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
            MainMenuScript.displayErrorFromWaitScreen("Could not add friend. Check your internet connection.");
        }
        else
        {
            ready = true;
            Debug.Log(www.text);
            MainMenuScript.closeWaitScreen();
        }
    }

    public IEnumerator RemoveFriend(string friendID, string token, string projectID = PROJECT_ID)
    {
        MainMenuScript.displayWaitScreen();
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
            MainMenuScript.displayErrorFromWaitScreen("Could not remove friend. Check your internet connection.");
        }
        else
        {
            ready = true;
            Debug.Log(www.text);
            MainMenuScript.closeWaitScreen();
        }
    }

    public IEnumerator GetFriends(string projectID = PROJECT_ID)
    {
        MainMenuScript.displayWaitScreen();
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
            MainMenuScript.displayErrorFromWaitScreen("Could not get friends. Check your internet connection.");
        }
        else
        {
            FriendList friendList = JsonUtility.FromJson<FriendList>(www.text);
            bool metadataUpdated = false;
            bool ownEggsUpdated = false;
            foreach (FriendData fd in friendList.Friends)
            {
                friends[fd.Friend.Id] = fd;
                if (fd.Friend.Metadata.EggsOwned == null)
                    fd.Friend.Metadata.initializeEggsOwned();
                if (fd.Friend.Metadata.FriendsEggs == null)
                    fd.Friend.Metadata.initializeFriendsEggs();
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
                foreach (OwnedEgg egg in fd.Friend.Metadata.FriendsEggs)
                {
                    if (egg.Id.StartsWith(userSession.User.Id))
                    {
                        if (eggsOwned.ContainsKey(egg.Id))
                        {
                            eggsOwned[egg.Id].updateCheckins(egg);
                        }
                        else
                        {
                            eggsOwned[egg.Id] = egg;
                        }
                        ownEggsUpdated = true;
                    }
                }
            }
            if (ownEggsUpdated) yield return UpdateMetadataWithEggs("Could not refresh your egg list. " + CHECK_YOUR_INTERNET_CONNECTION);
            else if (metadataUpdated) yield return UpdateMetadata("Could not get egg requests from friends. " + CHECK_YOUR_INTERNET_CONNECTION);
            ready = true;
            Debug.Log(www.text);
            MainMenuScript.closeWaitScreen();
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

    public IEnumerator UpdateMetadataWithEggs(string errorText)
    {
        // update the actual eggsOwned list on the metadata
        userSession.User.Metadata.initializeEggsOwned(eggsOwned.Values);
        yield return UpdateMetadata(errorText);
    }

	public IEnumerator UpdateMetadata(string errorText)
	{
        MainMenuScript.displayWaitScreen();
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
            MainMenuScript.displayErrorFromWaitScreen(errorText);
		}
		else
		{
            metadataUpdatedSuccessfully = true;
            ready = true;
			Debug.Log(www.text);
            Debug.Log("user metadata updated");
            MainMenuScript.closeWaitScreen();
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
public class UserMetadata
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

    [SerializeField]
    private KaijuList kaiju;

    [SerializeField]
    private EggList eggsOwned;
    public EggList EggsOwned
    {
        get { return eggsOwned; }
        private set { eggsOwned = value; }
    }

    [SerializeField]
    private List<OwnedEgg> friendsEggs;
    public List<OwnedEgg> FriendsEggs
    {
        get { return friendsEggs; }
        private set { friendsEggs = value; }
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

    public void initializeEggsOwned(IEnumerable<OwnedEgg> eggs)
    {
        eggsOwned = new EggList(eggs);
    }

    public void initialize()
    {
        eggsOwned = new EggList();
        friendsEggs = new List<OwnedEgg>();
        eggsCreated = 0;
        lastRampage = NO_RAMPAGE;
        score = 0;
        scoreMultiplier = 1;
        streakMarkers = new StreakPath();
        streakTimerStart = NO_STREAK;
    }

    public void initializeEggsOwned()
    {
        eggsOwned = new EggList();
    }

    public void initializeFriendsEggs()
    {
        friendsEggs = new List<OwnedEgg>();
    }

    public void incrementEggsCreated()
    {
        eggsCreated++;
    }
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

    override public bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        BasicMarker bm = (BasicMarker)obj;
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

    public MarkerWithSpatialId(string nm, Location location) : base(nm, location) {}
}

[System.Serializable]
public class HatchLocationMarker : MarkerWithSpatialId, CheckInPlace
{
    [SerializeField]
    private bool visited;

    public HatchLocationMarker(string nm, Location location) : base(nm, location)
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

    private SpatialMarker(string nm, Location location) : base(nm, location) { }

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

    public enum MarkerType
    {
        GENERIC,
        MAP_OVERLAY,
        BUILDING_TO_DESTROY,
        CHECK_IN_LOCATION
    }

    [SerializeField]
    private string type;

    public MarkerType Type
    {
        get
        {
            switch (type)
            {
                case MAP_OVERLAY:
                    return MarkerType.MAP_OVERLAY;
                case BUILDING_TO_DESTROY:
                    return MarkerType.BUILDING_TO_DESTROY;
                case CHECK_IN_LOCATION:
                    return MarkerType.CHECK_IN_LOCATION;
                default:
                    return MarkerType.GENERIC;
            }
        }
        set
        {
            switch (value)
            {
                case MarkerType.MAP_OVERLAY:
                    type = MAP_OVERLAY;
                    break;
                case MarkerType.BUILDING_TO_DESTROY:
                    type = BUILDING_TO_DESTROY;
                    break;
                case MarkerType.CHECK_IN_LOCATION:
                    type = CHECK_IN_LOCATION;
                    break;
                case MarkerType.GENERIC:
                    type = "";
                    break;
                default:
                    break;
            }
        }
    }

    [SerializeField]
    private string destroyedImagePath;
    [SerializeField]
    private string intactImagePath;
    [SerializeField]
    private ImageBounds imageBounds;

    // TODO figure the fields out

    public MarkerMetadata(MarkerType t)
    {
        Type = t;
    }
}

/** An immutable list of eggs. Use in the eggsOwned field. */
[System.Serializable]
public class EggList : IEnumerable<OwnedEgg>
{
    [SerializeField]
    private List<OwnedEgg> list;

    public EggList()
    {
        list = new List<OwnedEgg>();
    }

    public EggList(IEnumerable<OwnedEgg> eggs)
    {
        list = new List<OwnedEgg>(eggs);
    }

    public IEnumerator<OwnedEgg> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public Dictionary<string, OwnedEgg> makeDictionary()
    {
        Dictionary<string, OwnedEgg> dict = new Dictionary<string, OwnedEgg>();
        foreach (OwnedEgg egg in list)
        {
            dict[egg.Id] = egg;
        }
        return dict;
    }

    public int length()
    {
        return list.Count;
    }

    public OwnedEgg this[int index]
    {
        get { return list[index]; }
        // there should be no set!
    }
}

/** An immutable list of kaiju. */
[System.Serializable]
public class KaijuList : IEnumerable<Kaiju>
{
    [SerializeField]
    private List<Kaiju> list;

    public KaijuList()
    {
        list = new List<Kaiju>();
    }

    public KaijuList(IEnumerable<Kaiju> kaiju)
    {
        list = new List<Kaiju>(kaiju);
    }

    public IEnumerator<Kaiju> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public int length()
    {
        return list.Count;
    }

    public void hatchEgg(OwnedEgg egg)
    {
        list.Add(egg.KaijuEmbryo);
    }

    public Kaiju this[int index]
    {
        get { return list[index]; }
        // there should be no set!
    }
}

/*[System.Serializable]
public class PathMarker
{
    [SerializeField]
    private string markerId;
    public string Id
    {
        get { return markerId; }
        set { markerId = value; }
    }
        
    private LocationCoord markerLocation;

    [SerializeField]
    public double Latitude
    {
        get { return markerLocation.lat; }
        set { markerLocation.lat = value; }
    }

    [SerializeField]
    public double Longitude
    {
        get { return markerLocation.lon; }
        set { markerLocation.lon = value; }
    }

    public PathMarker(string id, double lat, double lon)
    {
        markerId = id;
        markerLocation = new LocationCoord();
        markerLocation.lat = lat;
        markerLocation.lon = lon;
    }

    public PathMarker(Marker m)
    {

    }
} */

// the streak path, containing marker ids
[System.Serializable]
public class StreakPath : IEnumerable<string>
{
    [SerializeField]
    private List<string> _markerIds;

    public StreakPath()
    {
        _markerIds = new List<string>();
    }

    public StreakPath(IEnumerable<string> markerIds)
    {
        _markerIds = new List<string>(markerIds);
    }

    public IEnumerator<string> GetEnumerator()
    {
        return _markerIds.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _markerIds.GetEnumerator();
    }

    public void resetPath()
    {
        _markerIds.Clear();
    }

    public void addMarkerId(string markerId)
    {
        _markerIds.Add(markerId);
    }

    public int length()
    {
        return _markerIds.Count;
    }

    public string this[int index]
    {
        get { return _markerIds[index]; }
        // there should be no set!
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