using UnityEngine;

public class ControlPanel : MonoBehaviour
{
    [SerializeField] GameObject[] satellites;
    [SerializeField] Material disabledMaterial;
    [SerializeField] Material enabledMaterial;
    [Space]
    [SerializeField] AudioSource errorAudio;
    [SerializeField] GameObject panel;
    [Space]
    [SerializeField] SatelliteLever lever;
    [SerializeField] OrganicInput organicInput;
    [Space]
    [SerializeField] bool skipPuzzle = false;

    int enabledSatelliteCount;
    bool puzzleActive = true;

    private void Start()
    {
        RandomizeSatellites();

        if (skipPuzzle)
        {
            Debug.Log("Skipping puzle control panel");
        }
    }

    void RandomizeSatellites()
    {
        //shuffle satellites array
        for (int i = 0; i < 200; i++)
        {
            int randomIndex1 = Random.Range(0, satellites.Length);
            int randomIndex2 = Random.Range(0, satellites.Length);
            GameObject sattelite1 = satellites[randomIndex1];
            GameObject sattelite2 = satellites[randomIndex2];
            satellites[randomIndex1] = sattelite2;
            satellites[randomIndex2] = sattelite1;
        }

        enabledSatelliteCount = RandomRangeExcluding(0, satellites.Length, lever.GetCurrentInput() - 1); //makes sure the number can never be the same as the current select number to avoid brute forcing
        for (int i = 0; i < satellites.Length; i++)
        {
            if (i > enabledSatelliteCount)
            {
                satellites[i].GetComponent<MeshRenderer>().material = disabledMaterial;
            }
            else
            {
                satellites[i].GetComponent<MeshRenderer>().material = enabledMaterial;
            }
        }
        enabledSatelliteCount++;
    }

    public void OnButtonPress()
    {
        if (!puzzleActive) return;

        if (lever.GetCurrentInput() == enabledSatelliteCount && organicInput.CheckCorrectObject() || skipPuzzle)
        {
            panel.SetActive(false);
            puzzleActive = false;
        }
        else
        {
            RandomizeSatellites();
            errorAudio.Play();
        }
    }

    int RandomRangeExcluding(int minInclusive, int MaxExclusive, int ExcludingNumber)
    {
        int number = Random.Range(minInclusive, MaxExclusive);
        if (ExcludingNumber == number)
        {
            if (number == satellites.Length)
            {
                number--;
                return number;
            }
            number++;
            return number;
        }

        return number;
    }
}
