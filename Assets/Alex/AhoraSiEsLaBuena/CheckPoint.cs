using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    MapsV2 maps;

    public void SetMaps(MapsV2 map)
    {
        maps = map;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            maps.CheckPointCollected(this);
        }
    }
}
