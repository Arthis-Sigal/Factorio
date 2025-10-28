using UnityEngine;

public class WoodFactory : Factory
{
    public override void Start()
    {
        resourceTag = "WoodOre";

        if (outputRessourcesType == null || outputRessourcesType.Length == 0)
        {
            outputRessourcesType = new OutputRessourcesType[1];
            outputRessourcesType[0] = new OutputRessourcesType { nom = "Wood" };
        }

        base.Start();
    }
}
