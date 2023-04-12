using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.SceneManagement;

public class TrolleyController_StartScene : MonoBehaviour
{
    public PathCreator pathCreator;
    public PathCreator[] otherPathCreators;

    [SerializeField] GameObject trolley;
    public GameObject trolleyObject;
    [SerializeField] GameObject wheel_front;
    [SerializeField] GameObject wheel_back;
    public GameObject titleText;



    public float speed;
    public float moveDistance;
    public float pushScale;
    public bool isRunning;

    Vector3 endPos;


    void Start()
    {
        endPos = pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1);
    }

    void Update()
    {
        //‚±‚±‚ðƒ^ƒCƒ€ƒ‰ƒCƒ“‚Å“®‚©‚·

        if(isRunning)
        {
            moveDistance += speed * Time.deltaTime;
            trolley.transform.position = pathCreator.path.GetPointAtDistance(moveDistance, EndOfPathInstruction.Loop);
            trolley.transform.rotation = pathCreator.path.GetRotationAtDistance(moveDistance, EndOfPathInstruction.Loop);

            rotateWheel();
        }

    }

    private void rotateWheel()
    {
        wheel_front.transform.Rotate(moveDistance, 0, 0);
        wheel_back.transform.Rotate(moveDistance, 0, 0);
    }

    private void OnSceneLoaded(Scene loaded)
    {
        titleText.GetComponent<TextMeshProSimpleAnimator>().enabled = true;
    }


}
