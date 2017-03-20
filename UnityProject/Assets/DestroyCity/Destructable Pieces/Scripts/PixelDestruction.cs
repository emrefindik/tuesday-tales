using System;
using System.Threading;
using Assets.Scripts;
using MathUtil;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PixelDestruction : MonoBehaviour
{
	//public PlayerAnimate ourPlayerAnimation;
	public Texture2D[] SourceTexture; // original image
                                    //public Material RenderMat; // our render material
                                    //public Material dPixelRenderMat;

    public CityMesh[] cityMeshes;
    private CityMesh activeCityMesh;
    public World World;
	public int DestructionResolution = 2; // the resolution of our destruction in pixels

	public CustomPhysics physics; // has a list of all physics objects, and uses their velocity to move them
	public CustomRayCast rayCast; // used to detect collisions and stuff
	public Explode explode; // when a player shoots, we blow up the City
	//private Controls controls; // our players control script
	//public Player player; // our player script

	//private List<Vector2> previousPlayerPixels = new List<Vector2> ();
	private List<Vector2> previousDynamicPixels = new List<Vector2> ();

	List<DynamicPixel> SpawnedPixels = new List<DynamicPixel>(); // a list of all active dPixels
	
	private Color32[] clearPixels;

    public Vector2 meshPosition;
    public Vector2 bottomCoord;
    public Vector2 topCoord;
    public const float baseVel = 3000.0f;
    private float convertCoeff;

    //List<int> leftPos = new List<int>();
    //List<int> rightPos = new List<int>();
    //float buildingFallVelocity = 10;

    //// to allow use of coroutines in non-monobehavior scripts
    //public void StartShootTimer()
    //{
    //	StartCoroutine("ShootTimer");
    //}

    //IEnumerator ShootTimer()
    //{
    //	if(!player.canShoot)
    //	{
    //		yield return new WaitForSeconds(0.15f);
    //		player.canShoot = true;
    //	}
    //}

    void Start()
	{
		Application.targetFrameRate = -1;

		physics = new CustomPhysics (); // initialize the physics
		rayCast = new CustomRayCast();
		rayCast.pD = this;
		//explode = new Explode ();
		explode.pD = this;
        //player = new Player (-100, 500); // create the player
        //ourPlayerAnimation.X = player.x;
        //ourPlayerAnimation.Y = player.y;
        //ourPlayerAnimation.Enabled = true;
        //player.pD = this;
        //player.physics = physics;
        //physics.Add (player); // Add player to physics
        //player.targetCamera = Camera.main.GetComponent<CameraFollow>();
        //controls.player = player;

        //controls = new Controls(this, physics);

        //int minX = this.World.Minimum.x;
        //int maxX = this.World.Maximum.x;
        //for (int y = this.World.Minimum.y; y < this.World.Maximum.y; y++)
        //{
        //    leftPos.Add(minX);
        //    rightPos.Add(maxX);
        //}
        intializeCities();

    }
	
    void intializeCities()
    {
        for (int i = 0; i < cityMeshes.Length; i++){
            cityMeshes[i].GenerateMesh();
            cityMeshes[i].InitializeWithCityTexture(SourceTexture[i]);
        }
        switchCity(0);
    }

    public void switchCity(int i)
    {
        this.activeCityMesh = cityMeshes[i];
        this.World = this.activeCityMesh.City;

        meshPosition.x = activeCityMesh.transform.position.x;
        meshPosition.y = activeCityMesh.transform.position.y;
        convertCoeff = activeCityMesh.scaleCoeff;
        bottomCoord.x = (meshPosition.x - 0.5f * convertCoeff * activeCityMesh.MeshWidth);
        bottomCoord.y = (meshPosition.y - 0.5f * convertCoeff * activeCityMesh.MeshHeight);
        topCoord.x = (meshPosition.x + 0.5f * convertCoeff * activeCityMesh.MeshWidth);
        topCoord.y = (meshPosition.y + 0.5f * convertCoeff * activeCityMesh.MeshHeight);

    }

	// Update is called once per frame
	void Update()
	{
	    //	Debug.Log("active physics objects: " + physics.activePhysicsObjects);

		UnityEngine.Profiling.Profiler.BeginSample("Game.Update");

		//controls.Update ();
		CalculateWorld ();

	    physics.Update();

        UnityEngine.Profiling.Profiler.EndSample();
	}

	void CalculateWorld()
	{
        UnityEngine.Profiling.Profiler.BeginSample("Game.CalculateWorld");

		/* Check for Dynamic Pixels and draw them */
		if(SpawnedPixels.Count > 0) // if there are active dPixels
		{
			for(int i = 0; i < SpawnedPixels.Count; i++) // iterate through all active dPixels
			{
				int x = (int)SpawnedPixels[i].getX(); // then we get our current pos
				int y = (int)SpawnedPixels[i].getY();
                if (this.World.IsInside(new Vector2I(x, y))) // only set the pixel if its within boundaries
				{
				    if (this.World == null)
				    {
				        previousDynamicPixels.Add(new Vector2(x, y));
                        // and set our pixels currently // TODO: scale to destruction resolution!
				        this.addPixel(SpawnedPixels[i].col,x, y);
				    }
				}
				else
				{
//					Debug.LogWarning("Warning: pixel outside boundaries in calculate world!");
					physics.Remove(SpawnedPixels[i]); // TEMPORARY TESTING OF REMOVAL OF ROGUE PIXELS
					SpawnedPixels.RemoveAt(i); // TEMPORARY TESTING OF REMOVAL OF ROGUE PIXELS
				}
			}
		}

		///* Check for Player and draw them */
		//ourPlayerAnimation.X = player.getX();
		//ourPlayerAnimation.Y = player.getY();

		// old player drawing below
//		if (player != null)
//		{
//			// draw our player EXPERIMENTAL
//			for(int iteration = 0; iteration < player.playerHeight; iteration++)
//			{
//				for(int i = 0; i < player.playerWidth ;i++)
//				{
//					previousPlayerPixels.Add(new Vector2((int) player.x + (i - (player.playerWidth / 2)), (int) player.y + (iteration - (player.playerHeight / 2))));
//					//world.SetPixel((int) player.x + (i - 8), (int) player.y + (iteration - 8), Color.red);
//
//					if(isPixelSolid((int) player.x + (i - (player.playerWidth / 2)), (int) player.y + (iteration - (player.playerHeight / 2))))  //world.GetPixel((int) player.x + (i - (player.playerWidth / 2)), (int) player.y + (iteration - (player.playerHeight / 2))).a != 0) // if the pixel is solid in world, were supposed to be colliding
//					{
//						//Debug.LogError("Collision Overlap - Player/world at X: " + (player.x + (i - (player.playerWidth / 2))) + " Y: " + (player.y + (iteration - (player.playerHeight / 2))));
//					}
//					else // we only draw our pixels if were not overlapping the ground
//					{
//						if(iteration > player.playerHeight / 2)
//						{
//							dPixelsWorld.SetPixel((int) player.x + (i - (player.playerWidth / 2)), (int) player.y + (iteration - (player.playerHeight / 2)), Color.green);
//						}
//						else if(iteration < player.playerHeight / 2 && iteration < player.playerHeight / 4)
//						{
//							dPixelsWorld.SetPixel((int) player.x + (i - (player.playerWidth / 2)), (int) player.y + (iteration - (player.playerHeight / 2)), Color.white);
//						}
//						else
//						{
//							dPixelsWorld.SetPixel((int) player.x + (i - (player.playerWidth / 2)), (int) player.y + (iteration - (player.playerHeight / 2)), Color.gray);
//						}
//						dynamicPixelUpdate = true;
//					}
//				}
//			}
//
////			Debug.Log("setting pixel: " + player.x + " " + player.y);
//			//worldUpdate = true;
//		}

        UnityEngine.Profiling.Profiler.EndSample();
	}
	
	public bool isPixelSolid (int x, int y)
	{
		if (this.World.IsInside(new Vector2I(x, y)))
		{
            var pixel = this.World.CityLayer.GetAt(new Vector2I(x, y));
		    return !(pixel.a == 0);
		}
		return true; // border IS solid
	}

	public void addPixel (Color c, int x, int y)
	{
        if (this.World.IsInside(new Vector2I(x, y)))
		{
            this.World.ChangeCityColorAt(new Vector2I(x, y), c);
		}
	}

	public void removePixel (int x, int y, Direction direction)
	{
        if (this.World.IsInside(new Vector2I(x, y)))
		{
            if (this.World != null)
            {
                this.World.ChangeCityAlphaAt(new Vector2I(x, y), 0);

                //// check for left & right
                //int index = y - this.World.Minimum.y;
                //int left = leftPos[index];
                //int right = rightPos[index];

                //if(direction == Direction.FromLeft && left < x)
                //{
                //    leftPos[index] = x;
                //    left = x;
                //}
                //else if(direction == Direction.FromRight && right > x)
                //{
                //    rightPos[index] = x;
                //    right = x;
                //}

                //if (left >= right)
                //{
                //    buildingFall(y, direction);
                //}

            }
		}
	}

    //public void buildingFall(int _y, Direction direction)
    //{
    //    float velX = direction == Direction.FromLeft ? buildingFallVelocity : -buildingFallVelocity;

    //    for (int y = _y; y < World.Maximum.y; y++)
    //    {
    //        int index = y - World.Minimum.y;
    //        int minX = leftPos[index];
    //        int maxX = rightPos[index];
    //        for (int x = minX; x < maxX; x++)
    //        {
    //            removePixel(x, y, direction);
    //            //CreateDynamicPixel(getColor(x, y), x, y, velX, 0, DestructionResolution);
    //        }
    //    }

    //}

	public Color getColor (int x, int y)
	{
        if (this.World.IsInside(new Vector2I(x, y)))
            return this.World.CityLayer.GetAt(new Vector2I(x, y));
		return Color.clear;
	}

	public float[] getNormal (int x, int y)
	{
		// First find all nearby solid pixels, and create a vector to the average solid pixel from (x,y)
		float avgX = 0;
		float avgY = 0;
		for (int w = -3; w <= 3; w++)
		{
			for (int h = -3; h <= 3; h++)
			{
				if (isPixelSolid (x + w, y + h))
				{
					avgX -= w;
					avgY -= h;
				}
			}
		}
		float len = Mathf.Sqrt (avgX * avgX + avgY * avgY); // get the distance from (x,y)
	    if (len == 0)
	    {
	        return new float[0];
	    }
		return new float[]{avgX / len, avgY / len}; // normalize the vector by dividing by that distance
	}

	public void CreateDynamicPixel(Color c, float x, float y, float vX, float vY, int size)
	{
        if (!this.World.IsInside(new Vector2I((int)x, (int)y)))
		{
			Debug.LogError("ERROR: Attempted to create dynamic pixel out of bounds!");
		}
		else
		{
            //Debug.Log("Creating Dynamic Pixels");
			DynamicPixel newPixel = new DynamicPixel (this.World, c, x, y, vX, vY, size); // creates pixel with color, position, velocity, and size info
			newPixel.pD = this;
			newPixel.Start ();
			SpawnedPixels.Add(newPixel); // track our new pixel with our list
			newPixel.stickiness = 1000; // determine how slow a pixel must go to "stick" to City
			physics.Add (newPixel); // Add to our physics
		}
	}
	
	public void DestroyDynamicPixel(DynamicPixel pixel)
	{
		physics.Remove (pixel); // and physics
		SpawnedPixels.Remove(pixel); // Remove it from our list7
	}

    public int worldToImageY(float yWorld)
    {
        return (int)((yWorld - meshPosition.y) / convertCoeff);
    }

    public int worldToImageX(float xWorld)
    {
        return (int)((xWorld - meshPosition.x) / convertCoeff);
    }

    public float ImageXToWorld (int xImage)
    {
        return (float)(xImage * convertCoeff + meshPosition.x);
    }

    public float ImageYToWorld(int yImage)
    {
        return (float)(yImage * convertCoeff + meshPosition.y);
    }

    public void smashCity(Vector3 mouseWorld)
    {
            // World Coordinate
            //Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
            //Debug.Log("mouse (World):" + mouseWorld);
            float velocity = baseVel;
            float startX = World.Maximum.x;
            if (mouseWorld.x > meshPosition.x)
            {
                velocity = -velocity;
            }
            else
            {
                startX =World.Minimum.x;
            }

            //Debug.Log(startX);
            // Mesh Coordinate
            // create the bullet at 1500 px/sec, and Add it to our Physics
            int y = worldToImageY(mouseWorld.y);
            Bullet bullet = new Bullet(startX, y, velocity, 0);
            bullet.pD = this; // set the pixel destruction reference
            bullet.Start(); // init the bullet
            physics.Add(bullet); // Add to physics
    }

}