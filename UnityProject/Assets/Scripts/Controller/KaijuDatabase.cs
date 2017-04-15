using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaijuDatabase : MonoBehaviour {
    public static KaijuDatabase instance;

    private const int NO_DOWNLOAD = -1;

    private const string PHOTO_SERVER_PATH = "http://tuesday-tales.etc.cmu.edu/Photos/";
    private const string HAND_SPRITE_PATH = PHOTO_SERVER_PATH + "hand"; // TODO CHANGE THIS
    private const string HEAD_SPRITE_PATH = PHOTO_SERVER_PATH + "head"; // TODO CHANGE THIS
    //private const string EYE_SPRITE_PATH = PHOTO_SERVER_PATH + "eye"; // TODO CHANGE THIS
    private const string BODY_SPRITE_PATH = PHOTO_SERVER_PATH + "body"; // TODO CHANGE THIS
    private const string EGG_SPRITE_PATH = PHOTO_SERVER_PATH + "egg"; // TODO CHANGE THIS
	private const string PNG = ".png";

    public static string handSpriteAtIndex(int index)
    {
        return HAND_SPRITE_PATH + index.ToString() + PNG;
    }

    public static string headSpriteAtIndex(int index)
    {
        return HEAD_SPRITE_PATH + index.ToString() + PNG;
    }

    public static string bodySpriteAtIndex(int index)
    {
        return BODY_SPRITE_PATH + index.ToString() + PNG;
    }

    /*public static string eyeSpriteAtIndex(int index)
    {
        return EYE_SPRITE_PATH + index.ToString();
    } */

    public static string eggSpriteAtIndex(int index)
    {
        return EGG_SPRITE_PATH + index.ToString() + PNG;
    }

    private Dictionary<int, Sprite> _handSprites;
    public Dictionary<int, Sprite> HandSprites
    {
        get { return _handSprites; }
        private set { _handSprites = value; }
    }

    private Dictionary<int, Sprite> _headSprites;
    public Dictionary<int, Sprite> HeadSprites
    {
        get { return _headSprites; }
        private set { _headSprites = value; }
    }

    private Dictionary<int, Sprite> _bodySprites;
    public Dictionary<int, Sprite> BodySprites
    {
        get { return _bodySprites; }
        private set { _bodySprites = value; }
    }

    private Dictionary<int, Sprite> _eggSprites;
    public Dictionary<int, Sprite> EggSprites
    {
        get { return _eggSprites; }
        private set { _bodySprites = value; }
    }

    private int _handDownloadingIndex;
    private int _headDownloadingIndex;
    private int _bodyDownloadingIndex;
    private int _eggDownloadingIndex;

    void Start()
    {
        instance = this;
        _handDownloadingIndex = NO_DOWNLOAD;
        _headDownloadingIndex = NO_DOWNLOAD;
        _bodyDownloadingIndex = NO_DOWNLOAD;
        _eggDownloadingIndex = NO_DOWNLOAD;
        _eggSprites = new Dictionary<int, Sprite>();
        _bodySprites = new Dictionary<int, Sprite>();
        _headSprites = new Dictionary<int, Sprite>();
        _handSprites = new Dictionary<int, Sprite>();

    }

    private IEnumerator checkAndDownloadHandSprite(int index, CoroutineResponse response)
    {
        response.reset();
        while (_handDownloadingIndex == index) yield return null; // this image is currently being downloaded
        if (_handSprites.ContainsKey(index))
        {
            response.setSuccess(true);
            yield break;
        }
        _handDownloadingIndex = index;
        WWW www = new WWW(handSpriteAtIndex(index));
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
			_handSprites[index] = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
            response.setSuccess(true);
        }
        else
        {
            Debug.Log(www.error);
            MessageController.single.displayError(null, "Failed to load kaiju image. " + SpatialClient2.CHECK_YOUR_INTERNET_CONNECTION);
            response.setSuccess(false);
        }
        _handDownloadingIndex = NO_DOWNLOAD;
    }

    private IEnumerator checkAndDownloadHeadSprite(int index, CoroutineResponse response)
    {
        response.reset();
        while (_headDownloadingIndex == index) yield return null; // this image is currently being downloaded
        if (_headSprites.ContainsKey(index))
        {
            response.setSuccess(true);
            yield break;
        }
        _headDownloadingIndex = index;
        WWW www = new WWW(headSpriteAtIndex(index));
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
			_headSprites[index] = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
            response.setSuccess(true);
        }
        else
        {
            Debug.Log(www.error);
            MessageController.single.displayError(null, "Failed to load kaiju image. " + SpatialClient2.CHECK_YOUR_INTERNET_CONNECTION);
            response.setSuccess(false);
        }
        _headDownloadingIndex = NO_DOWNLOAD;
    }

    private IEnumerator checkAndDownloadBodySprite(int index, CoroutineResponse response)
    {
        response.reset();
        while (_bodyDownloadingIndex == index) yield return null; // this image is currently being downloaded
        if (_bodySprites.ContainsKey(index))
        {
            response.setSuccess(true);
            yield break;
        }
        _bodyDownloadingIndex = index;
        WWW www = new WWW(bodySpriteAtIndex(index));
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
			_bodySprites[index] = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
            response.setSuccess(true);
        }
        else
        {
            Debug.Log(www.error);
            MessageController.single.displayError(null, "Failed to load kaiju image. " + SpatialClient2.CHECK_YOUR_INTERNET_CONNECTION);
            response.setSuccess(false);
        }
        _bodyDownloadingIndex = NO_DOWNLOAD;
    }

    public IEnumerator checkAndDownloadEggSprite(int index, CoroutineResponse response)
    {
        response.reset();
        while (_eggDownloadingIndex == index) yield return null; // this image is currently being downloaded
        if (_eggSprites.ContainsKey(index))
        {
            response.setSuccess(true);
            yield break;
        }
        _eggDownloadingIndex = index;
        WWW www = new WWW(eggSpriteAtIndex(index));
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
			_eggSprites[index] = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
            response.setSuccess(true);
        }
        else
        {
            Debug.Log(www.error);
            MessageController.single.displayError(null, "Failed to load egg image. " + SpatialClient2.CHECK_YOUR_INTERNET_CONNECTION);
            response.setSuccess(false);
        }
        _eggDownloadingIndex = NO_DOWNLOAD;
    }



	// TODO: store the links to the 
	string [] imageLinks;

	public Sprite [] eyeSprites;

	public int generateEgg()
	{
        // Randomly Generate an egg
        //int index = (int)(Random.Range(0, eggSprites.Length-1));
        return Random.Range(1, OwnedEgg.NUMBER_OF_EGG_IMAGES+1);
		// TODO: pop out a ui to input name of the Egg, then call createEggForSelf
	}

	Location[] generateLocations()
	{
		// TODO Generate locations
		return null;
	}

    public IEnumerator initialize(int handSpriteIndex, int headSpriteIndex, int bodySpriteIndex, CoroutineResponse response)
    {
        response.reset();
        CoroutineResponse handResponse = new CoroutineResponse();
        CoroutineResponse bodyResponse = new CoroutineResponse();
        CoroutineResponse headResponse = new CoroutineResponse();

        StartCoroutine(checkAndDownloadBodySprite(bodySpriteIndex, bodyResponse));
        StartCoroutine(checkAndDownloadHandSprite(handSpriteIndex, handResponse));
        StartCoroutine(checkAndDownloadHeadSprite(headSpriteIndex, headResponse));

        while (handResponse.Success == null || bodyResponse.Success == null || headResponse.Success == null)
            yield return null;
        response.setSuccess(true);
    }
}

/* public class KaijuSprites : MonoBehaviour
{
    private Sprite hand;
    public Sprite Hand
    {
        get { return hand; }
        private set { hand = value; }
    }

    private Sprite body;
    public Sprite Body
    {
        get { return body; }
        private set { body = value; }
    }

    private Sprite head;
    public Sprite Head
    {
        get { return head; }
        private set { head = value; }
    }
} */