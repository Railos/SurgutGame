using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference selectSlot1Action;
    [SerializeField] private InputActionReference selectSlot2Action;

    [Header("Weapons")]
    [SerializeField] private GameObject[] weaponObjects;
    [SerializeField] private int startIndex;

    private int currentIndex = -1;

    private void OnEnable()
    {
        if (selectSlot1Action != null)
        {
            selectSlot1Action.action.Enable();
        }

        if (selectSlot2Action != null)
        {
            selectSlot2Action.action.Enable();
        }
    }

    private void Start()
    {
        if (weaponObjects == null || weaponObjects.Length == 0)
        {
            return;
        }

        SwitchTo(startIndex, true);
    }

    private void Update()
    {
        if (weaponObjects == null || weaponObjects.Length == 0)
        {
            return;
        }

        if (IsTriggered(selectSlot1Action))
        {
            SwitchTo(0, false);
            return;
        }

        if (IsTriggered(selectSlot2Action))
        {
            SwitchTo(1, false);
        }
    }

    private void OnDisable()
    {
        if (selectSlot1Action != null)
        {
            selectSlot1Action.action.Disable();
        }

        if (selectSlot2Action != null)
        {
            selectSlot2Action.action.Disable();
        }
    }

    private bool IsTriggered(InputActionReference actionReference)
    {
        if (actionReference == null)
        {
            return false;
        }

        return actionReference.action.triggered;
    }

    private void SwitchTo(int index, bool forceAll)
    {
        int clampedIndex = Mathf.Clamp(index, 0, weaponObjects.Length - 1);
        if (!forceAll && clampedIndex == currentIndex)
        {
            return;
        }

        if (forceAll)
        {
            for (int i = 0; i < weaponObjects.Length; i++)
            {
                SetWeaponActive(i, i == clampedIndex);
            }
        }
        else
        {
            SetWeaponActive(currentIndex, false);
            SetWeaponActive(clampedIndex, true);
        }

        currentIndex = clampedIndex;
    }

    private void SetWeaponActive(int index, bool active)
    {
        if (index < 0 || index >= weaponObjects.Length)
        {
            return;
        }

        GameObject weaponObject = weaponObjects[index];
        if (weaponObject == null)
        {
            return;
        }

        if (weaponObject.activeSelf != active)
        {
            weaponObject.SetActive(active);
        }
    }
}
