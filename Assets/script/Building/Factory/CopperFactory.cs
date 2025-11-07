public class CopperFactory : Factory
{
    public override void Start()
    {
        resourceTag = "CopperOre";

        if (outputRessourcesType == null || outputRessourcesType.Length == 0)
        {
            outputRessourcesType = new OutputRessourcesType[1];
            outputRessourcesType[0] = new OutputRessourcesType { nom = "Copper" };
        }
        base.Start();
    }

}
