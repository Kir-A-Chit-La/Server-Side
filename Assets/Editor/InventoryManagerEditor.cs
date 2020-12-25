using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InventoryManager))]
public class InventoryManagerEditor : Editor
{
    private InventoryManager inventoryManager;
    private void OnEnable() => inventoryManager = (InventoryManager)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Open Player Inventory"))
        {
            InventoryManagerEditorWindow.OpenWindow(inventoryManager);
        }
    }
}
