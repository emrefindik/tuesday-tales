using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggController : MonoBehaviour
{

    public static EggController instance;

    private List<Egg> eggs;

    public List<Egg> Eggs
    {
        get
        {
            return eggs;
        }
        private set
        {
            eggs = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /** update the list of eggs the player has
     * by connecting to either our or Spatial's
     * server. TODO gather actual friend data
     * */
    public void updateEggs()
    {

    }
}
