using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer instance;

    protected PlayerInventory playerInventory;

    public LayerMask groundLayerMask;

    protected GameObject _buildingPrefab;
    protected GameObject _toBuild;

    protected Camera _mainCamera;

    protected Ray _ray;
    protected RaycastHit _hit;

    public Canvas mainCanvas;
    public GameObject buildingModeUIPrefab;
    public GameObject ItemSlotPrefabBuilding;
    protected GameObject uiInstance;
    private Button closeButton;
    Dictionary<string, int> PLayerItems;

    public bool isOnEditMode;
    private PlayerInteraction playerInteraction;

    //liste des batiments disponibles à la construction
    public class BuildingListContainer
    {
        public string nom;
    }
    public BuildingListContainer[] buildingListContainer;


    private void Awake()
    {
        instance = this;
        _mainCamera = Camera.main;
        _buildingPrefab = null;
        isOnEditMode = false;

        if (buildingListContainer == null || buildingListContainer.Length == 0)
        {
            buildingListContainer = new BuildingListContainer[4];
            buildingListContainer[0] = new BuildingListContainer { nom = "Factory" };
            buildingListContainer[1] = new BuildingListContainer { nom = "Rafinery" };
            buildingListContainer[2] = new BuildingListContainer { nom = "Conveyor" };
            buildingListContainer[3] = new BuildingListContainer { nom = "Stockage" };
        }
    }
    public void SetBuildingPrefab(GameObject prefab)
    {
        _buildingPrefab = prefab;
        _PrepareBuilding();
        EventSystem.current.SetSelectedGameObject(null); //cancel UI Nav
    }

    protected virtual void _PrepareBuilding()
    {
        if (_toBuild) Destroy(_toBuild);
        _toBuild = Instantiate(_buildingPrefab);
        _toBuild.SetActive(false);

        BuildingManager m = _toBuild.GetComponent<BuildingManager>();
        m.isFixed = false;
        m.SetPlacementMode(PlacementMode.Valid);
    }

    [System.Obsolete]
    public void ToggleBuildingMode()
    {
        isOnEditMode = true;
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogError("❌ Aucun PlayerInventory trouvé dans la scène !");
                return;
            }
        }

        playerInteraction = FindObjectOfType<PlayerInteraction>();
        playerInteraction.setUiIsOpen(true);

        // Créer le panneau principal
        if (uiInstance != null)
        {
            Debug.LogError("Instance déjà existance");
            return;
        }
        uiInstance = Instantiate(buildingModeUIPrefab, mainCanvas.transform);
        uiInstance.transform.localScale = Vector3.one;

        // On récupère le RectTransform du prefab
        RectTransform rect = uiInstance.GetComponent<RectTransform>();

        // On s’assure que le parent est bien le Canvas (en mode UI)
        rect.SetParent(mainCanvas.transform, false);

        // Réinitialise la taille et l’échelle
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;



        loadInventory();

        closeButton = uiInstance.transform.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(delegate { DestroyUI(); });


    }

    [System.Obsolete]
    private void DestroyUI()
    {
        Destroy(uiInstance);
        isOnEditMode = false;
        playerInteraction = FindObjectOfType<PlayerInteraction>();
        playerInteraction.setUiIsOpen(false);
    }


    public void UpdateBuildingModeUI()
    {
        if (uiInstance == null) return;

        loadInventory();
        
    }

    private Sprite GetItemIcon(string itemName)
    {
        // Si tu as une ressource nommée comme l’item dans Resources/Icons/
        Sprite icon = Resources.Load<Sprite>($"Icons/{itemName}");
        if (icon == null)
            icon = Resources.Load<Sprite>("Icons/default_icon");
        return icon;
    }
    
    private void loadInventory()
    {
        //-----PlayerInventory-----//
        // Conteneur (avec GridLayoutGroup)
        Transform PlayerContainer = uiInstance.transform.Find("Panel");
        if (PlayerContainer == null)
        {
            Debug.LogError("PlayerItemContainer non trouvé dans le prefab !");
            return;
        }

        // Nettoyer le conteneur (si déjà ouvert)
        foreach (Transform child in PlayerContainer)
            Destroy(child.gameObject);
        PLayerItems = playerInventory.inventory.GetAllItems();

        foreach (var item in PLayerItems)
        {
            if (item.Value <= 0) continue;

            foreach (var building in buildingListContainer)
            {
                if (!item.Key.Contains(building.nom))
                    continue;

                GameObject slot = Instantiate(ItemSlotPrefabBuilding, PlayerContainer);

                // Trouver les composants
                Image icon = slot.transform.Find("Button").GetComponent<Image>();
                Button button = slot.transform.Find("Button").GetComponent<Button>();
                TMP_Text countText = slot.transform.Find("Button/CountText").GetComponent<TMP_Text>();

                // Appliquer les données
                icon.sprite = GetItemIcon(item.Key);
                countText.text = item.Value.ToString();
                //on regarde si un prefab de bâtiment correspond à l’item
                GameObject buildingPrefab = Resources.Load<GameObject>($"prefab/{item.Key}");
                button.onClick.AddListener(delegate { SetBuildingPrefab(buildingPrefab); });
            }
            

        }
        
    }
    
}
