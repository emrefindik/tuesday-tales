﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelControl : MonoBehaviour {

    private Vector2 fingerStart;
    private Vector2 fingerEnd;
    public GameObject buildings;
    public GameObject[] realBuilding = new GameObject[4];
    public GameObject[] realBuildingbuildingCover = new GameObject[4];
    public GameObject[] structures = new GameObject[4];
    public bool switchNow=false;
    private PixelDestruction pD;
    float[] levelPositions;

    // GUI
    Vector2 barPos = new Vector2(20, 40);
    Vector2 barSize = new Vector2(Screen.width-40, Screen.height/10);
    public Texture2D barFull;
    public Texture2D barEmpty;
    public int score;
    const int scoreBase = 100;

    //float[] progress;
    int[] progressCount;
    public int num_of_pieces;
    public int num_of_people;
    float progressStep;
    
    private float tolerance = 300;
    private int level = 0;

    float currentTime = 0;
    bool currentTimeOff = true;

    int buildingDestroyedCount;
    int number_of_buildings;
    bool win;   // check this status : whether has win

    public enum Movement
    {
        Left,
        Right,
        Up,
        Down
    };

    public List<Movement> movements = new List<Movement>();

    void Start() {
        initLevel();
    }

    void initLevel()
    {
        pD = FindObjectOfType(typeof(PixelDestruction)) as PixelDestruction;

        number_of_buildings = realBuilding.Length;
        levelPositions = new float[number_of_buildings];
        //progress = new float[number_of_buildings];
        progressCount = new int[number_of_buildings];

        for (int i = 0; i < number_of_buildings; i++)
        {
            levelPositions[i] = -realBuilding[i].transform.position.x;
            //progress[i] = 1.0f;
            progressCount[i] = num_of_pieces + num_of_people;
            structures[i].GetComponent<PolygonCollider2D>().enabled = false;
        }

        progressStep = 1.0f / (num_of_pieces + num_of_people);
        score = 0;
        buildingDestroyedCount = 0;

        win = false;
    }

    private void OnGUI()
    {
        GUI.BeginGroup(new Rect(barPos.x, barPos.y, barSize.x, barSize.y));
        GUI.Box(new Rect(0, 0, barSize.x, barSize.y), barEmpty);
        GUI.BeginGroup(new Rect(0, 0, barSize.x * (((float)progressCount[level]) / (num_of_people + num_of_pieces)), barSize.y));
        GUI.Box(new Rect(0, 0, barSize.x, barSize.y), barFull);
        GUI.EndGroup();
        GUI.EndGroup();

        GUI.skin.label.fontSize = (int)(Screen.height * 0.05);
        GUI.Label(new Rect(20, (int)(Screen.height * 0.85), Screen.width/3, (int)(Screen.height * 0.1)), score.ToString());
    }

    public void increaseProgress(int amount)
    {
        //progress[level] -= amount * progressStep;
        progressCount[level] -= amount;
        //Debug.Log(level);
        //Debug.Log(progress[level]);
        increaseScore(scoreBase * amount);
        //Debug.Log(progressCount[level]);
        if (progressCount[level] == 0)
        {
            structures[level].GetComponent<PolygonCollider2D>().enabled = true;
            //Debug.Log("setCollider");
            progressCount[level]--;
            buildingDestroyedCount++;
        }
    }

    public void increaseScore(int increment)
    {
        score += increment;
    }

    void Update()
    {
        // Check win
        if(buildingDestroyedCount == number_of_buildings)
        {
            win = true;
            buildingDestroyedCount = -1;
        }

        if (Input.GetMouseButtonDown(0))
        {
            fingerStart = Input.mousePosition;
            fingerEnd = Input.mousePosition;
        }

        //GetMouseButton instead of TouchPhase.Moved
        //This returns true if the LMB is held down in standalone OR
        //there is a single finger touch on a mobile device
        if (Input.GetMouseButton(0))
        {
            fingerEnd = Input.mousePosition;

            //There was some movement! The tolerance variable is to detect some useful movement
            //i.e. an actual swipe rather than some jitter. This is the same as the value of 80
            //you used in your original code.
            if (Mathf.Abs(fingerEnd.x - fingerStart.x) > tolerance ||
               Mathf.Abs(fingerEnd.y - fingerStart.y) > tolerance)
            {

                //There is more movement on the X axis than the Y axis
                if (Mathf.Abs(fingerStart.x - fingerEnd.x) > Mathf.Abs(fingerStart.y - fingerEnd.y))
                {
                    //Right Swipe
                    if ((fingerEnd.x - fingerStart.x) > 0)
                        movements.Add(Movement.Right);
                    //Left Swipe
                    else
                        movements.Add(Movement.Left);
                }

                //More movement along the Y axis than the X axis
                else
                {
                    //Upward Swipe
                    if ((fingerEnd.y - fingerStart.y) > 0)
                        movements.Add(Movement.Up);
                    //Downward Swipe
                    else
                        movements.Add(Movement.Down);
                }

                //After the checks are performed, set the fingerStart & fingerEnd to be the same
                fingerStart = fingerEnd;

                //Now let's check if the Movement pattern is what we want
                //In this example, I'm checking whether the pattern is Left, then Right, then Left again
                // Debug.Log(CheckForPatternMove(0, 3, new List<Movement>() { Movement.Left, Movement.Right, Movement.Left }));
                if (CheckForPatternMove(0, 1, new List<Movement>() { Movement.Left }))
                {
                    switchNow = true;
                    if (level < 3 )
                    {
                        level = level + 1;
                    }
                    
                }

                if (CheckForPatternMove(0, 1, new List<Movement>() { Movement.Right }))
                {
                    switchNow = true;
                    if (level>0  )
                    level= level-1;
                }
            }
        }

        //GetMouseButtonUp(0) instead of TouchPhase.Ended
        if (Input.GetMouseButtonUp(0))
        {
            fingerStart = Vector2.zero;
            fingerEnd = Vector2.zero;
            movements.Clear();
        }

        if (switchNow)
        {
            goToLevel(level);
        }
        pD.switchCity(level);
}

    void goToLevel(int level)
    {
        float currentX = 0;
        currentX = buildings.transform.position.x;
        if (Mathf.Abs(currentX - levelPositions[level]) >= .02f)
        {
            GetcurrentTime();
            float newPos = 0;
            newPos = Mathf.Lerp(currentX, levelPositions[level], (Time.time - currentTime) * .1f);
            buildings.transform.position = new Vector3(newPos, buildings.transform.position.y, buildings.transform.position.z);
        }
        else
        {
            for(int i=0; i<realBuilding.Length; i++)
            {
                realBuilding[i].SetActive(false);
                realBuildingbuildingCover[i].SetActive(true);
            }
            realBuilding[level].SetActive(true);
            realBuildingbuildingCover[level].SetActive(false);
            switchNow = false;
            currentTimeOff = true;
        }
    }

    //void goTolevel0()
    //{
    //    float i = 0;
    //    i = buildings.transform.position.x;
    //    if (Mathf.Abs(i-levelPositions[0])>=.02f)
    //    {
    //        GetcurrentTime();
    //        float newPos = 0;
    //        newPos = Mathf.Lerp(i, levelPositions[0], (Time.time-currentTime) * .1f);
    //        buildings.transform.position = new Vector3(newPos, buildings.transform.position.y, buildings.transform.position.z);
    //    }
    //    else
    //    {
    //        realBuilding[0].SetActive(true);
    //        realBuildingbuildingCover[0].SetActive(false);

    //        realBuilding[1].SetActive(false);
    //        realBuildingbuildingCover[1].SetActive(true);
    //        realBuilding[2].SetActive(false);
    //        realBuildingbuildingCover[2].SetActive(true);
    //        realBuilding[3].SetActive(false);
    //        realBuildingbuildingCover[3].SetActive(true);
    //        switchNow = false;
    //        currentTimeOff = true;
    //    }
    //}

    //void goTolevel1()
    //{
    //    float i = 0;
    //    i = buildings.transform.position.x;
    //    if (Mathf.Abs(i - levelPositions[1]) >= .02f)
    //    {
    //        GetcurrentTime();
    //        float newPos = 0;
    //        newPos = Mathf.Lerp(i, levelPositions[1], (Time.time-currentTime) * .1f);
    //        buildings.transform.position = new Vector3(newPos, buildings.transform.position.y, buildings.transform.position.z);

    //    }
    //    else
    //    {
    //        realBuilding[1].SetActive(true);
    //        realBuildingbuildingCover[1].SetActive(false);

    //        realBuilding[0].SetActive(false);
    //        realBuildingbuildingCover[0].SetActive(true);
    //        realBuilding[2].SetActive(false);
    //        realBuildingbuildingCover[2].SetActive(true);
    //        realBuilding[3].SetActive(false);
    //        realBuildingbuildingCover[3].SetActive(true);
    //        switchNow = false;
    //        currentTimeOff = true;
    //    }
    //}

    //void goTolevel2()
    //{
    //    GetcurrentTime();
    //    float i = 0;
    //    i = buildings.transform.position.x;
    //    if (Mathf.Abs(i - levelPositions[2]) >= .02f)
    //    {
    //        float newPos = 0;
    //        newPos = Mathf.Lerp(i, levelPositions[2], (Time.time-currentTime) * .1f);
    //        buildings.transform.position = new Vector3(newPos, buildings.transform.position.y, buildings.transform.position.z);
    //    }
    //    else
    //    {
    //        realBuilding[2].SetActive(true);
    //        realBuildingbuildingCover[2].SetActive(false);

    //        realBuilding[0].SetActive(false);
    //        realBuildingbuildingCover[0].SetActive(true);
    //        realBuilding[1].SetActive(false);
    //        realBuildingbuildingCover[1].SetActive(true);
    //        realBuilding[3].SetActive(false);
    //        realBuildingbuildingCover[3].SetActive(true);
    //        switchNow = false;
    //        currentTimeOff = true;
    //    }
    //}

    //void goTolevel3()
    //{
    //    GetcurrentTime();
    //    float i = 0;
    //    i = buildings.transform.position.x;
    //    if (Mathf.Abs(i - levelPositions[3]) >= .02f)
    //    {
    //        float newPos = 0;
    //        newPos = Mathf.Lerp(i, levelPositions[3], (Time.time-currentTime) * .1f);
    //        buildings.transform.position = new Vector3(newPos, buildings.transform.position.y, buildings.transform.position.z);
    //    }
    //    else
    //    {
    //        realBuilding[3].SetActive(true);
    //        realBuildingbuildingCover[3].SetActive(false);

    //        realBuilding[0].SetActive(false);
    //        realBuildingbuildingCover[0].SetActive(true);
    //        realBuilding[1].SetActive(false);
    //        realBuildingbuildingCover[1].SetActive(true);
    //        realBuilding[2].SetActive(false);
    //        realBuildingbuildingCover[2].SetActive(true);
    //        switchNow = false;
    //        currentTimeOff = true;
    //    }
    //}


    private bool CheckForPatternMove(int startIndex, int lengthOfPattern, List<Movement> movementToCheck)
    {
        if (switchNow == true)
        {
            return false;
        }
        //If the currently stored movements are fewer than the length of the pattern to be detected
        //it can never match the pattern. So, let's get out
        if (lengthOfPattern > movements.Count)
            return false;

        //In case the start index for the check plus the length of the pattern
        //exceeds the movement list's count, it'll throw an exception, so lets get out
        if (startIndex + lengthOfPattern > movements.Count)
            return false;

        //Populate a temporary list with the respective elements
        //from the movement list
        List<Movement> tMovements = new List<Movement>();
        for (int i = startIndex; i < startIndex + lengthOfPattern; i++)
            tMovements.Add(movements[i]);

        //Now check whether the sequence of movements is the same as the pattern you want to check for
        //The SequenceEqual method is in the System.Linq namespace
        return tMovements.SequenceEqual(movementToCheck);

    }



    void GetcurrentTime()
    {
        if (currentTimeOff==true)
        {
            currentTimeOff = false;
            currentTime = Time.time;
        }
    }


}