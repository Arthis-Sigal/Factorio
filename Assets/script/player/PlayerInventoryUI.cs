using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerInventoryUI : MonoBehaviour
{
    private PlayerInventory playerInventory;
    [SerializeField] private GameObject inventoryUIPrefab;
    [SerializeField] private Canvas mainCanvas;

    public GameObject ItemSlotPrefab;

    public GameObject uiInstance;

    public static Action<PlayerInventory> getPlayerInventory;

    void OnEnable()
    {
        getPlayerInventory += ChangePlayerInventory;
    }

    void OnDisable()
    {
        getPlayerInventory -= ChangePlayerInventory;
    }

    void ChangePlayerInventory(PlayerInventory NewPlayerInventory)
    {
        playerInventory = NewPlayerInventory;
    }
    
    public void OpenPlayerInventory()
    {
        if (playerInventory == null)
        {
            if (playerInventory == null)
            {
                Debug.LogError("❌ Aucun PlayerInventory trouvé dans la scène !");
                return;
            }
        }
        
        Debug.Log(inventoryUIPrefab.name);


        // Créer le panneau principal
        uiInstance = Instantiate(inventoryUIPrefab, mainCanvas.transform);
        uiInstance.transform.localScale = Vector3.one;

        // On récupère le RectTransform du prefab
        RectTransform rect = uiInstance.GetComponent<RectTransform>();

        // On s’assure que le parent est bien le Canvas (en mode UI)
        rect.SetParent(mainCanvas.transform, false);

        // Positionne l’UI au centre du Canvas
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        // Réinitialise la taille et l’échelle
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;



        // Conteneur (avec GridLayoutGroup)
        Transform container = uiInstance.transform.Find("ItemContainer");
        if (container == null)
        {
            Debug.LogError("ItemContainer non trouvé dans le prefab !");
            return;
        }

        // Nettoyer le conteneur (si déjà ouvert)
        foreach (Transform child in container)
            Destroy(child.gameObject);

        // Récupérer les items du joueur
        Dictionary<string, int> items = playerInventory.inventory.GetAllItems();
        
        foreach (var item in items)
        {
            if (item.Value <= 0) continue;
            GameObject slot = Instantiate(ItemSlotPrefab, container);

            // Trouver les composants
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            TMP_Text countText = slot.transform.Find("CountText").GetComponent<TMP_Text>();

            // Appliquer les données
            icon.sprite = GetItemIcon(item.Key);
            countText.text = item.Value.ToString();
       
        }
    }

    // Exemple pour récupérer l’icône associée à un nom d’objet
    private Sprite GetItemIcon(string itemName)
    {
        // Si tu as une ressource nommée comme l’item dans Resources/Icons/
        Sprite icon = Resources.Load<Sprite>($"Icons/{itemName}");
        if (icon == null)
            icon = Resources.Load<Sprite>("Icons/default_icon");
        return icon;
    }

    public void CloseInventory()
    {
        Destroy(uiInstance);
    }
}
