using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 100f;
    private Camera cam;

    [Header("Factory References")]
    public IronFactory Ironfactory;
    public WoodFactory Woodfactory;
    public CharcoalFactory Charcoalfactory;
    public IronRafinery IronRafinery;

    [Header("Rafinery References")]
    public PlayerInventoryUI playerInventoryUI;
    [SerializeField] private Canvas mainCanvas;


    void Start()
    {
        cam = Camera.main;
    }

    [Obsolete]
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            {
                // Vérifie si l'objet est un bâtiment qui implémente IProductionBuilding
                IronFactory Ironfactory = hit.collider.GetComponentInParent<IronFactory>();
                if (Ironfactory != null)
                {
                    Ironfactory.SetCanvas(mainCanvas);
                    Ironfactory.OpenFactoryUI();
                }
          
                

                WoodFactory Woodfactory = hit.collider.GetComponentInParent<WoodFactory>();
                if (Woodfactory != null)
                {
                    Woodfactory.SetCanvas(mainCanvas);
                    Woodfactory.OpenFactoryUI();  
                }
 


                CharcoalFactory CharcoalFactory = hit.collider.GetComponentInParent<CharcoalFactory>();
                if (CharcoalFactory != null)
                {
                    CharcoalFactory.SetCanvas(mainCanvas);
                    CharcoalFactory.OpenFactoryUI();   
                }

                IronRafinery IronRafinery = hit.collider.GetComponentInParent<IronRafinery>();
                if (IronRafinery != null)
                {
                    IronRafinery.SetCanvas(mainCanvas);
                    IronRafinery.OpenRafineryUI();
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            // On récupère (ou crée) la référence s’il n’y en a pas déjà une
            if (playerInventoryUI == null)
            {
                playerInventoryUI = FindObjectOfType<PlayerInventoryUI>();
                if (playerInventoryUI == null)
                {
                    Debug.LogError("Aucun PlayerInventoryUI trouvé dans la scène !");
                    return;
                }
            }

            // Si un inventaire est déjà ouvert, on le ferme
            if (playerInventoryUI.uiInstance != null)
            {
                playerInventoryUI.CloseInventory();
            }
            else // Sinon on l’ouvre
            {
                playerInventoryUI.OpenPlayerInventory();
            }
        }

    }
}
