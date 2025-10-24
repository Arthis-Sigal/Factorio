using UnityEngine;

public class IronRafinery : Rafinery
{
   

    public void Start()
    {
        if (inputRessourcesType == null || inputRessourcesType.Length == 0)
        {
            inputRessourcesType = new InputRessourcesType[2];
            inputRessourcesType[0] = new InputRessourcesType { nom = "Iron"};
            inputRessourcesType[1] = new InputRessourcesType { nom = "MetalScrap"};
        }

        if (fuelRessources == null || fuelRessources.Length == 0)
        {
            fuelRessources = new FuelRessources[2];
            fuelRessources[0] = new FuelRessources { nom = "Charcoal", fuelValue = 5 };
            fuelRessources[1] = new FuelRessources { nom = "Wood", fuelValue = 2 };
        }
        
        OutputRessourcesType = "IronIngot";
    }
}
