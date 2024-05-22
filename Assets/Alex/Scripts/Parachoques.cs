using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Parachoques : MonoBehaviour
{
    public string tags;
    public UnityEvent choque;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == tags)
        {
            choque.Invoke();
        }
    }
}
