﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OwnEggMenuItem : FriendEggMenuItem
{
    public static OwnedEgg eggToSend;

    /*[SerializeField]
    private Button _sendToFriendButton;
    [SerializeField]
    private Button _hatchButton; */
    [SerializeField]
    private GameObject _hatchPanel;
    [SerializeField]
    private GameObject _checkInPanel;

    private void Start()
    {
        refreshView();
    }

    // TODO implement this
    public void sendToFriendButtonHandler()
    {
        eggToSend = _egg;
        MainMenuScript.EggsCanvas.enabled = false;
        MainMenuScript.FriendsCanvas.enabled = true;
    }
        
    override protected IEnumerator updateServer()
    {
        yield return SpatialClient2.single.UpdateMetadata(MainMenuScript.EggsCanvas, "Could not check in egg " + _egg.Name + ". " + SpatialClient2.CHECK_YOUR_INTERNET_CONNECTION);
    }

    public void onHatch()
    {
        SpatialClient2.single.hatchEgg(_egg);
        Destroy(gameObject);
    }

    override protected void refreshView()
    {
        _hatchPanel.SetActive(_egg.Hatchable);
        _checkInPanel.SetActive(!_egg.Hatchable);
    }
}