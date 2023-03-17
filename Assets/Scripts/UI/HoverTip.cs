using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float timeToWait;
    private string tipToShow = "Ammo cost: ";

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("HOVERED");
        
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("EXITED");

        StopAllCoroutines();
        HoverTipManager.OnMouseLoseFocus();
    }

    private void ShowMessage() 
    {
        InventorySlot chosenSlot = gameObject.GetComponent<InventorySlot>();
        var item = chosenSlot.Weapon;

        HoverTipManager.OnMouseHover(tipToShow + item.AmmoCost1, Input.mousePosition);
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        ShowMessage();
    }
}