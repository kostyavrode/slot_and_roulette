using Mkey;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyBonusManager : MonoBehaviour
{
    public LobbyMenuController menuController;
    private void Awake()
    {
        int day = DateTime.Now.Day;
        if (PlayerPrefs.GetInt("DailyDay")==day)
        {
            this.gameObject.SetActive(false);
        }
    }
    public void ChangeBalance()
    {
        //Balance += value;
        
        string key = "mk_slot_coins";
        //int t = (int)value;
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key)+100);
        PlayerPrefs.SetInt("DailyDay", DateTime.Now.Day);
        PlayerPrefs.Save();
        menuController.RefreshBalance();
    }
}
