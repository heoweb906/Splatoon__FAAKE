using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameObject humanPlayer;
    public GameObject octoPlayer;
    public GameObject gun_obj;
    public GameObject curlingStone;

    public ParticleSystem mainparticle;
    public ParticleSystem bulletparticle; // 총알 파티클 시스템 컴포넌트
    public ParticleSystem effectparticle; // 총열 파티클 시스템 컴포넌트
    public ParticleSystem colliderparticle;

    private Rigidbody rigid;
    public Camera mainCamera;
    public Transform curlingPosition;

    public float speed;  // 플레이어 스피드
    public float hAxis; // 이동 시 수평 값을 위한 변수
    public float vAxis; // 이동 시 수직 값을 위한 변수
    public float jumpForce; // 점프 힘
    public float turnSpeed; // 회전 속도
    public float forceMagnitude = 20f; // 컬링 던지는 힘의 크기

    public bool jDown; // 점프 키
    private bool change; // 오징어 변신
    private bool fireOn; // 공격 시작
    private bool fireOff; // 공격 종료
    private bool fireCurling; // 컬링 공격

    private bool isJumping; // 점프 중인지 여부를 나타내는 변수
    private bool isFiring; // 공격을 하고 있는지

    private Vector3 moveVec; // 플레이어의 이동 값
    private Vector3 playerLookDirc; // 플레이어 회전 할 때 사용할 값(1)
    private Quaternion playerRotateVec; // 플레이어 회전 할 때 사용할 값(2)
    private Vector3 gunLookDirc; // 총 회전 할 때 사용할 값(1)
    private Quaternion gunRotateVec; // 총 회전 할 때 사용할 값(2)
    


    private void Awake()
    {
        CamLock();
        rigid = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        GetInput();
        Move();
        PlayerTurn();
        ChangeOcto();
        Attack();
        CurlingBomb();
        CamLock();
    }


    private void CamLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void GetInput()  // 입력을 받는 함수
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        jDown = Input.GetButtonDown("Jump");
        change = Input.GetButton("Change");
        fireOn = Input.GetButtonDown("Fire1");
        fireOff = Input.GetButtonUp("Fire1");
        fireCurling = Input.GetButtonDown("Fire2");
    }

    private void Move() // 이동을 관리하는 함수
    {
        // 입력에 따른 이동 방향 계산
        Vector3 moveDirection = new Vector3(hAxis, 0f, vAxis).normalized;

        // 현재 카메라의 전방 벡터를 가져와서 이동 방향을 변환
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        moveVec = cameraForward * moveDirection.z + cameraRight * moveDirection.x;

        transform.position += moveVec * speed * Time.deltaTime;


        if (jDown && !isJumping)
        {
            Jump();
        }
    }



    private void PlayerTurn()  // 플레이어 회전(총 회전도 같이 관리함)
    {
        playerLookDirc = new Vector3(hAxis, 0, vAxis);
        playerLookDirc = mainCamera.transform.TransformDirection(playerLookDirc);
        playerLookDirc = playerLookDirc.normalized;
        playerLookDirc.y = 0;

        if (!isFiring)
        {
            if (moveVec != Vector3.zero)
            {
                playerRotateVec = Quaternion.LookRotation(playerLookDirc);
                transform.rotation = Quaternion.Slerp(transform.rotation, playerRotateVec, Time.deltaTime * turnSpeed);
            }
        }
        else if (isFiring)
        {
            playerRotateVec = mainCamera.transform.localRotation;
            playerRotateVec.x = 0;
            playerRotateVec.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, playerRotateVec, Time.deltaTime * turnSpeed);

            gunLookDirc = Camera.main.transform.forward;
            Quaternion gunRotateVec = Quaternion.LookRotation(gunLookDirc);
            gun_obj.transform.rotation = gunRotateVec;
        }
    }
    private void GunTurn()
    {
        //gunLookDirc = Camera.main.transform.forward;

        //Quaternion gunRotateVec = Quaternion.LookRotation(gunLookDirc);
        //Quaternion newRotation = gun_obj.transform.rotation;

        //newRotation.x = gunRotateVec.x; // 원하는 x 축 값으로 변경해주세요
        //newRotation.y = transform.rotation.y;
        //newRotation.z = transform.rotation.z;
    }



    private void Jump() // 점프를 처리하는 함수
    {
        isJumping = true;
        rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ChangeOcto()
    {
        humanPlayer.SetActive(change ? false : true);
        octoPlayer.SetActive(change ? true : false);
    }


    private void Attack()
    {
        if (fireOn) // 이런식으로 한번만 활성화를 안하고 파티클.Play()를 계속하니까 파티클이 겹쳐서 너무 많이 생성됨
        {
            FireBullet();
        }
        if (fireOff)
        {
            StopBullet();
        }
    }

    private void FireBullet()
    {
        isFiring = true;
        mainparticle.Play();
        bulletparticle.Play();
        effectparticle.Play();
        colliderparticle.Play();
    }
    private void StopBullet()
    {
        isFiring = false;
        mainparticle.Stop();
        bulletparticle.Stop();
        effectparticle.Stop();
        colliderparticle.Stop();
    }


    private void CurlingBomb()
    {
        if(fireCurling)
        {
            GameObject newObject = Instantiate(curlingStone, curlingPosition.position, curlingPosition.rotation);
            Vector3 forceDirection = transform.forward; 

            Rigidbody rb = newObject.GetComponent<Rigidbody>();
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        }
    }








    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false;
        }
    }

















}

