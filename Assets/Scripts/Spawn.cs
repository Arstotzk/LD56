using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject playerBot;
    public void SpawnBot(GameObject previosBot, bool isResapwn)
    {
        var newBot = GameObject.Instantiate(playerBot);
        newBot.SetActive(true);
        newBot.GetComponent<PlayerInput>().enabled = true;
        var newPosition = playerBot.transform.position;
        newPosition.z = 0;
        newBot.transform.position = newPosition;
        newBot.GetComponent<PlayerInput>().DialogUnshow();
        if (isResapwn)
        {
            newBot.GetComponent<PlayerInput>().previousBot = null;
            newBot.GetComponent<PlayerInput>().maxAvailableBots = previosBot.GetComponent<PlayerInput>().maxAvailableBots;
            newBot.GetComponent<PlayerInput>().availableBots = previosBot.GetComponent<PlayerInput>().availableBots;
        }
        else
        {
            newBot.GetComponent<PlayerInput>().previousBot = previosBot;
            newBot.GetComponent<PlayerInput>().maxAvailableBots = previosBot.GetComponent<PlayerInput>().maxAvailableBots;
            newBot.GetComponent<PlayerInput>().availableBots = previosBot.GetComponent<PlayerInput>().availableBots - 1;
        }
        newBot.GetComponent<PlayerInput>().SetAvailableBots();
        int LayerIgnoreRaycast = LayerMask.NameToLayer("Grabbing");
        previosBot.layer = LayerIgnoreRaycast;
        previosBot.GetComponent<PlayerInput>().groundChecker.SetActive(false);

    }
}
