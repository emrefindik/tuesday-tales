using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController single;
    public Camera mainMenuCamera;
    public Camera destroyCityCamera;
    public GameObject menuScene;
    public GameObject destroyCityScene;

    // Use this for initialization
    void Start()
    {
        single = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void goToMainMenu()
    {
        mainMenuCamera.enabled = true;
        menuScene.SetActive(true);
        destroyCityCamera.enabled = false;
        destroyCityScene.SetActive(false);
    }

    public void goToDestroyCity()
    {
        mainMenuCamera.enabled = false;
        menuScene.SetActive(false);
        destroyCityCamera.enabled = true;
        destroyCityScene.SetActive(true);
    }
}
