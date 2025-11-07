using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuilding : BuildingManager
{
    [Header("Prefab Building Reference")]
    public GameObject IronFactory;
    public GameObject WoodFactory;
    public GameObject CopperFactory;
    public GameObject IronRafinery;
    public GameObject CharcoalRafinery;
    public GameObject BoltRafinery;
    public GameObject CopperRafinery;
    public GameObject CopperWireRafinery;
    public GameObject MagnetRafinery;
    public GameObject StockageBuilding;
    public GameObject Conveyor1;
    public GameObject Conveyor2;
    public GameObject Conveyor3;
    public GameObject Conveyor4;
    public GameObject Conveyor5;
    public GameObject Conveyor6;

    private class BuildingList
    {
        public GameObject type;
        public int price;
        public string name;
    }
    private BuildingList[] buildingList;

    [Header("UI Reference")]
    public GameObject shopUIPrefab;
    public GameObject ItemSlotPrefab;
    private Button closeButton;
    private TMP_Text moneyTxt;

    [System.Obsolete]
    void Start()
    {

        if (buildingList == null || buildingList.Length == 0)
        {
            buildingList = new BuildingList[16];
            buildingList[0] = new BuildingList { type = IronFactory, price = 100, name = IronFactory.name };
            buildingList[1] = new BuildingList { type = WoodFactory, price = 50, name = WoodFactory.name };
            buildingList[2] = new BuildingList { type = IronRafinery, price = 150, name = IronRafinery.name };
            buildingList[3] = new BuildingList { type = CharcoalRafinery, price = 150, name = CharcoalRafinery.name };
            buildingList[4] = new BuildingList { type = Conveyor1, price = 10, name = Conveyor1.name };
            buildingList[5] = new BuildingList { type = Conveyor2, price = 10, name = Conveyor2.name };
            buildingList[6] = new BuildingList { type = Conveyor3, price = 10, name = Conveyor3.name };
            buildingList[7] = new BuildingList { type = Conveyor4, price = 10, name = Conveyor4.name };
            buildingList[8] = new BuildingList { type = Conveyor5, price = 10, name = Conveyor5.name };
            buildingList[9] = new BuildingList { type = Conveyor6, price = 10, name = Conveyor6.name };
            buildingList[10] = new BuildingList { type = BoltRafinery, price = 200, name = BoltRafinery.name };
            buildingList[11] = new BuildingList { type = CopperFactory, price = 125, name = CopperFactory.name };
            buildingList[12] = new BuildingList { type = CopperRafinery, price = 150, name = CopperRafinery.name };
            buildingList[13] = new BuildingList { type = CopperWireRafinery, price = 200, name = CopperWireRafinery.name };
            buildingList[14] = new BuildingList { type = MagnetRafinery, price = 500, name = MagnetRafinery.name };
            buildingList[15] = new BuildingList { type = StockageBuilding, price = 300, name = StockageBuilding.name };
        }

        playerMoney = FindObjectOfType<PlayerMoney>();
    }

    [System.Obsolete]
    public void OpenShopUI()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();

        if (uiInstance != null)
        {
            Debug.LogError("Instance déjà existance");
            return;
        }

        // Instancie le prefab du UI à l'intérieur du Canvas
        uiInstance = Instantiate(shopUIPrefab, mainCanvas.transform);
        uiInstance.transform.SetParent(mainCanvas.transform, false);
        uiInstance.transform.localPosition = Vector3.zero;
        uiInstance.transform.localScale = Vector3.one;


        // Récupère les éléments du prefab UI
        closeButton = uiInstance.transform.Find("CloseButton").GetComponent<Button>();
        moneyTxt = uiInstance.transform.Find("icon/PlayerMoney").GetComponent<TMP_Text>(); 

        moneyTxt.text = playerMoney.GetMoneyAmount().ToString();

        // Connecte les événements
        closeButton.onClick.AddListener(DestroyUi);


        Transform shopContainer = uiInstance.transform.Find("ShopContainer");

        foreach (var building in buildingList)
        {
            GameObject slot = Instantiate(ItemSlotPrefab, shopContainer);
            Image icon = slot.transform.Find("Button").GetComponent<Image>();
            TMP_Text priceTxt = slot.transform.Find("Button/Price").GetComponent<TMP_Text>();
            Button button = slot.transform.Find("Button").GetComponent<Button>();
            TMP_Text nameTxt = slot.transform.Find("BuildingName").GetComponent<TMP_Text>();

            icon.sprite = GetItemIcon(building.name);

            nameTxt.text = building.name;
            priceTxt.text = building.price.ToString();
            button.onClick.AddListener(delegate { BuyBuilding(building.name, building.price); });
        }

    }




    private void UpdateUI()
    {
        moneyTxt.text = playerMoney.GetMoneyAmount().ToString();
    }

    private void BuyBuilding(string building, int price)
    {
        if (playerMoney.GetMoneyAmount() < price) return;

        playerInventory.inventory.AddItem(building, 1);
        playerMoney.RemoveMoney(price);
        Debug.Log(building + " acheter");
        UpdateUI();
    }

 
}
