using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RotationController : MonoBehaviour
{

    [SerializeField]
    int Time;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(RotateWithTimer());
    }

    IEnumerator RotateWithTimer()
    {
        Time = (int)(System.DateTime.UtcNow.Hour * 60f + System.DateTime.UtcNow.Minute);

        //0.25 = 360 / 1440 (Minutes in a day) Time => Rotation
        transform.rotation = Quaternion.Euler(new Vector3(0f, Time * -0.25f, 0f));

        yield return new WaitForSeconds(50f);

        StartCoroutine(RotateWithTimer());
    }

}
