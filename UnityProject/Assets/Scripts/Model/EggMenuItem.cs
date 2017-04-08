using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggMenuItem : FriendEggMenuItem
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
}
