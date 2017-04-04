using UnityEngine;
using System.Collections;
//
public class Controls
{
    // much of this has to be replaced by unity's control methods...

    public PixelDestruction pD;
    public CustomPhysics physics;
    public const float baseVel = 1500.0f;

    

    public Controls(PixelDestruction pD, CustomPhysics pS)
    {
        this.pD = pD;
        this.physics = pS;

    }

	//public void Update()
 //   {
 //       if (Input.GetMouseButtonDown(0))
 //       {
 //           // World Coordinate
 //           Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
 //           Debug.Log("mouse (World):" + mouseWorld);
 //           float velocity = baseVel;
 //           float startX = pD.World.Maximum.x;
 //           if (mouseWorld.x > pD.meshPosition.x)
 //           {
 //               velocity = -velocity;
 //           }
 //           else
 //           {
 //               startX = pD.World.Minimum.x;
 //           }

 //           Debug.Log(startX);
 //           // Mesh Coordinate
 //           // create the bullet at 1500 px/sec, and Add it to our Physics
 //           int y = pD.worldToImageY(mouseWorld.y);
 //           Bullet bullet = new Bullet(startX, y, velocity, 0);
 //           bullet.pD = pD; // set the pixel destruction reference
 //           bullet.Start(); // init the bullet
 //           physics.Add(bullet); // Add to physics
 //       }

 //   }

}