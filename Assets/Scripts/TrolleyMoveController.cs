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

    public float speed;
    public float moveDistance;

    Vector3 endPos;

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
    }
}
