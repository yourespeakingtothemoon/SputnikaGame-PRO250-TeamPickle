using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotation : MonoBehaviour
{
    public Quaternion rotation;

    void Start()
    {
        rotation = transform.parent.localRotation;
    }

    void Update()
    {
        transform.localRotation = Quaternion.Inverse(transform.parent.localRotation) * rotation * transform.localRotation;
        rotation = transform.parent.localRotation;
    }
}
