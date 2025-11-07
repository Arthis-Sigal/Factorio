using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 100f;
    private Camera cam;

    public BuildingPlacer buildingPlacer;
 

    [Header("Rafinery References")]
    public PlayerInventoryUI playerInventoryUI;
    private PlayerInventory playerInventory;
    [SerializeField] private Canvas mainCanvas;

    [Header("PlayerUIReference")]
    public GameObject pauseMenu;

    [Header("UIRestriction")]
    private bool uiIsOpen;
    private bool gameIsPaused;


    void Start()
    {
        cam = Camera.main;
        gameIsPaused = false;
        Time.timeScale = 1f;
    }

    [Obsolete]
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused) ResumeGame();
            else PauseGame();
            
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            {
                if (buildingPlacer.isOnEditMode)
                {
                    BuildingManager building = hit.collider.GetComponentInParent<BuildingManager>();
                    //Debug.Log($"🧱 Suppression du bâtiment : {building.name}");
                    if (building != null && building.tag != "Indestructible")
                    {
                        playerInventory = FindObjectOfType<PlayerInventory>();
                        building.name = building.name.Replace("(Clone)", "").Trim();
                        playerInventory.inventory.AddItem(building.name, 1);

                        if (building.inventory.GetAllItems().Count > 0)
                        {
                            foreach (var item in building.inventory.GetAllItems())
                            {
                                playerInventory.inventory.AddItem(item.Key, item.Value);
                            }
                        }

                        buildingPlacer.UpdateBuildingModeUI();



                        // Destroy Building
                        Destroy(building.gameObject);

                    }
                    return;
                }

                if (uiIsOpen == true) return; //If an UI is open, don't

                // check is the object is a building
                IronFactory Ironfactory = hit.collider.GetComponentInParent<IronFactory>();
                if (Ironfactory != null)
                {
                    Ironfactory.SetCanvas(mainCanvas);
                    Ironfactory.OpenFactoryUI();
                    setUiIsOpen(true);
                }



                WoodFactory Woodfactory = hit.collider.GetComponentInParent<WoodFactory>();
                if (Woodfactory != null)
                {
                    Woodfactory.SetCanvas(mainCanvas);
                    Woodfactory.OpenFactoryUI();
                    setUiIsOpen(true);
                }

                CopperFactory CopperFactory = hit.collider.GetComponentInParent<CopperFactory>();
                if (CopperFactory != null)
                {
                    CopperFactory.SetCanvas(mainCanvas);
                    CopperFactory.OpenFactoryUI();
                    setUiIsOpen(true);
                }

                CharcoalRafinery CharcoalRafinery = hit.collider.GetComponentInParent<CharcoalRafinery>();
                if (CharcoalRafinery != null)
                {
                    CharcoalRafinery.SetCanvas(mainCanvas);
                    CharcoalRafinery.OpenRafineryUI();
                    setUiIsOpen(true);
                }
                
                IronRafinery IronRafinery = hit.collider.GetComponentInParent<IronRafinery>();
                if (IronRafinery != null)
                {
                    IronRafinery.SetCanvas(mainCanvas);
                    IronRafinery.OpenRafineryUI();
                    setUiIsOpen(true);
                } 
                
                MagnetRafinery MagnetRafinery = hit.collider.GetComponentInParent<MagnetRafinery>();
                if (MagnetRafinery != null)
                {
                    Debug.Log("Heho");
                    MagnetRafinery.SetCanvas(mainCanvas);
                    MagnetRafinery.OpenRafineryUI();
                    setUiIsOpen(true);
                }

                BoltRafinery BoltRafinery = hit.collider.GetComponentInParent<BoltRafinery>();
                if (BoltRafinery != null)
                {
                    BoltRafinery.SetCanvas(mainCanvas);
                    BoltRafinery.OpenRafineryUI();
                    setUiIsOpen(true);
                }

                CopperRafinery CopperRafinery = hit.collider.GetComponentInParent<CopperRafinery>();
                if (CopperRafinery != null)
                {
                    CopperRafinery.SetCanvas(mainCanvas);
                    CopperRafinery.OpenRafineryUI();
                    setUiIsOpen(true);
                }

                CopperWireRafinery CopperWireRafinery = hit.collider.GetComponentInParent<CopperWireRafinery>();
                if (CopperWireRafinery != null)
                {
                    CopperWireRafinery.SetCanvas(mainCanvas);
                    CopperWireRafinery.OpenRafineryUI();
                    setUiIsOpen(true);
                }

                QuestInstance QuestInstance = hit.collider.GetComponentInParent<QuestInstance>();
                if (QuestInstance != null)
                {
                    QuestInstance.SetCanvas(mainCanvas);
                    QuestInstance.OpenQuestUI();
                    setUiIsOpen(true);
                }

                ShopBuilding ShopBuilding = hit.collider.GetComponentInParent<ShopBuilding>();
                if (ShopBuilding != null)
                {
                    ShopBuilding.SetCanvas(mainCanvas);
                    ShopBuilding.OpenShopUI();
                    setUiIsOpen(true);
                }
                Stockage Stockage = hit.collider.GetComponentInParent<Stockage>();
                if (Stockage != null)
                {
                    Stockage.SetCanvas(mainCanvas);
                    Stockage.OpenStockageUI();
                    setUiIsOpen(true);
                }
            }
        } 

        if (Input.GetKeyDown(KeyCode.I))
        {
            // Check is inventory is'nt open
            if (playerInventoryUI == null)
            {
                playerInventoryUI = FindObjectOfType<PlayerInventoryUI>();
                if (playerInventoryUI == null)
                {
                    Debug.LogError("No PlayerInventoryUI found");
                    return;
                }
            }

            // If inventory open, close it
            if (playerInventoryUI.uiInstance != null) 
            {
                playerInventoryUI.CloseInventory();
                setUiIsOpen(false);
            }
            else // else open it
            {
                playerInventoryUI.OpenPlayerInventory();
                setUiIsOpen(true);
            }
        }
    }
    private void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        gameIsPaused = true;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        gameIsPaused = false;
    }
    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        gameIsPaused = false;
    }

    public void setUiIsOpen(bool a)
    {
        uiIsOpen = a;
    }
}


