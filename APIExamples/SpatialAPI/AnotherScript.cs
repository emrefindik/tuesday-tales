using UnityEngine;
using System.Collections;

public class AnotherScript : MonoBehaviour {

    SpatialClient2 spatialClient;
    GoogleMap gmClient;
    // Use this for initialization
    void Start () {
        spatialClient = GetComponent<SpatialClient2>();
        gmClient = GetComponent<GoogleMap>();
        if (spatialClient != null)
        {
            //spatialClient.GetProjectInfo("588fb546604ae700118697c5");
            //StartCoroutine(spatialClient.GetProjectInfo("588fb546604ae700118697c5"));

            //StartCoroutine(spatialClient.CreateMarker(spatialClient.projectID, 40.433, -79.964, "test", "description", null));

            StartCoroutine(spatialClient.GetMarkersByProject(spatialClient.projectID));
            
        }

	}
	
	// Update is called once per frame
	void Update () {

        if (spatialClient.ready)
        {
            gmClient.swapMarkers(spatialClient.markers);
            spatialClient.ready = false;
        }
    }
}
