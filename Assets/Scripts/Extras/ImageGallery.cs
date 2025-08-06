using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageGallery : MonoBehaviour
{
    [SerializeField] private Image currentImage; // La imagen grande que muestra el sprite
    [SerializeField] private TextMeshProUGUI currentText;
    [SerializeField] private TextMeshProUGUI currentPhaseText;
    [SerializeField] private float typingSpeed;
    [SerializeField] private List<Sprite> imageList; // Lista de imágenes a mostrar
    [SerializeField] private List<string> textList;
    [SerializeField] private List<string> phaseTextList;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    private int _currentIndex = 0;

    void Start()
    {
        nextButton.onClick.AddListener(NextImage);
        prevButton.onClick.AddListener(PreviousImage);
    }

    void NextImage()
    {
        _currentIndex = (_currentIndex + 1) % imageList.Count;
        UpdateImage();
    }

    void PreviousImage()
    {
        _currentIndex = (_currentIndex - 1 + imageList.Count) % imageList.Count;
        UpdateImage();
    }

    void UpdateImage()
    {
        if (imageList.Count > 0 && currentImage && currentText)
        {
            currentImage.sprite = imageList[_currentIndex];
            currentPhaseText.text = phaseTextList[_currentIndex];
            StopAllCoroutines();
            StartCoroutine(TextCoroutine());
        }
    }

    private IEnumerator TextCoroutine()
    {
        currentText.text = "";
        string fullText = textList[_currentIndex];

        foreach (char c in fullText)
        {
            currentText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
