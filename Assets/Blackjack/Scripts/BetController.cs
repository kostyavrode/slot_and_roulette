using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BetController : MonoBehaviour
{
    private int currentBid=1;
    public TMP_Text currentBidText;
    public TMP_Text balance;
    public GameManager gameManager;
    public Button increaseButton;
    public Button decreaseButton;
    public Button dealButton;
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
            if (PlayerPrefs.GetInt("mk_slot_coins") >= currentBid)
            {
                dealButton.interactable = true;
            }
            else
            {
                dealButton.interactable = false;
            }
        }
    }
    public void DecreaseBet()
    {
        if (currentBid >= 2 && currentBid <= 2000)
        {
            currentBid /=2;
            gameManager.betSize = currentBid;
            currentBidText.text = currentBid.ToString();
            if (PlayerPrefs.GetInt("mk_slot_coins") >= currentBid)
            {
                dealButton.interactable = true;
            }
            else
            {
                dealButton.interactable = false;
            }
        }
    }
    public void SetButtonsState(bool t)
    {
        increaseButton.interactable=t;
        decreaseButton.interactable=t;
    }
    public void ShowMoney()
    {
        balance.text = PlayerPrefs.GetInt("mk_slot_coins").ToString();
    }
}
