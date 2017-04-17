using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KaijuScreenController : MonoBehaviour {

    private const float SWIPE_LENGTH_RATIO = 0.5f; // TODO test
    private const float MAX_SWIPE_TIME = 0.3f; // TODO test

    [SerializeField]
    private Transform _kaijuMenuContentPanel;
    [SerializeField]
    private GameObject _kaijuMenuItemPrefab;
    [SerializeField]
    private RectTransform _kaijuMenuViewport;
    //[SerializeField]
    //private GameObject _paddingPanel;

    // swipe detection parameters
    private float _swipeStartPosition;
    private float _swipeStartTime;
    private KaijuMenuData _menuData;

    public Kaiju SelectedKaiju
    {
        get { return _menuData.SelectedKaiju; }
    }

    // Use this for initialization
    void Start () {
		_menuData = new KaijuMenuData(_kaijuMenuContentPanel, Instantiate(_kaijuMenuItemPrefab), _kaijuMenuViewport);
    }
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<Canvas> ().enabled) {
			if (Input.touches.Length > 0) {
				Touch touch = Input.GetTouch (0);
				if (touch.phase == TouchPhase.Began)
					touchStarted (touch.position.x);
				else if (touch.phase == TouchPhase.Ended)
					touchEnded (touch.position.x);
			} else if (Input.GetMouseButtonDown (0))
				touchStarted (Input.mousePosition.x);
			else if (Input.GetMouseButtonUp (0))
				touchEnded (Input.mousePosition.x);
		}
	}

	private void touchStarted(float swipeStartPosition)
    {
        // record start position and time of swipe
		Debug.Log("mouse down");
		_swipeStartPosition = swipeStartPosition;
		_swipeStartTime = Time.time;
    }

	private void touchEnded(float swipeEndPosition)
    {
		Debug.Log("mouse up " + (Time.time - _swipeStartTime).ToString() + ", " + (swipeEndPosition - _swipeStartPosition).ToString());
        if (Time.time - _swipeStartTime <= MAX_SWIPE_TIME)
        {
            float minimumSwipeLength = Screen.width * SWIPE_LENGTH_RATIO;
			if (swipeEndPosition - _swipeStartPosition >= minimumSwipeLength)
            {
                _menuData.previousKaiju();
            }
			else if (swipeEndPosition - _swipeStartPosition <= -minimumSwipeLength)
            {
                _menuData.nextKaiju();
            }
        }
    }

    public void addKaijuMenuItem(Kaiju k)
    {
        _menuData.addKaijuMenuItem(k);
    }

	public void previousKaiju()
	{
		_menuData.previousKaiju ();
	}

	public void nextKaiju ()
	{
		_menuData.nextKaiju ();
	}

}

public class KaijuMenuData
{
    private KaijuMenuState _state;
    private Transform _kaijuMenuContentPanel;
    private List<KaijuMenuItem> _kaijuMenuItems;
    private KaijuMenuItem _firstPaddingKaijuMenuItem;
    private GameObject _lastPadding;
    private GameObject _kaijuMenuItemPrefab;
    public Kaiju SelectedKaiju
    {
        get
        {
            if (_state.KaijuIndex > _kaijuMenuItems.Count - 1) _state.KaijuIndex = _kaijuMenuItems.Count - 1;
			Debug.Log ("Kaiju Index " + _state.KaijuIndex);
            if (_state.KaijuIndex < 0) return null;
            return _kaijuMenuItems[_state.KaijuIndex].Kaiju;
        }
    }

