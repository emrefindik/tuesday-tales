using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FriendEggMenuItem : GenericEggMenuItem
{
	[SerializeField]
	private Text _friendNameText;

	private FriendData _friend;
	public FriendData Friend
	{
		get { return _friend; }
		set {
			_friend = value;
			_friendNameText.text = "Owner: " + _friend.Friend.getName();
		}
	}

    override protected IEnumerator updateServer()
    {
        _egg.addHelper(SpatialClient2.single.userId);
        yield return SpatialClient2.single.addOrUpdateEggInFriendsEggs(_egg);
    }

    override protected void refreshView()
    {
        if (_egg.Hatchable) Destroy(gameObject);
    }

	override protected void initializeOtherText()
	{

	}

    /*protected IEnumerator checkInButtonHandler(Canvas openCanvas)
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
    } */



}
