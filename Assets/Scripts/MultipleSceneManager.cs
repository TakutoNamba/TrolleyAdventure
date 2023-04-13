using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MultipleSceneManager : MonoBehaviour
{
    public GameObject trolleyObject;
    public GameObject _timeline;


    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        
    }

    void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        if(loadedScene.name == "StartScene")
        {
        }
        
        if(loadedScene.name == "Main")
        {
            Debug.Log(loadedScene.name);
        }
    }
}
