public class IronFactory : Factory
{
    public override void Start()
    {

        resourceTag = "IronOre";

        if (outputRessourcesType == null || outputRessourcesType.Length == 0)
        {
            outputRessourcesType = new OutputRessourcesType[1];
            outputRessourcesType[0] = new OutputRessourcesType { nom = "Iron" };
        }

        base.Start();
        
    }
}
