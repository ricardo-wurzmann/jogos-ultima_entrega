using UnityEngine;

public static class VisualFeedback
{
    public static void ObjectiveCollected(Vector3 position)
    {
        Burst(position, new Color(1f, 0.72f, 0.25f, 0.95f), 18, 1.1f);
        CameraJuice.Shake(0.06f, 0.12f);
    }

    public static void TrapTriggered(Vector3 position)
    {
        Burst(position, new Color(1f, 0.28f, 0.16f, 0.9f), 22, 1.35f);
        CameraJuice.Shake(0.14f, 0.2f);
    }

    public static void PlayerSpotted(Vector3 position)
    {
        Burst(position, new Color(1f, 0.18f, 0.08f, 0.8f), 14, 0.8f);
        CameraJuice.Shake(0.08f, 0.12f);
    }

    public static void LevelCompleted(Vector3 position)
    {
        Burst(position, new Color(0.55f, 1f, 0.42f, 0.95f), 34, 1.8f);
        CameraJuice.Shake(0.1f, 0.22f);
    }

    private static void Burst(Vector3 position, Color color, int count, float speed)
    {
        GameObject burstObject = new GameObject("Visual Burst");
        burstObject.transform.position = position;

        ParticleSystem particles = burstObject.AddComponent<ParticleSystem>();
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.35f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.75f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.45f, speed);
        main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.14f);
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.enabled = false;

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.08f;

        ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 120;
        renderer.material = new Material(Shader.Find("Sprites/Default"));

        particles.Emit(count);
        Object.Destroy(burstObject, 1.2f);
    }
}
