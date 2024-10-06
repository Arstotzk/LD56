using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheker : MonoBehaviour
{

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            transform.parent.GetComponent<PlayerInput>().SetIsOnGround(true);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            transform.parent.GetComponent<PlayerInput>().SetIsOnGround(false);
        }
    }
}
