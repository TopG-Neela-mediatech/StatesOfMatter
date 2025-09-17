using UnityEngine;
using DG.Tweening;

public class SolidSystemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private int gridHeight = 5;
    [SerializeField] private float spacing = 0.5f;
    [SerializeField] private Color particleColor = Color.blue;

    [SerializeField] private Transform parentTransform;


    [Header("Vibration Settings")]
    [SerializeField] private Vector2 vibrationOffset = new Vector2(0.05f, 0.05f);
    [SerializeField] private float vibrationDuration = 1f;

    private void Start()
    {
        CreateCenteredSolidGrid();
    }

    private void CreateCenteredSolidGrid()
    {
        Vector3 origin = transform.position;
        float totalWidth = (gridWidth - 1) * spacing;
        float totalHeight = (gridHeight - 1) * spacing;
        Vector3 startOffset = new Vector3(-totalWidth / 2f, -totalHeight / 2f, 0);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = origin + startOffset + new Vector3(x * spacing, y * spacing, 0);
                GameObject particle = Instantiate(particlePrefab, position, Quaternion.identity, parentTransform);

                var spriteRenderer = particle.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                    spriteRenderer.color = particleColor;

                ApplyVibration(particle.transform);
            }
        }
    }

    private void ApplyVibration(Transform particleTransform)
    {
        Vector3 targetOffset = new Vector3(
            Random.Range(-vibrationOffset.x, vibrationOffset.x),
            Random.Range(-vibrationOffset.y, vibrationOffset.y),
            0f);

        particleTransform.DOLocalMove(
            particleTransform.localPosition + targetOffset,
            vibrationDuration
        ).SetLoops(-1, LoopType.Yoyo)
         .SetEase(Ease.InOutSine);
    }
}
