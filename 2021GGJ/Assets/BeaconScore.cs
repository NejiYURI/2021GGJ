using UnityEngine;

/// <summary>
/// �T����������Ƴ]�w
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
    /// �P�T�����Z��(�ʤ���)
    /// </summary>
    [SerializeField]
    public float Distance;

    /// <summary>
    /// ����
    /// </summary>
    [SerializeField]
    public float Score;
}

