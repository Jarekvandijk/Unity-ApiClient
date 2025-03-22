using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ExampleApp : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> prefabObjects;
    private static bool isFirstWorldCreated = false;

    [Header("Test data")]
    public User user;
    public Environment2D environment2D;
    public Object2D object2D;

    [Header("Dependencies")]
    public UserApiClient userApiClient;
    public WebClient webClient;
    public Environment2DApiClient enviroment2DApiClient;
    public Object2DApiClient object2DApiClient;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField NameInput;
    public TMP_InputField MaxLengthInput;
    public TMP_InputField MaxHeightInput;
    public TMPro.TextMeshProUGUI LabelPassword;
    public TMPro.TextMeshProUGUI LabelNameEnvironment;
    public TMPro.TextMeshProUGUI LabelEnvironmentMaximum;
    public GameObject loginPanel;

    [SerializeField] private string usernameString;
    [SerializeField] private string passwordString;
    [SerializeField] private string NameEnvironmentString;
    [SerializeField] private string MaxLengthEnvironmentString;
    [SerializeField] private string MaxHeightEnvironmentString;

    private int passwordlength;
    private int lowercase;
    private int uppercase;
    private int number;
    private int nonAlpha;
    private bool passwordUsable;

    private int nameLength;
    private bool nameUsable;
    private int environmentCount = 0;


    #region Login

    [ContextMenu("User/Register")]

    public void InputFieldChanged()
    {
        passwordlength = 0;
        lowercase = 0;
        uppercase = 0;
        number = 0;
        nonAlpha = 0;
        passwordUsable = false;
        usernameString = usernameInput.text;
        passwordString = passwordInput.text;
        foreach (char check in passwordString)
        {
            if (char.IsWhiteSpace(check))
            {
                LabelPassword.text = "geen spaties toegestaan!";
                return;
            }
            passwordlength++;
            if (char.IsLower(check))
            {
                lowercase++;
            }
            else if (char.IsUpper(check))
            {
                uppercase++;
            }
            else if (char.IsNumber(check))
            {
                number++;
            }
            else
            {
                nonAlpha++;
            }
        }
        if (passwordlength < 10)
        {
            LabelPassword.text = "wachtwoord moet minimaal 10 karakters bevatten!";
            return;
        }
        if (lowercase == 0 || uppercase == 0 || number == 0 || nonAlpha == 0)
        {
            LabelPassword.text = "wachtwoord moet minimaal een kleine letter, grote letter, cijfer en een niet-alphanumeriek karakter bevatten!";
        }
        else
        {
            passwordUsable = true;
            LabelPassword.text = "";
        }
    }


    public void LoginButton()
    {
        if (passwordUsable)
        {
            user.email = usernameString;
            user.password = passwordString;
            Debug.Log(usernameString);
            Debug.Log(passwordString);
        }
        else { return; }
        Login();
    }

    public void CreateAccountButton()
    {
        if (passwordUsable)
        {
            user.email = usernameString;
            user.password = passwordString;
            Debug.Log(usernameString);
            Debug.Log(passwordString);
        }
        Register();
    }

