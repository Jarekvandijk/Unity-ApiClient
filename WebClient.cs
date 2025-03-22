using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    //public string baseUrl;
    public string baseUrl = "https://localhost:7033";
    //private string token = "CfDJ8AJ_WZu6gi9NsOVGoUTbngE768lOjZ66J0fP-WfZ8HIbqIY0DGumewejzkpIpFR6Rumf6Ya0NG8hcnzalbh-6TZpl-RPpfS-PJ0CO9zINMwhwEZko2GJ5wfLJhQKTLQpFAHCOr2u-1UmF5--cOBaHWILC7eTUC9_tIk0KMI50AgiSCWfjgeBCkbYf4YwHDwBsdQknG_cSWW90Ww-u_hgVrXQHunH4SHaPY0_Qwe3wr_r0TzlKlhUBGVZg8Tg9UzU_p3ulNZlhio7S9TJo6NBz0IPl93kLU7PCi0M7uCQNjH7MqSN35JRvj2yLJ0PTOfRKXfaP0O924Hk-wVglK8uCSDsiF5LA_G8vKI5pFqBdaN5S1nfhawWyvYMyKrfI3QLCPLyNlga9M_KhWJt4VP6DtRGWpOjm6Ve9yk_VOYx9dNaiXNZPHCUZS7aTpIWwuehsNDWIlIOidtR-04NbfolUWfL0_BmhJa6cMOL3RAeIHVl-03FpvXxGkkSl5A0aIXfvjSblXnOwGimoFCx4SuPE6LSyXLE20NRwtuWW6pSYQuu1RD-Pj514zeTF5BpI7-vcRxVERJeRz93Ukuot5jazxgOtXWgNDYNnM0n9rXvrvbC7W1v6hxWNQt7MnmJ0PCvJ2hqzm2tdWypZl2tsm2NwhppTRPxQ-PYv_1ixB7ph-XhJDQG0J37sDiz4Qva4e8PUKbHKRW4Qng5g-tk9XJLgic";
    private string token;

    public void SetToken(string token)
    {
        this.token = token;
        Debug.Log("Token set: " + token);
    }
    public async Awaitable<IWebRequestReponse> SendGetRequest(string route)
    {
        UnityWebRequest webRequest = CreateWebRequest("GET", route, "");
        return await SendWebRequest(webRequest);
    }

    public async Awaitable<IWebRequestReponse> SendGetRequest(string route, string data)
    {
        UnityWebRequest webRequest = CreateWebRequest("GET", route, data);
        return await SendWebRequest(webRequest);
    }

    public async Awaitable<IWebRequestReponse> SendPostRequest(string route, string data)
    {
        UnityWebRequest webRequest = CreateWebRequest("POST", route, data);
        return await SendWebRequest(webRequest);
    }
    public async Awaitable<IWebRequestReponse> SendPutRequest(string route, string data)
    {
        UnityWebRequest webRequest = CreateWebRequest("PUT", route, data);
        return await SendWebRequest(webRequest);
    }

    public async Awaitable<IWebRequestReponse> SendDeleteRequest(string route)
    {
        UnityWebRequest webRequest = CreateWebRequest("DELETE", route, "");
        return await SendWebRequest(webRequest);
    }

    private UnityWebRequest CreateWebRequest(string type, string route, string data)
    {
        string url = baseUrl + route;
        Debug.Log("Creating " + type + " request to " + url + " with data: " + data);

        data = RemoveIdFromJson(data); // Backend throws error if it receiving empty strings as a GUID value.
        var webRequest = new UnityWebRequest(url, type);
        byte[] dataInBytes = new UTF8Encoding().GetBytes(data);
        webRequest.uploadHandler = new UploadHandlerRaw(dataInBytes);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        AddToken(webRequest);
        return webRequest;
    }

    private async Awaitable<IWebRequestReponse> SendWebRequest(UnityWebRequest webRequest)
    {
        await webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                string responseData = webRequest.downloadHandler.text;
                return new WebRequestData<string>(responseData);
            default:
                return new WebRequestError(webRequest.error);
        }
    }
 
    private void AddToken(UnityWebRequest webRequest)
    {
        webRequest.SetRequestHeader("Authorization", "Bearer " + token);
    }

    private string RemoveIdFromJson(string json)
    {
        return json.Replace("\"id\":\"\",", "");
    }
}

[Serializable]
public class Token
{
    public string tokenType;
    public string accessToken;
}
