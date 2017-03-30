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

    /** Sets the friend field of this egg. Called by a FriendMenuItem */
    public void sendToFriend()
    {        
        MainMenuScript.EggsCanvas.enabled = true;
        MainMenuScript.FriendsCanvas.enabled = false;
        EggMenuItem.eggToSend._friendUserID = _friend.Friend.Id;
        //fd.friend.metadata.friendsEggs.Add(_egg); don't do this since we can't update friends' metadata
        StartCoroutine(SpatialClient2.single.UpdateMetadata("Could not send egg " + EggMenuItem.eggToSend._name + " to " + _friend.Friend.getName() + ". " + SpatialClient2.CHECK_YOUR_INTERNET_CONNECTION));
    }
}
