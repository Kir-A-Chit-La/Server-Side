using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public class DBManager : MonoBehaviour
{
    /// result format #1:   Success|ID|Username|Salt|Password
    /// result format #2:   Failed: {error}
    public static DBManager instance;
    [SerializeField]
    private string url = "http://localhost/database";
    private string path;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
        path = Application.persistentDataPath;
    }

    public IEnumerator Register(int _fromClient, string username)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        using (UnityWebRequest www = UnityWebRequest.Post(url + "/register.php", form))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string[] _formatedHandler = www.downloadHandler.text.Split('|');
                if(_formatedHandler[0] == "Success")
                {
                    ServerSend.RegistrationResult(_fromClient, true, Int32.Parse(_formatedHandler[1]), _formatedHandler[2], Int32.Parse(_formatedHandler[3]));
                }
                else
                {
                    ServerSend.RegistrationResult(_fromClient, false);
                }
            }
        }
    }

    public IEnumerator Verificate(int _fromClient, int id, string salt, string hash)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("salt", salt);
        form.AddField("hash", hash);
        using (UnityWebRequest www = UnityWebRequest.Post(url + "/verificate.php", form))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string[] _formatedHandler = www.downloadHandler.text.Split('|');
                if(_formatedHandler[0] == "Success")
                {
                    ServerSend.VerificationResult(_fromClient, true);
                }
                else
                {
                    Debug.Log(_formatedHandler[1]);
                    ServerSend.VerificationResult(_fromClient, false);
                }
            }
        }
    }

    public IEnumerator ChangeAvatar(int _fromClient, int id, int avatar)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("avatar", avatar);
        using (UnityWebRequest www = UnityWebRequest.Post(url + "/change_avatar.php", form))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string[] _formatedHandler = www.downloadHandler.text.Split('|');
                if(_formatedHandler[0] == "Failed")
                {
                    Debug.Log(_formatedHandler[1]);
                }
            }
        }
    }

    public IEnumerator CreateCharacter(int _fromClient, int id, string name, string gender, string spawnPos, string spawnRot)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("name", name);
        form.AddField("gender", gender);
        form.AddField("spawnPos", spawnPos);
        form.AddField("spawnRot", spawnRot);
        using (UnityWebRequest www = UnityWebRequest.Post(url + "/create_character.php", form))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string[] _formatedHandler = www.downloadHandler.text.Split('|');
                if(_formatedHandler[0] == "Success")
                {
                    ServerSend.CharacterCreationResult(_fromClient, true, Int32.Parse(_formatedHandler[1]), name, gender, Client.spawnPosition, Quaternion.Euler(Client.spawnRotation));
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    ServerSend.CharacterCreationResult(_fromClient, false, 0, "", "", Vector3.zero, Quaternion.identity);
                }
            }
        }
    }

    public IEnumerator GetAccountData(int _fromClient, int id, string username)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("username", username);
        string file_path = Path.Combine(path, "data.dat");
        using (UnityWebRequest www = UnityWebRequest.Post(url + "/get_account_data.php", form))
        {
            www.downloadHandler = new DownloadHandlerFile(file_path);
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string[] _formatedHandler = File.ReadAllText(file_path).Split('|');
                if(_formatedHandler[0] =="Success")
                {
                    Debug.Log(File.ReadAllText(file_path));
                    int _avatar = Int32.Parse(_formatedHandler[1]);
                    bool _isVerificated = Boolean.Parse(_formatedHandler[2]);
                    int _charCount = Int32.Parse(_formatedHandler[3]);
                    if(_charCount > 0)
                    {
                        Dictionary<int, Account.Character> characters = new Dictionary<int, Account.Character>();
                        for(int i = 0; i < _charCount; i++)
                        {
                            int _id = Int32.Parse(_formatedHandler[4+5*i]);
                            characters.Add(_id, new Account.Character(_id, _formatedHandler[5+5*i], _formatedHandler[6+5*i], JsonUtility.FromJson<Vector3>(_formatedHandler[7+5*i]), JsonUtility.FromJson<Quaternion>(_formatedHandler[8+5*i])));
                        }
                        ServerSend.AccountData(_fromClient, true, _avatar, _isVerificated, _charCount, characters);
                        Server.clients[_fromClient].account = new Account(id, username, _avatar, _isVerificated, characters);
                    }
                    else
                    {
                        ServerSend.AccountData(_fromClient, true, _avatar, _isVerificated, _charCount);
                    }
                }
                else
                {
                    Debug.Log(File.ReadAllText(file_path));
                    ServerSend.AccountData(_fromClient, false, 0, false, 0);
                }
            }
        }
    }

    public IEnumerator LogInRequest(int _fromClient, string username)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        using (UnityWebRequest www = UnityWebRequest.Post(url + "/get_salt.php", form))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string[] _formatedHandler = www.downloadHandler.text.Split(' ');
                if(_formatedHandler[0] == "Success")
                {
                    ServerSend.AccountSalt(_fromClient, true, _formatedHandler[1]);
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    ServerSend.AccountSalt(_fromClient, false);
                }
            }
        }
    }

    public IEnumerator LogIn(int _fromClient, string username, string hash)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("hash", hash);
        string file_path = Path.Combine(path, "login.dat");
        using (UnityWebRequest www = UnityWebRequest.Post(url + "/login.php", form))
        {
            www.downloadHandler = new DownloadHandlerFile(file_path);
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string[] _formatedHandler = File.ReadAllText(file_path).Split('|');
                if(_formatedHandler[0] == "Success")
                {
                    Debug.Log(File.ReadAllText(file_path));
                    int id = Int32.Parse(_formatedHandler[1]);
                    int avatar = Int32.Parse(_formatedHandler[2]);
                    int charCount = Int32.Parse(_formatedHandler[3]);
                    if(charCount > 0)
                    {
                        Dictionary<int, Account.Character> characters = new Dictionary<int, Account.Character>();
                        for(int i = 0; i < charCount; i++)
                        {
                            int _id = Int32.Parse(_formatedHandler[4+5*i]);
                            characters.Add(_id, new Account.Character(_id, _formatedHandler[5+5*i], _formatedHandler[6+5*i], JsonUtility.FromJson<Vector3>(_formatedHandler[7+5*i]), JsonUtility.FromJson<Quaternion>(_formatedHandler[8+5*i])));
                        }
                        ServerSend.LogInResult(_fromClient, true, id, avatar, charCount, characters);
                        Server.clients[_fromClient].account = new Account(id, username, avatar, true, characters);
                    }
                    else
                    {
                        ServerSend.LogInResult(_fromClient, true, id, avatar, charCount);
                    }
                }
                else
                {
                    Debug.Log(File.ReadAllText(file_path));
                    ServerSend.LogInResult(_fromClient, false, 0, 0, 0);
                }
            }
        }
    }
}
