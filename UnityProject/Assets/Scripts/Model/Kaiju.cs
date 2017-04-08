using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Kaiju : ISerializationCallbackReceiver
{
    [SerializeField]
    private SerializableColor _color;

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

    [SerializeField]
    private int _handType;
    [SerializeField]
    private int _headType;
    [SerializeField]
    private int _bodyType;
    // TODO make enums for hand, head and body types

    [SerializeField]
    private string _name;
    public string Name
    {
        get { return _name; }
        private set { _name = value; }
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

    public Kaiju(Color color, int handType, int headType, int bodyType, string name)
    {
        MonsterColor = color;
        _handType = handType;
        _headType = headType;
        _bodyType = bodyType;
        _name = name;
        _helpers = null; // not an empty list. the list will be created once the egg hatches.
    }

    /** Hatch this kaiju. DOES NOT add it to the user metadata */
    public void hatch(OwnedEgg egg)
    {
        if (_helpers == null) _helpers = new IdList(egg.Helpers);
    }

    public bool isHatched()
    {
        return _helpers != null;
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