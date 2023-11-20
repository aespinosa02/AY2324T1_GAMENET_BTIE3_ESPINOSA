using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerShooting : MonoBehaviourPunCallbacks
{
    public Transform turretTransform;
    public Transform barrelTransform;

    public GameObject bulletSpawnLoc;

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        
    [Header("Mouse Look Variable")]
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 10F;
	public float sensitivityY = 10F;

	public float minimumX = -90F;
	public float maximumX = 90;

	public float minimumY = -20F;
	public float maximumY = 20F;

    [Header("HP Related Items")]
    public float startHealth = 100;
    public float currentHealth;
    public Image healthBar;

	float rotationY = 0F;

    public bool isControlEnabled;

    void Start()
    {
        isControlEnabled = false;
        currentHealth = startHealth;
        healthBar.fillAmount = currentHealth / startHealth;
    }

	void LateUpdate ()
	{
        if (isControlEnabled)
        {
            TurretControl();
            Shoot();
        }
	}
	
    void TurretControl()
    {
		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = turretTransform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			turretTransform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		} 
		else if (axes == RotationAxes.MouseX)
		{
			turretTransform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			turretTransform.localEulerAngles = new Vector3(-rotationY, turretTransform.localEulerAngles.y, 0);
		} 
    }

    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Fire");
            if (GetComponent<PlayerSetup>().isRaycast)
            {
                FireRayCast();
            }
            else if (!GetComponent<PlayerSetup>().isRaycast)
            {
                FireProjectile();
            }
        }
    }

    public void FireRayCast()
    {
        RaycastHit hit;
        Ray ray = new Ray(barrelTransform.position, barrelTransform.forward);

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 20);
            }
        }
    }

    public void FireProjectile()
    { 
        GameObject bullet = PhotonNetwork.Instantiate("BulletPrefab", bulletSpawnLoc.transform.position, bulletSpawnLoc.transform.rotation);
        bullet.GetComponentInChildren<BulletBehaviour>().bulletOwner = photonView.Owner.NickName;
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.currentHealth -= damage;
        this.healthBar.fillAmount = currentHealth / startHealth;

        if (currentHealth == 0)
        {
           GetComponent<BattleRoyaleController>().OnPlayerDeath();
           DeathRaceManager.instance.KillReportUpdate(info.Sender, info.photonView.Owner);
        }
    }
}
