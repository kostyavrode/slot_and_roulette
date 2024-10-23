using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BetController : MonoBehaviour
{
    private int currentBid=1;
    public TMP_Text currentBidText;
    public TMP_Text balance;
    public GameManager gameManager;
    private void Start()
    {
        ShowMoney();
        currentBidText.text = currentBid.ToString();
    }
    public void IncreaseBet()
    {
        if (currentBid >= 1 && currentBid<=1000)
        {
            currentBid *=2;
            gameManager.betSize = currentBid;
            currentBidText.text=currentBid.ToString();
        }
    }
    public void DecreaseBet()
    {
        if (currentBid >= 2 && currentBid <= 1000)
        {
            currentBid /=2;
            gameManager.betSize = currentBid;
            currentBidText.text = currentBid.ToString();
        }
    }
    public void ShowMoney()
    {
        balance.text = PlayerPrefs.GetInt("mk_slot_coins").ToString();
    }
}
