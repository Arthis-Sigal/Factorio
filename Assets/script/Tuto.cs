using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tuto : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutoPanel;
    public Button nextButton;
    public Button prevButton;
    public Image image;
    public TMP_Text text;

    [System.Serializable]
    private class TutoPage
    {
        public string imageName;
        public string text;
    }

    private TutoPage[] tutoList;
    private int currentPage = 0;

    void Start()
    {
        //Creation Tuto pages
        tutoList = new TutoPage[]
        {
            new TutoPage { imageName = "Keyboard", text = "Put your mouse on a building and press E to interact." },
            new TutoPage { imageName = "Quest", text = "Quest building: check it to know your next quest." },
            new TutoPage { imageName = "Shop", text = "Buy new buildings here." },
            new TutoPage { imageName = "BuildingMode", text = "In build mode, place buildings from your inventory. Press E on one to remove it." }
        };

        //Turn On panel
        tutoPanel.SetActive(true);

        //Give event to button
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);

        //Display first page
        UpdateTutoPage();
    }

    private void UpdateTutoPage()
    {
        if (tutoList == null || tutoList.Length == 0)
            return;

        // Load Image
        Sprite sprite = Resources.Load<Sprite>($"Tuto/{tutoList[currentPage].imageName}");
        if (sprite != null)
            image.sprite = sprite;
        else
            Debug.LogWarning($"Image introuvable : Tuto/{tutoList[currentPage].imageName}");

        // Update Text
        text.text = tutoList[currentPage].text;

        prevButton.interactable = currentPage > 0;

        // If last page, change next to finish
        TMP_Text nextButtonText = nextButton.GetComponentInChildren<TMP_Text>();
        if (nextButtonText != null)
            nextButtonText.text = (currentPage == tutoList.Length - 1) ? "Finish" : "Next";
    }

    private void NextPage()
    {
        if (currentPage < tutoList.Length - 1)
        {
            currentPage++;
            UpdateTutoPage();
        }
        else
        {
            // LastPage, close tuto
            CloseTuto();
        }
    }

    private void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateTutoPage();
        }
    }

    private void CloseTuto()
    {
        tutoPanel.SetActive(false);
    }
}
