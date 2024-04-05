using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class ItemEffect
{
    public string itemName; //아이템의 이름 (키값)
    [Tooltip("HP, DP, SP, HUNGRY, THIRSTY, SATISFY만 가능합니다.")]
    public string[] part; //부위
    public int[] num; //수치
}

public class ItemEffectDatabase : MonoBehaviour
{
    [SerializeField]
    private ItemEffect[] itemEffects;

    //필요한 컴포넌트
    [SerializeField]
    private StatusController playerStatus;
    [SerializeField]
    private WeaponManager weaponManager;
    [SerializeField]
    private SlotToolTip slotToolTip;

    private const string HP = "HP", DP = "DP", Sp = "SP", HUNGRY = "HUNGRY", THIRSTY = "THIRSTY", SATISFY = "SATISFY";

    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        slotToolTip.ShowTooltip(_item, _pos);
    }

    public void HideToolTip()
    {
        slotToolTip.HideToolTip();
    }

    public void UseItem(Item _item)
    {
        if (_item.itemType == Item.ItemType.Equipment)
        {
            StartCoroutine(weaponManager.ChangeWeaponCoroutine(_item.weaponType, _item.itemName));
        }

        else if (_item.itemType == Item.ItemType.Used)
        {
            for (int x = 0; x < itemEffects.Length; x++)
            {
                if (itemEffects[x].itemName == _item.itemName)
                {
                    for (int y = 0; y < itemEffects[x].part.Length; y++)
                    {
                        switch (itemEffects[x].part[y])
                        {
                            case HP:
                                playerStatus.IncreaseHP(itemEffects[x].num[y]);
                                break;
                            case DP:
                                playerStatus.IncreaseDP(itemEffects[x].num[y]);
                                break;
                            case Sp:
                                playerStatus.IncreaseSP(itemEffects[x].num[y]);
                                break;
                            case HUNGRY:
                                playerStatus.IncreaseHungry(itemEffects[x].num[y]);
                                break;
                            case THIRSTY:
                                playerStatus.IncreaseThirsty(itemEffects[x].num[y]);
                                break;
                            case SATISFY:
                                break;
                            default:
                                Debug.Log("잘못된 Status 부위. HP, DP, SP, HUNGRY, THIRSTY, SATISFY만 가능합니다.");
                            break;
                        }
                        Debug.Log(_item.itemName + " 을(를) 사용했습니다.");
                    }
                    return;
                }
            }
            Debug.Log("ItemEffectDatabase에 일치하는 ItemName이 없습니다.");
        }
    }
}
