using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    public void StartReset()
    {
        StartCoroutine(ResetState());
    }

    private IEnumerator ResetState()
    {
        yield return new WaitForSeconds(10);
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
