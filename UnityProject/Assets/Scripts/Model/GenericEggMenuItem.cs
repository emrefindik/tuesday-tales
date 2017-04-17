using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class GenericEggMenuItem : MonoBehaviour {

    public const string COMMA = ", ";

    public const double EGG_SIZE_INCREMENT = 1.0;

    // maximum distance between check in point and user's current point, in meters
    public const double MAX_CHECK_IN_DISTANCE = 400.0;

    [SerializeField]
    private Image _eggImage;
    [SerializeField]
    private Text _eggNameText;
    [SerializeField]
    private Text _checkInLocationText;
    [SerializeField]
    private Button _checkInButton;

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
            _eggImage.sprite = _egg.Sprite;
            _eggNameText.text = _egg.Name;
			resetCheckInLocationText();
			initializeOtherText();
            disableCheckInButton();
        }
    }

	protected void resetCheckInLocationText()
	{
		_checkInLocationText.text = "";
		if (!_egg.Hatchable) {
			foreach (CheckInPlace marker in _egg.PlacesToTake)
				if (marker.needToBeVisited())
					_checkInLocationText.text += marker.getDescriptor () + "\n";
		}
	}

    public void onCheckIn()
    {
        StartCoroutine(checkInButtonHandler());
    }

    private IEnumerator checkInButtonHandler()
    {
        _egg.checkIn();
        yield return updateServer();
		resetCheckInLocationText ();
        refreshView();
    }

    protected abstract IEnumerator updateServer();

    /** Called upon checking in to refresh how the egg menu item looks */
    protected abstract void refreshView();

	protected abstract void initializeOtherText();

    public void enableCheckInButton()
    {
        _checkInButton.gameObject.SetActive(true);
    }

    public void disableCheckInButton()
    {
        _checkInButton.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
