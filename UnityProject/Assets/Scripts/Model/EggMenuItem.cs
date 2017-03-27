using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggMenuItem : FriendEggMenuItem
{
    public Button _sendToFriendButton;

    // TODO implement this
    public void sendToFriendButtonHandler()
    {
        if (_egg._friendID == null)
        {
            Debug.Log("no friend");
            EggMenuController.instance._refererItem = this;
            EggMenuController.instance.EggsCanvas.enabled = false;
            EggMenuController.instance.FriendsEggsCanvas.enabled = false;
            EggMenuController.instance.FriendsCanvas.enabled = true;
        }
    }
}
