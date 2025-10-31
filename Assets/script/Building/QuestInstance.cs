using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class QuestInstance : BuildingManager
{
    public GameObject QuestUIPrefab;

    private Button closeButton;
    private Button questButton;
    private TMP_Text rewardTxt;
    private TMP_Text questDescriptionTxt;

    private PlayerMoney playerMoney;

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

    public void Start()
    {

        playerMoney = FindObjectOfType<PlayerMoney>();

        if (questIngredients == null || questIngredients.Length == 0)
        {
            questIngredients = new QuestIngredients[4];
            questIngredients[0] = new QuestIngredients { nom = "Iron", UnlockedAtLevel = 2 };
            questIngredients[1] = new QuestIngredients { nom = "Wood", UnlockedAtLevel = 1 };
            questIngredients[2] = new QuestIngredients { nom = "Charcoal", UnlockedAtLevel = 5 };
            questIngredients[3] = new QuestIngredients { nom = "IronIngot", UnlockedAtLevel = 7 };
        }

        if (currentQuestIngredients == null || currentQuestIngredients.Length == 0) //first quest
        {
            currentQuestIngredients = new CurrentQuestIngredients[questIngredientList];
            currentQuestIngredients[0] = new CurrentQuestIngredients { nom = "Wood", numberAsked = questIngredientCount };

        }
    }
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

    private void updateUI()
    {
        generateUI();
    }

    private void generateUI()
    {
        closeButton = uiInstance.transform.Find("CloseButton").GetComponent<Button>();
        questButton = uiInstance.transform.Find("QuestButton").GetComponent<Button>();
        rewardTxt = uiInstance.transform.Find("RewardTxt").GetComponent<TMP_Text>();
        questDescriptionTxt = uiInstance.transform.Find("QuestDescriptionTxt").GetComponent<TMP_Text>();



        //button listeners
        closeButton.onClick.AddListener(DestroyUi);
        questButton.onClick.AddListener(CheckQuestCompletion);

        rewardTxt.text = "Reward: " + questRewardAmount + " Gold";
        //on affcihe tout le tableau dans quest description
        questDescriptionTxt.text = "Bring me:\n";
        foreach (var ingredient in currentQuestIngredients)
        {
            questDescriptionTxt.text += "- " + ingredient.numberAsked + " " + ingredient.nom + "\n";
            questDescriptionTxt.text += "\n";
        }
    }

    private void CheckQuestCompletion()
    {
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

    private void NextQuest()
    {
        // Calcul reward et nombre d'ingrédients
        questRewardAmount = (questRewardAmount * NumberOfQuestsCompleted) / 2;
        questIngredientList = Mathf.Max(1, questIngredientList * (NumberOfQuestsCompleted / 2));

        // Filtrer les ingrédients disponibles selon le niveau
        var availableIngredients = questIngredients
            .Where(i => i.UnlockedAtLevel <= NumberOfQuestsCompleted)
            .ToList();

        if (availableIngredients.Count == 0)
        {
            Debug.LogWarning("Aucun ingrédient débloqué !");
            return;
        }

        // Limiter le nombre à la taille de la liste possible
        int ingredientCount = Mathf.Min(questIngredientList, availableIngredients.Count);

        // Reset la liste de la prochaine quête
        currentQuestIngredients = new CurrentQuestIngredients[ingredientCount];

        // Tirer les ingrédients sans doublon
        for (int i = 0; i < ingredientCount; i++)
        {
            int index = UnityEngine.Random.Range(0, availableIngredients.Count + 1);
            var chosen = availableIngredients[index];

            int amount = UnityEngine.Random.Range(10 * NumberOfQuestsCompleted / 2, 20 * NumberOfQuestsCompleted);
            currentQuestIngredients[i] = new CurrentQuestIngredients
            {
                nom = chosen.nom,
                numberAsked = amount
            };

            // Retirer l’ingrédient tiré → empêche doublon
            availableIngredients.RemoveAt(index);
        }

        Debug.Log("Nouvelle quête générée !");
        updateUI();
    }


        
}