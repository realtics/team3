using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class JsonStreamer : MonoBehaviour
{
    public T Load<T>(string path)
    {
        string json = null;
        string allpath = pathForDocumentsFile(path);

        if (File.Exists(allpath))
        {
            FileStream file = new FileStream(allpath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);
            json = sr.ReadLine();

            sr.Close();
            file.Close();
        }

        return JsonUtility.FromJson<T>(json);
    }

    public void Save(object myObject, string path)
    {
        string str = JsonUtility.ToJson(myObject);


        string allPath = pathForDocumentsFile(path);
        FileStream file = new FileStream(allPath, FileMode.Create, FileAccess.Write);

        StreamWriter sw = new StreamWriter(file);
        sw.WriteLine(str);

        sw.Close();
        file.Close();
    }

    string pathForDocumentsFile(string filename)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(Path.Combine(path, "Documents"), filename);
        }

        else if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }

        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
    }
}
