using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaijuDatabase : MonoBehaviour {

	// for eggs
	public Sprite []eggSprites;
	// TODO: store the links to the 
	string [] imageLinks;

	// for kaiju
	public Sprite [] handSprites;
	public Sprite [] headSprites;
	public Sprite [] eyeSprites;
	public Sprite [] bodySprites;

	// TODO: return owned egg instead
	public int generateEgg()
	{
		//if (Random.Range (0, 1) < 0.2)
			//return -1;
		// Randomly Generate an egg
		int index = (int)(Random.Range(0, eggSprites.Length-1));
		return index;
		// TODO: pop out a ui to input name of the Egg
	}

	Location[] generateLocations()
	{
		// TODO Generate locations
		return null;
	}
}

