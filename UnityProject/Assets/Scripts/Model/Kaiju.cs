using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Kaiju : ISerializationCallbackReceiver
{
    [SerializeField]
    private SerializableColor _color;

    [SerializeField]
    private int _handType;
	public int HandType
	{
		get { return _handType; }
	}
    public Sprite HandSprite
    {
        get { return KaijuDatabase.instance.HandSprites[_handType]; }
    }

    [SerializeField]
    private int _headType;
	public int HeadType
	{
		get { return _headType; }
	}
    public Sprite HeadSprite
    {
        get { return KaijuDatabase.instance.HeadSprites[_headType]; }
    }

    [SerializeField]
    private int _bodyType;
	public int BodyType
	{
		get { return _bodyType; }
	}
    public Sprite BodySprite
    {
        get { return KaijuDatabase.instance.BodySprites[_bodyType]; }
    }    

    [SerializeField]
    private string _name;
    public string Name
    {
        get { return _name; }
        private set { _name = value; }
    }

    [SerializeField]
    private string _givenName;
    public string GivenName
    {
        get { return _givenName; }
        set { _givenName = value; }
    }

    [SerializeField]
    /** The user IDs (NOT FRIEND ID!) of the friend that helped you hatch this kaiju's egg */
    private IdList _helpers;
    public IEnumerable<string> Helpers
    {
        get {
            if (_helpers != null) return _helpers;
            return Enumerable.Empty<string>();
        }
    }

    private Color _clr;
    public Color MonsterColor
    {
        get
        {
            return _clr;
        }
        private set
        {
            _clr = value;            
            _color.updateValues(value);
        }
    }

    public Kaiju(Color color, int handType, int headType, int bodyType, string name)
    {
        _color = new SerializableColor();
        MonsterColor = color;
        _handType = handType;
        _headType = headType;
        _bodyType = bodyType;
        _name = name;
        _givenName = "";
        _helpers = null; // not an empty list. the list will be created once the egg hatches.
    }

    /** Hatch this kaiju. DOES NOT add it to the user metadata */
    /*public void hatch(OwnedEgg egg)
    {
        if (_helpers == null) _helpers = new IdList(egg.Helpers);
    }

    public bool isHatched()
    {
        return _helpers != null;
    } */

    /** Adds the friend user ID to the list of friends who helped this egg hatch,
      * if that friend does not already exist in that list. */
    public void addHelper(string friendUserId)
    {
        Debug.Log(_helpers.containsId(friendUserId));
        if (!_helpers.containsId(friendUserId)) _helpers.add(friendUserId);
    }

    public IEnumerator initializeSprites(CoroutineResponse response)
    {        
        yield return KaijuDatabase.instance.initialize(_handType, _headType, _bodyType, response);
    }

    public void OnBeforeSerialize()
    {
        _color.updateValues(_clr);
    }

    public void OnAfterDeserialize()
    {
        _clr = new Color((float)_color.Red, (float)_color.Green, (float)_color.Blue, (float)_color.Alpha);
    }
}

[System.Serializable]
public class SerializableColor
{
    [SerializeField]
    private double red;
    public double Red
    {
        get { return red; }
        private set {
            if (value >= 0.0 && value <= 1.0)
                red = value;
            else
                Debug.Log("The red field must be between 0.0 and 1.0. The value was " + value.ToString());
        }
    }

    [SerializeField]
    private double green;
    public double Green
    {
        get { return green; }
        private set
        {
            if (value >= 0.0 && value <= 1.0)
                green = value;
            else
                Debug.Log("The green field must be between 0.0 and 1.0. The value was " + value.ToString());
        }
    }

    [SerializeField]
    private double blue;
    public double Blue
    {
        get { return blue; }
        private set
        {
            if (value >= 0.0 && value <= 1.0)
                blue = value;
            else
                Debug.Log("The blue field must be between 0.0 and 1.0. The value was " + value.ToString());
        }
    }

    [SerializeField]
    private double alpha;
    public double Alpha
    {
        get { return alpha; }
        private set
        {
            if (value >= 0.0 && value <= 1.0)
                alpha = value;
            else
                Debug.Log("The alpha field must be between 0.0 and 1.0. The value was " + value.ToString());
        }
    }

    public void updateValues(Color color)
    {
        red = color.r;
        green = color.g;
        blue = color.b;
        alpha = color.a;
    }
}

[System.Serializable]
public abstract class ItemWithFrequency<T>
{
    [SerializeField]
    protected int _frequency;
    public int Frequency
    {
        get { return _frequency; }
    }

    public abstract T getItem();
    protected abstract void setItem(T item);

    /* Used when picking a random kaiju out of a list.
     * Do not serialize. */
    protected float _index;
    public float Index
    {
        get { return _index; }
        set { _index = value; }
    }

    public ItemWithFrequency(T kaiju, int frequency)
    {
        setItem(kaiju);
        _frequency = frequency;
    }
}

[System.Serializable]
public class KaijuWithFrequency : ItemWithFrequency<Kaiju>
{
    [SerializeField]
    protected Kaiju _item;

    public KaijuWithFrequency(Kaiju kaiju, int frequency) : base(kaiju, frequency) { }

    override public Kaiju getItem()
    {
        return _item;
    }

    override protected void setItem(Kaiju item)
    {
        _item = item;
    }
}

[System.Serializable]
public class LocationWithFrequency : ItemWithFrequency<LocationCombinationData>
{
    [SerializeField]
    protected LocationCombinationData _item;

    public LocationWithFrequency(LocationCombinationData locData, int frequency) : base(locData, frequency) { }

    override public LocationCombinationData getItem()
    {
        return _item;
    }

    override protected void setItem(LocationCombinationData item)
    {
        _item = item;
    }
}
