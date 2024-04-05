using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(GunController))]
public class WeaponManager : MonoBehaviour
{
    //무기 중복 교체 실행 방지
    public static bool isChangeWeapon = false;

    //현재 무기와 현재 무기의 애니메이션
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

    //현재 무기의 타입
    [SerializeField]
    private string currentWeaponType;

    //무기 교체 딜레이, 무기 교체가 완전히 끝난 시점
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    //무기 종류들 전부 관리
    [SerializeField]
    private Gun[] guns;
    [SerializeField]
    private CloseWeapon[] hands;
    [SerializeField]
    private CloseWeapon[] axes; 
    [SerializeField]
    private CloseWeapon[] pickAxes;


    //관리 차원에서 쉽게 무기 접근이 가능하도록 만듦
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> pickAxeDictionary = new Dictionary<string, CloseWeapon>();

    //필요한 컴포넌트
    [SerializeField]
    private HandController handController;
    [SerializeField]
    private GunController gunController;
    [SerializeField]
    private AxeController axeController;
    [SerializeField]
    private PickAxeController pickAxeController;

    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }
        for (int i = 0; i < hands.Length; i++)
        {
            handDictionary.Add(hands[i].closeWeaponName, hands[i]);
        }
        for (int i = 0; i < axes.Length; i++)
        {
            axeDictionary.Add(axes[i].closeWeaponName, axes[i]);
        }
        for (int i = 0; i < pickAxes.Length; i++)
        {
            pickAxeDictionary.Add(pickAxes[i].closeWeaponName, pickAxes[i]);
        }

    }

    void Update()
    {
        if(!isChangeWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartCoroutine(ChangeWeaponCoroutine("HAND", "Hand"));
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachineGun1"));
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                StartCoroutine(ChangeWeaponCoroutine("AXE", "Axe"));
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                StartCoroutine(ChangeWeaponCoroutine("PICKAXE", "PickAxe"));

        }
    }

    //무기 교체 코루틴
    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction();
        WeaponChange(_type, _name);

        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        currentWeaponType = _type;
        isChangeWeapon = false;
    }

    //무기 취소 함수
    private void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                gunController.CancelFineSight();
                gunController.CancelReload();
                GunController.isActivate = false;
                break;
            case "HAND":
                HandController.isActivate = false;
                break;
            case "AXE":
                AxeController.isActivate = false;
                break;
            case "PICKAXE":
                PickAxeController.isActivate = false;
                break;

        }
    }

    //무기 교체 함수
    private void WeaponChange(string _type, string _name)
    {
        if (_type == "GUN")
            gunController.GunChange(gunDictionary[_name]);
        else if (_type == "HAND")
            handController.CloseWeaponChange(handDictionary[_name]);
        else if (_type == "AXE")
            axeController.CloseWeaponChange(axeDictionary[_name]);
        else if (_type == "PICKAXE")
            pickAxeController.CloseWeaponChange(pickAxeDictionary[_name]);
    }
}
