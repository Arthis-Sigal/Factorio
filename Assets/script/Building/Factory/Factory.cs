using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Factory : BuildingManager
{

    [Header("Factory Parameter")]
    public string resourceTag;
    public string resourceType;
    private float timer = 0f;

    [Header("Factory UI Reference")]
    public GameObject factoryUIPrefab;
    public TMP_Text nameText;
    public TMP_Text SliderAmountValue;
    public TMP_Text FactoryInventory;
    public Slider GetAmount;
    public int GetAmountInt;
    public Button collectButton;
    public Button destroyButton;
    public Button CloseButton;

 

    public virtual void Start()
    {
        foreach (var output in outputRessourcesType)
        {
            resourceType = output.nom;
        }
    }
    [System.Obsolete]
    public void OpenFactoryUI()
    {
  
        playerInventory = FindObjectOfType<PlayerInventory>();

        if (uiInstance != null)
        {
            Debug.LogError("Instance déjà existance");
            return;
        }

        // Instancie le prefab du UI à l'intérieur du Canvas
        uiInstance = Instantiate(factoryUIPrefab, mainCanvas.transform);
        uiInstance.transform.SetParent(mainCanvas.transform, false);
        uiInstance.transform.localPosition = Vector3.zero;
        uiInstance.transform.localScale = Vector3.one;
        

        // Récupère les éléments du prefab UI
        nameText = uiInstance.transform.Find("Panel/FactoryName").GetComponent<TMP_Text>();
        GetAmount = uiInstance.transform.Find("Panel/AmountSlider").GetComponent<Slider>();
        collectButton = uiInstance.transform.Find("Panel/GetButton").GetComponent<Button>();
        SliderAmountValue = uiInstance.transform.Find("Panel/SliderValueTxt").GetComponent<TMP_Text>();
        FactoryInventory = uiInstance.transform.Find("Panel/Inventory").GetComponent<TMP_Text>();
        destroyButton = uiInstance.transform.Find("Panel/DestroyButton").GetComponent<Button>();
        CloseButton = uiInstance.transform.Find("Panel/CloseButton").GetComponent<Button>();

        // Configure le UI
        nameText.text = gameObject.name;
        GetAmount.maxValue = BuildingStockedRessources;
        GetAmount.value = 0;
        FactoryInventory.text = $"{BuildingStockedRessources} / {BuildingStorageMax}";
        SliderAmountValue.text = $"{GetAmountInt}";

        // Connecte les événements
        GetAmount.onValueChanged.AddListener(OnProductionChanged);
        collectButton.onClick.AddListener(OnCollectClicked);
        CloseButton.onClick.AddListener(DestroyUi);
        destroyButton.onClick.AddListener(DestroyBuilding);
    }

    private void OnProductionChanged(float value)
    {
        GetAmountInt = Mathf.RoundToInt(value);
        SliderAmountValue.text = $"{GetAmountInt}";
    }

    public void OnCollectClicked()
    {
        GiveResourcesToPlayer(playerInventory);
        Debug.Log($"Ressources collectées depuis {gameObject.name}");
    }
    
    public bool CheckIfOnResource(string resourceTag)
    {
        Vector3 origin = transform.position + Vector3.up * 100f;
        float maxDist = 1000f;

        RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down, maxDist, ~0, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        if (hits.Length == 0)
            return false;

        foreach (var hit in hits)
        {
            // Ignorer le propre collider du bâtiment
            if (hit.collider.gameObject == this.gameObject)
                continue;

            // Vérifier si le tag correspond (directement ou sur la racine)
            if (hit.collider.CompareTag(resourceTag) || hit.collider.transform.root.CompareTag(resourceTag))
                return true;
        }

        return false;
    }

    public void GiveResourcesToPlayer(PlayerInventory player)
    {
        if (player == null)
        {
            Debug.LogError("PlayerInventory est NULL !");
            return;
        }

        if (inventory == null)
        {
            Debug.LogError($"{name} n’a pas d’inventaire assigné !");
            return;
        }

        if (inventory.GetItemAmount(resourceType) >= GetAmountInt)
        {
            inventory.RemoveItem(resourceType, GetAmountInt);
            player.inventory.AddItem(resourceType, GetAmountInt);
            BuildingStockedRessources -= GetAmountInt;
            Debug.Log($"{name} a donné {GetAmountInt} {resourceType} au joueur !");
        }
        else
        {
            Debug.Log($"{name} n’a pas assez de {resourceType} !");
        }
    }
    
    public void Update()
    {
        if (isFixed)
        {

            bool onResource = CheckIfOnResource(resourceTag);

            if (mainCanvas != null)
            {
                FactoryInventory.text = $"{BuildingStockedRessources} / {BuildingStorageMax}";
                GetAmount.maxValue = BuildingStockedRessources;
            }
            
            if (onResource && BuildingStockedRessources < BuildingStorageMax)
            {
                //Debug.Log(this.name + "Dans une foret");
                timer += Time.deltaTime;
                if (timer >= BuildingSpeed)
                {
                    timer = 0f;
                    BuildingStockedRessources += 1;
                    inventory.AddItem(resourceType, 1);
                    //Debug.Log($"{name} produit 1 bois. Total : {BuildingStockedRessources}");
                }
            }
            else if (BuildingStockedRessources >= BuildingStorageMax)
            {
                return; ;
            }
            else
            {
                return;
            }


        }
    }

}