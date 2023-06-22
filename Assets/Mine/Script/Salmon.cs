using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Salmon : MonoBehaviour
{
    public enum Type { S, M ,B };

    public int hp;
    public int power;
    //public float speed;

    public Transform target;

    public bool isChase; // �÷��̾ �i�� �ִ� �������� Ȯ���ϴ� �ڵ�


    Rigidbody rigid;
    NavMeshAgent nav;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        if (target != null)
        {
            StartChasing();
        }
    }
    void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position); // SetDestination() : ������ ��ġ ���� �Լ�
            nav.isStopped = !isChase;
        }
    }
    

    private void FixedUpdate()
    {
        FreezeVelocity();
    }





    private void StartChasing()
    {
        isChase = true;
        nav.enabled = true;
    }
    private void StopChasing()
    {
        isChase = false;
        nav.enabled = false;
    }


    void FreezeVelocity()  // �������� NavAgent �̵��� �������� �ʵ��� �����߰�
    {
        if (isChase)
        {
            rigid.angularVelocity = Vector3.zero;   // angularVelocity : ȸ�� ���� �ӵ�
            rigid.velocity = Vector3.zero;
        }
    }
}
