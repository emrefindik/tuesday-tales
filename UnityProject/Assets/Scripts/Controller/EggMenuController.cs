using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggMenuController : MonoBehaviour {

    public GameObject _eggMenuItemPrefab;
    public Transform _contentPanel;

    private Dictionary<OwnedEgg, GameObject> _eggMenuItems;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void addButtons(List<OwnedEgg> eggs)
    {
        foreach (OwnedEgg e in eggs)
        {
            GameObject eggMenuItem = GameObject.Instantiate(_eggMenuItemPrefab);
            eggMenuItem.transform.SetParent(_contentPanel);
            eggMenuItem.GetComponent<EggMenuItem>().Egg = e; // also updates the egg menu item's view
        }
    }
}
