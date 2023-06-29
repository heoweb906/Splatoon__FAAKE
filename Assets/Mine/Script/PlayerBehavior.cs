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
    public ParticleSystem bulletparticle; // 총알 파티클 시스템 컴포넌트
    public ParticleSystem effectparticle; // 총열 파티클 시스템 컴포넌트
    public ParticleSystem colliderparticle;

    private Rigidbody rigid;
    public Camera mainCamera;
    public Transform curlingPosition;


    public int hp; // 플레이어 체력
    public float speed;  // 플레이어 스피드
    public float hAxis; // 이동 시 수평 값을 위한 변수
    public float vAxis; // 이동 시 수직 값을 위한 변수
    public float jumpForce; // 점프 힘
    public float turnSpeed; // 회전 속도
    public float forceMagnitude = 20f; // 컬링 던지는 힘의 크기

    private bool jDown; // 점프 키
    public bool change; // 오징어 변신
    private bool fireOn; // 공격 시작
    private bool fireOff; // 공격 종료
    private bool fireCurling; // 컬링 공격
    private bool fireshark; // 샤크 웨이브
    public bool firesharkStop; // 샤크 중지
    public bool canChange; // 샤크 함수 작동시 필요한 변수

    private bool isJumping; // 점프 중인지 여부를 나타내는 변수
    private bool isFiring; // 공격을 하고 있는지
    private bool isSharkWave; // 궁극기를 사용하고 있는지
    private bool isSharkmove; // 궁극기 사용 중 움직임을 담당하는 변수

    private Vector3 moveVec; // 플레이어의 이동 값
    private Vector3 playerLookDirc; // 플레이어 회전 할 때 사용할 값(1)
    private Quaternion playerRotateVec; // 플레이어 회전 할 때 사용할 값(2)
    private Vector3 gunLookDirc; // 총 회전 할 때 사용할 값(1)
    private Quaternion gunRotateVec; // 총 회전 할 때 사용할 값(2)


    // 레이어 변경을 위해 필요한 변수들
    public GameObject targetObject;  // 레이어를 변경할 대상 오브젝트
    public string octopusLayer = "Octopus";  // 옥토뿌리레이어 이름
    public string playerLayer = "Player";  // 플레이어 레이어 이름
    public Color nowColor;
    public Color nowColorWall;
    public Color teamColor;
    public Color enemyColor;
    public bool swim; // 바닥에서 헤엄치고 있는지 확인
    public bool swimwall; // 벽에서 헤엄치고 있는지 확인
    public bool ourColor; // 지금 바닥의 색이 아군 진영인지
    public bool otherColor; // 지금 바닥의 색이 적 진영인지
    public bool ourColorWall; // 지금 벽의 색이 아군 진영인지



    // 테스트 중 WallClimbScript(Script 2)
    public float open = 100f;
    public float range = 1f;
    public bool TouchingWall = false;
    public float UpwardSpeed;




    private void Awake()
    {
        CamLock(); 
        rigid = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        CamLock(); // 게임 시작 시 카메라 락
        InvokeRepeating("CheckColorFloor", 0f, 0.1f); // 색깔 확인 함수, 연산량이 너무 많아서 InvokeRepeating로 반복하려고 뺐음
        InvokeRepeating("CheckColorWall", 0.1f, 0.12f);
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
        MoveForward();
        ChangeOcto();
        Drop();  // 플레이어를 아래 방향을 당기는 함수, 문어 변신시 너무 느리게 잉크로 들어가서 추가했음
        ChangeSpeed(); // 플레이어의 현재 상태에 따라 스피드를 바꾸는 함수


        // 벽타기 코드
        Shoot();

        if (Input.GetKey("w") && TouchingWall == true)
        {
            if(ourColorWall)
            {
                transform.position += Vector3.up * Time.deltaTime * UpwardSpeed;
                GetComponent<Rigidbody>().isKinematic = true;
                TouchingWall = false;
                GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        if (Input.GetKeyUp("w"))
        {
            GetComponent<Rigidbody>().isKinematic = false;
            TouchingWall = false;
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward,out hit, range))
        {
            Wall wall = hit.transform.GetComponent<Wall>();
            if(wall != null)
            {
                TouchingWall = true;
            }
        }
    }





    private void GetInput()  // 입력을 받는 함수
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        jDown = Input.GetButtonDown("Jump");
        change = Input.GetButton("Change");
        fireOn = Input.GetButtonDown("Fire1");
        fireOff = Input.GetButtonUp("Fire1");
        fireshark = Input.GetButtonDown("SharkWave");
        firesharkStop = Input.GetButtonDown("SharkWaveStop");

        if(Input.GetButtonDown("Fire2"))
        {
            CurlingBomb();
        }
    }

    private void Move() // 이동을 관리하는 함수
    {
        if(!isSharkWave && !canChange)
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

    private void PlayerTurn()  // 플레이어 회전(총 회전도 같이 관리함)
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
    private void Drop()
    {
        rigid.AddForce(Vector3.down * 15f, ForceMode.Impulse);
    }

    private void Jump() // 점프를 처리하는 함수
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

        if (octoPlayer.activeSelf)
        {
            if (ourColor || ourColorWall)
            {
                ChangeLayertoOctopus(targetObject, octopusLayer);
            }
            else
            {
                ChangeLayertoPlayer(targetObject, playerLayer);
            }
        }
        else
        {
            ChangeLayertoPlayer(targetObject, playerLayer);
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
        
            GameObject newObject = Instantiate(curlingStone, curlingPosition.position, curlingPosition.rotation);
            Vector3 forceDirection = transform.forward; 

            Rigidbody rb = newObject.GetComponent<Rigidbody>();
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        
    }

    void CheckColorFloor()
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
                nowColor = color;
            }
        }

        if(0.89f <= nowColor.r && nowColor.r <= 0.98f && 0.70f <= nowColor.g && nowColor.g <= 0.78f && 0.30f <= nowColor.b&& nowColor.b <= 0.42f)
        {
            ourColor = true;
        }
        else
        {
            ourColor = false;
        }
        if (0.18f <= nowColor.r && nowColor.r <= 0.23f && 0.66f <= nowColor.g && nowColor.g <= 0.72f && 0.44f <= nowColor.b && nowColor.b <= 0.50f)
        {
            otherColor = true;
        }
        else
        {
            otherColor = false;
        }
    }

    void CheckColorWall()
    {
        Vector3 rayOrigin = transform.position - transform.forward * 0.2f;
        Ray ray2 = new Ray(rayOrigin, transform.forward);
        RaycastHit hit2;

        if (Physics.Raycast(ray2, out hit2, 3f))
        {
            Paintable paintable = hit2.collider.GetComponent<Paintable>();

            if (paintable != null)
            {
                RenderTexture maskTexture = paintable.getMask();
                Texture2D maskTexture2D = new Texture2D(maskTexture.width, maskTexture.height, TextureFormat.RGBA32, false);
                RenderTexture.active = maskTexture;
                maskTexture2D.ReadPixels(new Rect(0, 0, maskTexture.width, maskTexture.height), 0, 0);
                maskTexture2D.Apply();

                Vector2 pixelUV = hit2.textureCoord;
                pixelUV.x *= maskTexture.width;
                pixelUV.y *= maskTexture.height;

                Color color = maskTexture2D.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                nowColorWall = color;
            }
            else
            {
                nowColorWall = Color.black;
            }
        }
        else
        {
            nowColorWall = Color.black;
        }

        if (0.89f <= nowColorWall.r && nowColorWall.r <= 0.98f && 0.70f <= nowColorWall.g && nowColorWall.g <= 0.78f && 0.30f <= nowColorWall.b && nowColorWall.b <= 0.42f)
        {
            ourColorWall = true;
        }
        else
        {
            ourColorWall = false;
        }
    }

    private void ChangeSpeed()
    {
        speed = 8;
        jumpForce = 4000;

        if (!change)
        {
            if (otherColor)
            {
                speed = 4;
            }
        }
        else if (change)
        {
            speed = 4;
            if (swim)
            {
                jumpForce = 5000;
                speed = 12;
            }
            if (otherColor)
            {
                speed = 2;
            }
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

