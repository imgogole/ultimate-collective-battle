using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    private static EventSystemManager instance;

    void Awake()
    {
        // Assurer qu'il n'y a qu'une seule instance de EventSystemManager dans la scène
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Marquer cette instance comme l'instance unique
        instance = this;

        // Ne pas détruire l'EventSystem lors du chargement d'une nouvelle scène
        DontDestroyOnLoad(gameObject);

        // Vérifier s'il existe déjà un EventSystem dans la scène
        EventSystem existingEventSystem = FindObjectOfType<EventSystem>();
        if (existingEventSystem != null && existingEventSystem.gameObject != gameObject)
        {
            // S'il en existe déjà un, le détruire
            Destroy(existingEventSystem.gameObject);
        }
    }

    void Start()
    {
        // Vous pouvez ajouter des initialisations supplémentaires ici si nécessaire
    }

    void Update()
    {
        // Vous pouvez ajouter des mises à jour supplémentaires ici si nécessaire
    }

    void OnEnable()
    {
        // S'abonner à l'événement de chargement de scène
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Se désabonner de l'événement de chargement de scène
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Vous pouvez ajouter des actions spécifiques à l'arrivée d'une nouvelle scène ici
    }
}

