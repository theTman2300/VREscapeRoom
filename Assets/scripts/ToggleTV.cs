using System.ComponentModel;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer), typeof(MeshFilter))]
public class ToggleTV : MonoBehaviour
{
    [SerializeField] VideoClip[] clips;
    [SerializeField] Material whiteMaterial;
    [SerializeField] bool isPlaying = false;
    VideoPlayer player;
    MeshRenderer meshRenderer;
    Material material;

    private void OnEnable()
    {
        player = GetComponent<VideoPlayer>();
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
    }

    public void TogglePlayStop()
    {
        isPlaying = !isPlaying;
        if (isPlaying)
        {
            meshRenderer.material = whiteMaterial;
            player.clip = clips[Random.Range(0, clips.Length)];
            player.Play();
        }
        else
        {
            player.Stop();
            meshRenderer.material = material;
        }
    }
}
