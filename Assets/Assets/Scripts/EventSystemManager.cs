using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    private static EventSystemManager instance;

    void Awake()
    {
        // Assurer qu'il n'y a qu'une seule instance de EventSystemManager dans la sc�ne
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Marquer cette instance comme l'instance unique
        instance = this;

        // Ne pas d�truire l'EventSystem lors du chargement d'une nouvelle sc�ne
        DontDestroyOnLoad(gameObject);

        // V�rifier s'il existe d�j� un EventSystem dans la sc�ne
        EventSystem existingEventSystem = FindObjectOfType<EventSystem>();
        if (existingEventSystem != null && existingEventSystem.gameObject != gameObject)
        {
            // S'il en existe d�j� un, le d�truire
            Destroy(existingEventSystem.gameObject);
        }
    }

    void Start()
    {
        // Vous pouvez ajouter des initialisations suppl�mentaires ici si n�cessaire
    }

    void Update()
    {
        // Vous pouvez ajouter des mises � jour suppl�mentaires ici si n�cessaire
    }

    void OnEnable()
    {
        // S'abonner � l'�v�nement de chargement de sc�ne
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Se d�sabonner de l'�v�nement de chargement de sc�ne
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Vous pouvez ajouter des actions sp�cifiques � l'arriv�e d'une nouvelle sc�ne ici
    }
}

