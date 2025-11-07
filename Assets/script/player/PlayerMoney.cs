using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    private int playerMoney;

    private void Start()
    {
        playerMoney = 9999999;
    }
    public void AddMoney(int amount)
    {
        playerMoney += amount;
    }

    public void RemoveMoney(int amount)
    {
        playerMoney -= amount;
    }

    public int GetMoneyAmount()
    {
        return playerMoney;
    }

}