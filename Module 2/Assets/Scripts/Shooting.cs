using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public GameObject hitEffectPrefab;

    [Header("HP Related Items")]
    public float startHealth = 100;
    private float currentHealth;
    public Image healthBar;

    private Animator animator;

    void Start()
    {
        currentHealth = startHealth;
        healthBar.fillAmount = currentHealth / startHealth;

        animator = this.GetComponent<Animator>();
        
    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);
            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 20);
            }
        }
    }
    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.currentHealth -= damage;
        this.healthBar.fillAmount = currentHealth / startHealth;

        if (currentHealth == 0)
        {
            Die();
            IncreaseScore(info);
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGO = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGO, 0.2f);
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        float respawnTime = 5.0f;

        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<TextMeshProUGUI>().text = "You are killed. Respawn in "
                + respawnTime.ToString(".00");
        }

        animator.SetBool("isDead", false);
        respawnText.GetComponent<TextMeshProUGUI>().text = "";

        this.transform.position = GameManager.instance.SpawnPlayer();
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        currentHealth = 100;
        healthBar.fillAmount = currentHealth / startHealth;
    }

    private void IncreaseScore(PhotonMessageInfo info)
    {
        info.Sender.AddScore(1);

        if (info.Sender.GetScore() == 9)
        {
            GameManager.instance.EndScreen(info.Sender);
        }
                    
        GameManager.instance.KillReportUpdate(info.Sender, info.photonView.Owner);
        Debug.Log(info.Sender.NickName + " has a score of " + info.Sender.GetScore());
    }
}
