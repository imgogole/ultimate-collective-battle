using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSettings : MonoBehaviour
{
    [Header("General")]

    public List<GameObject> settingsPanels = new List<GameObject>();

    [Header("General")]

    public Button surrenderButton;
    public Button leaveButton;

    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public TMP_Dropdown maxFpsDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fullscreenDropdown;

    public GameObject settingsPanel;
    public GameObject keyBindingPanel;
    public List<Button> keyBindingButtons;

    private float masterVolume;
    private float sfxVolume;
    private float musicVolume;
    private int maxFPS;
    private int resolutionIndex;
    private int fullscreen;

    private bool isSettingsOpened;
    private int resolutionsCountChoice;

    [Header("Key bindings")]

    private KeyCode leftKey = KeyCode.Q;
    private KeyCode rightKey = KeyCode.D;
    private KeyCode jumpKey = KeyCode.Z;
    private KeyCode leftAttackKey = KeyCode.Mouse0;
    private KeyCode rightAttackKey = KeyCode.Mouse1;
    private KeyCode activeSpellKey = KeyCode.Alpha2;
    private KeyCode ultimateSpellKey = KeyCode.Alpha1;
    private KeyCode itemKey = KeyCode.Alpha3;

    public static KeyCode LeftKey => Instance.leftKey;
    public static KeyCode RightKey => Instance.rightKey;
    public static KeyCode JumpKey => Instance.jumpKey;
    public static KeyCode LeftAttackKey => Instance.leftAttackKey;
    public static KeyCode RightAttackKey => Instance.rightAttackKey;
    public static KeyCode ActiveSpellKey => Instance.activeSpellKey;
    public static KeyCode UltimateSpellKey => Instance.ultimateSpellKey;
    public static KeyCode ItemKey => Instance.itemKey;

    public static float SFXVolume => instance.masterVolume * instance.sfxVolume;
    public static float MusicVolume => instance.masterVolume * instance.musicVolume;

    private static GameSettings instance;
    public static GameSettings Instance => instance;

    public static bool IsOpen => Instance.isOpen;

    private bool isOpen;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializeDropdowns();
        LoadSettings();
        CloseSettingsPanel();
        isSettingsOpened = false;

        keyBindingPanel.SetActive(false);

        InitializeKeyBindingsButtons();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !ReliableUIManager.Instance.IsLoading)
        {
            isSettingsOpened = !isSettingsOpened;
            if (isSettingsOpened)
            {
                OpenSettingsPanel();
            }
            else
            {
                CloseSettingsPanel();
            }
        }
    }

    public void SetPanel(int Index)
    {
        foreach (GameObject panel in settingsPanels) panel.SetActive(false);
        settingsPanels[Index].SetActive(true);
    }

    public void OpenSettingsPanel()
    {
        SetPanel(0);
        settingsPanel.SetActive(true);
        surrenderButton.gameObject.SetActive(SceneManager.GetActiveScene().buildIndex != 0);
        leaveButton.gameObject.SetActive(SceneManager.GetActiveScene().buildIndex != 0);
        isOpen = true;
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        isOpen = false;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetInt("MaxFPS", maxFpsDropdown.value);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenDropdown.value);
        
        LoadSettings();
    }

    private void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 50f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 50f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 50f);
        maxFPS = PlayerPrefs.GetInt("MaxFPS", 2); // Default to 60 FPS
        resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutionsCountChoice - 1); // Default to the first resolution
        fullscreen = PlayerPrefs.GetInt("Fullscreen", 1); // Default to fullscreen

        masterVolumeSlider.SetValueWithoutNotify(masterVolume);
        sfxVolumeSlider.SetValueWithoutNotify(sfxVolume);
        musicVolumeSlider.SetValueWithoutNotify(musicVolume);
        maxFpsDropdown.SetValueWithoutNotify(maxFPS);
        resolutionDropdown.SetValueWithoutNotify(resolutionIndex);
        fullscreenDropdown.SetValueWithoutNotify(fullscreen);


        ApplySettings();
    }

    public void ResetSettings()
    {
        // Remet les paramètres par défaut
        masterVolumeSlider.value = 50f;
        sfxVolumeSlider.value = 50f;
        musicVolumeSlider.value = 50f;
        maxFpsDropdown.value = 2; // Default to 60 FPS
        resolutionDropdown.value = resolutionsCountChoice - 1; // Default to the maximum resolution
        fullscreenDropdown.value = 1; // Default to fullscreen

        // Sauvegarde et applique les paramètres
        SaveSettings();
    }

    private void InitializeDropdowns()
    {
        maxFpsDropdown.ClearOptions();
        resolutionDropdown.ClearOptions();
        fullscreenDropdown.ClearOptions();

        // Populate Max FPS Dropdown
        maxFpsDropdown.AddOptions(new List<string> { "30", "60", "90", "120", "150", "180", "210", "240" });

        // Populate Resolution Dropdown without duplicates
        Resolution[] resolutions = Screen.resolutions;
        List<string> resolutionOptions = new List<string>();
        HashSet<string> uniqueResolutions = new HashSet<string>();

        foreach (Resolution resolution in resolutions)
        {
            string resolutionString = resolution.width + "x" + resolution.height;

            // Avoid duplicates
            if (uniqueResolutions.Add(resolutionString))
            {
                resolutionOptions.Add(resolutionString);
            }
        }

        resolutionsCountChoice = resolutionOptions.Count;
        resolutionDropdown.AddOptions(resolutionOptions);

        // Populate Fullscreen Dropdown
        fullscreenDropdown.AddOptions(new List<string> { "Off", "On" });
    }

    private int GetTargetFPS(int dropdownValue)
    {
        int[] fpsOptions = { 30, 60, 90, 120, 150, 180, 210, 240 };
        return fpsOptions[dropdownValue];
    }

    public void ApplySettings()
    {
        // Applique les paramètres au jeu (excluant les volumes)
        ApplyResolution();
        ApplyFullscreen();
        ApplyMaxFramerate();

        AudioManager.AdjustVolume(sfxVolume * masterVolume, musicVolume * masterVolume);
    }

    private void ApplyResolution()
    {
        Resolution resolution = Screen.resolutions[Mathf.Min(resolutionIndex * 2, Screen.resolutions.Length -1)];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void ApplyFullscreen()
    {
        Screen.fullScreen = (fullscreen == 1);
    }

    private void ApplyMaxFramerate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = GetTargetFPS(maxFPS);
    }

    public void SaveKeyBindings()
    {
        PlayerPrefs.SetString("LeftKey", leftKey.ToString());
        PlayerPrefs.SetString("RightKey", rightKey.ToString());
        PlayerPrefs.SetString("JumpKey", jumpKey.ToString());
        PlayerPrefs.SetString("LeftAttackKey", leftAttackKey.ToString());
        PlayerPrefs.SetString("RightAttackKey", rightAttackKey.ToString());
        PlayerPrefs.SetString("ActiveSpellKey", activeSpellKey.ToString());
        PlayerPrefs.SetString("UltimateSpellKey", ultimateSpellKey.ToString());
        PlayerPrefs.SetString("ItemKey", itemKey.ToString());
    }

    private void LoadKeyBindings()
    {
        leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LeftKey", "Q"));
        rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RightKey", "D"));
        jumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("JumpKey", "Z"));
        leftAttackKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LeftAttackKey", "Mouse0"));
        rightAttackKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RightAttackKey", "Mouse1"));
        activeSpellKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("ActiveSpellKey", "Alpha2"));
        ultimateSpellKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("UltimateSpellKey", "Alpha1"));
        itemKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("ItemKey", "Alpha3"));
    }

    private void InitializeKeyBindingsButtons()
    {
        LoadKeyBindings();

        for (int i = 0; i < keyBindingButtons.Count; i++)
        {
            int index = i;
            keyBindingButtons[i].onClick.AddListener(() => AskKeyBindingChange(index));
        }

        UpdateKeyBindingsButtons();
    }

    private void UpdateKeyBindingsButtons()
    {
        for (int i = 0; i < keyBindingButtons.Count; i++)
        {
            keyBindingButtons[i].GetComponentInChildren<TMP_Text>().text = GetKeyBindingText(i);
        }
    }

    private string GetKeyBindingText(int index)
    {
        switch (index)
        {
            case 0: return KeyName(leftKey.ToString());
            case 1: return KeyName(rightKey.ToString());
            case 2: return KeyName(jumpKey.ToString());
            case 3: return KeyName(leftAttackKey.ToString());
            case 4: return KeyName(rightAttackKey.ToString());
            case 5: return KeyName(activeSpellKey.ToString());
            case 6: return KeyName(ultimateSpellKey.ToString());
            case 7: return KeyName(itemKey.ToString());
            default: return "";
        }
    }

    private void AskKeyBindingChange(int index)
    {
        keyBindingPanel.SetActive(true);
        StartCoroutine(WaitForKey(index));
    }

    private IEnumerator WaitForKey(int index)
    {
        KeyCode newKey = KeyCode.None;

        while (newKey == KeyCode.None)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    newKey = keyCode;
                    break;
                }
            }

            yield return null;
        }

        SetKeyBinding(index, newKey);
        SaveKeyBindings();

        keyBindingPanel.SetActive(false);
        UpdateKeyBindingsButtons();
    }

    private void SetKeyBinding(int index, KeyCode newKey)
    {
        switch (index)
        {
            case 0: leftKey = newKey; break;
            case 1: rightKey = newKey; break;
            case 2: jumpKey = newKey; break;
            case 3: leftAttackKey = newKey; break;
            case 4: rightAttackKey = newKey; break;
            case 5: activeSpellKey = newKey; break;
            case 6: ultimateSpellKey = newKey; break;
            case 7: itemKey = newKey; break;
        }
    }

    public static string KeyName(string name)
    {
        switch (name)
        {
            case "Mouse0":
                return "Left click";
            case "Mouse1":
                return "Right click";
            case "Mouse2":
                return "Scroll Wheel";
            case "KeypadPeriod":
                return "Dot";
            case "KeypadDivide":
                return "/";
            case "KeypadMultiply":
                return "*";
            case "KeypadMinus":
                return "-";
            case "KeypadPlus":
                return "+";
            case "KeypadEnter":
                return "Enter";
            case "KeypadEquals":
                return "=";
            case "Hash":
                return "#";
            case "Percent":
                return "%";
            case "Ampersand":
                return "&";
            case "Asterisk":
                return "*";
            case "LeftBracket":
                return "[";
            case "RightBracket":
                return "]";
            case "LeftParen":
                return "(";
            case "RightParen":
                return ")";
            case "At":
                return "@";
            case "Alpha0":
                return "0";
            case "Alpha1":
                return "1";
            case "Alpha2":
                return "2";
            case "Alpha3":
                return "3";
            case "Alpha4":
                return "4";
            case "Alpha5":
                return "5";
            case "Alpha6":
                return "6";
            case "Alpha7":
                return "7";
            case "Alpha8":
                return "8";
            case "Alpha9":
                return "9";
            default:
                return name;
        }
    }
}

