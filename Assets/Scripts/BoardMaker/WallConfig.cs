using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallConfig : MonoBehaviour, IPointerClickHandler
{
    public bool IsActive => mesh.enabled;

    [SerializeField]public int wallType;

    public int BackOrLeftPart;
    public int ForwardOrRightPart;

    public bool IsHorizon;

    MeshRenderer mesh;

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(BackOrLeftPart + " : " + ForwardOrRightPart);
        mesh.enabled = !mesh.enabled;
    }
}
