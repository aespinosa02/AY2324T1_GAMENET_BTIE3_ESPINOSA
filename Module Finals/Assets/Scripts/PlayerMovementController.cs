using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class PlayerMovementController : MonoBehaviour
{
    private RigidbodyFirstPersonController rigidbodyFirstPersonController;

    public float sensitivityX = 5F;
	public float sensitivityY = 5F;

    public bool isControlEnabled;

    private void Start()
    {
        rigidbodyFirstPersonController = GetComponent<RigidbodyFirstPersonController>();
        isControlEnabled = false;
    }

    void LateUpdate()
    {
        if (isControlEnabled)
        { 
        rigidbodyFirstPersonController.joystickInputAxis.x = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        rigidbodyFirstPersonController.joystickInputAxis.y = Input.GetAxisRaw("Vertical") * Time.deltaTime;

        rigidbodyFirstPersonController.mouseLook.lookInputAxis.x = Input.GetAxis("Mouse X") * sensitivityX;
        rigidbodyFirstPersonController.mouseLook.lookInputAxis.y = Input.GetAxis("Mouse Y") * sensitivityY;
        }
    }
}
