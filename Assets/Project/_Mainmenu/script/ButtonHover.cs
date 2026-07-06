using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CursorFollowUI cursor;

    public bool isExitButton = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("ENTER : " + gameObject.name);

        if (isExitButton)
            cursor.SetSad();
        else
            cursor.SetPoint();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("EXIT : " + gameObject.name);

        cursor.SetHappy();
    }
}