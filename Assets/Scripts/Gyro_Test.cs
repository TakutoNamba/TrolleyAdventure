using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyro_Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;

    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = Input.gyro.attitude;
        float tiltAngle = Mathf.Clamp(rotation.eulerAngles.z, -30, 30);
        float modified_tiltAngle = (tiltAngle / 60) + 1;

        Debug.Log(rotation.eulerAngles.z);
    }
}