public async void Register()
    {
        IWebRequestReponse webRequestResponse = await userApiClient.Register(user);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                Debug.Log("Register succes!");
                // TODO: Handle succes scenario;/
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Register error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    [ContextMenu("User/Login")]
    public async void Login()
    {
        IWebRequestReponse webRequestResponse = await userApiClient.Login(user);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                Debug.Log("Login succes!");
                LabelPassword.text = "Succesvol ingelogd, maak een wereld om te starten!";

                //userApiClient.SetAccessToken(dataResponse.Data);
                AddBearerToken();

                SceneManager.LoadScene("EnvironmentMaker");
                // TODO: Todo handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Login error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    #endregion

    #region Environment

    [ContextMenu("Environment2D/Read all")]
    public async void ReadEnvironment2Ds()
    {
        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.ReadEnvironment2Ds();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:
                List<Environment2D> environment2Ds = dataResponse.Data;
                Debug.Log("List of environment2Ds: ");
                environment2Ds.ForEach(environment2D => Debug.Log(environment2D.id));
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Read environment2Ds error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public void CreateWorldButton()
    {
        NameEnvironmentString = NameInput.text;
        MaxLengthEnvironmentString = MaxLengthInput.text;
        MaxHeightEnvironmentString = MaxHeightInput.text;

        if (environmentCount >= 5)
        {
            LabelEnvironmentMaximum.text = "Maximaal aantal werelden (5) bereikt!";
            Debug.Log("Maximaal aantal werelden (5) bereikt!");
            return;
        }

        if (nameUsable)
        {
            environment2D.name = NameEnvironmentString;
            environment2D.maxLength = float.Parse(MaxLengthEnvironmentString);
            environment2D.maxHeight = float.Parse(MaxHeightEnvironmentString);

            Debug.Log(NameEnvironmentString);
            Debug.Log(MaxLengthEnvironmentString);
            Debug.Log(MaxHeightEnvironmentString);

            Debug.Log("Access Token: " + userApiClient.AccessToken);
            CreateEnvironment2D();
        }
        else
        {
            Debug.Log("De wereldnaam is niet geldig.");
        }
    }

    [ContextMenu("Environment2D/CreateEnvironment")]
    public async void CreateEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.CreateEnvironment(environment2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Environment2D> dataResponse:
                environment2D.id = dataResponse.Data.id;
                Debug.Log("Create environment2D succes!");
                environmentCount++;
                if (!isFirstWorldCreated)
                {
                    isFirstWorldCreated = true;
                    SceneManager.LoadScene("Wereld 1");
                }
                LabelEnvironmentMaximum.text = $"Aantal werelden: {environmentCount}/5";
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Create environment2D error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }


    //[ContextMenu("Environment2D/CreateEnvironment")]
    //public async void CreateEnvironment2D()
    //{
    //    NameEnvironmentString = NameInput.text;
    //    MaxLengthEnvironmentString = MaxLengthInput.text;
    //    MaxHeightEnvironmentString = MaxHeightInput.text;

    //    if (environmentCount >= 5)
    //    {
    //        LabelEnvironmentMaximum.text = "Maximaal aantal werelden (5) bereikt!";
    //        Debug.Log("Maximaal aantal werelden (5) bereikt!");
    //        return;
    //    }

    //    if (nameUsable) 
    //    {
    //        environment2D.name = NameEnvironmentString;
    //        environment2D.maxLength = float.Parse(MaxLengthEnvironmentString);
    //        environment2D.maxHeight = float.Parse(MaxHeightEnvironmentString);

    //        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.CreateEnvironment(environment2D);

    //        switch (webRequestResponse)
    //        {
    //            case WebRequestData<Environment2D> dataResponse:
    //                environment2D.id = dataResponse.Data.id;
    //                Debug.Log("Create environment2D succes!");
    //                environmentCount++;
    //                if (!isFirstWorldCreated)
    //                {
    //                    isFirstWorldCreated = true;
    //                    SceneManager.LoadScene("Wereld 1");
    //                }
    //                LabelEnvironmentMaximum.text = $"Aantal werelden: {environmentCount}/5";
    //                // TODO: Handle succes scenario.
    //                break;
    //            case WebRequestError errorResponse:
    //                string errorMessage = errorResponse.ErrorMessage;
    //                Debug.Log("Create environment2D error: " + errorMessage);
    //                // TODO: Handle error scenario. Show the errormessage to the user.
    //                break;
    //            default:
    //                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("De wereldnaam is niet geldig.");
    //    }
    //}

    [ContextMenu("Environment2D/Delete")]
    public async void DeleteEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await enviroment2DApiClient.DeleteEnvironment(environment2D.id);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Delete environment error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public void NameEnvironmentChanged()
    {
        nameLength = 0;
        nameUsable = false;
        NameEnvironmentString = NameInput.text;

        if (string.IsNullOrWhiteSpace(NameEnvironmentString))
        {
            LabelNameEnvironment.text = "De naam van de wereld mag niet leeg zijn!";
            return;
        }

        nameLength = NameEnvironmentString.Length;

        if (nameLength < 1 || nameLength > 25)
        {
            LabelNameEnvironment.text = "De naam van de wereld moet tussen de 1 en 25 karakters bevatten!";
            return;
        }

        nameUsable = true;
        LabelNameEnvironment.text = "";
    }

    public void OpenEnvironmentMaker()
    {
        SceneManager.LoadScene("EnvironmentMaker");
    }


    private void AddBearerToken()
    {
        //var webClient = FindAnyObjectByType<WebClient>();
        webClient.SetToken(userApiClient.AccessToken);
        var response = webClient.SendGetRequest("/Environment2D"); //  /environments
    }

    #endregion Environment

    #region Object2D

    //[ContextMenu("Object2D/Read all")]
    //public async void ReadObject2Ds()
    //{
    //    IWebRequestReponse webRequestResponse = await object2DApiClient.ReadObject2Ds(object2D.environmentId);

    //    switch (webRequestResponse)
    //    {
    //        case WebRequestData<List<Object2D>> dataResponse:
    //            List<Object2D> object2Ds = dataResponse.Data;
    //            Debug.Log("List of object2Ds: " + object2Ds);
    //            object2Ds.ForEach(object2D => Debug.Log(object2D.id));
    //            // TODO: Succes scenario. Show the enviroments in the UI
    //            GameObject prefab = prefabObjects.Find(p => p.name == obj.prefabId);
    //            Vector3 position = new Vector3(object2D.positionX, object2D.positionY, 0);
    //            Instantiate(prefab, position, Quaternion.identity);

    //            break;
    //        case WebRequestError errorResponse:
    //            string errorMessage = errorResponse.ErrorMessage;
    //            Debug.Log("Read object2Ds error: " + errorMessage);
    //            // TODO: Error scenario. Show the errormessage to the user.
    //            break;
    //        default:
    //            throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
    //    }
    //}

    private Dictionary<string, GameObject> instantiatedObjects = new Dictionary<string, GameObject>();

    [ContextMenu("Object2D/Read all")]
    public async void ReadObject2Ds()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.ReadObject2Ds(object2D.environmentId);

        switch (webRequestResponse)
        {
            case WebRequestData<List<Object2D>> dataResponse:
                List<Object2D> object2Ds = dataResponse.Data;
                Debug.Log("List of object2Ds: ");

                foreach (var obj in object2Ds)
                {
                    Debug.Log(obj.id);

                    // Check of het object al bestaat in de dictionary
                    if (instantiatedObjects.ContainsKey(obj.id))
                    {
                        Debug.Log($"Object met ID '{obj.id}' bestaat al, wordt niet opnieuw geplaatst.");
                        continue; // Skip duplicaten
                    }

                    // Zoek de juiste prefab
                    GameObject prefab = prefabObjects.Find(p => p.name == obj.prefabId);

                    if (prefab != null)
                    {
                        Vector3 position = new Vector3(obj.positionX, obj.positionY, 0);
                        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);

                        // Voeg het object toe aan de dictionary
                        instantiatedObjects[obj.id] = newObject;
                    }
                    else
                    {
                        Debug.LogWarning($"Prefab met ID '{obj.prefabId}' niet gevonden!");
                    }
                }
                break;

            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Read object2Ds error: " + errorMessage);
                break;

            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    [ContextMenu("Object2D/Create")]
    public async void CreateObject2D()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.CreateObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Object2D> dataResponse:
                object2D.id = dataResponse.Data.id;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Create Object2D error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    [ContextMenu("Object2D/Update")]
    public async void UpdateObject2D()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.UpdateObject2D(object2D);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Update object2D error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }


    #endregion

}
