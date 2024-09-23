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
        var newVelocity = new Vector3(input.x * speed * Time.fixedDeltaTime, rb.velocity.y, input.y * speed * Time.fixedDeltaTime); //Temp variable; sets movement (input.x) and gravity (input.y)

        rb.velocity = newVelocity;
    }
}
