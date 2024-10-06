using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider soundSlider;
    public Slider sensativitySlider;
    public GameObject UI;

    public void ExitGame() 
    {
        Application.Quit();
    }
    public void ChangeSound()
    {
        var value = soundSlider.value;
        if (value == 0)
            value = 0.0001f;
        audioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
    }
    public void ChangeSensitivity()
    {
        var value = sensativitySlider.value;
    }
}
