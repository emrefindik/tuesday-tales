using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteControl : MonoBehaviour {
	public GameObject controllee;

	Component[] sprites;
	public void activateColor()
	{
		sprites = controllee.GetComponentsInChildren<SpriteRenderer>();

		foreach (SpriteRenderer sprite in sprites)
			sprite.color = Color.white;

	}

	public void deactivateColor()
	{
		sprites = controllee.GetComponentsInChildren<SpriteRenderer>();

		foreach (SpriteRenderer sprite in sprites)
			sprite.color = Color.gray;
	}

	public void activeCollider()
	{
		
	}
}
