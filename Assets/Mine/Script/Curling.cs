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


    private void OnCollisionEnter(Collision collision)  // ���̶� ����� �� ƨ��� �ڵ�
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            if (collision.contacts.Length > 0)
            {
                Vector3 normal = collision.contacts[0].normal;
                // ƨ��� ���� ���
                Vector3 bounceDirection = Vector3.Reflect(transform.forward, normal);
                float bounceForce = Mathf.Abs(rigid.velocity.x) + Mathf.Abs(rigid.velocity.z);
                GetComponent<Rigidbody>().AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            }
        }
    }


    private void OnDisable()
    {
        // ���� ������Ʈ�� ��ġ�� ���ο� ������Ʈ ����
        Instantiate(destoryParticle, transform.position, transform.rotation);
    }
}
