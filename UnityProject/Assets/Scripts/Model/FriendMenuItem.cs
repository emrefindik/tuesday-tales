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
            _friendNameText.text = _friend.Friend.getName();
        }
    }

    /** Sends an egg check in request to a friend */
    public void sendToFriend()
    {        
        MainMenuScript.EggsCanvas.enabled = true;
        MainMenuScript.FriendsCanvas.enabled = false;
		SpatialClient2.single.sendRequestToFriend(_friend);
    }
}
