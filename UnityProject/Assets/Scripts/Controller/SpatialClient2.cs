﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class SpatialClient2 : MonoBehaviour
{

    // Test Project ID: 588fb546604ae700118697c5
    const string baseURL = "https://spatial-api-poc.herokuapp.com";
    public const string PROJECT_ID = "58b070d3b4c96e00118b66ee"; // new test project ID

    public List<Marker> markers = new List<Marker> { };
    public UserList allUser = new UserList();
    public Project project;
    public bool ready = false;
    public bool lastStatus = false;
    public static SpatialClient2 single;
    public LoginResponse userSession = new LoginResponse();

    void Start()
    {
        ready = false;
        single = this;

        // TODO delete this
        StartCoroutine(LoginUser(new CoroutineResponse(), "hello", "hello"));
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
            if (rm.success)
            {
                project = rm.project;
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
            if (rm.success)
            {
                project = rm.project;
                ready = true;
                Debug.Log(www.text);
            }
            else
            {
                Debug.Log("Get project info failed.");
            }
        }
    }

    //Longitude must be between -180 and 180. lattitude must be between -90 and 90.
    public IEnumerator CreateMarker(double longitude, double latitude, string name, string description, Dictionary<string, string> metadata, string projectID = PROJECT_ID)
    {
        ready = false;

        string url = string.Format("{0}/v1/marker", baseURL);

        WWWForm form = new WWWForm();
        form.AddField("longitude", longitude.ToString());
        form.AddField("latitude", latitude.ToString());
        form.AddField("name", name);
        form.AddField("description", description);
        form.AddField("projectId", projectID);
        form.AddField("metadata", JsonUtility.ToJson(metadata));

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

    public IEnumerator GetMarkersByProject(string projectID = PROJECT_ID)
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
            if (rm.success)
            {
                markers = rm.markers;
                ready = true;
                Debug.Log(www.text);
            }
            else
            {
                Debug.Log("Get markers by project failed.");
            }
        }
    }

    public IEnumerator GetMarkersByDistance(double longitude, double latitude, string projectID = PROJECT_ID)
    {
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
            markers.Clear();
        }
        else
        {
            ResponseMarkersMessage rm = JsonUtility.FromJson<ResponseMarkersMessage>(www.text);
            if (rm.success)
            {
                markers = rm.markers;
                ready = true;
                Debug.Log(www.text);
            }
            else
            {
                Debug.Log("Get markers by Distance failed.");
            }
        }

    }

    public IEnumerator GetMarkersByDistance(double longitude, double latitude, double value, bool isMeter, string projectID = PROJECT_ID)
    {
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
            print(www.error);
        }
        else
        {
            ResponseMarkersMessage rm = JsonUtility.FromJson<ResponseMarkersMessage>(www.text);
            if (rm.success)
            {
                markers = rm.markers;
                ready = true;
                Debug.Log(www.text);
            }
            else
            {
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
            ready = true;
            Debug.Log(www.text);
            response.setSuccess(true);
        }
    }

    public IEnumerator AddFriend(string friendID, string token, string projectID = PROJECT_ID)
    {
        ready = false;

        string url = baseURL + "/v1/project-friend/add-friend";
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
        }
        else
        {
            ready = true;
            Debug.Log(www.text);
        }
    }

    public IEnumerator RemoveFriend(string friendID, string token, string projectID = PROJECT_ID)
    {
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
        }
        else
        {
            ready = true;
            Debug.Log(www.text);
        }
    }

    public IEnumerator GetFriends(string userID, string projectID = PROJECT_ID)
    {
        ready = false;

        string url = baseURL + "/v1/project-friend/get-friends-by-id";
        WWWForm form = new WWWForm();
        form.AddField("userId", userID);
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
}


[System.Serializable]
public class LoginResponse{
	public UserData user;
	public string token;
}

[System.Serializable]
public class UserData {
	public string username;
    public string _id;
	public string projectId;
    public bool __v;    // what is __v? will it affect json?
    public UserMetadata metadata; // TODO test whether get user returns metadata in Spatial
}

[System.Serializable]
public class UserMetadata
{
    public List<OwnedEgg> eggsOwned;
    public List<Egg> friendsEggs;
}

[System.Serializable]
public class ResponseMarkersMessage
{
    public bool success;
    public List<Marker> markers = new List<Marker> { };
}

[System.Serializable]
public class Marker
{
    public string _id;
    public string name;
    public string description;
    public string projectId;
    public string metadata;
    public bool __v;   // not sure int or float or bool
    public Location loc;

}

[System.Serializable]
public class Location
{
    public string type;
    public List<double> coordinates;
}


[System.Serializable]
public class ResponseProjectMessage
{
    public bool success;
    public Project project; 
}

[System.Serializable]
public class Project
{
    public bool __v;   // not sure int or float or bool
    public string name;
    public string category;
    public string email;
    public string _id;

}

[System.Serializable]
public class UserList
{
    public List<UserData> users;
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