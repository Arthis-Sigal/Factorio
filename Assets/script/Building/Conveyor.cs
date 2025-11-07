using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Conveyor : BuildingManager
{
    [Header("Conveyor Settings")]
    public Transform[] inputSlots;   // Points d'entrée
    public GameObject outputSlot;    // Point de sortie
    public int InputSlotCount;       // Nombre d'entrées attendues

    private float timer;
    private bool WrongItem = true;

    [Header("Visual Transfer")]
    public GameObject itemSpritePrefab;   // Un petit sprite ou cube pour représenter l’item

    private void Start()
    {
        // Initialise le tableau si besoin
        if (inputSlots == null || inputSlots.Length != InputSlotCount)
            inputSlots = new Transform[InputSlotCount];

        // Recherche des slots d’entrée dans le prefab déjà présent
        for (int i = 0; i < InputSlotCount; i++)
        {
            string slotName = $"In{i + 1}";
            Transform input = FindDeepChild(transform, slotName);

            if (input == null)
            {
                Debug.LogWarning($"⚠️ Slot d’entrée '{slotName}' introuvable dans {gameObject.name}");
                continue;
            }

            inputSlots[i] = input;
            Debug.Log($"✅ Slot d’entrée trouvé : {slotName}");
        }

        // 🔍 Recherche du slot de sortie
        Transform outTransform = FindDeepChild(transform, "Out");
        if (outTransform != null)
        {
            outputSlot = outTransform.gameObject;
            Debug.Log("✅ Slot de sortie trouvé !");
        }
        else
        {
            Debug.LogWarning($"⚠️ Aucun slot de sortie ('Out') trouvé dans {gameObject.name}");
        }
    }

    private void Update()
    {
        if (!isFixed) return;

        for (int i = 0; i < inputSlots.Length; i++)
        {
            Transform input = inputSlots[i];
            if (input == null) continue;

            float detectionRadius = 1.3f;
            Collider[] hitColliders = Physics.OverlapSphere(input.position, detectionRadius);

            // Utilisé pour éviter les doublons par parent
            HashSet<BuildingManager> alreadyChecked = new HashSet<BuildingManager>();

            foreach (var hitCollider in hitColliders)
            {
                // 🔹 On monte jusqu'au parent principal contenant BuildingManager
                BuildingManager building = hitCollider.GetComponentInParent<BuildingManager>();

                if (building == null || building == this)
                    continue;

                // Si déjà traité, on ignore
                if (alreadyChecked.Contains(building))
                    continue;
                alreadyChecked.Add(building);

                // ⛔ Ignore les entrées (In)
                if (hitCollider.transform.name.StartsWith("In"))
                    continue;

                // ✅ Autorise convoyeurs et bâtiments fixes
                if (building.isFixed)
                {
                    timer += Time.deltaTime;
                    if (timer < BuildingSpeed) break;
                    timer = 0f;
                    //Debug.Log($"📦 Objet détecté à l'entrée {i + 1} : {building.gameObject.name}");
                    // Ici, vous pouvez ajouter la logique pour transférer des objets
                    foreach (var item in building.inventory.GetAllItems())
                    {


                        WrongItem = true;
                        if (BuildingStockedRessources >= BuildingStorageMax)
                            break;

                        if (!building.name.Contains("Conveyor"))
                        {
                            foreach (var output in building.outputRessourcesType)
                            {
                                //Debug.Log(item.Key + " comparé à " + output.nom);
                                if (item.Key == output.nom)
                                {
                                    WrongItem = false;
                                    //Debug.Log("   - Item autorisé pour le transfert.");
                                    break;
                                }
                            }

                            if (WrongItem)
                            {
                                //Debug.Log("   - Item non autorisé, transfert annulé.");
                                continue;
                            }
                        }

                        if (item.Value > 0)
                        {
                            //Debug.Log($"   - Transfert de '{item.Key}' en cours...");
                            // Transfert d'un item
                            building.inventory.RemoveItem(item.Key, 1);
                            building.BuildingStockedRessources--;
                            inventory.AddItem(item.Key, 1);
                            BuildingStockedRessources++;
                            StartCoroutine(AnimateItemTransfer(outputSlot.transform.position, building.transform.position, item.Key));



                        }
                        //Debug.Log($"   - Transfert de '{item.Key}' effectué.");
                        //Debug.Log(inventory.GetAllItems());
                    }

                    Debug.DrawRay(input.position, Vector3.up * detectionRadius, Color.green, 1.0f);
                }
            }
        }
        
        //on gére output
        float outputDetectionRadius = 1.3f;
        Collider[] outputHitColliders = Physics.OverlapSphere(outputSlot.transform.position, outputDetectionRadius);
        foreach (var hitCollider in outputHitColliders)
        {
            Debug.Log("Output detection check avec " + hitCollider.gameObject.name);
            // 🔹 On monte jusqu'au parent principal contenant BuildingManager
            BuildingManager building = hitCollider.GetComponentInParent<BuildingManager>();

            if (building == null || building == this)
                continue;

            // ⛔ Ignore les entrées (In)
                if (hitCollider.transform.name.StartsWith("In"))
                    continue;

            // ✅ Autorise convoyeurs et bâtiments fixes
            if (building.isFixed)
            {
                Debug.Log("hello");
                if (BuildingStockedRessources <= 0) break;
                timer += Time.deltaTime;
                if (timer < BuildingSpeed) break;
                timer = 0f;

                // Ici, vous pouvez ajouter la logique pour transférer des objets
                foreach (var item in inventory.GetAllItems())
                {
                    Debug.Log("hello2");
                    if (building.BuildingStockedRessources >= building.BuildingStorageMax)
                        break;

                    if (item.Value > 0)
                    {
                        Debug.Log("hello3");
                        inventory.RemoveItem(item.Key, 1);
                        BuildingStockedRessources--;
                        building.inventory.AddItem(item.Key, 1);
                        building.BuildingStockedRessources++;
                    }
                    

                }
            }
        }
    }

    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform result = FindDeepChild(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }

    private IEnumerator AnimateItemTransfer(Vector3 start, Vector3 end, string itemName)
    {
        if (itemSpritePrefab == null)
        {
            Debug.LogWarning("⚠️ Aucun prefab visuel assigné pour l’animation d’item !");
            yield break;
        }


        // Instancie un visuel
        GameObject itemVisual = Instantiate(itemSpritePrefab, start, Quaternion.identity);
        itemVisual.name = $"ItemVisual_{itemName}";
 
        SpriteRenderer sr = itemVisual.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = Resources.Load<Sprite>($"Icons/{itemName}"); // Assure-toi que les sprites sont nommés correctement dans les ressources);
            sr.color = Color.Lerp(Color.white, Color.yellow, Random.value * 0.2f);
        }

        float elapsed = 0f;

        while (elapsed < BuildingSpeed)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / BuildingSpeed);
            itemVisual.transform.position = Vector3.Lerp(end, start, t);
            yield return null;
        }

        Destroy(itemVisual);
    }

}
