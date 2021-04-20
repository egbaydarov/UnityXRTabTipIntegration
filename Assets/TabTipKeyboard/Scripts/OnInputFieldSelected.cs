using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnInputFieldSelected : MonoBehaviour
{
    [SerializeField] GameObject Keyboard;
    [SerializeField] Vector3 KeyboardPositionOffset;
    [SerializeField] ExternTextField externTextField;
    InputField currentInputField;
    int ID = -1;

    void Start()
    {
        Keyboard.SetActive(false);
    }

    private void LateUpdate()
    {
        if (currentInputField != null)
        {
            currentInputField.text = externTextField.ExternTextFieldData;
            currentInputField.caretPosition = currentInputField.text.Length;
            currentInputField.ForceLabelUpdate();
        }
    }

    void Update()
    {
        var selectedField = EventSystem.current.currentSelectedGameObject?.GetComponent<InputField>();
        if (selectedField != null && selectedField.GetInstanceID() != ID)
        {
            currentInputField = selectedField;

            Keyboard.SetActive(true);
            externTextField.ClearExterTextField();
            ID = currentInputField.GetInstanceID();
            gameObject.transform.position = EventSystem.current.currentSelectedGameObject.transform.position;
            Keyboard.transform.localPosition = new Vector3(0, ((transform as RectTransform).sizeDelta.y + (Keyboard.transform as RectTransform).sizeDelta.y) / -2, 0)
                + KeyboardPositionOffset;

        }
    }
}
