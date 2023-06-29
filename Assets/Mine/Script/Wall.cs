using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public float climb = 2f;
    public void Commence(float amount)
    {
        climb -= amount;
        if (climb <= 0f)
        {
            Go();
        }
    }
    void Go()
    {
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        PlayerBehavior playerBehavior = collision.gameObject.GetComponent<PlayerBehavior>();
        if (playerBehavior != null)
        {
            playerBehavior.swimwall = true; // swim 값을 변경합니다.
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        PlayerBehavior playerBehavior = collision.gameObject.GetComponent<PlayerBehavior>();
        if (playerBehavior != null)
        {
            playerBehavior.swimwall = false; // swim 값을 변경합니다.
        }
    }
}
