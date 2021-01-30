using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// �]�w�����ܼ�(�Ω��L����ϥ�)
    /// </summary>
    public static GameManager gameManager;

    private void Awake()
    {
        //�ŧi(���n)
        gameManager = this;
    }

    /// <summary>
    /// �q�\�ƥ�:���a�i�J�T���d��
    /// </summary>
    public event Action<Model_BeaconTrigger> OnTriggerBeaconIn;
    /// <summary>
    /// �q�\�ƥ�Ĳ�o:���a�i�J�T���d��
    /// </summary>
    /// <param name="PlayerTag">���a����</param>
    /// <param name="AddScore">���W�[����</param>
    /// <param name="DisPer">�T���P���a�Z��(�ʤ���)</param>
    public void TriggerBeaconIn(Model_BeaconTrigger InputModel)
    {
        if (OnTriggerBeaconIn != null)
        {
            OnTriggerBeaconIn(InputModel);
        }
    }

    /// <summary>
    /// �q�\�ƥ�:���a���}�T���d��
    /// </summary>
    public event Action<string> OnTriggerBeaconExit;
    /// <summary>
    /// �q�\�ƥ�Ĳ�o:���a���}�T���d��
    /// </summary>
    /// <param name="PlayerTag">���a����</param>
    public void TriggerBeaconExit(string PlayerTag)
    {
        if (OnTriggerBeaconExit != null)
        {
            OnTriggerBeaconExit(PlayerTag);
        }
    }

    public event Action<string> OnTriggerPlayerHit;
    public void TriggerPlayerHit(string PlayerTag)
    {
        if (OnTriggerPlayerHit != null)
        {
            OnTriggerPlayerHit(PlayerTag);
        }
    }
}
