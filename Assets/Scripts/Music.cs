using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource audioSource;

    public void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
