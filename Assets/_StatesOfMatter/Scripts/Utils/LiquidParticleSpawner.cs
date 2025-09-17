using UnityEngine;

public class LiquidParticleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;  // Prefab with a square or circle image/sprite
    [SerializeField] private int particleCount = 100;
    [SerializeField] private Vector2 areaSize = new Vector2(4f, 2f);  // Size of the liquid container area
    [SerializeField] private float particleSpeed = 0.5f;
    [SerializeField] private Color particleColor = Color.blue;

    [SerializeField] private Transform parentTransform;

    private GameObject[] particles;
    private Vector2[] directions;

    void Start()
    {
        particles = new GameObject[particleCount];
        directions = new Vector2[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f),
                0);

            GameObject particle = Instantiate(particlePrefab, transform.position + randomPos, Quaternion.identity, parentTransform);

            var spriteRenderer = particle.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = particleColor;
            }

            particles[i] = particle;

            // Random initial direction
            directions[i] = Random.insideUnitCircle.normalized;
        }
    }

    void Update()
    {
        for (int i = 0; i < particleCount; i++)
        {
            Vector3 pos = particles[i].transform.localPosition;
            Vector2 dir = directions[i];

            // Move particle
            pos += (Vector3)(dir * particleSpeed * Time.deltaTime);

            // Bounce off container walls
            if (pos.x > areaSize.x / 2f || pos.x < -areaSize.x / 2f)
            {
                dir.x *= -1;
            }

            if (pos.y > areaSize.y / 2f || pos.y < -areaSize.y / 2f)
            {
                dir.y *= -1;
            }

            particles[i].transform.localPosition = pos;
            directions[i] = dir;
        }
    }
}
