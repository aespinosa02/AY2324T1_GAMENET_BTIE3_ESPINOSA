using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointTracker : MonoBehaviour
{
    public int index;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<RaceController>().enabled)
            {
                other.gameObject.GetComponent<RaceController>().SetRespawnIndex(index);
            }
        }
    }
}
