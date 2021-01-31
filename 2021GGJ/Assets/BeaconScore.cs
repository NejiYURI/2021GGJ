using UnityEngine;

/// <summary>
/// 癟腹方环だ计砞﹚
/// </summary>
[System.Serializable]
public class BeaconScore
{
    public BeaconScore()
    {
        this.Distance = 0;
        this.Score = 0;
    }

    /// <summary>
    /// 籔癟腹方禯瞒(κだゑ)
    /// </summary>
    [SerializeField]
    public float Distance;

    /// <summary>
    /// だ计
    /// </summary>
    [SerializeField]
    public float Score;
}

