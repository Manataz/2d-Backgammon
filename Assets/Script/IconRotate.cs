using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconRotate : MonoBehaviour
{
    public int dir;
    void Update()
    {
        float rotSpeed = 150 * Time.deltaTime;

        Quaternion rot = transform.rotation;
        Vector3 eulerRot = rot.eulerAngles;
        eulerRot.z += rotSpeed * dir;
        rot = Quaternion.Euler(eulerRot);
        transform.rotation = rot;
    }
}
