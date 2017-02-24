﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpatialClient2 : MonoBehaviour
{

    // Test Project ID: 588fb546604ae700118697c5
    const string baseURL = "https://spatial-api-poc.herokuapp.com";
    public const string projectID = "588fb546604ae700118697c5";
    public List<Marker> markers = new List<Marker> { };
    public Project project;
    public bool ready = false;
    public bool lastStatus = false;

    void Start()
    {
        ready = false;
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
            ReturnProjectMessage rm = JsonUtility.FromJson<ReturnProjectMessage>(www.text);
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
    public IEnumerator GetProjectInfo(string projectId)
    {
        ready = false;

        string url = string.Format("{0}/v1/project/{1}", baseURL, projectId);
        WWW www = new WWW(url);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
        }
        else
        {
            ReturnProjectMessage rm = JsonUtility.FromJson<ReturnProjectMessage>(www.text);
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

    public IEnumerator CreateMarker(string projectID, double longitude, double latitude, string name, string description, Dictionary<string, string> metadata)
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

    public IEnumerator GetMarkersByProject(string projectId)
    {
        ready = false;

        string url = string.Format("{0}/v1/markers-by-project?projectId={1}", baseURL, projectId);
        WWW www = new WWW(url);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
        }
        else
        {
            ReturnMarkersMessage rm = JsonUtility.FromJson<ReturnMarkersMessage>(www.text);
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

    public IEnumerator GetMarkersByDistance(string projectId, double longitude, double latitude)
    {
        ready = false;

        string url = string.Format("{0}/v1/markers-by-distance", baseURL);

        WWWForm form = new WWWForm();
        form.AddField("longitude", longitude.ToString());
        form.AddField("latitude", latitude.ToString());
        form.AddField("projectId", projectId);

        WWW www = new WWW(url, form);
        yield return www;

        // Post Process
        if (!string.IsNullOrEmpty(www.error))
        {
            print(www.error);
        }
        else
        {
            ReturnMarkersMessage rm = JsonUtility.FromJson<ReturnMarkersMessage>(www.text);
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

    public IEnumerator GetMarkersByDistance(string projectId, double longitude, double latitude, double value, bool isMeter)
    {
        ready = false;

        string url = string.Format("{0}/v1/markers-by-distance", baseURL);

        WWWForm form = new WWWForm();
        form.AddField("longitude", longitude.ToString());
        form.AddField("latitude", latitude.ToString());
        form.AddField("projectId", projectId);

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
            ReturnMarkersMessage rm = JsonUtility.FromJson<ReturnMarkersMessage>(www.text);
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

    public IEnumerator DeleteMarkerById(string projectId, string markerId)
    {
        ready = false;

        string url = string.Format("{0}/v1/marker/delete", baseURL);
        Debug.Log(url);

        WWWForm form = new WWWForm();
        form.AddField("markerId", markerId);
        form.AddField("projectId", projectId);

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
}

[System.Serializable]
public class ReturnMarkersMessage
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
public class ReturnProjectMessage
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