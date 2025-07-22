using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageGallery : MonoBehaviour
{
    public Image currentImage; // La imagen grande que muestra el sprite
    public List<Sprite> imageList; // Lista de imágenes a mostrar
    public Button nextButton;
    public Button prevButton;

    private int currentIndex = 0;

    void Start()
    {
        UpdateImage();

        nextButton.onClick.AddListener(NextImage);
        prevButton.onClick.AddListener(PreviousImage);
    }

    void NextImage()
    {
        currentIndex = (currentIndex + 1) % imageList.Count;
        UpdateImage();
    }

    void PreviousImage()
    {
        currentIndex = (currentIndex - 1 + imageList.Count) % imageList.Count;
        UpdateImage();
    }

    void UpdateImage()
    {
        if (imageList.Count > 0 && currentImage != null)
        {
            currentImage.sprite = imageList[currentIndex];
        }
    }
}
