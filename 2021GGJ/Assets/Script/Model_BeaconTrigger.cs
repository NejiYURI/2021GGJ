/// <summary>
/// ���a�i�J�T���d��Ĳ�o�Ҳ�
/// </summary>
public class Model_BeaconTrigger
{
    public Model_BeaconTrigger(string playertag, float addscore,float distance_per,bool isjam)
    {
        this.PlayerTag = playertag;
        this.Distance_Percentage = distance_per;
        this.AddScore = addscore;
        this.IsJam = isjam;
    }

    /// <summary>
    /// ���a����
    /// </summary>
    public string PlayerTag;

    /// <summary>
    /// �i�J�d�����[����
    /// </summary>
    public float AddScore;

    /// <summary>
    /// �Z��(�ʤ���)
    /// </summary>
    public float Distance_Percentage;

    /// <summary>
    /// �O�_�Q�Y�y�q
    /// </summary>
    public bool IsJam;
}
