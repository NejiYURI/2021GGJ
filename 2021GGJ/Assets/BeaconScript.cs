using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField]
    private int LifeTime;
    [SerializeField]
    private int LifeTimeCounter;

    public SpriteRenderer spriteRenderer;

    private float MaxDis;

    [SerializeField]
    private bool IsActive;

    private List<InFieldData> PlayerInfieldList;

    private void Awake()
    {
        this.thiscol = this.gameObject.GetComponent<CircleCollider2D>();
    }
    private void Start()
    {
        this.LifeTime = 0;
        this.IsActive = false;
        this.PlayerInfieldList = new List<InFieldData>();
        this.thiscol.enabled = false;
       // SetBeacon(new Vector2(Random.Range(-9, 9), Random.Range(-5, 5)), 5);
    }
    /// <summary>
    /// 此訊號增加分數量
    /// </summary>
    [SerializeField]
    private float ScoreAdd;

    private void FixedUpdate()
    {
        if (this.PlayerInfieldList.Count > 0)
        {
            MaxDis = this.PlayerInfieldList.OrderByDescending(x => x.Dis).First().Dis;
            if (MaxDis < 80f)
            {
                this.spriteRenderer.color = new Color(this.spriteRenderer.color.r, this.spriteRenderer.color.g, this.spriteRenderer.color.b, 1.2f - (MaxDis / 100));
            }

        }
        else
        {
            this.spriteRenderer.color = new Color(this.spriteRenderer.color.r, this.spriteRenderer.color.g, this.spriteRenderer.color.b, 0);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (this.IsActive)
        {
            //檢查碰撞物件層級是否為編號6(玩家Layer)
            if (collision.gameObject.layer == 6)
            {
                PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
                if (playerController.CheckState() == 2) {
                    this.PlayerInfieldList.RemoveAll(x => x.PlayerTag.Equals(collision.tag));
                    return;
                }
               
                float dis = Vector2.Distance(this.transform.position, collision.transform.position);
                float dis_per = (dis / this.thiscol.radius) * 100;
                if (dis_per > 100) dis_per = 100;
                if (!this.PlayerInfieldList.Exists(x => x.PlayerTag.Equals(collision.tag)))
                {
                    this.PlayerInfieldList.Add(new InFieldData { PlayerTag = collision.tag, Dis = dis_per });
                }
                else
                {
                    this.PlayerInfieldList.Where(x => x.PlayerTag == collision.tag).First().Dis = dis_per;
                }
                //Debug.Log(collision.tag + " In!! dis:" + dis_per);
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
                GameManager.gameManager.TriggerBeaconIn(new Model_BeaconTrigger(collision.tag, ScoreAdd/((this.PlayerInfieldList.Count==0)?1: this.PlayerInfieldList.Count), dis_per));
                GameManager.gameManager.TriggerSignalIsJam(this.PlayerInfieldList.Count > 1);
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
                //Debug.Log(collision.tag + " Exit!!");
                this.PlayerInfieldList.RemoveAll(x => x.PlayerTag.Equals(collision.tag));
                GameManager.gameManager.TriggerSignalIsJam(this.PlayerInfieldList.Count > 1);
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
        this.thiscol.enabled = true;
        //this.thiscol.radius = Random.Range(2,6);
        StartCoroutine(ScoreAddIEnum());
    }

    IEnumerator ScoreAddIEnum()
    {
        yield return new WaitForSeconds(1f);
        this.LifeTimeCounter++;
        if (this.LifeTimeCounter >= this.LifeTime)
        {
            this.IsActive = false;
            this.thiscol.enabled = false;
            this.spriteRenderer.color = new Color(this.spriteRenderer.color.r, this.spriteRenderer.color.g, this.spriteRenderer.color.b,0);
            this.PlayerInfieldList = new List<InFieldData>();
            GameManager.gameManager.TriggerSignalIsJam(false);
            GameManager.gameManager.TriggerBeaconReposition();
            //SetBeacon(new Vector2(Random.Range(-9, 9), Random.Range(-5, 5)), 5);
        }
        else
        {
            StartCoroutine(ScoreAddIEnum());
        }

    }

    public class InFieldData
    {
        public string PlayerTag;
        public float Dis;
    }

}



