using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curling : MonoBehaviour
{
    Rigidbody rigid;
    public GameObject destoryParticle;

  
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        Destroy(gameObject, 3f);
    }


    private void OnCollisionEnter(Collision collision)  // 벽이랑 닿았을 때 튕기는 코드
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


    private void OnDisable()
    {
        // 현재 오브젝트의 위치에 새로운 오브젝트 생성
        Instantiate(destoryParticle, transform.position, transform.rotation);
    }
}
