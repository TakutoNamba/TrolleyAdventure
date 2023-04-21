using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class SplineController_StartScene : MonoBehaviour
{
    public GameObject player;
    public GameObject splineObject;
    public Vector3 posModifier;
    private float dist;
    private Vector3 prevPos;
    private SplineContainer spline;
    public float speed;

    void Awake()
    {

    }

    void Start()
    {
        spline = splineObject.GetComponent<SplineContainer>();
        //prevPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        prevPos = new Vector3(0,0,0);
        //player.transform.position = spline.EvaluatePosition(getPercentage(spline, 0));
        //player.transform.position = new Vector3(player.transform.position.x + posModifier.x, player.transform.position.y + posModifier.y, player.transform.position.z + posModifier.z);
    }

    // Update is called once per frame
    void Update()
    {
        moveTrolley(spline);
        if(getPercentage(spline, dist) >= 1)
        {
            dist = 0;
        }
    }

    public void moveTrolley(SplineContainer splineContainer)
    {
        Debug.Log("hiii");
        dist += Time.deltaTime * speed;
        if (dist >= getTotalLength(spline))
        {
            dist = getTotalLength(spline);
        }
        player.transform.position = splineContainer.EvaluatePosition(getPercentage(splineContainer, dist));
        player.transform.position = new Vector3(player.transform.position.x + posModifier.x, player.transform.position.y + posModifier.y, player.transform.position.z + posModifier.z);

        if (prevPos != new Vector3(0,0,0))
        {
            Debug.Log("heyyy");
            player.transform.rotation = getCameraAngle(player.transform.position, prevPos);
        }

        prevPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

    }

    public Quaternion getCameraAngle(Vector3 curPos, Vector3 prevPos)
    {
        return Quaternion.LookRotation(curPos - prevPos, Vector3.up);
    }

    public float getPercentage(SplineContainer splineContainer, float curPoint)
    {
        return curPoint / getTotalLength(splineContainer);
    }

    public float getTotalLength(SplineContainer splineContainer)
    {
        return splineContainer.CalculateLength();
    }

}
