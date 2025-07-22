using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageGallery : MonoBehaviour
{
    public Image currentImage; // La imagen grande que muestra el sprite
    public TextMeshProUGUI currentText;
    public float typingSpeed;
    public List<Sprite> imageList; // Lista de imágenes a mostrar
    public List<string> textList;
    public Button nextButton;
    public Button prevButton;

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
