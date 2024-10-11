using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotsManager : MonoBehaviour
{
    public int maxAvailableBots = 1;
    private int _availableBots = 0;
    public int availableBots 
    { 
        get 
        { return _availableBots; }
        set 
        {
            if (value >= maxAvailableBots)
                _availableBots = maxAvailableBots - 1;
            else
                _availableBots = value;

            foreach (var bot in bots)
            {
                if (bot == null)
                {
                    bots.Remove(bot);
                }
            }
        }
    }
    public List<GameObject> bots;

    public GameObject GetPreviosBot()
    {
        if(bots.Count >= 2)
            return bots[bots.Count - 2];
        return null;
    }
    public void AddBot(GameObject bot)
    {
        bots.Add(bot);
    }
    public void DeleteBot(GameObject bot)
    {
        bots.Remove(bot);
    }
    public void Start()
    {
        _availableBots = maxAvailableBots - 1;
        if (bots.Count > 1)
            bots[0].GetComponent<PlayerInput>().SetAvailableBots();
    }
}
