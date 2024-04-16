using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.IO;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum AsyncState {None, Ask, Progress, Success, Error}

public class DownloadImageToFile : MonoBehaviour
{
    private string currentPath;
    public TextAsset textAsset;
    private CloudImages cloudImages;
    
    public AsyncState state = AsyncState.None;
    private string isError = "";
    private int countLinks = 0;
    
    public GameObject currentActive;
    private GameObject childAsk;
    private GameObject childProgress;
    private GameObject childSuccess;
    private GameObject childError;

    void Awake()
    {
        //Escoge la plataforma que se va a usar
        currentPath = CameraTransition.CreateJsonInAllPaths("DriveImages.json", textAsset);

        childAsk = transform.GetChild(0).gameObject;
        childProgress = transform.GetChild(1).gameObject;
        childSuccess = transform.GetChild(2).gameObject;
        childError = transform.GetChild(3).gameObject;
    }

    public static string FromShareToDownload(string url)
    {
        //Compartir enlace: https://drive.google.com/file/d/FILE_ID/view?usp=drive_link
        //Descargar imagen: https://drive.google.com/uc?export=download&id=FILE_ID
        string fileID = url.Substring(32, url.Length-52);
        return "https://drive.google.com/uc?export=download&id=" + fileID;
    }
    
    public void GetDriveJSON()
    {
        StartCoroutine(RetrieveData());
    }

    private IEnumerator RetrieveData()
    {        
        string jsonDriveShare = "https://drive.google.com/file/d/17b_s9InsrGkfJn8ozPQqm2iVMu0zqYUF/view?usp=drive_link";
        UnityWebRequest www = UnityWebRequest.Get(FromShareToDownload(jsonDriveShare));
        
        UpdateAsyncState(AsyncState.Progress);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"WebRequest: {www.error}");
            UpdateAsyncState(AsyncState.Error, www.error);
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);
            string folder = (Application.isMobilePlatform ? Application.persistentDataPath : Application.dataPath) + "/Drive";
            cloudImages = JsonUtility.FromJson<CloudImages>(www.downloadHandler.text);

            if (cloudImages.Ciclos.Count != cloudImages.Horarios.Count)
            {
                isError = "Error: Las imagenes de ciclos y horarios no son iguales";
                Debug.Log(isError);
            }

            StartCoroutine(GetDrivePNG(cloudImages.Sky, folder, "sky horario.png"));
            for(int i=0; i < cloudImages.Ciclos.Count; i++)
            {
                StartCoroutine(GetDrivePNG(cloudImages.Ciclos[i], folder, $"ciclo examenes {i+1}.png"));
                StartCoroutine(GetDrivePNG(cloudImages.Horarios[i], folder, $"horario {i+1}.png"));
            }
            yield return new WaitUntil( () => countLinks >= cloudImages.CountLinks() );
        }
        
        if (isError != string.Empty) UpdateAsyncState(AsyncState.Error, isError);
        else UpdateAsyncState(AsyncState.Success);
        
        isError = string.Empty;
        Debug.Log("     HHHEEECHCHOOO");
        www.Dispose();
    }

    private IEnumerator GetDrivePNG(string url, string path, string fileName)
    {
        UnityWebRequest www = UnityWebRequest.Get(FromShareToDownload(url));
        yield return www.SendWebRequest();
        
        if (isError == string.Empty && countLinks < cloudImages.CountLinks())
        {
            if (www.result != UnityWebRequest.Result.Success)
            {
                isError = www.error;
                Debug.Log($"PNG WebRequest: {isError}");
                countLinks = cloudImages.CountLinks();
            }
            else
            {
                Debug.Log("            Facilito el " + fileName);
                File.WriteAllBytes(path + "/" + fileName, www.downloadHandler.data);
                countLinks++;
            }
        }
        www.Dispose();
    }

    public void SetAsyncStateToDefault()
    {
        UpdateAsyncState(AsyncState.Ask);
        //Lo pongo aqui porque en la Coroutine principal hace que se descargue todo aunque haya error,
        //esto es por el WaitUntil que puse en caso de error
        //Por que hago tanto para el caso de un error si es una app simple, solo mia y que no puede salir mal?
        countLinks = 0;
    }

    private void UpdateAsyncState(AsyncState newState, string message = "")
    {
        state = newState;
        currentActive.SetActive(false);
        switch(newState)
        {
            case AsyncState.Ask:
                childAsk.SetActive(true);
                currentActive = childAsk;
            break;

            case AsyncState.Progress:
                childProgress.SetActive(true);
                currentActive = childProgress;
            break;

            case AsyncState.Success:
                childSuccess.SetActive(true);
                currentActive = childSuccess;
            break;

            case AsyncState.Error:
                childError.SetActive(true);
                currentActive = childError;

                childError.GetComponent<Text>().text += "\n" + message;
            break;
        }
    }
}

[System.Serializable]
public class CloudImages
{
    public string Sky;
    public List<string> Ciclos;
    public List<string> Horarios;

    public int CountLinks()
    {
        return 1 + Ciclos.Count + Horarios.Count;
    }
}