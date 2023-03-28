using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float timeToWait;
    private string tipToShow = "Ammo cost: ";
    private bool flag = false;
    private Vector2 mousePos;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("HOVERED");

        flag = true;

        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("EXITED");

        flag = false;

        StopAllCoroutines();
        HoverTipManager.OnMouseLoseFocus();
    }

    private void ShowMessage()
    {
        InventorySlot chosenSlot = gameObject.GetComponent<InventorySlot>();
        var item = chosenSlot.Weapon;
        mousePos = Input.mousePosition;

        HoverTipManager.OnMouseHover(tipToShow + item.AmmoCost1 + "<sprite name=\"" + item.AmmoType1 + "\">", mousePos);
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        ShowMessage();
    }

    private void Update()
    {
        if (flag == true)
        {
            ShowMessage();
        }
    }
}