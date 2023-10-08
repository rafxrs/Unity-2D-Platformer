using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] ScriptableWeapon w;
    Transform playerTransform;

    void Start()
    {
        
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    public new void Attack()
    {
        AttackAnimation();
    }

    void ShootArrowSameSpot()
    {
        // Debug.Log("ShootingArrow");
        Rigidbody2D arrowInstance = Instantiate(w.weaponPrefab, transform.position, Quaternion.identity);
        Vector2 throwDirection = -transform.right * (transform.localScale.x);
        float randomOffset = Random.Range(-0.25f, 0.25f);
        // Calculate the required initial velocity for the desired arc
        float horizontalDistance = w.launchForce * (w.launchDuration + randomOffset);
        float verticalDistance = (w.launchArcHeight + randomOffset);
        Vector2 initialVelocity = CalculateInitialVelocity(throwDirection, horizontalDistance, verticalDistance);

        // Apply the initial velocity to the spear
        arrowInstance.velocity = initialVelocity;
    }

    void ShootArrowOnPlayer()
    {
        Rigidbody2D arrowInstance = Instantiate(w.weaponPrefab, transform.position, transform.rotation);
        Vector2 launchDirection = (playerTransform.position - transform.position).normalized;

        // Calculate the required initial velocity for the desired arc
        float horizontalDistance = Vector2.Distance(transform.position, playerTransform.position);
        float verticalDistance = w.launchArcHeight;
        Vector2 initialVelocity = CalculateInitialVelocity(launchDirection, horizontalDistance, verticalDistance);

        // Apply the initial velocity to the spear
        arrowInstance.velocity = initialVelocity;
    }

    private void ThrowSpear()
    {
        Rigidbody2D spearInstance = Instantiate(w.weaponPrefab, transform.position, transform.rotation);
        Vector2 throwDirection = (playerTransform.position - transform.position).normalized;

        // Calculate the required initial velocity for the desired arc
        float horizontalDistance = Vector2.Distance(transform.position, playerTransform.position);
        float verticalDistance = w.launchArcHeight;
        Vector2 initialVelocity = CalculateInitialVelocity(throwDirection, horizontalDistance, verticalDistance);

        // Apply the initial velocity to the spear
        spearInstance.velocity = initialVelocity;
    }

    private Vector2 CalculateInitialVelocity(Vector2 direction, float horizontalDistance, float verticalDistance)
    {
        // Calculate the initial velocity components for the desired arc
        float horizontalVelocity = horizontalDistance / w.launchDuration;
        float verticalVelocity = (verticalDistance - (0.5f * Physics2D.gravity.y * Mathf.Pow(w.launchDuration, 2f))) / w.launchDuration;

        // Combine the components with the direction vector
        Vector2 initialVelocity = direction * horizontalVelocity;
        initialVelocity.y = verticalVelocity;

        return initialVelocity;
    }
}
