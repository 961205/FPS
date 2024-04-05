using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //���ǵ� ���� ����
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;

    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    //���� ����
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    //������ üũ ����
    private Vector3 lastPos;

    //�ɾ��� �� �󸶳� ������ �����ϴ� ����
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    //�� ���� ����
    private CapsuleCollider capsuleCollider;

    //�ΰ���
    [SerializeField]
    private float lookSensitivity;

    //ī�޶� �Ѱ�
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    //�ʿ��� ������Ʈ
    [SerializeField]
    private Camera theCamera;
    private Rigidbody rigid;
    private GunController gunController;
    private Crosshair crosshair;
    private StatusController statusController;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        rigid = GetComponent<Rigidbody>();
        gunController = FindObjectOfType<GunController>();
        crosshair = FindObjectOfType<Crosshair>();
        statusController = FindObjectOfType<StatusController>();

        //�ʱ�ȭ
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        float CheckMoveXZ = Move(); //CheckMoveXZ �� ���� ��ȯ���� �Ŀ�
        MoveCheck(CheckMoveXZ); // MoveCheck�� �Ű������� �־���

        if (!Inventory.inventoryActivated)
        {
            CameraRotation();
            CharacterRotation();
        }
    }

    //�ɱ� �õ�
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    //�ɱ� ����
    private void Crouch()
    {
        isCrouch = !isCrouch;
        crosshair.CrouchingAnimation(isCrouch);

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

    //�ε巯�� ���� ����
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while (_posY != applyCrouchPosY)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15)
                break;
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }

    //���� üũ
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        crosshair.JumpingAnimation(!isGround);
    }

    //���� �õ�
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && statusController.GetCurrentSP() > 0)
        {
            Jump();
        }
    }

    //����
    private void Jump()
    {

        //���� ���¿��� ���� �� ���� ���� ����
        if (isCrouch)
            Crouch();
        statusController.DecreaseSP(10);
        rigid.velocity = transform.up * jumpForce;
    }

    //�޸��� �õ�
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && statusController.GetCurrentSP() > 0)
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || statusController.GetCurrentSP() <= 0)
        {
            RunningCancel();
        }
    }

    //�޸��� ����
    private void Running()
    {
        if (isCrouch)
            Crouch();

        gunController.CancelFineSight();

        isRun = true;
        crosshair.RunningAnimation(isRun);
        statusController.DecreaseSP(1);
        applySpeed = runSpeed;
    }

    //�޸��� ���
    private void RunningCancel()
    {
        isRun = false;
        crosshair.RunningAnimation(isRun);
        applySpeed = walkSpeed;
    }

    //������ ����
    private float Move() //float���� ��ȯ���� ����
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");
        float moveXZAbsSum = Mathf.Abs(_moveDirX) + Mathf.Abs(_moveDirZ); //�������� �ִٸ� �� ���� moveXZAbsSum�� ����, �������� �������� moveXZAbsSum�� 0�� �Ǵ� ���� ���� ���� ������ �̿��� ����

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        rigid.MovePosition(transform.position + _velocity * Time.deltaTime);

        return moveXZAbsSum; //moveXZAbsSum�� ��ȯ
    }

    //������ üũ
    private void MoveCheck(float MoveXZ) //�Ű������� �ް� ��
    {
        if (!isRun && !isCrouch && isGround)
        {
            //if(Vector3.Distance(lastPos,transform.position) >= 0.01f)
            //Debug.Log(MoveXZ);
            if (MoveXZ != 0) //üũ, �������� �ִٸ� ����� 0�� �� �� �����Ƿ� isWalk�� ����Ǿ� ũ�ν��� �ٲ�
            {
                isWalk = true;
            }
            else
                isWalk = false;

            crosshair.WalkingAnimation(isWalk);
            lastPos = transform.position;
        }
    }

    //�¿� ĳ���� ȸ��
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    //���� ī�޶� ȸ��
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}
