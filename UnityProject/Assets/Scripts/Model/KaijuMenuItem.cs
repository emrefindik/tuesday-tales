using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KaijuMenuItem : MonoBehaviour
{
    public Text _kaijuNameText;
    public Text _kaijuGivenNameText;
    public Text _kaijuDescriptionText;
    public Image _kaijuHeadImage;
	public Image _kaijuBodyImage;
	public Image _kaijuLHandImage;
	public Image _kaijuRHandImage;

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
            _kaijuHeadImage.sprite = value.HeadSprite;
			//_kaijuImage.color = value.MonsterColor;
			_kaijuHeadImage.color = Color.white;
			_kaijuLHandImage.sprite = value.HandSprite;
			_kaijuRHandImage.sprite = value.HandSprite;
			_kaijuBodyImage.sprite = value.BodySprite;

			//_kaijuImage.color = value.MonsterColor;

            // TODO SET KAIJUIMAGE!!
            // TODO SET KAIJU DESCRIPTION TEXT!
        }
    }
}
