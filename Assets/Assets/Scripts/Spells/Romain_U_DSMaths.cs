using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Romain_U_DSMaths : MonoBehaviour
{
    public string Message;
    public Outline outline;

    public GameObject PaperPanel;

    private bool _IsDSOpen;
    private bool _IsDSTrigger;

    public int MatriceSize = 3;

    public List<List<int>> algebricMatrix = new List<List<int>>();
    public List<TMP_Text> textElements = new List<TMP_Text>();
    public TMP_InputField TextField;

    public bool IsDSOpen => _IsDSOpen;
    public bool IsDSTrigger => _IsDSTrigger;

    private static Romain_U_DSMaths instance;
    public static Romain_U_DSMaths Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ClosePaper();
        TriggerPaper(false);
        GenerateMatrice();
    }

    private void Update()
    {
        TriggerCast();

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log($"Determinant : {CalculateDeterminant(algebricMatrix)}");
        }
    }

    public void OnMouseEnter()
    {
        OnCursorEnterPaper();
    }

    public void OnMouseExit()
    {
        OnCursorExitPaper();
    }

    public void OnCursorEnterPaper()
    {
        TriggerPaper(!IsDSOpen);
    }

    public void OnCursorExitPaper()
    {
        TriggerPaper(false);
    }

    private void TriggerCast()
    {
        if (!ClientManager.Instance.Me().IsAbleToControl) return;

        if (IsDSTrigger && Input.GetMouseButtonDown(0))
        {
            OpenPaper();
            TriggerPaper(false);
        }
    }

    private void TriggerPaper(bool IsTriggered)
    {
        if (IsTriggered) UIManager.Instance.ShowStatInfo(Message);
        else UIManager.Instance.CloseStatInfo();
        outline.enabled = IsTriggered;
        _IsDSTrigger = IsTriggered;
    }

    public void OpenPaper()
    {
        if (GameManager.Instance.InRound)
        {
            GenerateMatrice();
            PaperPanel.SetActive(true);
        }
    }

    public void ClosePaper()
    {
        PaperPanel.SetActive(false);
    }

    public void CheckAnswer()
    {
        string AnswerGiven = TextField.text;
        int DetGiven = int.Parse(AnswerGiven);
        bool isDet = CheckDeterminant(DetGiven);

        if (isDet)
        {
            Entity me = ClientManager.Instance.Me();
            me.Teleport(GameManager.Instance.TeamSpawnOutLobby);

            UIManager.Instance.ShowGeneralInfo(GameManager.Instance.ClosedRoomGoodAnswer);
        }
        else
        {
            UIManager.Instance.ShowGeneralInfo(GameManager.Instance.ClosedRoomWrongAnswer);
        }

        ClosePaper();
    }

    void GenerateMatrice()
    {
        algebricMatrix = new List<List<int>>();
        for (int i = 0; i < MatriceSize; i++)
        {
            List<int> row = new List<int>();
            for (int j = 0; j < MatriceSize; j++)
            {
                // Ajouter des nombres aléatoires à la matrice
                row.Add(Random.Range(0, 4));
            }
            algebricMatrix.Add(row);
        }
        UpdateMatrice();
    }

    void UpdateMatrice()
    {
        for (int i = 0; i < MatriceSize; i++)
        {
            for (int j = 0; j < MatriceSize; j++)
            {

                int index = i * MatriceSize + j;

                textElements[index].text = algebricMatrix[i][j].ToString();
            }
        }
    }

    public bool CheckDeterminant(int value)
    {
        // Calculer le déterminant de la matrice
        // (Cette partie doit être complétée en fonction de votre propre logique pour calculer le déterminant)
        int determinant = CalculateDeterminant(algebricMatrix);
        Debug.Log($"Determinant of matrice : {determinant}");

        // Vérifier si la valeur correspond au déterminant
        return value == determinant;
    }

    int CalculateDeterminant(List<List<int>> matrix)
    {
        int size = matrix.Count;

        if (size == 1)
        {
            return matrix[0][0];
        }

        if (size == 2)
        {
            return matrix[0][0] * matrix[1][1] - matrix[0][1] * matrix[1][0];
        }

        int det = 0;

        for (int i = 0; i < size; i++)
        {
            det += matrix[0][i] * Cofactor(matrix, 0, i);
        }

        return det;
    }

    int Cofactor(List<List<int>> matrix, int row, int col)
    {
        int size = matrix.Count;

        List<List<int>> minor = new List<List<int>>();

        for (int i = 1; i < size; i++)
        {
            List<int> currentRow = new List<int>();
            for (int j = 0; j < size; j++)
            {
                if (j != col)
                {
                    currentRow.Add(matrix[i][j]);
                }
            }
            minor.Add(currentRow);
        }

        int sign = (row + col) % 2 == 0 ? 1 : -1;

        return sign * CalculateDeterminant(minor);
    }
}
