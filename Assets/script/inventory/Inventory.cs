using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [SerializeField] private Dictionary<string, int> items = new Dictionary<string, int>();

    // 🔹 Ajouter une ressource
    public void AddItem(string itemName, int amount)
    {
        if (!items.ContainsKey(itemName))
            items[itemName] = 0;

        items[itemName] += amount;
    }

    // 🔹 Retirer une ressource
    public bool RemoveItem(string itemName, int amount)
    {
        if (!items.ContainsKey(itemName) || items[itemName] < amount)
            return false;

        items[itemName] -= amount;
        return true;
    }

    // 🔹 Obtenir la quantité d’un item
    public int GetItemAmount(string itemName)
    {
        if (!items.ContainsKey(itemName)) return 0;
        return items[itemName];
    }

    // 🔹 Afficher tout le contenu (utile pour debug)
    public void PrintInventory(string ownerName)
    {
        Debug.Log($"=== Inventaire de {ownerName} ===");
        foreach (var kvp in items)
            Debug.Log($"{kvp.Key} : {kvp.Value}");
    }

    public Dictionary<string, int> GetAllItems()
    {
        return new Dictionary<string, int>(items);
    }

}
