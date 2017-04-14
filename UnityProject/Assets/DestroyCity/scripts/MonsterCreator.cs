using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCreator : MonoBehaviour {

	[HideInInspector]
	public int handType;
	[HideInInspector]
	public int headType;
	[HideInInspector]
	public int eyeType;
	[HideInInspector]
	public int bodyType;

	/*
	public Sprite [] handSprites;
	public Sprite [] headSprites;
	public Sprite [] eyeSprites;
	public Sprite [] bodySprites;
	*/

	public GameObject head;
	public GameObject leftEye;
	public GameObject body;
	public GameObject leftHand;
	public GameObject rightHand;

	Color monsterColor;
	//Color eyeColor;

	// Use this for initialization
	void Start () {
		//createRandomMonster ();
		//loadSprites ();
	}

	void createRandomMonster()
	{
		handType = Random.Range (0, KaijuDatabase.instance.handSprites.Length - 1);
		headType = Random.Range (0, KaijuDatabase.instance.headSprites.Length - 1);
		eyeType = Random.Range (0, KaijuDatabase.instance.eyeSprites.Length - 1);
		bodyType = Random.Range (0, KaijuDatabase.instance.bodySprites.Length - 1);
		monsterColor = Random.ColorHSV (0.0f, 1.0f, 0.5f, 1.0f, 0.6f, 1.0f);
		//eyeColor = Random.ColorHSV (0.0f, 0.5f, 0.8f, 1.0f, 0.9f, 1.0f);
	}

	public void setUpMonster(int headT, int handT, int bodyT, Color mColor)
    {		
		handType = handT;
		headType = headT;
		eyeType = Random.Range (0, KaijuDatabase.instance.eyeSprites.Length - 1);
		bodyType = bodyT;
		monsterColor = mColor;
		loadSprites ();
	}

	public void setUpMonster(Sprite headSprite, Sprite handSprite, Sprite bodySprite, Color mColor)
	{
		SpriteRenderer renderer;
		renderer = head.GetComponent<SpriteRenderer> ();
		renderer.sprite = headSprite;
		renderer.material.color = mColor;

		eyeType = Random.Range (0, KaijuDatabase.instance.eyeSprites.Length - 1);
		renderer = leftEye.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.eyeSprites [eyeType];
		//renderer.material.color = eyeColor;

		renderer = body.GetComponent<SpriteRenderer> ();
		renderer.sprite = bodySprite;
		renderer.material.color = mColor;

		renderer = leftHand.GetComponent<SpriteRenderer> ();
		renderer.sprite = handSprite;
		renderer.material.color = mColor;

		renderer = rightHand.GetComponent<SpriteRenderer> ();
		renderer.sprite = handSprite;
		renderer.color = mColor;

		head.SetActive (true);
		leftEye.SetActive (true);
		body.SetActive (true);
		leftHand.SetActive (true);
		rightHand.SetActive (true);
	}


	void loadSprites()
	{
		SpriteRenderer renderer;
		renderer = head.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.headSprites [headType];
		renderer.material.color = monsterColor;

		renderer = leftEye.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.eyeSprites [eyeType];
		//renderer.material.color = eyeColor;

		renderer = body.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.bodySprites [bodyType];
		renderer.material.color = monsterColor;

		renderer = leftHand.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.handSprites [handType];
		renderer.material.color = monsterColor;

		renderer = rightHand.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.handSprites [handType];
		renderer.color = monsterColor;

		head.SetActive (true);
		leftEye.SetActive (true);
		body.SetActive (true);
		leftHand.SetActive (true);
		rightHand.SetActive (true);
	}

}
