using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconScript : MonoBehaviour
{

    /// <summary>
    /// 距離與分數設定
    /// </summary>
    [SerializeField]
    public List<BeaconScore> ScoreDisList;

    /// <summary>
    /// 碰撞collider
    /// </summary>
    private CircleCollider2D thiscol;

    private int LifeTime;

    private int LifeTimeCounter;

    [SerializeField]
    private bool IsActive;

    private void Start()
    {
        this.thiscol = this.gameObject.GetComponent<CircleCollider2D>();
        this.LifeTime = 0;
        this.IsActive = false;
    }
    /// <summary>
    /// 此訊號增加分數量
    /// </summary>
    [SerializeField]
    private float ScoreAdd;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (this.IsActive)
        {
            //檢查碰撞物件層級是否為編號6(玩家Layer)
            if (collision.gameObject.layer == 6)
            {

                float dis = Vector2.Distance(this.transform.position, collision.transform.position);
                float dis_per = (dis / this.thiscol.radius) * 100;
                Debug.Log(collision.tag + " In!! dis:" + dis);
                foreach (var item in this.ScoreDisList)
                {
                    if (dis_per <= item.Distance)
                    {
                        ScoreAdd = item.Score;
                    }
                    else
                    {
                        break;
                    }
                }
                //觸發訂閱事件
                GameManager.gameManager.TriggerBeaconIn(new Model_BeaconTrigger(collision.tag,ScoreAdd, dis_per));
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (this.IsActive)
        {
            //檢查碰撞物件層級是否為編號6(玩家Layer)
            if (collision.gameObject.layer == 6)
            {
                Debug.Log(collision.tag + " Exit!!");
                //觸發訂閱事件
                GameManager.gameManager.TriggerBeaconExit(collision.tag);
            }
        }
    }

    public void SetBeacon(Vector2 Pos, List<BeaconScore> ScoreSet, int LifeTime)
    {
        this.transform.position = Pos;
        this.ScoreDisList = ScoreSet;
        this.LifeTime = LifeTime;
        this.LifeTimeCounter = 0;
        this.IsActive = true;
        StartCoroutine(ScoreAddIEnum());
    }

    IEnumerator ScoreAddIEnum()
    {
        yield return new WaitForSeconds(1f);
        this.LifeTimeCounter++;
        if (this.LifeTimeCounter >= this.LifeTime)
        {
            this.IsActive = false;
        }
        else
        {
            StartCoroutine(ScoreAddIEnum());
        }

    }

}



