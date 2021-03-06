/// <summary>
/// 玩家進入訊號範圍的觸發模組
/// </summary>
public class Model_BeaconTrigger
{
    public Model_BeaconTrigger(string playertag, float addscore,float distance_per)
    {
        this.PlayerTag = playertag;
        this.Distance_Percentage = distance_per;
        this.AddScore = addscore;
    }

    /// <summary>
    /// 玩家標籤
    /// </summary>
    public string PlayerTag;

    /// <summary>
    /// 進入範圍後欲加分數
    /// </summary>
    public float AddScore;

    /// <summary>
    /// 距離(百分比)
    /// </summary>
    public float Distance_Percentage;
}
