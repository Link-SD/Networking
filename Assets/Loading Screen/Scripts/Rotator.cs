using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float LoadingCircleSpeed;    
    private Transform LoadingCircle;

    private void Start()
    {
        LoadingCircle = transform.GetChild(0);
    }

    private void Update()
    {
        if (LoadingCircle == null)
            return;

        LoadingCircle.Rotate(-Vector3.forward * Time.deltaTime * LoadingCircleSpeed * 100);
    }
}
