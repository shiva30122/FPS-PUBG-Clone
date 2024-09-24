using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;             // Speed at which the bullet travels
    public float lifetime = 2f;           // Lifetime before the bullet is destroyed
    public float damageAmount = 10f;      // Damage amount the bullet will deal
    public bool applyGravity = false;     // Should gravity affect the bullet?
    public float gravity = -9.81f;        // Gravity value, only used if applyGravity is true

    private Vector3 velocity;             // Velocity of the bullet
    private Rigidbody rb;                 // Reference to the bullet's Rigidbody

    void Start()
    {
        rb = GetComponent<Rigidbody>();


        // Ensure the bullet moves forward relative to its initial rotation
        velocity = transform.forward * speed;

        // If gravity is applied, make sure the Rigidbody's gravity is also enabled
        rb.useGravity = applyGravity;

        // Set the initial velocity
        rb.velocity = velocity;

        // Destroy the bullet after its lifetime
        Destroy(gameObject, lifetime);
    }


    void Update()
    {
        // If gravity is applied, manually adjust the velocity to include gravity
        if (applyGravity)
        {
            velocity.y += gravity * Time.deltaTime; // Apply gravity over time
            rb.velocity = velocity;                // Update the Rigidbody velocity
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Access the Health_ script on the object the bullet collides with
        Health_ targetHealth = collision.gameObject.GetComponent<Health_>();
        
        if (targetHealth != null)
        {
            targetHealth.Take_Damage(damageAmount); // Apply damage
        }

        // Destroy the bullet after it hits something
        Destroy(gameObject);
    }
}
