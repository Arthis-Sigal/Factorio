using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stockage : BuildingManager
{

    [Header("Stockage Parameter")]
    public string resourceTag;
    public string resourceType;
    public bool IsWorking;
    private float timer;
    private float fuelRessourcesTimer;
    private float curentFuelValue;
    private string OnProductionItem;
    private bool firstProductionDone = false;

    [Header("Stockage UI Reference")]
    public GameObject InventoryUIPrefab;
    public GameObject ItemSlotPrefabBuilding;
    private Button destroyButton;
    private Button CloseButton;
    private Button upgradeButton;
    private TMP_Text StockageInventory;
    private TMP_Text upgradePrice;
  

    [System.Obsolete]
    public void OpenStockageUI()
    {
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogError("❌ Aucun PlayerInventory trouvé dans la scène !");
                return;
            }
        }


        // Créer le panneau principal
        if (uiInstance != null)
        {
            Debug.LogError("Instance déjà existance");
            return;
        }
        uiInstance = Instantiate(InventoryUIPrefab, mainCanvas.transform);
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

        GenerateItemSlot();

        //recupération de l'ui elements
        destroyButton = uiInstance.transform.Find("DestroyButton").GetComponent<Button>();
        CloseButton = uiInstance.transform.Find("CloseButton").GetComponent<Button>();
        upgradeButton = uiInstance.transform.Find("UpgradeButton").GetComponent<Button>();
        upgradePrice = uiInstance.transform.Find("UpgradePrice").GetComponent<TMP_Text>();
        StockageInventory = uiInstance.transform.Find("Inventory").GetComponent<TMP_Text>();

        StockageInventory.text = $"{BuildingStockedRessources} / {BuildingStorageMax}";
        upgradePrice.text = "Upgrade Price : " + BuildingUpgradePrice.ToString();

        //button listeners
        CloseButton.onClick.AddListener(DestroyUi);
        destroyButton.onClick.AddListener(DestroyStockage);
        upgradeButton.onClick.AddListener(UpgradeStockage);

        RefreshStockageUI();

    }

    [System.Obsolete]
    public void RefreshStockageUI()
    {
        if (uiInstance == null) return;

        // Mise à jour du texte d’inventaire
        StockageInventory.text = $"{BuildingStockedRessources} / {BuildingStorageMax}";

        GenerateItemSlot();

        upgradePrice.text = "Upgrade Price : " + BuildingUpgradePrice.ToString();
        if (BuildingLevel >= BuildingLevelMax)
        {
            upgradeButton.interactable = false;
            upgradePrice.text = "";
        }

    }

    [System.Obsolete]
    private void GenerateItemSlot()
    {
        // Rafraîchir le contenu des conteneurs
        Transform playerContainer = uiInstance.transform.Find("PlayerItemContainer");
        Transform stockageContainer = uiInstance.transform.Find("StockageItemContainer");

        if (playerContainer == null || stockageContainer == null)
        {
            Debug.LogWarning("Unfind item container in Stockage UI.");
            return;
        }

        // Nettoie les anciens slots
        foreach (Transform child in playerContainer)
            Destroy(child.gameObject);
        foreach (Transform child in stockageContainer)
            Destroy(child.gameObject);

        // Recrée les slots du joueur
        var playerItems = playerInventory.inventory.GetAllItems();
        foreach (var item in playerItems)
        {
            if (item.Value <= 0) continue;

            GameObject slot = Instantiate(ItemSlotPrefabBuilding, playerContainer);
            Image icon = slot.transform.Find("Button").GetComponent<Image>();
            TMP_Text countText = slot.transform.Find("Button/CountText").GetComponent<TMP_Text>();
            Button button = slot.transform.Find("Button").GetComponent<Button>();

            icon.sprite = GetItemIcon(item.Key);
            countText.text = item.Value.ToString();
            button.onClick.AddListener(delegate { PlayerToStockage(playerInventory, item.Key); });
        }

        // Create stockage slots
        var stockageItems = inventory.GetAllItems();
        foreach (var item in stockageItems)
        {
            if (item.Value <= 0) continue;

            GameObject slot = Instantiate(ItemSlotPrefabBuilding, stockageContainer);
            Image icon = slot.transform.Find("Button").GetComponent<Image>();
            TMP_Text countText = slot.transform.Find("Button/CountText").GetComponent<TMP_Text>();
            Button button = slot.transform.Find("Button").GetComponent<Button>();

            icon.sprite = GetItemIcon(item.Key);
            countText.text = item.Value.ToString();
            button.onClick.AddListener(delegate { StockageToPlayer(playerInventory, item.Key); });
        }

    }




    // Exemple pour récupérer l’icône associée à un nom d’objet
     public void CloseInventory()
    {
        Destroy(uiInstance);
    }

    [System.Obsolete]
    private void UpgradeStockage()
    {
        UpgradeBuilding();
        RefreshStockageUI();
    }

    private void DestroyStockage()
    {
        if (BuildingStockedRessources > 0) return;
        DestroyBuilding();

    }

    [System.Obsolete]
    public void PlayerToStockage(PlayerInventory player, string resourceType)
    {

        if (BuildingStockedRessources >= BuildingStorageMax) return; //if stockage full return

        int Amount = 1;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Amount = player.inventory.GetItemAmount(resourceType);
        }
        Debug.Log("Trying to transfer " + Amount + " of " + resourceType + " to Rafinery.");
        if (BuildingStockedRessources + Amount > BuildingStorageMax)
        {
            Amount = BuildingStorageMax - BuildingStockedRessources;
        }

        player.inventory.RemoveItem(resourceType, Amount);
        inventory.AddItem(resourceType, Amount);
        BuildingStockedRessources += Amount;
        RefreshStockageUI();

    }

    [System.Obsolete]
    public void StockageToPlayer(PlayerInventory player, string resourceType)
    {
        int Amount = 1;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Amount = inventory.GetItemAmount(resourceType);
        }
        inventory.RemoveItem(resourceType, Amount);
        player.inventory.AddItem(resourceType, Amount);
        BuildingStockedRessources -= Amount;
        RefreshStockageUI();

    }
}