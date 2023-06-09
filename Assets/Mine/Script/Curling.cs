using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curling : MonoBehaviour
{
    Rigidbody rigid;

    public ParticleSystem colliderparticle;
    private Renderer[] objectRenderers;
  

    void Awake()
    {
        colliderparticle.Play();
        rigid = GetComponent<Rigidbody>();
        objectRenderers = GetComponentsInChildren<Renderer>();
    }




    //private void Bomb()
    //{
    //    colliderparticle.Stop();
    //    bombParticle1.Play();
    //    //HideObjects();
    //    Invoke("BombStop", 0.2f);
    //}
    //private void BombStop()
    //{
    //    bombParticle1.Stop();

    //    Destroy(gameObject,2f);
    //}




    // 렌더러 보이고 안보이고 관리하는 함수들
    private void SetObjectVisibility(bool isVisible)
    {
        foreach (Renderer renderer in objectRenderers)
        {
            renderer.enabled = isVisible;
        }
    }
    public void ShowObjects()
    {
        SetObjectVisibility(true);
    }
    public void HideObjects()
    {
        SetObjectVisibility(false);
    }



    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            if (collision.contacts.Length > 0)
            {
                Vector3 normal = collision.contacts[0].normal;
                // 튕기는 방향 계산
                Vector3 bounceDirection = Vector3.Reflect(transform.forward, normal);
                float bounceForce = Mathf.Abs(rigid.velocity.x) + Mathf.Abs(rigid.velocity.z);
                GetComponent<Rigidbody>().AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            }
        }
    }
}
