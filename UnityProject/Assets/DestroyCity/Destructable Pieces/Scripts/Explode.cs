using UnityEngine;
using System.Collections;

public enum Direction
{
    FromRight = 0,
    FromLeft = 1
}

public class Explode: MonoBehaviour
{
	public PixelDestruction pD;
	CustomPhysics physics;
    const float dynamicRate = 0.2f;
    AudioSource explodeSound;

    //	CustomRayCast rayCast;
    private void Start()
    {
        explodeSound = GetComponent<AudioSource>();
    }

    /* Explode */
    // Creates an "explosion" by finding all pixels near a point and launching them away
    public void explode(int xPos, int yPos, float radius, Direction direction)
    {
        if (xPos < (pD.World.Minimum.x + pD.World.Width / 2) && direction == Direction.FromRight)
            return;
        if (xPos > (pD.World.Minimum.x + pD.World.Width / 2) && direction == Direction.FromLeft)
            return;

        bool soundPlayed = false;
        //Debug.Log(direction);
        float radiusSq = radius * radius;

        // Delete all side pixels
        int minX = pD.World.Minimum.x;
        int maxX = pD.World.Maximum.x;
        int minY = (yPos - (int)radius) > pD.World.Minimum.y ? (yPos - (int)radius) : pD.World.Minimum.y;
        int maxY = (yPos + (int)radius) < pD.World.Maximum.y ? (yPos + (int)radius) : pD.World.Maximum.y;
        if (direction == Direction.FromLeft)
            maxX = xPos;
        else
            minX = xPos;
       
        for (int y = minY; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                if (pD.isPixelSolid(x, y))
                {
                    pD.removePixel(x, y, direction);
                }
            }   
        }


        minX = (xPos - (int)radius) > pD.World.Minimum.x ? (xPos - (int)radius) : pD.World.Minimum.x;
        maxX = (xPos + (int)radius) < pD.World.Maximum.x ? (xPos + (int)radius) : pD.World.Maximum.x;
        // loop through every x from xPos-radius to xPos+radius
        for (int x = minX; x < maxX; x += pD.DestructionResolution)
        {

            // next loop through every y pos in this x column
            for (int y =minY; y < maxY; y += pD.DestructionResolution)
            {

                // first determine if this pixel (or if any contained within its square area) is solid
                int solidX = 0, solidY = 0;
                bool solid = false;
                // loop through every pixel from (xPos,yPos) to (xPos + destructionRes, yPos + destructionRes)
                // to find whether this area is solid or not
                for (int i = 0; i < pD.DestructionResolution && !solid; i++)
                {
                    for (int j = 0; j < pD.DestructionResolution && !solid; j++)
                    {
                        if (pD.isPixelSolid(x + i, y + j))
                        {
                            solid = true;
                            solidX = x + i;
                            solidY = y + j;
                            if(!soundPlayed)
                            {
                                explodeSound.Play();
                                soundPlayed = true;
                            }
                        }
                    }
                }
                if (solid) // we know this pixel is solid, now we need to find out if it's close enough
                {
                    float xDiff = x - xPos;
                    float yDiff = y - yPos;
                    float distSq = xDiff * xDiff + yDiff * yDiff;
                    // if the distance squared is less than radius squared, then it's within the explosion radius
                    if (distSq < radiusSq)
                    {
                        // finally calculate the distance
                        float distance = Mathf.Sqrt(distSq);

                        // the speed will be based on how far the pixel is from the center
                        float speed = 800 * (1 - distance / radius);

                        if (distance == 0)
                            distance = 0.001f; // prevent divide by zero in next two statements

                        // velocity
                        //float velX = speed * (xDiff + Random.Range(-10, 10)) / distance; //random (-10, 10)) / distance; 
                        //float velY = speed * (yDiff + Random.Range(-10, 10)) / distance;

                        // create the dynamic pixel
                        //DynamicPixel pixel = new DynamicPixel(terrain.getColor(solidX, solidY), x, y, velX, velY, terrain.destructionRes);

                        //40 % chance to create dynamic pixel
                        //if (Random.Range(0.0f, 1.0f) < dynamicRate)
                        //    pD.CreateDynamicPixel(pD.getColor(solidX, solidY), x, y, velX, velY, pD.DestructionResolution);

                        // Remove the static pixels
                        for (int i = 0; i < pD.DestructionResolution; i++)
                        {
                            for (int j = 0; j < pD.DestructionResolution; j++)
                            {
                                pD.removePixel(x + i, y + j, direction);
                            }
                        }
                    }
                }

            }
            
        }

    }
}