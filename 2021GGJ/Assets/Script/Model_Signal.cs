using UnityEngine;
[System.Serializable]
public class Model_Signal
{
    public Model_Signal()
    {
        this.Distance = 0;
    }

    [SerializeField]
    public float Distance;

    [SerializeField]
    public Sprite SignalIcon;
}
