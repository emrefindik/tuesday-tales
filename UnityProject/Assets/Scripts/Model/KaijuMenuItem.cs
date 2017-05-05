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
            _kaiju = value;
            _kaijuNameText.text = value.Name;
            _kaijuGivenNameText.text = value.GivenName;
            _kaijuImage.sprite = value.HeadSprite;
			//_kaijuImage.color = value.MonsterColor;
            // TODO SET KAIJU DESCRIPTION TEXT!
        }
    }
}
