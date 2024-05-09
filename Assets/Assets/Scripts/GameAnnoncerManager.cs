using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameAnnoncerManager : MonoBehaviour
{
    public Color allyColor, enemyColor, neutralColor;

    public GameObject killPanel;
    public Image authorKill, victimKill;
    public TMP_Text killReason;
    public Image killBackground;

    public GameObject doorOpenedPanel;
    public Image authorDoorOpen;
    public TMP_Text doorOpenReason;
    public Image doorBackground;

    public CanvasGroup canvasGroup;



    private static GameAnnoncerManager instance;
    public static GameAnnoncerManager Instance => instance;

    private Queue<GameAnnoncement> gameAnnoncements = new Queue<GameAnnoncement>();
    bool showAnnoncement = false;

    private void Awake()
    {
        instance = this;
        showAnnoncement = false;
        HideAndClear();
    }

    public void Hide()
    {
        killPanel.SetActive(false);
        doorOpenedPanel.SetActive(false);
    }

    public void HideAndClear()
    {
        StopAllCoroutines();

        killPanel.SetActive(false);
        doorOpenedPanel.SetActive(false);

        gameAnnoncements = new Queue<GameAnnoncement>();
    }


    public void AddEvent(AnnoncementType type, Entity author, Entity target = null)
    {
        GameAnnoncement annoncement = new GameAnnoncement(type, author, target);
        gameAnnoncements.Enqueue(annoncement);
    }

    private void Update()
    {
        if (gameAnnoncements.Count != 0 && !showAnnoncement)
        {
            showAnnoncement = true;

            Dictionary<string, GameAnnoncement> latestAnnoncements = new Dictionary<string, GameAnnoncement>();
            while (gameAnnoncements.Count > 0)
            {
                GameAnnoncement currentAnnoncement = gameAnnoncements.Dequeue();
                string key = $"{currentAnnoncement.author.BaseChampion.ID}_{currentAnnoncement.type}";
                latestAnnoncements[key] = currentAnnoncement;
            }
            foreach (var kvp in latestAnnoncements)
            {
                gameAnnoncements.Enqueue(kvp.Value);
            }

            StartCoroutine(Coroutine_ShowAnnoncement(gameAnnoncements.Dequeue()));
        }
    }

    IEnumerator Coroutine_ShowAnnoncement(GameAnnoncement gameAnnoncement)
    {
        bool isKill = gameAnnoncement.type == AnnoncementType.Kill;
        bool isDoorOpened = gameAnnoncement.type == AnnoncementType.DoorOpened;

        killPanel.SetActive(isKill);
        doorOpenedPanel.SetActive(isDoorOpened);

        bool isEnemy = ClientManager.Instance.IsEnemy(gameAnnoncement.author);

        if (isKill)
        {
            authorKill.sprite = gameAnnoncement.author.BaseChampion.Icon;
            victimKill.sprite = gameAnnoncement.target.BaseChampion.Icon;

            killReason.text = GetStreak(gameAnnoncement.author, isEnemy);
            killBackground.color = isEnemy ? enemyColor : allyColor;
        }
        else
        {
            authorDoorOpen.sprite = gameAnnoncement.author.BaseChampion.Icon;

            doorOpenReason.text = isEnemy ? "Porte alliée ouverte" : "Porte ennemie ouverte";
            doorBackground.color = isEnemy ? enemyColor : allyColor;
        }

        yield return FadeCanvasGroup(canvasGroup, 0f, 1f, 0.5f);
        yield return new WaitForSeconds(4f);
        yield return FadeCanvasGroup(canvasGroup, 1f, 0f, 0.5f);

        killPanel.SetActive(false);
        doorOpenedPanel.SetActive(false);

        yield return null;

        showAnnoncement = false;
    }

    private string GetStreak(Entity author, bool isEnemy)
    {
        int killStreak = author.KillStreak;
        if (killStreak == 1)
        {
            if (!isEnemy)
            {
                return "Ennemi tué";
            }
            else
            {
                return "Allié tué";
            }
        }
        else if (killStreak == 2)
        {
            if (!isEnemy)
            {
                return "Doublé";
            }
            else
            {
                return "Doublé ennemi";
            }
        }
        else if (killStreak == 3)
        {
            if (!isEnemy)
            {
                return "Triplé";
            }
            else
            {
                return "Triplé ennemi";
            }
        }
        else if (killStreak == 4)
        {
            if (!isEnemy)
            {
                return "Quadruplé";
            }
            else
            {
                return "Quadruplé ennemi";
            }
        }
        else if (killStreak >= 5)
        {
            if (!isEnemy)
            {
                return "Tuerie";
            }
            else
            {
                return "Tuerie ennemi";
            }
        }
        return "a tué";
    }

    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float targetAlpha, float duration)
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            canvasGroup.alpha = alpha;

            elapsedTime = Time.time - startTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}

public struct GameAnnoncement
{
    public AnnoncementType type;
    public Entity author;
    public Entity target;

    public GameAnnoncement(AnnoncementType type, Entity author, Entity target)
    {
        this.type = type;
        this.author = author;
        this.target = target;
    }
}

public enum AnnoncementType
{
    Kill,
    DoorOpened
}