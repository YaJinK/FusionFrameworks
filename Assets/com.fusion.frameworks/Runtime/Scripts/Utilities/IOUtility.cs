
using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

namespace Fusion.Frameworks.Utilities
{
    public class IOUtility
    {
        public static void Write(string output, string value)
        {
            StreamWriter fileSW = new StreamWriter(output);
            fileSW.Write(value);
            fileSW.Close();
        }

        public static string Read(string input)
        {
            FileInfo fileInfo = new FileInfo(input);
            System.Uri uri = new System.Uri(fileInfo.FullName);
            UnityWebRequest webRequest = UnityWebRequest.Get(uri);
            UnityWebRequestAsyncOperation requestAOp = webRequest.SendWebRequest();
            while (requestAOp.isDone == false)
            {
            }
            if (!(webRequest.result == UnityWebRequest.Result.ConnectionError) && !(webRequest.result == UnityWebRequest.Result.ProtocolError))
            {
                return webRequest.downloadHandler.text;
            } else
            {
                return null;
            }
        }
    }
}
