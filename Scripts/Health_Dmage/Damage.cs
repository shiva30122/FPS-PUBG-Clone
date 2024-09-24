using System.Collections;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public float Take_Damage; // Amount of damage applied
    
    public bool IsDestroyable; // Should the object be destroyed after a delay

    public float DestroyTime; // Time before destruction if destroyable
    public bool IsObstacal; // Is this object an obstacle with continuous damage

    public float DamageInterval = 1f; // Time between damage application for obstacles

    private Collider currentCollider; // To track the current collider for continuous damage

    private void OnTriggerEnter(Collider other)
    {
        var Get_Health = other.GetComponent<Health_>();
        if (Get_Health)
        {
            Get_Health.Take_Damage(Take_Damage); // Apply damage immediately

            if (IsObstacal)
            {
                // For obstacles, start applying damage over time
                currentCollider = other;
                StartCoroutine(ApplyDamageOverTime());
            }
            else
            {
                // For bullets or one-shot objects
                if (IsDestroyable)
                {
                    StartCoroutine(DelayedDestroy(DestroyTime)); // Destroy after delay
                }
                else
                {
                    Destroy(gameObject); // Destroy immediately
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsObstacal && other == currentCollider)
        {
            currentCollider = null; // Stop applying damage when exiting the obstacle
        }
    }

    private IEnumerator DelayedDestroy(float time)
    {
        yield return new WaitForSeconds(time); // Wait for the specified time
        Destroy(gameObject); // Destroy the GameObject after the wait
    }

    private IEnumerator ApplyDamageOverTime()
    {
        while (currentCollider != null)
        {
            var health = currentCollider.GetComponent<Health_>(); // Get the Health_ component from the current collider
            if (health != null)
            {
                health.Take_Damage(Take_Damage); // Apply damage
            }
            yield return new WaitForSeconds(DamageInterval); // Wait before applying damage again
        }
    }
}
