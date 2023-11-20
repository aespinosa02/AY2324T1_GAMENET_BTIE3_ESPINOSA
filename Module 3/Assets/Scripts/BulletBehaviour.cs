using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletBehaviour : MonoBehaviour
{
    public float speed;
    public Rigidbody rb;

    public string bulletOwner;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(LifeSpan());
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().Owner.NickName != bulletOwner)
        {
            Debug.Log(bulletOwner + " has hit " + other.gameObject.GetComponent<PhotonView>().Owner.NickName);
            Destroy(gameObject);
            other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 20);
        }
    }

    private IEnumerator LifeSpan()
    {
        yield return new WaitForSeconds(6f);
        Destroy(gameObject);
    }
}
