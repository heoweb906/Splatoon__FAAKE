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

    public bool isChase; // 플레이어를 쫒고 있는 상태인지 확인하는 코드


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
            nav.SetDestination(target.position); // SetDestination() : 도착할 위치 지정 함수
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


    void FreezeVelocity()  // 물리력이 NavAgent 이동을 방해하지 않도록 로직추가
    {
        if (isChase)
        {
            rigid.angularVelocity = Vector3.zero;   // angularVelocity : 회전 물리 속도
            rigid.velocity = Vector3.zero;
        }
    }
}
