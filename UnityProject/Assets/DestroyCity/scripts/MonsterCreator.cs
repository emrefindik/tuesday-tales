using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCreator : MonoBehaviour {

	[HideInInspector]
	public int handType;
	[HideInInspector]
	public int headType;
	//[HideInInspector]
	public int eyeType;
	[HideInInspector]
	public int bodyType;

	/*
	public Sprite [] handSprites;
	public Sprite [] headSprites;
	public Sprite [] eyeSprites;
	public Sprite [] bodySprites;
	*/

	public Sprite eyeSprite;

	public GameObject[] headGameobjects;
	GameObject head;
	public GameObject leftEye;
	public GameObject body;
	public GameObject leftHand;
	public GameObject rightHand;
	public GameObject leftHandIdle;
	public GameObject rightHandIdle;

	Color monsterColor;
	//Color eyeColor;

	void Awake()
	{
	}

	// Use this for initialization
	void Start () {
		//createRandomMonster ();
		//loadSprites ();
	}


	// Note: for this method, there's no way to know which animation needs to be loaded
	/*
	public void setUpMonster(Sprite headSprite, Sprite handSprite, Sprite bodySprite, Color mColor)
	{
		for (int i = 0; i < headGameobjects.Length; i++) {
			headGameobjects [i].SetActive (false);
		}

		// Get index
		head = headGameobjects[]
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

		renderer = leftHandIdle.GetComponent<SpriteRenderer> ();
		renderer.sprite = handSprite;
		renderer.material.color = mColor;

		renderer = rightHand.GetComponent<SpriteRenderer> ();
		renderer.sprite = handSprite;
		renderer.color = mColor;

		renderer = rightHandIdle.GetComponent<SpriteRenderer> ();
		renderer.sprite = handSprite;
		renderer.color = mColor;

		head.SetActive (true);
		leftEye.SetActive (true);
		body.SetActive (true);
		leftHand.SetActive (true);
		rightHand.SetActive (true);
	}
	*/

	/*
	void createRandomMonster()
	{
		handType = Random.Range (0, KaijuDatabase.instance.HandSprites.Length - 1);
		headType = Random.Range (0, KaijuDatabase.instance.HeadSprites.Length - 1);
		eyeType = Random.Range (0, KaijuDatabase.instance.eyeSprites.Length - 1);
		bodyType = Random.Range (0, KaijuDatabase.instance.BodySprites.Length - 1);
		monsterColor = Random.ColorHSV (0.0f, 1.0f, 0.5f, 1.0f, 0.6f, 1.0f);
		//eyeColor = Random.ColorHSV (0.0f, 0.5f, 0.8f, 1.0f, 0.9f, 1.0f);
	}
	*/

	public void setUpMonster(int headT, int handT, int bodyT, Color mColor)
	{		
		handType = handT;
		headType = headT;
		eyeType = Random.Range (0, KaijuDatabase.instance.eyeSprites.Length - 1);
		bodyType = bodyT;
		monsterColor = mColor;
		loadSprites ();
	}

	void loadSprites()
	{
		for (int i = 0; i < headGameobjects.Length; i++) {
			headGameobjects [i].SetActive (false);
		}

		SpriteRenderer renderer;
		head = headGameobjects[headType-1];
		head.SetActive (true);
		renderer = head.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.HeadSprites [headType];
		renderer.material.color = monsterColor;

		renderer = leftEye.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.eyeSprites [eyeType];
		//renderer.material.color = eyeColor;

		renderer = body.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.BodySprites [bodyType];
		renderer.material.color = monsterColor;

		renderer = leftHand.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.HandSprites [handType];
		renderer.material.color = monsterColor;

		renderer = leftHandIdle.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.HandSprites [handType];
		renderer.material.color = monsterColor;

		renderer = rightHand.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.HandSprites [handType];
		renderer.color = monsterColor;

		renderer = rightHandIdle.GetComponent<SpriteRenderer> ();
		renderer.sprite = KaijuDatabase.instance.HandSprites [handType];
		renderer.color = monsterColor;

		head.SetActive (true);
		leftEye.SetActive (true);
		body.SetActive (true);
		leftHand.SetActive (true);
		rightHand.SetActive (true);
	}

}
