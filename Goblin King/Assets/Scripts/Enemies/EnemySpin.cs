using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpin : MonoBehaviour
{
    [SerializeField] float spinSpeed = 10f;
    bool canSpin;

    void Update()
    {
        if(canSpin)
        {
            transform.Rotate(0,0,spinSpeed * Time.deltaTime);
        }
    }

    public void StartBodySpin()
    {
        canSpin = true;
    }

    public void StopBodySpin()
    {
        canSpin = false;
    }
}
