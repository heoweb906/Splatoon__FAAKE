using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        PlayerBehavior playerBehavior = collision.gameObject.GetComponent<PlayerBehavior>();
        if (playerBehavior != null)
        {
            playerBehavior.swim = true; // swim 값을 변경합니다.
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        PlayerBehavior playerBehavior = collision.gameObject.GetComponent<PlayerBehavior>();
        if (playerBehavior != null)
        {
            playerBehavior.swim = false; // swim 값을 변경합니다.
        }
    }
}
