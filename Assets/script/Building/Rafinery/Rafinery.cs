using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using NUnit.Framework;

public class Rafinery : BuildingManager
{

    [Header("Rafinery Parameter")]
    public string resourceTag;
    public string resourceType;
    public bool IsWorking;
    private float timer;
    private float fuelRessourcesTimer;
    private float curentFuelValue;
    private string OnProductionItem;
    private bool firstProductionDone = false;
    Dictionary<string, int> RafineryItems;
    Dictionary<string, int> PLayerItems;

    [Header("Rafinery UI Reference")]
    public GameObject InventoryUIPrefab;
    public GameObject ItemSlotPrefabBuilding;
    public Button WorkingButton;
    public TMP_Text WorkingButtonTxt;
    public Button destroyButton;
    public Button CloseButton;
    public Button UpgradeButton;
    public TMP_Text RafineryInventory;
    public Slider InputProgressBar;
    public Slider FuelProgressBar;
  

    [System.Obsolete]
    public void OpenRafineryUI()
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

        if (!IsWorking) timer = 0f;


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

        //-----PlayerInventory-----//
        // Conteneur (avec GridLayoutGroup)
        Transform PlayerContainer = uiInstance.transform.Find("PlayerItemContainer");
        if (PlayerContainer == null)
        {
            Debug.LogError("PlayerItemContainer non trouvé dans le prefab !");
            return;
        }

        // Nettoyer le conteneur (si déjà ouvert)
        foreach (Transform child in PlayerContainer)
            Destroy(child.gameObject);

        // Récupérer les items du joueur
        PLayerItems = playerInventory.inventory.GetAllItems();

        foreach (var item in PLayerItems)
        {
            if (item.Value <= 0) continue;
            GameObject slot = Instantiate(ItemSlotPrefabBuilding, PlayerContainer);

            // Trouver les composants
            Image icon = slot.transform.Find("Button").GetComponent<Image>();
            Button button = slot.transform.Find("Button").GetComponent<Button>();
            TMP_Text countText = slot.transform.Find("Button/CountText").GetComponent<TMP_Text>();

            // Appliquer les données
            icon.sprite = GetItemIcon(item.Key);
            countText.text = item.Value.ToString();
            button.onClick.AddListener(delegate { PlayerToRafinery(playerInventory, item.Key); });

        }

        //-----RafineryInventory-----//
        // Conteneur (avec GridLayoutGroup)
        Transform RafineryContainer = uiInstance.transform.Find("RafineryItemContainer");
        if (RafineryContainer == null)
        {
            Debug.LogError("RafineryItemContainer non trouvé dans le prefab !");
            return;
        }

        // Nettoyer le conteneur (si déjà ouvert)
        foreach (Transform child in RafineryContainer)
            Destroy(child.gameObject);

        // Récupérer les items du joueur
        RafineryItems = inventory.GetAllItems();

        foreach (var item in RafineryItems)
        {
            if (item.Value <= 0) continue;
            GameObject slot = Instantiate(ItemSlotPrefabBuilding, RafineryContainer);

            // Trouver les composants
            Image icon = slot.transform.Find("Button").GetComponent<Image>();
            Button button = slot.transform.Find("Button").GetComponent<Button>();
            TMP_Text countText = slot.transform.Find("Button/CountText").GetComponent<TMP_Text>();

            // Appliquer les données
            icon.sprite = GetItemIcon(item.Key);
            countText.text = item.Value.ToString();
            button.onClick.AddListener(delegate { RafineryToPlayer(playerInventory, item.Key); });

        }


        //recupération de l'ui elements
        WorkingButton = uiInstance.transform.Find("WorkingButton").GetComponent<Button>();
        WorkingButtonTxt = uiInstance.transform.Find("WorkingButton/WorkingButtonTxt").GetComponent<TMP_Text>();
        destroyButton = uiInstance.transform.Find("DestroyButton").GetComponent<Button>();
        CloseButton = uiInstance.transform.Find("CloseButton").GetComponent<Button>();
        UpgradeButton = uiInstance.transform.Find("UpgradeButton").GetComponent<Button>();
        RafineryInventory = uiInstance.transform.Find("Inventory").GetComponent<TMP_Text>();
        InputProgressBar = uiInstance.transform.Find("InputProgressBar").GetComponent<Slider>();
        FuelProgressBar = uiInstance.transform.Find("FuelProgressBar").GetComponent<Slider>();

