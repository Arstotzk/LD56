using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollider : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var player = collision.GetComponent<PlayerInput>();
            if(player != null)
                player.Destroy();
        }
    }
}
