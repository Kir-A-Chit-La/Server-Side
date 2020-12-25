using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InventoryManagerEditorWindow : ExtendedEditorWindow
{
    private static InventoryManager InventoryManager;
    private static Inventory inventory;
    public static void OpenWindow(InventoryManager inventoryManager)
    {
        InventoryManagerEditorWindow window = GetWindow<InventoryManagerEditorWindow>("Player Inventory");
        window.minSize = new Vector2(800, 400);
        window.serializedObject = new SerializedObject(inventoryManager);
        InventoryManager = inventoryManager;
    }
    private void OnGUI()
    {
        
    }
}
