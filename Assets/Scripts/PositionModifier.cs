using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionModifier : MonoBehaviour
{

    public Vector3 rotationModifier;
    void Start()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + rotationModifier.z);
    }

    void Update()
    {
        
    }
}
