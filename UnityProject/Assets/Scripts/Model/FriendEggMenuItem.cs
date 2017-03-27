using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendEggMenuItem : MonoBehaviour
{
    public const double EGG_SIZE_INCREMENT = 1.0;

    public Image _eggImage;
    public Text _eggNameText;
    public Text _friendNameText;

    public Button _checkInButton;

    protected OwnedEgg _egg;
    public OwnedEgg Egg
    {
        get
        {
            return _egg;
        }
        set
        {
            _egg = value;
            // TODO uncomment this after figuring out how to store the images _eggImage.sprite = e.egg.image;
            _eggNameText.text = value._egg._name;
            if (value._friendName != null) _friendNameText.text = value._friendName;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void checkInButtonHandler()
    {
        _egg._egg._size += EGG_SIZE_INCREMENT;
        // TODO check position and see if it maps with the egg's intended place of hatching
    }
}
