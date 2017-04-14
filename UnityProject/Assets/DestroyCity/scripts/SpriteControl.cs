using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteControl : MonoBehaviour {

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
		GameObject[] blocks = GameObject.FindGameObjectsWithTag ("block");

		foreach (GameObject block in blocks) {
			sprites = block.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer sprite in sprites) {
				sprite.color = Color.white;
				//sprite.material.shader = colorShader;
			}
		}

	}

	public void deactivateColor()
	{
		GameObject[] blocks = GameObject.FindGameObjectsWithTag ("block");

		foreach (GameObject block in blocks) {
			sprites = block.GetComponentsInChildren<SpriteRenderer> ();
			foreach (SpriteRenderer sprite in sprites) {
				sprite.color = Color.grey;
				//sprite.material.shader = grayShader;
			}
		}
		
	}

	public void activeCollider()
	{
		
	}
}
