using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] Transform HourHand;
    [SerializeField] Transform MinuteHand;
    [SerializeField] Transform SecondHand;



    private void Update()
    {
        float secondDegrees = (DateTime.Now.Second / 60f) * 360;
        float minuteDegrees = (DateTime.Now.Minute / 60f) * 360 + (secondDegrees / 60);
        float HourDegrees = ((DateTime.Now.Hour % 12) / 12f) * 360 + (minuteDegrees / 12);
        HourHand.localRotation = Quaternion.Euler(HourDegrees, 0, 0);
        MinuteHand.localRotation = Quaternion.Euler(minuteDegrees, 0, 0);
        SecondHand.localRotation = Quaternion.Euler(secondDegrees, 0, 0);
    }
}
