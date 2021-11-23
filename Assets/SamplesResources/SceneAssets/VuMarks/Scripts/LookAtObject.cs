/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    public Transform LookAtTargetTransform;

    void Update()
    {
        //if (LookAtTargetTransform != null)
        //{
        //    var targetDirection = transform.position - LookAtTargetTransform.position;
        //    transform.forward = targetDirection;
        //}

        //var pos = transform.position;
        //var cam = LookAtTargetTransform.position;

        //pos.x = 0;
        //pos.y = 0;

        //cam.x = 0;
        //cam.y = 0;

        //var targetRot = Quaternion.LookRotation(cam - pos);
        //transform.rotation = targetRot;

        transform.LookAt(LookAtTargetTransform);
        transform.localRotation.SetEulerRotation(0.0f,0.0f, transform.localRotation.eulerAngles.z);


    }
}
