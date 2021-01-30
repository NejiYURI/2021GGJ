using UnityEngine;

[System.Serializable]
public class BeaconScore
{
    public BeaconScore()
    {
        this.Distance = 0;
        this.Score = 0;
    }

    [SerializeField]
    public float Distance;

    [SerializeField]
    public float Score;
}

