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

    public void SetNormalRotation()
    {
        canSpin = false;
        // Go back to normal rotation
        transform.localRotation = new Quaternion(0.00000f, 0.00000f, 0.70711f, 0.70711f);
    }

    public void SetRotationOnSpawn(float rotation){
        transform.rotation = new Quaternion(0,0,rotation,0);
    }
}
