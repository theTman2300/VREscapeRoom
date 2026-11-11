using UnityEngine;

public class IntroScript : MonoBehaviour
{
    [SerializeField] Color fogColor = Color.black;
    [SerializeField] float fogDensity = 1;
    //[SerializeField] float fogEndDistance = 1f;
    [Space]
    [SerializeField] GameObject lights;
    [SerializeField] GameObject introColliders;
    [SerializeField] GameObject introLight;

    Color oldFogColor;
    //float oldFogStartDistance;
    //float oldFogEndDistance;
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
        //oldFogStartDistance = RenderSettings.fogStartDistance;
        //RenderSettings.fogStartDistance = 0;
        //oldFogEndDistance = RenderSettings.fogEndDistance;
        //RenderSettings.fogEndDistance = fogEndDistance;
    }

    public void SwitchOn()
    {
        Debug.Log("Test Success");
        RenderSettings.fogColor = oldFogColor;
        RenderSettings.fogDensity = oldFogDesnity;
        RenderSettings.fog = false;
        //RenderSettings.fogStartDistance = oldFogStartDistance;
        //RenderSettings.fogEndDistance = oldFogEndDistance;

        lights.SetActive(true);
        introColliders.SetActive(false);
        introLight.SetActive(false);
    }
}
