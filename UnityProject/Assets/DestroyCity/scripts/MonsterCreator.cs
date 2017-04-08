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

	KaijuDatabase kaijuDatabase;

	Color monsterColor;
	//Color eyeColor;

	// Use this for initialization
	void Start () {
		kaijuDatabase = GameObject.Find ("MainController").gameObject.GetComponent<KaijuDatabase>();
		//createRandomMonster ();
		//loadSprites ();
	}

	void createRandomMonster()
	{
		handType = Random.Range (0, kaijuDatabase.handSprites.Length - 1);
		headType = Random.Range (0, kaijuDatabase.headSprites.Length - 1);
		eyeType = Random.Range (0, kaijuDatabase.eyeSprites.Length - 1);
		bodyType = Random.Range (0, kaijuDatabase.bodySprites.Length - 1);
		monsterColor = Random.ColorHSV (0.0f, 1.0f, 0.5f, 1.0f, 0.6f, 1.0f);
		//eyeColor = Random.ColorHSV (0.0f, 0.5f, 0.8f, 1.0f, 0.9f, 1.0f);
	}

	public void setUpMonster(int headT, int handT, int bodyT, Color mColor)
	{
		kaijuDatabase = GameObject.Find ("MainController").gameObject.GetComponent<KaijuDatabase>();

		Debug.Log (kaijuDatabase);
		handType = handT;
		headType = headT;
		eyeType = Random.Range (0, kaijuDatabase.eyeSprites.Length - 1);
		bodyType = bodyT;
		monsterColor = mColor;
		loadSprites ();
	}

	void loadSprites()
	{
		SpriteRenderer renderer;
		renderer = head.GetComponent<SpriteRenderer> ();
		renderer.sprite = kaijuDatabase.headSprites [headType];
		renderer.material.color = monsterColor;

		renderer = leftEye.GetComponent<SpriteRenderer> ();
		renderer.sprite = kaijuDatabase.eyeSprites [eyeType];
		//renderer.material.color = eyeColor;

		renderer = body.GetComponent<SpriteRenderer> ();
		renderer.sprite = kaijuDatabase.bodySprites [bodyType];
		renderer.material.color = monsterColor;

		renderer = leftHand.GetComponent<SpriteRenderer> ();
		renderer.sprite = kaijuDatabase.handSprites [handType];
		renderer.material.color = monsterColor;

		renderer = rightHand.GetComponent<SpriteRenderer> ();
		renderer.sprite = kaijuDatabase.handSprites [handType];
		renderer.color = monsterColor;

		head.SetActive (true);
		leftEye.SetActive (true);
		body.SetActive (true);
		leftHand.SetActive (true);
		rightHand.SetActive (true);
	}

}
