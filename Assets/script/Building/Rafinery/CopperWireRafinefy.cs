public class CopperWireRafinery : Rafinery
{
   

    public void Start()
    {
        if (inputRessourcesType == null || inputRessourcesType.Length == 0)
        {
            inputRessourcesType = new InputRessourcesType[1];
            inputRessourcesType[0] = new InputRessourcesType { nom = "CopperIngot", inputQuantity = 1};
        }

        if (fuelRessources == null || fuelRessources.Length == 0)
        {
            fuelRessources = new FuelRessources[2];
            fuelRessources[0] = new FuelRessources { nom = "Charcoal", fuelValue = 5 };
            fuelRessources[1] = new FuelRessources { nom = "Wood", fuelValue = 2 };
        }


        if (outputRessourcesType == null || outputRessourcesType.Length == 0)
        {
            outputRessourcesType = new OutputRessourcesType[1];
            outputRessourcesType[0] = new OutputRessourcesType { nom = "CopperWire", outputQuantity = 2};
        }
    }
}
