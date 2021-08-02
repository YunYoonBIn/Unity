using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

    [SerializeField]
    private float walkSpeed;
    // 걷는 속도
    
    [SerializeField]
    private float lookSensitivity;
    // 민감도
   
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0;
    //마우스 각도

    [SerializeField]
    private Camera theCamera;

    private Rigidbody myRigid;
        //콜리더로 충돌 영역을 설정하고 , 리지드바디는 콜리더로 물리학을 입히는 것
    void Start() {

 
        myRigid = GetComponent<Rigidbody>();
    
        
    }

    void Update()
    {

        Move();
        CameraRotation();
        CharacterRotation();

    }

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

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;
         //속도
        //normalized :
        // (1 , 0 , 0) (0 , 0 , 1) =
        // (1 , 0 , 1) = 2  -> normalized =
        // (0.5 , 0 , 0.5) = 1

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
       // 순간이동 하듯이 움직이면 안되니까 Time.deltaTime를 이용
    }

    private void CameraRotation()
    {
        //상하 캐릭터 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        //마우스가 확 올라가면 안되니까 lookSensitivity라는 변수로 조정
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit , cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);

    }

    private void CharacterRotation()
    {
        //좌우 카메라 회전
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));

    }
}
//Vector은 3원수 X Y Z