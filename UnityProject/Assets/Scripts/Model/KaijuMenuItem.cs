using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KaijuMenuItem : MonoBehaviour
{
    public Text _kaijuNameText;
    public Text _kaijuGivenNameText;
    public Text _kaijuDescriptionText;
    public Image _kaijuImage;

    protected Kaiju _kaiju;
    public Kaiju Kaiju
    {
        get
        {
            return _kaiju;
        }
        set
        {
			Debug.Log ("y");
            _kaiju = value;
			Debug.Log ("yo");
            _kaijuNameText.text = value.Name;
			Debug.Log (value.GivenName);
            _kaijuGivenNameText.text = value.GivenName;
			Debug.Log ("yo2");
            _kaijuImage.sprite = value.HeadSprite;
			Debug.Log ("yo3");
			_kaijuImage.color = value.MonsterColor;
			Debug.Log ("yo4");
            // TODO SET KAIJUIMAGE!!
            // TODO SET KAIJU DESCRIPTION TEXT!
        }
    }
}
