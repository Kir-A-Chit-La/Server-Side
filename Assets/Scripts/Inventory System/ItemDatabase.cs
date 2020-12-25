#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[CreateAssetMenu(fileName = "New Item Database", menuName = "Custom/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private Item[] _items;

    public Item GetItemReference(string itemId)
    {
        foreach(Item item in _items)
        {
            if(item.Id == itemId)
                return item;
        }
        return null;
    }
    public Item GetItemCopy(string itemId)
    {
        Item item = GetItemReference(itemId);
        if(item == null)
            return null;
        
        return item.GetCopy();
    }
    #if UNITY_EDITOR
    private void LoadItems() => _items = FindAssetsByType<Item>("Assets/Items");
    private void OnValidate() => LoadItems();
    private void OnEnable() => EditorApplication.projectChanged += LoadItems;
    private void OnDisable() => EditorApplication.projectChanged -= LoadItems;

    public static T[] FindAssetsByType<T>(params string[] folders) where T : Object
    {
        string type = typeof(T).ToString().Replace("UnityEngine.", "");

        string[] guids;
        if(folders == null || folders.Length == 0)
            guids = AssetDatabase.FindAssets("t:" + type);
        else
            guids = AssetDatabase.FindAssets("t:" + type, folders);

        T[] assets = new T[guids.Length];

        for(int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
        return assets;
    }
    #endif
}