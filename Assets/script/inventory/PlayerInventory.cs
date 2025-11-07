using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory = new Inventory();

    // Exemple : ramasser du fer


    void Start()
    {
        PlayerInventoryUI.getPlayerInventory?.Invoke(this);

        inventory.AddItem("Iron", 1000);
        inventory.AddItem("Wood", 1000);
        inventory.AddItem("Charcoal", 1000);
        inventory.AddItem("IronIngot", 1000);
        inventory.AddItem("Bolt", 1000);
        inventory.AddItem("Copper", 1000);
        inventory.AddItem("CopperIngot", 1000);
        inventory.AddItem("CopperWire", 1000);
        inventory.AddItem("Magnet", 1000);
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
