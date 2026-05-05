using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VisualPolishBootstrap : MonoBehaviour
{
    private const string RuntimeRootName = "Runtime Visual Polish";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (FindFirstObjectByType<VisualPolishBootstrap>() != null) return;

        GameObject root = new GameObject(RuntimeRootName);
        DontDestroyOnLoad(root);
        root.AddComponent<VisualPolishBootstrap>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private IEnumerator Start()
    {
        yield return ApplyPolishAfterFrame();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ApplyPolishAfterFrame());
    }

    private IEnumerator ApplyPolishAfterFrame()
    {
        yield return null;

        ClearGeneratedVisuals();

        Camera camera = Camera.main;
        if (camera != null)
        {
            ConfigureCamera(camera);
            CreateCloudShadowOverlay(camera);
            CreateAmbientParticles(camera);
        }

        ConfigureExistingLights();
        AddPlayerLight();
        AddWorldHighlights();
    }

    private void ClearGeneratedVisuals()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void ConfigureCamera(Camera camera)
    {
        camera.allowHDR = true;

        UniversalAdditionalCameraData cameraData = camera.GetComponent<UniversalAdditionalCameraData>();
        if (cameraData == null)
        {
            cameraData = camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
        }

        cameraData.renderPostProcessing = true;
        cameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;

        if (camera.GetComponent<CameraJuice>() == null)
        {
            camera.gameObject.AddComponent<CameraJuice>();
        }

        GameObject volumeObject = new GameObject("Global Post Processing");
        volumeObject.transform.SetParent(transform);

        Volume volume = volumeObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 20f;
        volume.profile = CreateVolumeProfile();
    }

    private VolumeProfile CreateVolumeProfile()
    {
        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        profile.name = "Runtime Cozy Post Processing";

        Bloom bloom = profile.Add<Bloom>(true);
        bloom.threshold.Override(0.85f);
        bloom.intensity.Override(0.32f);
        bloom.scatter.Override(0.62f);
        bloom.tint.Override(new Color(1f, 0.96f, 0.74f));

        Vignette vignette = profile.Add<Vignette>(true);
        vignette.intensity.Override(0.18f);
        vignette.smoothness.Override(0.45f);
        vignette.color.Override(new Color(0.04f, 0.08f, 0.05f));

        ColorAdjustments color = profile.Add<ColorAdjustments>(true);
        color.contrast.Override(18f);
        color.saturation.Override(9f);
        color.colorFilter.Override(new Color(1f, 0.96f, 0.88f));

        ChromaticAberration chromaticAberration = profile.Add<ChromaticAberration>(true);
        chromaticAberration.intensity.Override(0.025f);

        FilmGrain grain = profile.Add<FilmGrain>(true);
        grain.type.Override(FilmGrainLookup.Thin1);
        grain.intensity.Override(0.08f);
        grain.response.Override(0.6f);

        return profile;
    }

    private void ConfigureExistingLights()
    {
        Light2D[] lights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
        foreach (Light2D light in lights)
        {
            if (light.lightType != Light2D.LightType.Global) continue;

            light.intensity = 0.7f;
            light.color = new Color(0.78f, 0.88f, 0.72f);
        }
    }

    private void AddPlayerLight()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        if (player.transform.Find("Soft Player Light") != null) return;

        Light2D light = CreatePointLight("Soft Player Light", player.transform, Vector3.zero);
        light.color = new Color(1f, 0.86f, 0.52f);
        light.intensity = 0.85f;
        light.pointLightInnerRadius = 0.35f;
        light.pointLightOuterRadius = 2.8f;
    }

    private void AddWorldHighlights()
    {
        foreach (Objective objective in FindObjectsByType<Objective>(FindObjectsSortMode.None))
        {
            AddBob(objective.gameObject, 0.07f, 1.8f);
            AddHighlightLight(objective.transform, "Objective Glow", new Color(1f, 0.72f, 0.35f), 0.55f, 1.3f);
        }

        foreach (LevelExit exit in FindObjectsByType<LevelExit>(FindObjectsSortMode.None))
        {
            AddBob(exit.gameObject, 0.035f, 1.2f);
            AddHighlightLight(exit.transform, "Exit Glow", new Color(0.55f, 1f, 0.5f), 0.75f, 2.2f);
        }

        foreach (NoiseTrap trap in FindObjectsByType<NoiseTrap>(FindObjectsSortMode.None))
        {
            AddBob(trap.gameObject, 0.035f, 1.5f);
            AddHighlightLight(trap.transform, "Trap Flicker", new Color(1f, 0.48f, 0.25f), 0.35f, 1.4f);
        }
    }

    private void AddBob(GameObject target, float amplitude, float speed)
    {
        if (target.GetComponent<AmbientBob>() != null) return;

        AmbientBob bob = target.AddComponent<AmbientBob>();
        bob.amplitude = amplitude;
        bob.speed = speed;
    }

    private void AddHighlightLight(Transform parent, string lightName, Color color, float intensity, float radius)
    {
        if (parent.Find(lightName) != null) return;

        Light2D light = CreatePointLight(lightName, parent, Vector3.zero);
        light.color = color;
        light.intensity = intensity;
        light.pointLightInnerRadius = radius * 0.15f;
        light.pointLightOuterRadius = radius;
    }

    private Light2D CreatePointLight(string lightName, Transform parent, Vector3 localPosition)
    {
        GameObject lightObject = new GameObject(lightName);
        lightObject.transform.SetParent(parent);
        lightObject.transform.localPosition = localPosition;

        Light2D light = lightObject.AddComponent<Light2D>();
        light.lightType = Light2D.LightType.Point;
        light.blendStyleIndex = 1;
        light.falloffIntensity = 0.75f;

        return light;
    }

    private void CreateCloudShadowOverlay(Camera camera)
    {
        Sprite sprite = Sprite.Create(CreateSoftNoiseTexture(256), new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f), 16f);

        for (int i = 0; i < 2; i++)
        {
            GameObject overlay = new GameObject(i == 0 ? "Drifting Cloud Shadow" : "Slow Canopy Shadow");
            overlay.transform.SetParent(transform);

            SpriteRenderer renderer = overlay.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = i == 0 ? new Color(0.02f, 0.06f, 0.03f, 0.13f) : new Color(0.07f, 0.03f, 0.02f, 0.08f);
            renderer.sortingOrder = 80 + i;

            MovingOverlay moving = overlay.AddComponent<MovingOverlay>();
            moving.cameraToFollow = camera;
            moving.baseScale = i == 0 ? new Vector3(1.8f, 1.15f, 1f) : new Vector3(1.35f, 1.65f, 1f);
            moving.offset = i == 0 ? new Vector2(0.5f, 0.1f) : new Vector2(-0.6f, 0.25f);
            moving.speed = i == 0 ? new Vector2(0.08f, 0.05f) : new Vector2(-0.04f, 0.035f);
            moving.driftAmount = i == 0 ? new Vector2(1.4f, 0.45f) : new Vector2(0.75f, 0.35f);
        }
    }

    private Texture2D CreateSoftNoiseTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float u = x / (float)size;
                float v = y / (float)size;
                float noise = Mathf.PerlinNoise(u * 4.5f + 3.1f, v * 4.5f + 8.7f);
                float secondNoise = Mathf.PerlinNoise(u * 9.5f + 12.4f, v * 9.5f + 2.9f);
                float alpha = Mathf.SmoothStep(0.46f, 0.82f, noise * 0.75f + secondNoise * 0.25f);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        return texture;
    }

    private void CreateAmbientParticles(Camera camera)
    {
        GameObject particlesObject = new GameObject("Ambient Floating Leaves");
        particlesObject.transform.SetParent(transform);

        FollowCamera follow = particlesObject.AddComponent<FollowCamera>();
        follow.cameraToFollow = camera;
        follow.zOffset = 8f;

        ParticleSystem particles = particlesObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(5f, 9f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.12f, 0.35f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.035f, 0.085f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 6.28f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.95f, 0.72f, 0.28f, 0.38f), new Color(0.35f, 0.8f, 0.45f, 0.22f));
        main.maxParticles = 70;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 5f;

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(18f, 11f, 0.1f);

        ParticleSystem.VelocityOverLifetimeModule velocity = particles.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.18f, 0.08f);
        velocity.y = new ParticleSystem.MinMaxCurve(-0.03f, 0.08f);
        velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        ParticleSystem.ColorOverLifetimeModule color = particles.colorOverLifetime;
        color.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.95f, 0.74f, 0.33f), 0f),
                new GradientColorKey(new Color(0.38f, 0.78f, 0.42f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.35f, 0.18f),
                new GradientAlphaKey(0.25f, 0.78f),
                new GradientAlphaKey(0f, 1f)
            });
        color.color = gradient;

        ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 90;
        renderer.material = new Material(Shader.Find("Sprites/Default"));
    }
}
