using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

    // 스피드 조정 변수
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float crouchSpeed;
    


    private float applySpeed;

    // 점프
    [SerializeField]
    private float jumpForce;    

    // 상태 변수 
    private bool isRun = false;
    private bool isGround = true;
    private bool isCrouch = false;

    // 앉았을 때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    // 땅 착지 여부
    private CapsuleCollider capsuleCollider;

    // 민감도
    [SerializeField]
    private float lookSensitivity;

    // 카메라 한계
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0;
  
    // 필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;

    private Rigidbody myRigid;
        //콜리더로 충돌 영역을 설정하고 , 리지드바디는 콜리더로 물리학을 입히는 것
    void Start() 
    {

        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        // 카메라를 내려야되서 상대적인 변수인 로컬트렌스폼 사용
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;

}

    void Update()
    {
        // 함수
        TryCrouch();
        IsGround();
        TryJump();
        TryRun();
        Move();
        CameraRotation();
        CharacterRotation();

    }

    // 키보드의 입력은 마우스와 같이 Down, Hold, Up  이 세 가지 과정으로 나누어져서 처리된다. Down은 키보드를 누르는 순간을 의미하고, Hold는 누른 상태를 유지하는 것, Up은 키보드에서 손을 떼는 것을 의미함

    //앉기 시도
    private void TryCrouch()
    {
        // 왼쪽 컨트롤을 눌렀을 때 조건 충족
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }

    }
    // 점프 시도
    private void TryJump()
    {
        // 왼쪽 스페이스를 눌렀을 떄 조건 충족
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true)
        {
            jump();
        }
    }
    // 실제로 앉는 함수
    private void Crouch()
    {
        // if (isCrouch)
        //  is Crouch = false;
        // else
        //  is Crouch = true;
        isCrouch = !isCrouch;

        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }
        StartCoroutine(CrouchCoroutine());
    }

    // 코로틴 = 병렬처리
    // 부드러운 앉기 동작
    IEnumerator CrouchCoroutine()
    {
         
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;
        while(_posY != applyCrouchPosY)
        {
            count++;
            // 보간 이용
            // 보간의 단점 : 정확히 0 과 1에 딱 떨어지지 않고 계속 반복해서 실행하기 때문에 약간의 오차가 있음
            // 단점을 보안하려면 보간의 범위를 지정해주는 조건문을 써줘야됨
            // if (count > 15)
            // break;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.1f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            // 1프레임 마다 실행
            if (count > 15)
                break;
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }
    // 실제로 점프하는 함수
    private void jump()
    {
        //앉은 상태에서 점프시 앉은 상태 해체
        if(isCrouch)
            Crouch();
        // valocity = 현제 움직이고 있는 속도
        myRigid.velocity = transform.up * jumpForce;
    }
    // 지면 체크
    private void IsGround()
    {
        // 대각선 경사면 이런곳에 있으면 약간의 오차가 발생 할 수 있어서 약간의 여유를 주기위해 + 0.1f
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);

    }
    // 달리기 시도
    private void TryRun() 
    {
        
        // 왼쪽 쉬프트를 누르고 있으면 조건 충족  
        if (Input.GetKey(KeyCode.LeftShift))
        {
        // 쉬프트를 누르면 뛰는 상태
            Running();
        }
        // 왼쪽 쉬프트를 때면 조건 충족
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
        // 쉬프트를 때면 달리기 캔슬
            RunningCancel();
        }
    }
    // 실제로 달리는 함수
    private void Running()
    {

        if (isCrouch)
            Crouch();
        // 뛰는 상태니까 트루
        isRun = true;
        // 어플라이 스피드를 런 스피드에 대입
        applySpeed = runSpeed;

    }
    // 실제로 달리다가 취소되는 함수
    private void RunningCancel()
    {
        
        // 뛰다가 캔슬(멈춤) 됐으니까 펄스
        isRun = false;
        // 다시 걷는 속도로 되야되니까 다시 대입
        applySpeed = walkSpeed;


    }
    // 움직임 실행
    private void Move()
    {

        float _moveDirX = Input.GetAxisRaw("Horizontal");
        //w , s 나 키보드 앞뒤 방향키를 누르면 1과 -1 이 리턴되면서 _MoveDirZ에 들어가게 됨
        float _moveDirZ = Input.GetAxisRaw("Vertical");
        //w , s 나 키보드 앞뒤 방향키를 누르면 1과 -1 이 리턴되면서 _MoveDirZ에 들어가게 됨.

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        //오른쪽 방향키를 누르면(1, 0, 0) * 1 왼쪽 방향키를 누르면 (-1, 0, 0) * -1
        Vector3 _moveVertical = transform.forward * _moveDirZ;
        //앞 방향키를 누르면(0, 0, 1) * 1 뒤 방향키를 누르면 (0, 0, -1) * -1

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;
        //속도
        //normalized :
        // (1 , 0 , 0) (0 , 0 , 1) =
        // (1 , 0 , 1) = 2  -> normalized =
        // (0.5 , 0 , 0.5) = 1

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
       // 순간이동 하듯이 움직이면 안되니까 Time.deltaTime를 이용
    }

     //상하 캐릭터 회전
    private void CameraRotation()
    {
     
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        //마우스가 확 올라가면 안되니까 lookSensitivity라는 변수로 조정
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit , cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);

    }

    //좌우 카메라 회전
    private void CharacterRotation()
    {
       
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));

    }
}
//Vector은 3원수 X Y Z