    public KaijuMenuData(Transform kaijuMenuContentPanel, GameObject kaijuMenuItemPrefab, RectTransform viewport)
    {
        _kaijuMenuContentPanel = kaijuMenuContentPanel;
        _kaijuMenuItemPrefab = kaijuMenuItemPrefab;
		Vector3[] corners = new Vector3[4];
		kaijuMenuContentPanel.gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
		Debug.Log ("sizedeltax: " + (corners [2].x - corners [0].x));
		_kaijuMenuItemPrefab.GetComponent<LayoutElement>().minWidth = corners [2].x - corners [0].x;
		float kaijuMenuItemWidth = _kaijuMenuItemPrefab.GetComponent<LayoutElement>().minWidth;
        _state = new KaijuMenuState(_kaijuMenuContentPanel, kaijuMenuItemWidth);        
        _kaijuMenuItems = new List<KaijuMenuItem>();

        GameObject firstPadding = GameObject.Instantiate(_kaijuMenuItemPrefab);
		_firstPaddingKaijuMenuItem = firstPadding.GetComponent<KaijuMenuItem>();
		//firstPadding.GetComponent<RectTransform>().sizeDelta = new Vector2(((viewport.sizeDelta.x - kaijuMenuItemWidth) * 0.5f), _kaijuMenuItemPrefab.GetComponent<RectTransform>().sizeDelta.y);
        /*Vector2 endCorner = new Vector2(1.0f, 1.0f);
        firstPadding.transform.GetChild(0).GetComponent<RectTransform>().pivot = endCorner;
        firstPadding.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = endCorner; */
        firstPadding.transform.SetParent(_kaijuMenuContentPanel, false);

        _lastPadding = GameObject.Instantiate(_kaijuMenuItemPrefab);
        //_lastPadding.GetComponent<RectTransform>().sizeDelta = firstPadding.GetComponent<RectTransform>().sizeDelta;
        /*endCorner.x = 0.0f;
        endCorner.y = 0.0f;
        _lastPadding.transform.GetChild(0).GetComponent<RectTransform>().pivot = endCorner;
        _lastPadding.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = endCorner; */
    }

    public void previousKaiju()
    {
        if (_kaijuMenuItems.Count > 1)
        {
            _state.KaijuIndex = (_state.KaijuIndex  + _kaijuMenuItems.Count - 1) % _kaijuMenuItems.Count;
        }
    }

    public void nextKaiju()
    {
        if (_kaijuMenuItems.Count > 1)
        {
            _state.KaijuIndex = (_state.KaijuIndex + 1) % _kaijuMenuItems.Count;
        }
    }

    public void addKaijuMenuItem(Kaiju k)
    {
        _lastPadding.transform.SetParent(null, false);
        GameObject kaijuMenuItem = GameObject.Instantiate(_kaijuMenuItemPrefab);
        kaijuMenuItem.transform.SetParent(_kaijuMenuContentPanel, false);
        kaijuMenuItem.GetComponent<KaijuMenuItem>().Kaiju = k;
        _kaijuMenuItems.Add(kaijuMenuItem.GetComponent<KaijuMenuItem>());
        _state.KaijuIndex = _kaijuMenuItems.Count - 1;
        if (_kaijuMenuItems.Count >= 2)
        {
			if (_kaijuMenuItems.Count == 2) {
				_lastPadding.GetComponent<KaijuMenuItem> ().Kaiju = _kaijuMenuItems [0].Kaiju;
			}
			_firstPaddingKaijuMenuItem.Kaiju = _kaijuMenuItems[_kaijuMenuItems.Count-1].Kaiju;
        }
        _lastPadding.transform.SetParent(_kaijuMenuContentPanel, false);
    }
}

public class KaijuMenuState
{
    private const int NO_KAIJU = -1;

    private Transform _kaijuMenuContentPanel;
    private float _initialPanelPositionX;
    private Vector3 _panelPosition;
    private float _kaijuMenuItemWidth;
    private int _kaijuIndex;
    public int KaijuIndex
    {
        get
        {
            return _kaijuIndex;
        }
        set
        {
            _kaijuIndex = value;
			_panelPosition.x = _initialPanelPositionX - _kaijuMenuItemWidth * (_kaijuIndex + 1);
            _kaijuMenuContentPanel.localPosition = _panelPosition;
			Debug.Log ("kaiju index; " + _kaijuIndex.ToString ());
        }
    }

    public KaijuMenuState(Transform kaijuMenuContentPanel, float kaijuMenuItemWidth)
    {
        _kaijuMenuItemWidth = kaijuMenuItemWidth;
        _kaijuMenuContentPanel = kaijuMenuContentPanel;
        _panelPosition = kaijuMenuContentPanel.localPosition;
        _initialPanelPositionX = kaijuMenuContentPanel.localPosition.x;
        _kaijuIndex = NO_KAIJU;
    }
}
