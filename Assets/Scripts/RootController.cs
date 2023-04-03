using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.Playables;

public class RootController : MonoBehaviour
{
    private PlayableDirector director;

    enum gameState
    {
        Start,
        Playing,
        Finish
    }

    public float totalDistance;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    private void TimelineStart()
    {

    }






}
