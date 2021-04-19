using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyboardEventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool IsHovered;

    [SerializeField] CopyTextureBuffer texture;
    [SerializeField] CursorMovement cursor;
    PointerEventData frameEventData;

    public void OnPointerDown(PointerEventData eventData)
    {
        cursor.PointerDown();
    }

    public void OnPointerOver(PointerEventData eventData)
    {
        Vector3 DesktopPos = texture.ImageVectorToDesktopPos(transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition));
        cursor.PointerMove(new Vector2Int((int)DesktopPos.x, (int)DesktopPos.y));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        frameEventData = eventData;
        IsHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cursor.PointerUp();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            enabled = false;
            Debug.LogError("KeyboardEventHandeler: No Event System In scene (Add manually for keyboard gestures support)");
        }

        if (FindObjectOfType<BaseInputModule>() == null)
        {
            enabled = false;
            Debug.LogError("KeyboardEventHandeler: No Input Module In scene (Add manually for keyboard gestures support)");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsHovered && frameEventData != null)
        {
            OnPointerOver(frameEventData);
        }
    }

}
