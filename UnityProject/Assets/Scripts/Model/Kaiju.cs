using System;
using System.Collections.Generic;
using UnityEngine;

public class Kaiju
{
    [SerializeField]
    private SerializableColor _color;

    private Color _clr;
    public Color MonsterColor
    {
        get
        {
            if (_clr == null)
            {
                _clr = new Color((float)_color.Red, (float)_color.Green, (float)_color.Blue, (float)_color.Alpha);
            }
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

    public Kaiju(Color color, int handType, int headType, int bodyType, string name)
    {
        MonsterColor = color;
        _handType = handType;
        _headType = headType;
        _bodyType = bodyType;
        _name = name;
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