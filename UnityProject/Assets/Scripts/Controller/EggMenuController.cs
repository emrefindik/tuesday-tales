using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggMenuController {

    private GameObject _eggMenuItemPrefab;
    private Transform _eggMenuContentPanel;

    private GameObject _friendMenuItemPrefab;
    private Transform _friendMenuContentPanel;

    // Temporary. May get rid of it if we decided to get rid of sending eggs completely
    private Canvas _friendsCanvas;
    public Canvas FriendsCanvas
    {
        get
        {
            return _friendsCanvas;
        }
        private set
        {
            _friendsCanvas = value;
        }
    }

    // Displays all of the player's own eggs
    private Canvas _eggsCanvas;
    public Canvas EggsCanvas
    {
        get
        {
            return _eggsCanvas;
        }
        private set
        {
            _eggsCanvas = value;
        }
    }

    // Displays all the eggs from all the friends
    private Canvas _friendsEggsCanvas;
    public Canvas FriendsEggsCanvas
    {
        get
        {
            return _friendsEggsCanvas;
        }
        private set
        {
            _friendsEggsCanvas = value;
        }
    }

    public static EggMenuController instance;

    public EggMenuItem _refererItem;

    //private Dictionary<OwnedEgg, GameObject> _eggMenuItems;

    public static void createInstance(Canvas eggsCanvas, Canvas friendsCanvas, Canvas friendsEggsCanvas, GameObject eggMenuItemPrefab, Transform eggMenuContentPanel, GameObject friendMenuItemPrefab, Transform friendMenuContentPanel)
    {
        instance = new EggMenuController(eggsCanvas, friendsCanvas, friendsEggsCanvas, eggMenuItemPrefab, eggMenuContentPanel, friendMenuItemPrefab, friendMenuContentPanel);
    }

    private EggMenuController(Canvas eggsCanvas, Canvas friendsCanvas, Canvas friendsEggsCanvas, GameObject eggMenuItemPrefab, Transform eggMenuContentPanel, GameObject friendMenuItemPrefab, Transform friendMenuContentPanel)
    {
        _refererItem = null;
        _eggsCanvas = eggsCanvas;
        _friendsCanvas = friendsCanvas;
        _friendsEggsCanvas = friendsEggsCanvas;
        _eggMenuItemPrefab = eggMenuItemPrefab;
        _eggMenuContentPanel = eggMenuContentPanel;
        _friendMenuItemPrefab = friendMenuItemPrefab;
        _friendMenuContentPanel = friendMenuContentPanel;
    }

    public void addButtons()
    {
        foreach (OwnedEgg e in SpatialClient2.single.userSession.user.metadata.eggsOwned)
        {
            GameObject eggMenuItem = GameObject.Instantiate(_eggMenuItemPrefab);
            eggMenuItem.transform.SetParent(_eggMenuContentPanel);
            eggMenuItem.GetComponent<EggMenuItem>().Egg = e; // also updates the egg menu item's view
        }
        foreach (FriendData fd in SpatialClient2.single.friendList.friends)
        {
            foreach (OwnedEgg e in fd.friend.metadata.eggsOwned)
            {
                GameObject friendEggMenuItem = GameObject.Instantiate(_friendMenuItemPrefab);
                friendEggMenuItem.transform.SetParent(_friendMenuContentPanel);
                friendEggMenuItem.GetComponent<EggMenuItem>().Egg = e; // also updates the egg menu item's view
            }
        }
    }
}
