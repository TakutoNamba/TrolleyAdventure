using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class TrolleyMoveController : MonoBehaviour
{
    [SerializeField]
    PathCreator pathCreator;

    [SerializeField]
    GameObject trolley;

    [SerializeField]
    GameObject trolleyObject;


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

        prevPos = trolley.transform.position;

    }

    private void throwTrolley()
    {
        Vector3 dir = trolley.transform.position - prevPos;
        Rigidbody rb = trolleyObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(dir * pushScale, ForceMode.Impulse);
    }
}
