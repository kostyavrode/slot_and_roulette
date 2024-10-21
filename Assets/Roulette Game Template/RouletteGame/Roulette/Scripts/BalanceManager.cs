using System;
using UnityEngine;

public class BalanceManager : MonoBehaviour {

    public static int Balance { get; private set; } = 0;

    public static void SetBalance()
    {
        Balance = PlayerPrefs.GetInt("mk_slot_coins");
        Debug.Log("Balance: " + Balance);
        SceneRoulette.UpdateLocalPlayerText();
    }

    public static void ChangeBalance(int value)
    {
        Balance += value;
        SceneRoulette.UpdateLocalPlayerText();
        string key = "mk_slot_coins";
        int t=(int)value;
        PlayerPrefs.SetInt(key, t);
        PlayerPrefs.Save();
    }

    public void ResetBalance(int balance)
    {
        Balance = balance;
        SceneRoulette.UpdateLocalPlayerText();
    }
}
