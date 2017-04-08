using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class GenericEggMenuItem : MonoBehaviour {

    public const string COMMA = ", ";

    public const double EGG_SIZE_INCREMENT = 1.0;

    // maximum distance between check in point and user's current point, in meters
    public const double MAX_CHECK_IN_DISTANCE = 400.0;

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
            _eggNameText.text = value.Name;
            // TODO show names of all helper friends
            _friendNameText.text = "";
            bool firstElement = true;
            foreach (string friendUserID in _egg.Helpers)
            {
                if (firstElement)
                    firstElement = false;
                else
                    _friendNameText.text += COMMA;
                _friendNameText.text += SpatialClient2.single.getNameOfFriend(friendUserID);
            }
        }
    }

    public void checkInButtonHandler()
    {
        StartCoroutine(checkInEgg());
    }

    public abstract IEnumerator checkInEgg();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
