using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory = new Inventory();

    // Exemple : ramasser du fer


    void Start()
    {
        PlayerInventoryUI.getPlayerInventory?.Invoke(this);

        inventory.AddItem("WoodFactory", 1);
    }
    
    public void PickupIron(int amount)
    {
        inventory.AddItem("Iron", amount);
        // Debug.Log("Le joueur a ramassé " + amount + " fer");
    }

    // Exemple : donner du fer à une usine
    public void GiveIronToFactory(IronFactory factory, int amount)
    {
        if (inventory.RemoveItem("Iron", amount))
        {
            factory.inventory.AddItem("Iron", amount);
            // Debug.Log("Donne " + amount + " fer à " + factory.name);
        }
    }
}
