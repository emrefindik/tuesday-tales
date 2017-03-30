using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggMenuItem : FriendEggMenuItem
{
    public static OwnedEgg eggToSend;

    public Button _sendToFriendButton;
    
    public void ownEggCheckInButtonHandler()
    {
        StartCoroutine(checkInButtonHandler(MainMenuScript.EggsCanvas));
    }

    // TODO implement this
    public void sendToFriendButtonHandler()
    {
        if (_egg._friendUserID == null)
        {
            Debug.Log("no friend");
            eggToSend = _egg;
            MainMenuScript.EggsCanvas.enabled = false;
            MainMenuScript.FriendsCanvas.enabled = true;
        }
    }
}
