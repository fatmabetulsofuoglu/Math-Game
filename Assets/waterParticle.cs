using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterParticle : MonoBehaviour
{
    public float freezeTime;
    private void OnEnable()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        Invoke("Disable", freezeTime);
    }

    private void Disable()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }
    private void OnDisable()
    {
        CancelInvoke();
    }
}
