using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;


public class QuestInstance : BuildingManager
{
    public GameObject QuestUIPrefab;

    private Button closeButton;
    private Button questButton;
    private TMP_Text rewardTxt;
    private TMP_Text questDescriptionTxt;
    private Button nextQuestButton;

    //Gestion quéte
    private int NumberOfQuestsCompleted = 0;
    private int questRewardAmount = 100;
    private int questIngredientCount = 10;
    private int questIngredientList = 1;

    public class QuestIngredients
    {
        public string nom;
        public int UnlockedAtLevel;
    }
    public QuestIngredients[] questIngredients;

    public class CurrentQuestIngredients
    {
        public string nom;
        public int numberAsked;
    }
    public CurrentQuestIngredients[] currentQuestIngredients;

    [Obsolete]
    public void Start()
    {
           if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogError("❌ Aucun PlayerInventory trouvé dans la scène !");
                return;
            }
        }

        if (questIngredients == null || questIngredients.Length == 0)
        {
            questIngredients = new QuestIngredients[9];
            questIngredients[0] = new QuestIngredients { nom = "Iron", UnlockedAtLevel = 2 };
            questIngredients[1] = new QuestIngredients { nom = "Wood", UnlockedAtLevel = 1 };
            questIngredients[2] = new QuestIngredients { nom = "Charcoal", UnlockedAtLevel = 7 };
            questIngredients[3] = new QuestIngredients { nom = "IronIngot", UnlockedAtLevel = 9 };
            questIngredients[4] = new QuestIngredients { nom = "Bolt", UnlockedAtLevel = 15 };
            questIngredients[5] = new QuestIngredients { nom = "Copper", UnlockedAtLevel = 5 };
            questIngredients[6] = new QuestIngredients { nom = "CopperIngot", UnlockedAtLevel = 11 };
            questIngredients[7] = new QuestIngredients { nom = "CopperWire", UnlockedAtLevel = 18 };
            questIngredients[8] = new QuestIngredients { nom = "Magnet", UnlockedAtLevel = 25 };
        }

        if (currentQuestIngredients == null || currentQuestIngredients.Length == 0) //first quest
        {
            currentQuestIngredients = new CurrentQuestIngredients[questIngredientList];
            currentQuestIngredients[0] = new CurrentQuestIngredients { nom = "Wood", numberAsked = questIngredientCount };

        }

    }

    [Obsolete]
    public void OpenQuestUI()
    {
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogError("❌ Aucun PlayerInventory trouvé dans la scène !");
                return;
            }
        }

        // Créer le panneau principal
        if (uiInstance != null)
        {
            Debug.LogError("Instance déjà existance");
            return;
        }
        uiInstance = Instantiate(QuestUIPrefab, mainCanvas.transform);
        uiInstance.transform.localScale = Vector3.one;

        // On récupère le RectTransform du prefab
        RectTransform rect = uiInstance.GetComponent<RectTransform>();

        // On s’assure que le parent est bien le Canvas (en mode UI)
        rect.SetParent(mainCanvas.transform, false);

        // Positionne l’UI au centre du Canvas
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        // Réinitialise la taille et l’échelle
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;



        // Récupérer les items du joueur
        PLayerItems = playerInventory.inventory.GetAllItems();

        generateUI();
    }

    [Obsolete]
    private void updateUI()
    {
        generateUI();
    }

    [Obsolete]
    private void generateUI()
    {
        closeButton = uiInstance.transform.Find("CloseButton").GetComponent<Button>();
        questButton = uiInstance.transform.Find("QuestButton").GetComponent<Button>();
        rewardTxt = uiInstance.transform.Find("RewardTxt").GetComponent<TMP_Text>();
        questDescriptionTxt = uiInstance.transform.Find("QuestDescriptionTxt").GetComponent<TMP_Text>(); 
        nextQuestButton = uiInstance.transform.Find("NextQuestButton").GetComponent<Button>();



        //button listeners
        closeButton.onClick.AddListener(DestroyUi);
        questButton.onClick.AddListener(CheckQuestCompletion);
        nextQuestButton.onClick.AddListener(NextQuest);
        if (NumberOfQuestsCompleted >= 1) nextQuestButton.interactable = true;
        else nextQuestButton.interactable = false;

        rewardTxt.text = "Reward: " + questRewardAmount + " Gold";
        //on affcihe tout le tableau dans quest description
        questDescriptionTxt.text = "Bring me:\n";
        foreach (var ingredient in currentQuestIngredients)
        {
            questDescriptionTxt.text += "- " + ingredient.numberAsked + " " + ingredient.nom + "\n";
            questDescriptionTxt.text += "\n";
        }
    }

    [Obsolete]
    private void CheckQuestCompletion()
    {
        if (playerMoney == null)
        {
            playerMoney = FindObjectOfType<PlayerMoney>();
            if (playerMoney == null)
            {
                Debug.LogError("❌ Aucun PlayerMoney trouvé dans la scène !");
                return;
            }
        }
        //Vérifier si le joueur a les ingrédients nécessaires
        bool hasAllIngredients = true;
        foreach (var ingredient in currentQuestIngredients)
        {
            if (playerInventory.inventory.GetItemAmount(ingredient.nom) < ingredient.numberAsked)
            {
                hasAllIngredients = false;
                break;
            }
        }

        if (hasAllIngredients)
        {
            //Récompenser le joueur
            playerMoney.AddMoney(questRewardAmount);
            NumberOfQuestsCompleted++;
            Debug.Log("Quête complétée ! Récompense : " + questRewardAmount + " Gold.");

            //Retirer les ingrédients de l'inventaire du joueur
            foreach (var ingredient in currentQuestIngredients)
            {
                playerInventory.inventory.RemoveItem(ingredient.nom, ingredient.numberAsked);
            }
            NextQuest();
        }
        else
        {
            Debug.Log("Ingrédients manquants pour compléter la quête.");
        }
    }

    [Obsolete]
    private void NextQuest()
    {
        // Récompense
        questRewardAmount = Mathf.Max(50, questRewardAmount + 25 * NumberOfQuestsCompleted);

        // Filtrer les ingrédients débloqués
        var filtered = questIngredients
            .Where(q => q.UnlockedAtLevel <= NumberOfQuestsCompleted)
            .ToList();

        if (filtered.Count == 0)
        {
            Debug.LogWarning("⚠️ Aucun ingrédient débloqué encore !");
            return;
        }

        // Nombre max = nb dispo
        questIngredientList = Mathf.Min(1 + NumberOfQuestsCompleted / 2, filtered.Count);

        Debug.Log($"✅ Quête {NumberOfQuestsCompleted}, items demandés : {questIngredientList}/{filtered.Count} disponibles");

        currentQuestIngredients = new CurrentQuestIngredients[questIngredientList];
        List<int> usedIndexes = new List<int>();

        for (int i = 0; i < questIngredientList; i++)
        {
            int safety = 200;  // sécurité anti freeze
            int rnd;

            do
            {
                rnd = UnityEngine.Random.Range(0, filtered.Count);
                safety--;

                if (safety <= 0)
                {
                    Debug.LogError("❌ Sécurité activée : impossible de trouver un nouvel ingrédient unique !");
                    break;
                }

            } while (usedIndexes.Contains(rnd));

            usedIndexes.Add(rnd);

            int quantity = UnityEngine.Random.Range(
                3 * NumberOfQuestsCompleted,
                8 * NumberOfQuestsCompleted + 1
            );

            currentQuestIngredients[i] = new CurrentQuestIngredients
            {
                nom = filtered[rnd].nom,
                numberAsked = quantity
            };

            Debug.Log($"📦 Quête → {quantity} x {filtered[rnd].nom}");
        }

        updateUI();
        Debug.Log("🎯 Nouvelle quête générée !");
    }




        
}