using UnityEngine;

public class ParticleSystemSpawner : MonoBehaviour
{
    private ParticleSystem particleEffect;

    void Start()
    {
        CreateBoxParticleSystem();
    }

    void CreateBoxParticleSystem()
    {
        // Create a new GameObject to hold the Particle System
        GameObject psGO = new GameObject("BoxParticleSystem");
        psGO.transform.position = Vector3.zero;
        psGO.transform.SetParent(null, true);
        // Add Particle System Component
        particleEffect = psGO.AddComponent<ParticleSystem>();
        var main = particleEffect.main;
        main.loop = true;
        main.startLifetime = 5f;
        main.startSpeed = 1f;
        main.startSize = 0.1f;
        main.maxParticles = 500;

        var emission = particleEffect.emission;
        emission.rateOverTime = 100;

        var shape = particleEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(5f, 5f, 0.1f);  // Flat box for 2D effect

        var renderer = particleEffect.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Sprites/Default")); // Simple material

        // Optional: Color over Lifetime (blue → red)
        var colorOverLifetime = particleEffect.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        // Play the particle system
        particleEffect.Play();
    }
}
