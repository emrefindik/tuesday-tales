using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FriendMenuItem : MonoBehaviour
{
    public Text _friendNameText;

    public Button _selectFriendButton;

    protected FriendData _friend;
    public FriendData Friend
    {
        get
        {
            return _friend;
        }
        set
        {
            _friend = value;
            _friendNameText.text = value.Friend.getName();
        }
    }

    /** Sends an egg check in request to a friend */
    public void sendToFriend()
    {        
        MainMenuScript.EggsCanvas.enabled = true;
        MainMenuScript.FriendsCanvas.enabled = false;
        if (OwnEggMenuItem.eggToSend.addRequest(_friend.Friend.Id))
            StartCoroutine(SpatialClient2.single.UpdateMetadata(MainMenuScript.FriendsCanvas, "Could not send request to " + _friend.Friend.getName() + ". " + SpatialClient2.CHECK_YOUR_INTERNET_CONNECTION));
        else
            MessageController.single.displayError(MainMenuScript.FriendsCanvas, "You already sent a request to " + _friend.Friend.getName() + '!');
    }
}
