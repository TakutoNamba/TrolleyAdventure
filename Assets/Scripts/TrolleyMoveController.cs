using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class TrolleyMoveController : MonoBehaviour
{
    public PathCreator pathCreator;
    public PathCreator[] otherPathCreators;

    [SerializeField]
    GameObject trolley;


    public GameObject trolleyObject;
    [SerializeField] GameObject wheel_front;
    [SerializeField] GameObject wheel_back;




    public float speed;
    public float moveDistance;
    public float pushScale;

    Vector3 endPos;
    Vector3 prevPos;

    void Start()
    {
        endPos = pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1);
    }

    void Update()
    {
        //‚±‚±‚ðƒ^ƒCƒ€ƒ‰ƒCƒ“‚Å“®‚©‚·
        //moveDistance += speed * Time.deltaTime;
        trolley.transform.position = pathCreator.path.GetPointAtDistance(moveDistance, EndOfPathInstruction.Stop);
        trolley.transform.rotation = pathCreator.path.GetRotationAtDistance(moveDistance, EndOfPathInstruction.Stop);

        if (trolley.transform.position == endPos)
        {
            throwTrolley();
        }
        else
        {
            rotateWheel();
        }

        prevPos = trolley.transform.position;

    }

    private void throwTrolley()
    {
        Vector3 dir = trolley.transform.position - prevPos;
        Rigidbody rb = trolleyObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(dir * pushScale, ForceMode.Impulse);
    }

    private void rotateWheel()
    {
        wheel_front.transform.Rotate(moveDistance, 0, 0);
        wheel_back.transform.Rotate(moveDistance, 0, 0);
    }
}
