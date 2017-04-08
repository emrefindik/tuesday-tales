using System.Collections;
using UnityEngine.UI;

public class OwnEggMenuItem : FriendEggMenuItem
{
    public static OwnedEgg eggToSend;

    public Button _sendToFriendButton;

    // TODO implement this
    public void sendToFriendButtonHandler()
    {
        eggToSend = _egg;
        MainMenuScript.EggsCanvas.enabled = false;
        MainMenuScript.FriendsCanvas.enabled = true;
    }
        
    override public IEnumerator checkInEgg()
    {
        MessageController.single.displayWaitScreen(MainMenuScript.EggsCanvas);
        _egg.checkIn();
        yield return SpatialClient2.single.UpdateMetadata(MainMenuScript.EggsCanvas, "Could not check in egg " + _egg.Name + ". " + SpatialClient2.CHECK_YOUR_INTERNET_CONNECTION);
        MessageController.single.closeWaitScreen();
    }
}
