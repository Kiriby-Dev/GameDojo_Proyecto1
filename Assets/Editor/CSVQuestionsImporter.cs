using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVQuestionsImporter
{
    [MenuItem("Tools/Importar Preguntas desde CSV")]
    public static void ImportQuestions()
    {
        string path = EditorUtility.OpenFilePanel("Seleccionar archivo CSV", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++) // salta la cabecera
        {
            string[] values = lines[i].Split(',');

            if (values.Length < 6)
            {
                Debug.LogWarning($"Línea {i + 1} incompleta.");
                continue;
            }

            QuestionData newQuestion = ScriptableObject.CreateInstance<QuestionData>();
            newQuestion.question = values[0];
            newQuestion.correctAnswer = values[1];
            newQuestion.wrongAnswers = new string[] { values[2], values[3], values[4] };
            if (System.Enum.TryParse(values[5], true, out QuestionData.Difficulty diff))
            {
                newQuestion.difficulty = diff;
            }

            string fileName = $"Pregunta_{i}_{newQuestion.difficulty}";
            string assetPath = $"Assets/Resources/Questions/{fileName}.asset";

            AssetDatabase.CreateAsset(newQuestion, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Preguntas importadas correctamente.");
    }
}
