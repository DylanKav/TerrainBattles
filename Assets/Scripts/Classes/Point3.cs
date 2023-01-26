using TMPro;
using UnityEngine;

public record Point3
{
    public Vector3 Position;
    public bool isOn;

    public Point3(Vector3 position)
    {
        isOn = false;
        Position = position;
    }

    public void ToggleState(bool onOff)
    {
        isOn = onOff;
    }
}
