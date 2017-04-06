using UnityEngine;
using System.Collections;

/* Bullet */
// Acts similarly to PhysicsPixel
class Bullet : PhysicsObj
{
	public int ID = -1; // -1 is an invalid ID, and needs to be changed after spawn

	float x, y; // position
	float lastX, lastY; // last position
	float velX, velY; // velocity
	
	//DestructibleTerrain dT; // these vars and Start() are all about gettin everything connected...
	public PixelDestruction pD;
	//CustomTerrain terrain;
	CustomPhysics physics;
	//CustomRenderer renderer;
	CustomRayCast rayCast;
	Explode explode;
	
	//bool firstRun = true;
	
	public void Start()
	{
		physics = pD.physics;
		rayCast = pD.rayCast;
		explode = pD.explode;
	}
	
	// Constructor
	public Bullet (float x, float y, float vX, float vY)
	{
		this.x = x;
		this.y = y;
		lastX = x;
		lastY = y;
		velX = vX;
		velY = vY;
	}
  
	// methods implemented as a PhysicsObj
	public float getX ()
	{
		return x;
	}

	public float getY ()
	{
		return y;
	}

	public float getVX ()
	{
		return velX;
	}

	public float getVY ()
	{
		return velY;
	}

	public void setX (float pX)
	{
		x = pX;
	}

	public void setY (float pY)
	{
		y = pY;
	}

	public void setVX (float vX)
	{
		velX = vX;
	}

	public void setVY (float vY)
	{
		velY = vY;
	}

	public int getID()
	{
		return ID;
	}

	public void setID(int newID)
	{
		ID = newID;
	}

	public void checkConstraints ()
	{
		int[] collision = rayCast.rayCast((int)lastX, (int)lastY, (int)x, (int)y);
		if (collision.Length > 0)
		{
            Direction direction = velX > 0 ? Direction.FromLeft : Direction.FromRight;
//			pD.DestroyBullet();
			physics.Remove (this);
			explode.explode (collision [2], collision [3], 100, direction); //60);

            //Debug.Log("last x " + lastX);
            //Debug.Log("last y " + lastY);
            //Debug.Log("x " + x);
            //Debug.Log("y " + y);
            //Debug.Log("vx " + velX);
            //Debug.Log("vy " + velY);
            //Debug.Log("explode x " + collision[2]);
            //Debug.Log("explode y " + collision[3]);

        }
		lastX = x;
		lastY = y;
	}
}