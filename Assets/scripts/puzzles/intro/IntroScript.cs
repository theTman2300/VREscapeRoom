using UnityEngine;

public class IntroScript : MonoBehaviour
{
    [SerializeField] bool skipIntro = false;
    [Space]
    [SerializeField] Color fogColor = Color.black;
    [SerializeField] float fogDensity = 1;
    [Space]
    [SerializeField] GameObject lights;
    [SerializeField] GameObject introColliders;
    [SerializeField] GameObject introLight;

    Color oldFogColor;
    float oldFogDesnity;

    private void Start()
    {
        lights.SetActive(false);
        introLight.SetActive(true);

        oldFogColor = RenderSettings.fogColor;
        RenderSettings.fogColor = fogColor;
        oldFogDesnity = RenderSettings.fogDensity;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fog = true;

        if (skipIntro) SwitchOn();
    }

    public void SwitchOn()
    {
        RenderSettings.fogColor = oldFogColor;
        RenderSettings.fogDensity = oldFogDesnity;
        RenderSettings.fog = false;

        lights.SetActive(true);
        introColliders.SetActive(false);
        introLight.SetActive(false);
    }
}
