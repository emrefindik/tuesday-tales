using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** The egg class */
[Serializable]
public class OwnedEgg
{
    public const double STARTING_EGG_SIZE = 1.0;

    /** The user ID (NOT FRIEND ID!) of the friend that is currently holding on to the egg */
    public string _friendUserID;

    /** The egg ID */
    public string _id;

    public string _name;
    public double _size;

    // Locations that this egg can hatch.
    // TODO if we can anyhow convert this to "any beach" as opposed to a list of specific locations, that would be great
    public List<Location> _hatchLocations;

    // TODO add other relevant fields

    private OwnedEgg(string name, string id, List<Location> hatchLocations)
    {
        _name = name;
        _id = id;
        _friendUserID = null;
        _hatchLocations = hatchLocations;
        _size = STARTING_EGG_SIZE;
    }

    /**  */
    public static IEnumerator createEggForSelf(GameObject eggMenuItemPrefab, Transform eggMenuContentPanel, string eggName = "", List<Location> hatchLocations = null)
    {
        if (SpatialClient2.single.isLoggedIn())
        {
            // assign the hexadecimal representation of the egg creation number as custom part of id
            OwnedEgg egg;
            if (hatchLocations == null)
                egg = new OwnedEgg(eggName, SpatialClient2.single.newEggIdForSelf(), new List<Location>());
            else
                egg = new OwnedEgg(eggName, SpatialClient2.single.newEggIdForSelf(), hatchLocations);
            yield return SpatialClient2.single.addEggToSelf(egg);
            GameObject eggMenuItem = GameObject.Instantiate(eggMenuItemPrefab);
            eggMenuItem.transform.SetParent(eggMenuContentPanel, false);
            eggMenuItem.GetComponent<EggMenuItem>().Egg = egg; // also updates the egg menu item's view
        }
        else
        {
            Debug.Log("tried to create egg while not logged in");
        }
    }

    public static IEnumerator createEggForFriend(FriendData friend, string eggName = "", List<Location> hatchLocations = null)
    {
        if (SpatialClient2.single.isLoggedIn())
        {
            OwnedEgg egg;
            // assign the hexadecimal representation of the egg creation number as custom part of id
            if (hatchLocations == null)
                egg = new OwnedEgg(eggName, SpatialClient2.single.newEggIdForFriend(friend), new List<Location>());
            else
                egg = new OwnedEgg(eggName, SpatialClient2.single.newEggIdForFriend(friend), hatchLocations);
            yield return SpatialClient2.single.addEggToFriendsEggs(egg);
        }
        else
        {
            Debug.Log("tried to create egg while not logged in");
        }
    }
}
