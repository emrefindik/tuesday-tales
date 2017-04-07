using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteControl : MonoBehaviour {
	public GameObject controllee;

	Shader colorShader;
	Shader grayShader;

	Component[] sprites;

	void Start()
	{
		colorShader = Shader.Find ("Sprites/Default");
		grayShader = Shader.Find ("Sprites/Diffuse");
		deactivateColor ();
	}
	public void activateColor()
	{
		sprites = controllee.GetComponentsInChildren<SpriteRenderer>();

		foreach (SpriteRenderer sprite in sprites) {
			sprite.color = Color.white;
			//sprite.material.shader = colorShader;
		}

	}

	public void deactivateColor()
	{
		sprites = controllee.GetComponentsInChildren<SpriteRenderer>();

		foreach (SpriteRenderer sprite in sprites) {
			sprite.color = new Color(109, 109, 109);
			//sprite.material.shader = grayShader;
		}
		
	}

	public void activeCollider()
	{
		
	}
}
