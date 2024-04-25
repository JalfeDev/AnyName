using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GetLocalImages : MonoBehaviour
{
    public enum ImageType {Sky, Horario, Examenes}
    public ImageType type;
    public RawImage rawImageSky;

    int numCycles;
    string routeLocal;

    [Tooltip("Aqui se guardaran las imagenes locales")]
    public Texture2D[] tex;
    [Tooltip("Esto son los datos de una imagen local")]
    private byte[] byteTexture;

    void Awake()
    {
        routeLocal = (Application.isMobilePlatform) ? Application.persistentDataPath : Application.dataPath + "/Drive";
        numCycles = PlayerPrefs.GetInt("NumCyclesImages");
        //tex = new Texture2D[numCycles];
        
        switch(type)
        {
            case ImageType.Sky:
                byteTexture = File.ReadAllBytes(routeLocal + "/sky horario.png");
                tex[0] = new Texture2D(32,32);
                Debug.Log("Sky: " + ImageConversion.LoadImage(tex[0], byteTexture, true).ToString());
                rawImageSky.texture = tex[0];
            break;

            case ImageType.Horario:
                for (int i=0; i < numCycles; i++)
                {
                    byteTexture = File.ReadAllBytes(routeLocal + $"/horario {i+1}.png");
                    tex[i] = new Texture2D(128,64);
                    Debug.Log($"Horario {i+1} Start: " + ImageConversion.LoadImage(tex[i], byteTexture, true).ToString());
                }
            break;

            case ImageType.Examenes:
                DownloadOneExam(numCycles-1);
            break;
        }
    }

    //Esto es el "Primero cargas uno y cargas otro con los botones", FUNCIONA
    public void DownloadOneExam(int index)
    {
        byteTexture = File.ReadAllBytes(routeLocal + $"/ciclo examenes {index+1}.jpg");
        tex[index] = new Texture2D(32,32);
        Debug.Log($"Examenes {index+1}: " + ImageConversion.LoadImage(tex[index], byteTexture, true).ToString());
    }


    //Esto es el "Primero cargas uno y luego el resto", no funciona tanto
    // private void Start()
    // {
    //     if (type == ImageType.Examenes)
    //     {
    //         for (int i=0; i < numCycles-1; i++)
    //         {
    //             byteTexture = File.ReadAllBytes(routeLocal + $"/ciclo examenes {i+1}.jpg");
    //             tex[i] = new Texture2D(32,32);
    //             Debug.Log($"Examenes {i+1}: " + ImageConversion.LoadImage(tex[i], byteTexture, true).ToString());
    //         }
    //     }
    // }
}
