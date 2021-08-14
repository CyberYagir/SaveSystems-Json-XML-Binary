using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void ChangeSaveType(TMP_Dropdown drop)
    {
        SavesManager.instance.ChangeType((SavesManager.SaveType)drop.value);
    }
    public void Save()
    {
        SavesManager.instance.Save();
    }

    public void Load()
    {
        SavesManager.instance.Load();
    }
}
