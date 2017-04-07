using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggGenerator : MonoBehaviour {

	public Sprite []eggSprites;

	// TODO: store the links to the 
	string [] imageLinks;

	// TODO: return owned egg instead
	public int generate()
	{
		Location[] hatchLocations = generateLocations ();
		// Randomly Generate an egg
		int index = (int)(Random.Range(0, eggSprites.Length-1));
		return index;
		// TODO: pop up an input to enter a name
	}

	Location[] generateLocations()
	{
		// TODO Generate locations
		return null;
	}
}

