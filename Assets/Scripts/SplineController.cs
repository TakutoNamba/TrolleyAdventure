using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineController : MonoBehaviour
{
    public bool isRunning;
    private float dist;
    public float speed;
    public GameObject trolleyObject;
    public GameObject spline;
    public Vector3 posModifier;
    private Vector3 prevPos;
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunning)
        {
            moveTrolley(spline.GetComponent<SplineContainer>());
        }
    }

    public void moveTrolley(SplineContainer splineContainer)
    {
        dist += Time.deltaTime * speed;
        trolleyObject.transform.position = splineContainer.EvaluatePosition(getPercentage(splineContainer, dist));
        trolleyObject.transform.position = new Vector3(trolleyObject.transform.position.x + posModifier.x, trolleyObject.transform.position.y + posModifier.y, trolleyObject.transform.position.z + posModifier.z);
        trolleyObject.transform.rotation = Quaternion.Euler(0, getCameraAngle(trolleyObject.transform.position, prevPos), 0);

        prevPos = new Vector3(trolleyObject.transform.position.x, trolleyObject.transform.position.y, trolleyObject.transform.position.z);
    }

    public float getCameraAngle(Vector3 curPos, Vector3 prevPos)
    {
        return Mathf.Atan2(curPos.x - prevPos.x, curPos.z - prevPos.z);
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
