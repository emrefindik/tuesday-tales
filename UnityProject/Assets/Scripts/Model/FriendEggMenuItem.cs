using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FriendEggMenuItem : MonoBehaviour
{
    public const double EGG_SIZE_INCREMENT = 1.0;

    // maximum distance between check in point and user's current point, in meters
    public const double MAX_CHECK_IN_DISTANCE = 400.0;

    public Image _eggImage;
    public Text _eggNameText;
    public Text _friendNameText;

    public Button _checkInButton;

    protected OwnedEgg _egg;
    public OwnedEgg Egg
    {
        get
        {
            return _egg;
        }
        set
        {
            _egg = value;
            // TODO uncomment this after figuring out how to store the images _eggImage.sprite = e.egg.image;
            _eggNameText.text = value._name;
            if (value._friendUserID != null) _friendNameText.text = SpatialClient2.single.getNameOfFriend(value._friendUserID);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void friendEggCheckInButtonHandler()
    {
        if (_egg._friendUserID != null)
        {
            StartCoroutine(checkInButtonHandler(MainMenuScript.FriendsEggsCanvas));
        }
        else
        {
            MainMenuScript.displayError("Your friend " + SpatialClient2.single.getNameOfFriend(_egg._friendUserID) +
                " is currently holding onto this egg, they have to send it back to you first.");
        }
    }

    protected IEnumerator checkInButtonHandler(Canvas openCanvas)
    {
        foreach (Location loc in _egg._hatchLocations)
        {
            if (Geography.withinDistance(loc.Latitude, loc.Longitude, Input.location.lastData.latitude, Input.location.lastData.longitude, MAX_CHECK_IN_DISTANCE))
            {
                yield return checkInEgg();                
                openCanvas.enabled = false;
                MainMenuScript.CheckedInCanvas.GetComponent<Text>().text = 
                    "Checked in " + _egg._name + " at " + Input.location.lastData.latitude.ToString()
                    + ", " + Input.location.lastData.longitude.ToString();
                MainMenuScript.CheckedInCanvas.enabled = true;
                yield break;
            }
        }
        // could not check in since too far from desired location
        MainMenuScript.displayError("You are too far from a desirable location to check in " + _egg._name + '!');
    }

    public IEnumerator checkInEgg()
    {
        _egg._size += EGG_SIZE_INCREMENT;
        yield return SpatialClient2.single.UpdateMetadata("Could not check in egg " + _egg._name + ". " + SpatialClient2.CHECK_YOUR_INTERNET_CONNECTION);
    }

}
