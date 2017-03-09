using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggMenuItem : MonoBehaviour
{

    public Image _eggImage;
    public Text _eggNameText;
    public Text _friendNameText;

    // Will figure out later what this button does
    public Button _button;

    private OwnedEgg _egg;
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
            if (value._friend != null) _friendNameText.text = value._friend;
        }
    }

    // Use this for initialization
    void Start()
    {
        _button.onClick.AddListener(buttonHandler);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // TODO implement this
    // Don't know what this does yet! Grow the egg maybe?
    private void buttonHandler()
    {

    }
}