        // Connecte les événements 
        if (IsWorking)
        {
            WorkingButton.onClick.AddListener(IsNotWorkBuilding);
            WorkingButtonTxt.text = "Turn Off";
        }
        if (!IsWorking)
        {
            WorkingButton.onClick.AddListener(IsWorkBuilding);
            WorkingButtonTxt.text = "Turn On";
        }
        RafineryInventory.text = $"{BuildingStockedRessources} / {BuildingStorageMax}";
 
        //production bar setup
        InputProgressBar.maxValue = BuildingSpeed;
        InputProgressBar.value = timer;
        //fuel bar setup
        FuelProgressBar.maxValue = curentFuelValue;
        FuelProgressBar.value = fuelRessourcesTimer;

        //button listeners
        CloseButton.onClick.AddListener(DestroyUi);
        destroyButton.onClick.AddListener(DestroyRafinery);
        UpgradeButton.onClick.AddListener(UpgradeRafinery);

    }
    
    [System.Obsolete]
    public void RefreshRafineryUI()
    {
        if (uiInstance == null) return;

        // Mise à jour du texte d’inventaire
        RafineryInventory.text = $"{BuildingStockedRessources} / {BuildingStorageMax}";

        // Rafraîchir le contenu des conteneurs
        Transform playerContainer = uiInstance.transform.Find("PlayerItemContainer");
        Transform rafineryContainer = uiInstance.transform.Find("RafineryItemContainer");

        if (playerContainer == null || rafineryContainer == null)
        {
            Debug.LogWarning("Unfind item container in Rafinery UI.");
            return;
        }

        // Nettoie les anciens slots
        foreach (Transform child in playerContainer)
            Destroy(child.gameObject);
        foreach (Transform child in rafineryContainer)
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
            button.onClick.AddListener(delegate { PlayerToRafinery(playerInventory, item.Key); });
        }

        // Recrée les slots de la raffinerie
        var rafineryItems = inventory.GetAllItems();
        foreach (var item in rafineryItems)
        {
            if (item.Value <= 0) continue;

            GameObject slot = Instantiate(ItemSlotPrefabBuilding, rafineryContainer);
            Image icon = slot.transform.Find("Button").GetComponent<Image>();
            TMP_Text countText = slot.transform.Find("Button/CountText").GetComponent<TMP_Text>();
            Button button = slot.transform.Find("Button").GetComponent<Button>();

            icon.sprite = GetItemIcon(item.Key);
            countText.text = item.Value.ToString();
            button.onClick.AddListener(delegate { RafineryToPlayer(playerInventory, item.Key); });
        }
    }

    [System.Obsolete]
    public void Update()
    {
        if (BuildingLevel >= BuildingLevelMax)
        {
            UpgradeButton.interactable = false;
        }

        if (isFixed)
        {
            //crafting process 1
            foreach (var output in outputRessourcesType)
            {
                OnProductionItem = output.nom;

                if (!firstProductionDone && IsWorking)
                {
                    timer = 0f;
                    foreach (var fuel in fuelRessources)
                    {
                        if (inventory.GetItemAmount(fuel.nom) > 0)
                        {
                            inventory.RemoveItem(fuel.nom, 1);
                            BuildingStockedRessources--;
                            curentFuelValue = fuel.fuelValue;
                            fuelRessourcesTimer = curentFuelValue;
                            break;
                        }
                    }
                    firstProductionDone = true;
                    if(uiInstance != null)
                    {
                        InputProgressBar.maxValue = BuildingSpeed;
                        InputProgressBar.value = timer;
                        FuelProgressBar.maxValue = curentFuelValue;
                        FuelProgressBar.value = fuelRessourcesTimer;
                    }
                    RefreshRafineryUI();
                }

            timer += Time.deltaTime;
            //production bar update
            if ((inventory.GetItemAmount(inputRessourcesType[0].nom) > 0 || inventory.GetItemAmount(inputRessourcesType[1].nom) > 0) && IsWorking && fuelRessourcesTimer > 0 && uiInstance != null)
            {
                InputProgressBar.maxValue = BuildingSpeed;
                InputProgressBar.value = timer;
            }

            if (timer < BuildingSpeed) return;
            timer = 0f;
                
            if (IsWorking)
            {              
                // Si plus de fuel en cours, on en consommer un nouveau
                if (fuelRessourcesTimer <= 0 && (inventory.GetItemAmount(inputRessourcesType[0].nom) > 0 || inventory.GetItemAmount(inputRessourcesType[1].nom) > 0))
                {
                    foreach (var fuel in fuelRessources)
                    {
                        if (inventory.GetItemAmount(fuel.nom) > 0)
                        {
                            inventory.RemoveItem(fuel.nom, 1);
                            BuildingStockedRessources--;
                            curentFuelValue = fuel.fuelValue;
                            fuelRessourcesTimer = curentFuelValue;
                            break;
                        }
                    }
                }

                // Si toujours pas de fuel après la tentative
                if (fuelRessourcesTimer <= 0) return;


                // Vérifie les ressources d'entrée
                string inputUsed = null;
                foreach (var input in inputRessourcesType)
                {
                    if (inventory.GetItemAmount(input.nom) > 0)
                    {
                        inputUsed = input.nom;
                        break;
                    }
                }

                //Si il reste des ressources d'entrée
                if (inputUsed != null)
                {
                    // Consomme l’entrée
                    inventory.RemoveItem(inputUsed, 1);
                    // Produit la sortie
                    inventory.AddItem(OnProductionItem, 1);
                }



                InputProgressBar.value = timer;
                RefreshRafineryUI();
                timer = 0f;

            }
            // Diminue la durée du fuel
            if (fuelRessourcesTimer > 0) fuelRessourcesTimer--;
            //fuel bar update
            if (uiInstance != null)
            {
                FuelProgressBar.maxValue = curentFuelValue;
                FuelProgressBar.value = fuelRessourcesTimer;
            }


            }
            
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

    [System.Obsolete]
    private void UpgradeRafinery()
    {
        UpgradeBuilding();
        RefreshRafineryUI();
    }

    private void DestroyRafinery()
    {
        if (BuildingStockedRessources > 0) return;
        DestroyBuilding();
    
    }

    [System.Obsolete]
    public void IsWorkBuilding()
    {
        IsWorking = true;
        WorkingButton.onClick.RemoveAllListeners();
        WorkingButton.onClick.AddListener(IsNotWorkBuilding);
        WorkingButtonTxt.text = "Turn Off";
        timer = 0f;

    }

    [System.Obsolete]
    public void IsNotWorkBuilding()
    {
        IsWorking = false;
        WorkingButton.onClick.RemoveAllListeners();
        WorkingButton.onClick.AddListener(IsWorkBuilding);
        WorkingButtonTxt.text = "Turn On";
        timer = 0f;
        InputProgressBar.value = timer;

    }

    [System.Obsolete]
    public void PlayerToRafinery(PlayerInventory player, string resourceType)
    {

        if (BuildingStockedRessources >= BuildingStorageMax) return; //if rafinery full return

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
        RefreshRafineryUI();

    }

    [System.Obsolete]
    public void RafineryToPlayer(PlayerInventory player, string resourceType)
    {
        int Amount = 1;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Amount = inventory.GetItemAmount(resourceType);
        }
        
        inventory.RemoveItem(resourceType, Amount);
        player.inventory.AddItem(resourceType, Amount);
        BuildingStockedRessources -= Amount;
        RefreshRafineryUI();
        
    }


}