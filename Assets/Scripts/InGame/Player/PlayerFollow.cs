using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField] Transform trFollow;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, trFollow.position, 0.5f);
    }
}
