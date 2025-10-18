using UnityEngine;

public class GasParticleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private int particleCount = 100;
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(4f, 2f);
    [SerializeField] private float particleSpeed = 2.0f;
    [SerializeField] private Color particleColor = Color.gray;
    [SerializeField] private Transform spawnParent;

    private GameObject[] particles;
    private Vector2[] directions;

    private void Start()
    {
        particles = new GameObject[particleCount];
        directions = new Vector2[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f),
                0);

            GameObject particle = Instantiate(particlePrefab, transform.position + randomPos, Quaternion.identity, spawnParent);

            var spriteRenderer = particle.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.color = particleColor;

            particles[i] = particle;

            directions[i] = Random.insideUnitCircle.normalized;
        }
    }

    private void Update()
    {
        for (int i = 0; i < particleCount; i++)
        {
            Vector3 pos = particles[i].transform.localPosition;
            Vector2 dir = directions[i];

            // Move particle
            pos += (Vector3)(dir * particleSpeed * Time.deltaTime);

            // Check boundaries and bounce
            if (pos.x > spawnAreaSize.x / 2f || pos.x < -spawnAreaSize.x / 2f)
            {
                dir.x *= -1;
                pos.x = Mathf.Clamp(pos.x, -spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
            }

            if (pos.y > spawnAreaSize.y / 2f || pos.y < -spawnAreaSize.y / 2f)
            {
                dir.y *= -1;
                pos.y = Mathf.Clamp(pos.y, -spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
            }

            particles[i].transform.localPosition = pos;
            directions[i] = dir.normalized;
        }
    }
}
