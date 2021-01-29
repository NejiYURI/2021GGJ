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
    public event Action<string,int> OnTriggerBeaconIn;
    /// <summary>
    /// �q�\�ƥ�Ĳ�o:���a�i�J�T���d��
    /// </summary>
    /// <param name="PlayerTag">���a����</param>
    /// <param name="AddScore">���W�[����</param>
    public void TriggerBeaconIn(string PlayerTag,int AddScore)
    {
        if (OnTriggerBeaconIn != null)
        {
            OnTriggerBeaconIn(PlayerTag, AddScore);
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
}
