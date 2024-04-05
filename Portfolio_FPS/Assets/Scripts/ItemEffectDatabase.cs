using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class ItemEffect
{
    public string itemName; //�������� �̸� (Ű��)
    [Tooltip("HP, DP, SP, HUNGRY, THIRSTY, SATISFY�� �����մϴ�.")]
    public string[] part; //����
    public int[] num; //��ġ
}

public class ItemEffectDatabase : MonoBehaviour
{
    [SerializeField]
    private ItemEffect[] itemEffects;

    //�ʿ��� ������Ʈ
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
                                Debug.Log("�߸��� Status ����. HP, DP, SP, HUNGRY, THIRSTY, SATISFY�� �����մϴ�.");
                            break;
                        }
                        Debug.Log(_item.itemName + " ��(��) ����߽��ϴ�.");
                    }
                    return;
                }
            }
            Debug.Log("ItemEffectDatabase�� ��ġ�ϴ� ItemName�� �����ϴ�.");
        }
    }
}
