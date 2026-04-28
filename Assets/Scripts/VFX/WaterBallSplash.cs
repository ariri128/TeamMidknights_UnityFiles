using UnityEngine;

public class WaterBallSplash : MonoBehaviour
{
    public GameObject splashPrefab;

    public float speed = 12f;
    public float lifetime = 4f;
    public float splashRadius = 2.5f;
    [Tooltip("Damage dealt to the General by the water ball (guards are still killed instantly).")]
    public int generalDamage = 75;
    public LayerMask collisionLayers = ~0;

    private Vector3 moveDirection;
    private bool hasLaunched = false;
    private bool hasHit = false;

    public void Launch(Vector3 direction)
    {
        moveDirection = direction.normalized;
        hasLaunched = true;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!hasLaunched || hasHit)
        {
            return;
        }

        float moveDistance = speed * Time.deltaTime;

        if (Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, moveDistance, collisionLayers))
        {
            transform.position = hit.point;
            Hit();
            return;
        }

        transform.position += moveDirection * moveDistance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Hit();
    }

    private void OnTriggerEnter(Collider other)
    {
        Hit();
    }

    private void Hit()
    {
        if (hasHit)
        {
            return;
        }

        hasHit = true;

        Vector3 spawnPosition = transform.position;

        // Raycast down to find ground
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, 5f))
        {
            spawnPosition = groundHit.point;
        }

        Debug.Log("Spawning splash at " + spawnPosition);

        if (splashPrefab != null)
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, groundHit.normal);
            Instantiate(splashPrefab, spawnPosition, rotation);
        }

        Collider[] hits = Physics.OverlapSphere(spawnPosition, splashRadius);

        for (int i = 0; i < hits.Length; i++)
        {
            GuardHealth guardHealth = hits[i].GetComponentInParent<GuardHealth>();
            GeneralHealth generalHealth = hits[i].GetComponentInParent<GeneralHealth>();

            if (guardHealth != null)
            {
                guardHealth.KillImmediately();
            }
            else if (generalHealth != null)
            {
                generalHealth.TakeDamage(generalDamage);
            }
        }

        Destroy(gameObject);
    }
}
