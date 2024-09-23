using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    Rigidbody rb;
    private Vector2 input;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); //Finds the rigidbody componenet from a game obejct
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate() //Based on physics framerate (not always consistent)
    {
        var newInput = GetCameraInput(input, Camera.main);

        var newVelocity = new Vector3(newInput.x * speed * Time.fixedDeltaTime, rb.velocity.y, newInput.z * speed * Time.fixedDeltaTime); //Temp variable; sets movement and gravity based on camera position (newInput)

        rb.velocity = newVelocity;
    }

    Vector3 GetCameraInput(Vector2 input, Camera cam) //setting movement based on camera position
    {
        Vector3 camRight = cam.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        return input.x * camRight + input.y * camForward; //multiplying input by the camera vector then adding both x and y
    }
}
