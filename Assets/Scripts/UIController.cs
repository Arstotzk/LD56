using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject dialogUI;
    public TMP_Text textAvailableBots;
    public TMP_Text textMaxAvailableBots;

    public void SetAvailableBots(int value) 
    {
        textAvailableBots.text = "Bots available: " + value;
    }
    public void SetMaxAvailableBots(int value)
    {
        textMaxAvailableBots.text = "Bots max available: " + value;
    }
}
