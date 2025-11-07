using System.Collections.Generic;
using UnityEngine;


public enum PlacementMode
{
    Fixed,
    Valid,
    Invalid
}

public class BuildingManager : MonoBehaviour
{
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;
    protected PlayerInventory playerInventory;
    private PlayerInteraction playerInteraction;

    public MeshRenderer[] meshComponents;
    private Dictionary<MeshRenderer, List<Material>> initialMaterials;

    [HideInInspector] public bool hasValidPlacement;
    [HideInInspector] public bool isFixed;

    [Header("General Building Parameters")]
    public int BuildingStorageMax;
    public int BuildingStockedRessources = 0;
    public int BuildingLevel;
    public int BuildingLevelMax;
    public float BuildingSpeed;
    protected int BuildingUpgradePrice;
    public class InputRessourcesType
    {
        public string nom;
        public int inputQuantity;
    }
    public class FuelRessources
    {
        public string nom;
        public int fuelValue;
    }
      public class OutputRessourcesType
    {
        public string nom;
        public int outputQuantity;
    }
    public InputRessourcesType[] inputRessourcesType;
    public OutputRessourcesType[] outputRessourcesType;
    public FuelRessources[] fuelRessources;

    [Header("General Building UI")]
    public GameObject uiInstance;
    public Inventory inventory = new Inventory();

    protected Canvas mainCanvas;
    public Dictionary<string, int> PLayerItems;

    protected PlayerMoney playerMoney; 

    private int _nObstacles;

    [System.Obsolete]
    private void Awake()
    {
        hasValidPlacement = true;
        isFixed = true;
        _nObstacles = 0;

        BuildingStorageMax = BuildingStorageMax * BuildingLevel;
        BuildingSpeed = BuildingSpeed / BuildingLevel;
        BuildingUpgradePrice = 400 * BuildingLevel;

        _InitializeMaterials(); 

        playerInteraction = FindObjectOfType<PlayerInteraction>();
    }

    public void SetCanvas(Canvas canvas)
    {
        mainCanvas = canvas;
    }

    protected void DestroyUi()
    {
        Destroy(uiInstance);
        playerInteraction.setUiIsOpen(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isFixed) return;

        // ignore ground objects
        if (_IsGround(other.gameObject)) return;

        _nObstacles++;
        SetPlacementMode(PlacementMode.Invalid);
    }

    private void OnTriggerExit(Collider other)
    {
        if (isFixed) return;

        // ignore ground objects
        if (_IsGround(other.gameObject)) return;

        _nObstacles--;
        if (_nObstacles == 0)
            SetPlacementMode(PlacementMode.Valid);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _InitializeMaterials();
    }
#endif

    public void SetPlacementMode(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            isFixed = true;
            hasValidPlacement = true;


        }
        else if (mode == PlacementMode.Valid)
        {
            hasValidPlacement = true;
        }
        else
        {
            hasValidPlacement = false;
        }
        SetMaterial(mode);
    }

    public void SetMaterial(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            foreach (MeshRenderer r in meshComponents)
                r.sharedMaterials = initialMaterials[r].ToArray();
        }
        else
        {
            Material matToApply = mode == PlacementMode.Valid
                ? validPlacementMaterial : invalidPlacementMaterial;

            Material[] m; int nMaterials;
            foreach (MeshRenderer r in meshComponents)
            {
                nMaterials = initialMaterials[r].Count;
                m = new Material[nMaterials];
                for (int i = 0; i < nMaterials; i++)
                    m[i] = matToApply;
                r.sharedMaterials = m;
            }
        }
    }

    private void _InitializeMaterials()
    {
        if (initialMaterials == null)
            initialMaterials = new Dictionary<MeshRenderer, List<Material>>();
        if (initialMaterials.Count > 0)
        {
            foreach (var l in initialMaterials) l.Value.Clear();
            initialMaterials.Clear();
        }

        foreach (MeshRenderer r in meshComponents)
        {
            initialMaterials[r] = new List<Material>(r.sharedMaterials);
        }
    }

    private bool _IsGround(GameObject o)
    {
        return ((1 << o.layer) & BuildingPlacer.instance.groundLayerMask.value) != 0;
    }

    protected void DestroyBuilding()
    {
        Destroy(uiInstance);
        Destroy(gameObject);
        string BuildingName = gameObject.name.Replace("(Clone)", "");
        playerInventory.inventory.AddItem(BuildingName, 1); // Ajoute de l'argent au joueur lors de la destruction du bâtiment
        Debug.Log("Bâtiment détruit, 1 " + gameObject.name + " ajouté à l'inventaire du joueur.");
    }

    [System.Obsolete]
    protected void UpgradeBuilding()
    {
        playerMoney = FindObjectOfType<PlayerMoney>();


        if (BuildingLevel >= BuildingLevelMax) return;
        if (BuildingLevel * 400 > playerMoney.GetMoneyAmount()) return;
        BuildingUpgradePrice = 400 * BuildingLevel;
        playerMoney.RemoveMoney(BuildingUpgradePrice);
        BuildingLevel++;
        BuildingUpgradePrice = 400 * BuildingLevel;
        BuildingStorageMax = BuildingStorageMax * BuildingLevel;
        BuildingSpeed = BuildingSpeed / BuildingLevel;
    }

       protected Sprite GetItemIcon(string itemName)
    {
        // Si tu as une ressource nommée comme l’item dans Resources/Icons/
        Sprite icon = Resources.Load<Sprite>($"Icons/{itemName}");
        if (icon == null)
            icon = Resources.Load<Sprite>("Icons/default_icon");
        return icon;
    }
}