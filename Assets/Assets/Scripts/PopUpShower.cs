using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalButton
{
    public string Text { get; private set; }
    public System.Action Callback { get; private set; }

    public ModalButton(string text, System.Action callback)
    {
        Text = text;
        Callback = callback;
    }
}

public class PopUpShower : MonoBehaviour
{
    public GameObject popUpPanel;

    public TMP_Text titleText;
    public TMP_Text messageText;
    public Transform buttonContainer;
    public GameObject buttonPrefab;

    private static PopUpShower instance;

    public List<Button> existingButtons = new List<Button>();

    void Awake()
    {
        // Assurer qu'il n'y a qu'une seule instance de PopUpShower dans la scène
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Marquer cette instance comme l'instance unique
        instance = this;

        // Ne pas détruire ModalPopup lors du chargement d'une nouvelle scène
        DontDestroyOnLoad(gameObject);

        Close();
    }

    void ClearButtons()
    {
        foreach (var button in existingButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public static void Show(string title, string message, params ModalButton[] buttons)
    {
        if (instance == null)
        {
            Debug.LogError("ModalPopup instance not found. Make sure to attach the ModalPopup script to a GameObject in the scene.");
            return;
        }

        // Afficher le titre et le message
        instance.titleText.text = title;
        instance.messageText.text = message;

        // Effacer les boutons existants
        instance.ClearButtons();

        // Afficher les nouveaux boutons ou réutiliser les boutons existants
        for (int i = 0; i < buttons.Length; i++)
        {
            Action action = buttons[i].Callback;

            Button button = instance.GetButton(i);
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<TMP_Text>().text = buttons[i].Text;
            button.onClick.RemoveAllListeners(); // Supprimer les écouteurs existants
            button.onClick.AddListener(() => OnButtonClick(action));
            button.interactable = true;
        }

        instance.popUpPanel.SetActive(true);
    }

    Button GetButton(int index)
    {
        return existingButtons[index];
    }

    static void OnButtonClick(System.Action callback)
    {
        // Exécuter l'action de rappel du bouton
        callback?.Invoke();

        GameSettings.Instance.CloseSettingsPanel();

        // Fermer la pop-up après avoir exécuté l'action
        instance.Close();
    }

    void Close()
    {
        // Effacer le titre, le message et les boutons
        titleText.text = "";
        messageText.text = "";
        ClearButtons();

        popUpPanel.SetActive(false);
    }
}
