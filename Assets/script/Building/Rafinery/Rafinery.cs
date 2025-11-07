using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Rafinery UI Reference")]
    public GameObject InventoryUIPrefab;
    public GameObject ItemSlotPrefabBuilding;
    private Button WorkingButton;
    public TMP_Text WorkingButtonTxt;
    private Button destroyButton;
    private Button CloseButton;
    private Button upgradeButton;
    private TMP_Text RafineryInventory;
    private Slider InputProgressBar;
    private Slider FuelProgressBar;
    private TMP_Text upgradePrice;
    private TMP_Text recipeText;
  

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

        GenerateItemSlot();

        //recupération de l'ui elements
        WorkingButton = uiInstance.transform.Find("WorkingButton").GetComponent<Button>();
        WorkingButtonTxt = uiInstance.transform.Find("WorkingButton/WorkingButtonTxt").GetComponent<TMP_Text>();
        destroyButton = uiInstance.transform.Find("DestroyButton").GetComponent<Button>();
        CloseButton = uiInstance.transform.Find("CloseButton").GetComponent<Button>();
        upgradeButton = uiInstance.transform.Find("UpgradeButton").GetComponent<Button>();
        RafineryInventory = uiInstance.transform.Find("Inventory").GetComponent<TMP_Text>();
        InputProgressBar = uiInstance.transform.Find("InputProgressBar").GetComponent<Slider>();
        FuelProgressBar = uiInstance.transform.Find("FuelProgressBar").GetComponent<Slider>();
        upgradePrice = uiInstance.transform.Find("UpgradePrice").GetComponent<TMP_Text>();
        recipeText = uiInstance.transform.Find("RecipeText").GetComponent<TMP_Text>();
        DisplayRecipe(recipeText);

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
        InputProgressBar.interactable = false;
        //fuel bar setup
        FuelProgressBar.maxValue = curentFuelValue;
        FuelProgressBar.value = fuelRessourcesTimer;
        FuelProgressBar.interactable = false;

        upgradePrice.text = "Upgrade Price : " + BuildingUpgradePrice.ToString();

        //button listeners
        CloseButton.onClick.AddListener(DestroyUi);
        destroyButton.onClick.AddListener(DestroyRafinery);
        upgradeButton.onClick.AddListener(UpgradeRafinery);

        RefreshRafineryUI();

    }

    [System.Obsolete]
    public void RefreshRafineryUI()
    {
        if (uiInstance == null) return;

        // Mise à jour du texte d’inventaire
        RafineryInventory.text = $"{BuildingStockedRessources} / {BuildingStorageMax}";

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

    public void DisplayRecipe(TMP_Text recipeText)
    {
        if (recipeText == null)
        {
            Debug.LogWarning("TMP_Text est null !");
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.AppendLine("Recipe :");

        // Inputs
        sb.Append("Input : ");
        for (int i = 0; i < inputRessourcesType.Length; i++)
        {
            var input = inputRessourcesType[i];
            sb.Append($"{input.inputQuantity} {input.nom}");
            if (i < inputRessourcesType.Length - 1)
                sb.Append(", "); // séparer par des virgules
        }
        sb.AppendLine();

        // Fuels
        sb.Append("Fuel : ");
        for (int i = 0; i < fuelRessources.Length; i++)
        {
            var fuel = fuelRessources[i];
            sb.Append(fuel.nom);
            if (i < fuelRessources.Length - 1)
                sb.Append(", ");
        }
        sb.AppendLine();

        // Outputs
        sb.Append("Output : ");
        for (int i = 0; i < outputRessourcesType.Length; i++)
        {
            var output = outputRessourcesType[i];
            sb.Append($"{output.outputQuantity} {output.nom}");
            if (i < outputRessourcesType.Length - 1)
                sb.Append(", ");
        }

        // Assigner le texte au TMP_Text
        recipeText.text = sb.ToString();
    }

    [System.Obsolete]
    public void Update()
    {

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
                //on verifie que les ressources sont disponible
                int recipeCount = 0;
                bool hasAllResources = true;
                foreach (var input in inputRessourcesType)
                {
                    if (inventory.GetItemAmount(input.nom) >= input.inputQuantity)
                        recipeCount++;
                    else
                    {
                        hasAllResources = false;
                        break;
                    }

                }
            //production bar update
            if (hasAllResources && IsWorking && fuelRessourcesTimer > 0 && uiInstance != null)
            {
                InputProgressBar.maxValue = BuildingSpeed;
                InputProgressBar.value = timer;
            }

            if (timer < BuildingSpeed) return;
            timer = 0f;
                
            if (IsWorking)
            {              
                // Si plus de fuel en cours, on en consommer un nouveau
                if (fuelRessourcesTimer <= 0 && hasAllResources)
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


                //si les ressources sont disponible, on les retire et on crée la sortie
                if (recipeCount == inputRessourcesType.Length)
                    {
                        foreach (var input in inputRessourcesType)
                        {
                            if (inventory.GetItemAmount(input.nom) >= input.inputQuantity)
                            {
                                //Si il reste des ressources d'entrée
                                if (input.nom != null)
                                {
                                    // Consomme l’entrée
                                    inventory.RemoveItem(input.nom, input.inputQuantity);
                                }
                            }
                        }
                        inventory.AddItem(OnProductionItem, output.outputQuantity);
                    }
              
               


                if (uiInstance != null)
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