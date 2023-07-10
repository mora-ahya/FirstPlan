using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPItemUseEventConfig : MonoBehaviour, IGameEventConfig
{
    public bool IsActive { get { return isActive; } set { isActive = value; gameObject.SetActive(isActive); } }
    public int EventID => (int)FPGameEventKind.Item;
    public int ItemID => itemId;

    [SerializeField] bool isActive = true;
    [SerializeField] int itemId = 0;

    void Awake()
    {
        //Image image = GetComponent<Image>();
        //image.sprite = 
        //if (isActive == false)
        //{
        //    gameObject.SetActive(false);
        //}
    }
}
