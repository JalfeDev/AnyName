using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExamsScheduleChanger : MonoBehaviour
{
    Text text;  //Texto de "Ciclo 1"
    public int index;
    public int maxIndex;
    public Button leftButton;
    public Button rightButton;
    
    [Tooltip("Esto es para acceder a los png locales")]
    public GetLocalImages localImages;

    public RawImage examScroll;

    void Start()
    {
        text = GetComponent<Text>();
        //Ahora sera en funcion a la cantidad de imagenes, PlayerPrefs.GetInt("IndexSemester") - 1; //Empiezan desde el 1 hasta el 3, este es desde el 0 hasta el 2
        maxIndex = PlayerPrefs.GetInt("NumCyclesImages");
        index = maxIndex - 1;
        UpdateButtons();
    }

    public void ChangeExam(int add)
    {
        index += add;
        localImages.DownloadOneExam(index); //Esto es por el metodo "Descarga 1 to 1"
        PlayerPrefs.SetInt("IndexSemester", index + 1);
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        examScroll.texture = localImages.tex[index];
        
        text.text = $"Ciclo {index+1}";
        
        if (index == 0)
        {
            leftButton.interactable = false;
            rightButton.interactable = true;
        }
        else if (index == maxIndex-1)//(index == localImages.tex.Length-1)
        {
            rightButton.interactable = false;
            leftButton.interactable = true;
        }
        else
        {
            leftButton.interactable = true;
            rightButton.interactable = true;
        }
    }
}