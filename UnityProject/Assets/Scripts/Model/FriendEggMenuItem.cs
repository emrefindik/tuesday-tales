using System.Collections;

public class FriendEggMenuItem : GenericEggMenuItem
{
    override public IEnumerator checkInEgg()
    {
        MessageController.single.displayWaitScreen(MainMenuScript.EggsCanvas);
        _egg.checkIn();
        yield return SpatialClient2.single.addOrUpdateEggInFriendsEggs(_egg);
        MessageController.single.closeWaitScreen();
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
