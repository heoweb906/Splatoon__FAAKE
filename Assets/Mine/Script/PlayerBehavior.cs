using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameObject humanPlayer;
    public GameObject octoPlayer;
    public GameObject gun_obj;
    public GameObject curlingStone;
    public GameObject shark;
    public GameObject sharkBomb;

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

    private bool jDown; // ���� Ű
    public bool change; // ��¡�� ����
    private bool fireOn; // ���� ����
    private bool fireOff; // ���� ����
    private bool fireCurling; // �ø� ����
    private bool fireshark; // ��ũ ���̺�
    public bool firesharkStop; // ��ũ ����
    public bool canChange;
    
    private bool isJumping; // ���� ������ ���θ� ��Ÿ���� ����
    private bool isFiring; // ������ �ϰ� �ִ���
    private bool isSharkWave; // �ñر⸦ ����ϰ� �ִ���
    private bool isSharkmove; // �ñر� ��� �� �������� ����ϴ� ����

    private Vector3 moveVec; // �÷��̾��� �̵� ��
    private Vector3 playerLookDirc; // �÷��̾� ȸ�� �� �� ����� ��(1)
    private Quaternion playerRotateVec; // �÷��̾� ȸ�� �� �� ����� ��(2)
    private Vector3 gunLookDirc; // �� ȸ�� �� �� ����� ��(1)
    private Quaternion gunRotateVec; // �� ȸ�� �� �� ����� ��(2)


    // ���̾� ������ ���� �ʿ��� ������
    public GameObject targetObject;  // ���̾ ������ ��� ������Ʈ
    public string octopusLayer = "Octopus";  // ����Ѹ����̾� �̸�
    public string playerLayer = "Player";  // �÷��̾� ���̾� �̸�
    public Color teamColor;
    public bool ourColor;


    private void Awake()
    {
        CamLock();
        rigid = GetComponent<Rigidbody>();
    }

    private void CamLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        GetInput();
        Move();
        PlayerTurn();
        Attack();
        CurlingBomb();
        CamLock();
        MoveForward();
        CheckColor();


        ChangeOcto();
        if (Input.GetButtonDown("Change"))
        {
            ChangeLayertoOctopus(targetObject, octopusLayer);
        }
        if (Input.GetButtonUp("Change"))
        {
            ChangeLayertoPlayer(targetObject, playerLayer);
        }
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
        fireshark = Input.GetButtonDown("SharkWave");
        firesharkStop = Input.GetButtonDown("SharkWaveStop");   
    }


    private void Move() // �̵��� �����ϴ� �Լ�
    {
        if(!isSharkWave && !canChange)
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
        
        if(fireshark && isSharkWave == false)
        {
            StartCoroutine(SharkWave1());
        }
        if (firesharkStop && isSharkWave)
        {
            isSharkmove = false;
            isSharkWave = false;
            Invoke("SharkWave2", 0.4f);
        }
    }


    IEnumerator SharkWave1()
    {
        canChange = true;
        isSharkWave = true;
        humanPlayer.SetActive(false);
        shark.SetActive(true);
        yield return new WaitForSeconds(0.3f);

        isSharkmove = true;

       
        yield return new WaitForSeconds(1.2f);

        isSharkmove = false;

        yield return new WaitForSeconds(0.4f);
        if(isSharkWave)
        {
            SharkWave2();
        }
        
    }
    private void MoveForward()
    {
        if (isSharkmove)
        {
            Vector3 newPosition = transform.position + transform.forward * 20f * Time.deltaTime;
            transform.position = newPosition;
        }
    }
    private void SharkWave2()
    {
        isSharkWave = false;
        canChange = false;
        humanPlayer.SetActive(true);
        shark.SetActive(false);
        Instantiate(sharkBomb, transform.position, transform.rotation);
    }




    private void PlayerTurn()  // �÷��̾� ȸ��(�� ȸ���� ���� ������)
    {
        if(!isSharkWave && !canChange)
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
        
    }



    private void Jump() // ������ ó���ϴ� �Լ�
    {
        isJumping = true;
        rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ChangeOcto()
    {
        if(!canChange)
        {
            humanPlayer.SetActive(change ? false : true);
            octoPlayer.SetActive(change ? true : false);
        }
     
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


    void ChangeLayertoOctopus(GameObject targetObject, string octopusLayer)
    {
        int newLayer = LayerMask.NameToLayer(octopusLayer);

        if (newLayer != -1)
        {
            targetObject.layer = newLayer;
        }
        else
        { }
    }
    void ChangeLayertoPlayer(GameObject targetObject, string playerLayer)
    {
        int newLayer = LayerMask.NameToLayer(playerLayer);

        if (newLayer != -1)
        {
            targetObject.layer = newLayer;
        }
        else
        { }
    }


    void CheckColor()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit,1.2f))
        {
            Paintable paintable = hit.collider.GetComponent<Paintable>();
            
            if (paintable != null)
            {
                RenderTexture maskTexture = paintable.getMask();
                Texture2D maskTexture2D = new Texture2D(maskTexture.width, maskTexture.height, TextureFormat.RGBA32, false);
                RenderTexture.active = maskTexture;
                maskTexture2D.ReadPixels(new Rect(0, 0, maskTexture.width, maskTexture.height), 0, 0);
                maskTexture2D.Apply();

                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= maskTexture.width;
                pixelUV.y *= maskTexture.height;

                Color color = maskTexture2D.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                teamColor = color;
                Debug.Log("Hit color: " + color); // ���� �� �Ʒ� �ִ� ���� ���
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * 1.2f, Color.red);
    }
   





    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false;
        }
    }

}

