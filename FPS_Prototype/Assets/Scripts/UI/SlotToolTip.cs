using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotToolTip : MonoBehaviour
{
    //필요한 컴포넌트들
    [SerializeField]
    private GameObject go_Base;

    [SerializeField]
    private TextMeshProUGUI itemNameTXT;
    [SerializeField]
    private TextMeshProUGUI itemDescTXT;
    [SerializeField]
    private TextMeshProUGUI howToUsedTXT;

    public void ShowTooltip(Item _item, Vector3 _pos)
    {
        go_Base.SetActive(true);
        _pos += new Vector3(go_Base.GetComponent<RectTransform>().rect.width * 0.5f, go_Base.GetComponent<RectTransform>().rect.height * 0.5f, 0f);
        go_Base.transform.position = _pos;

        itemNameTXT.text = _item.itemName;
        itemDescTXT.text = _item.itemDesc;

        if (_item.itemType == Item.ItemType.Equipment)
        {
            howToUsedTXT.text = "R Clik To Equip";
        }
        else if (_item.itemType == Item.ItemType.Used)
        {
            howToUsedTXT.text = "R Clik To Eat";
        }
        else
            howToUsedTXT.text = "";
    }

    public void HideToolTip()
    {
        go_Base.SetActive (false);
    }
}
