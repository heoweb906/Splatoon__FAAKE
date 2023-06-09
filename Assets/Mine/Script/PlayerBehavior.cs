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
    public ParticleSystem bulletparticle; // �Ѿ� ��ƼŬ �ý��� ������Ʈ
    public ParticleSystem effectparticle; // �ѿ� ��ƼŬ �ý��� ������Ʈ
    public ParticleSystem colliderparticle;

    private Rigidbody rigid;
    public Camera mainCamera;
    public Transform curlingPosition;

    public float speed;  // �÷��̾� ���ǵ�
    public float hAxis; // �̵� �� ���� ���� ���� ����
    public float vAxis; // �̵� �� ���� ���� ���� ����
    public float jumpForce; // ���� ��
    public float turnSpeed; // ȸ�� �ӵ�
    public float forceMagnitude = 20f; // �ø� ������ ���� ũ��

    public bool jDown; // ���� Ű
    private bool change; // ��¡�� ����
    private bool fireOn; // ���� ����
    private bool fireOff; // ���� ����
    private bool fireCurling; // �ø� ����

    private bool isJumping; // ���� ������ ���θ� ��Ÿ���� ����
    private bool isFiring; // ������ �ϰ� �ִ���

    private Vector3 moveVec; // �÷��̾��� �̵� ��
    private Vector3 playerLookDirc; // �÷��̾� ȸ�� �� �� ����� ��(1)
    private Quaternion playerRotateVec; // �÷��̾� ȸ�� �� �� ����� ��(2)
    private Vector3 gunLookDirc; // �� ȸ�� �� �� ����� ��(1)
    private Quaternion gunRotateVec; // �� ȸ�� �� �� ����� ��(2)
    


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

    private void GetInput()  // �Է��� �޴� �Լ�
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        jDown = Input.GetButtonDown("Jump");
        change = Input.GetButton("Change");
        fireOn = Input.GetButtonDown("Fire1");
        fireOff = Input.GetButtonUp("Fire1");
        fireCurling = Input.GetButtonDown("Fire2");
    }

    private void Move() // �̵��� �����ϴ� �Լ�
    {
        // �Է¿� ���� �̵� ���� ���
        Vector3 moveDirection = new Vector3(hAxis, 0f, vAxis).normalized;

        // ���� ī�޶��� ���� ���͸� �����ͼ� �̵� ������ ��ȯ
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



    private void PlayerTurn()  // �÷��̾� ȸ��(�� ȸ���� ���� ������)
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

        //newRotation.x = gunRotateVec.x; // ���ϴ� x �� ������ �������ּ���
        //newRotation.y = transform.rotation.y;
        //newRotation.z = transform.rotation.z;
    }



    private void Jump() // ������ ó���ϴ� �Լ�
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
        if (fireOn) // �̷������� �ѹ��� Ȱ��ȭ�� ���ϰ� ��ƼŬ.Play()�� ����ϴϱ� ��ƼŬ�� ���ļ� �ʹ� ���� ������
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
