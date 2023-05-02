using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Figma
{
    public class FigmaAutoLayout : EditorWindow
    {
        private string _fileKey = "XAyX5PfYRlphBi0g4tUDv3";
        private File _file;
        private const string Token = "figd_vxMGa2vWYQLYuwvIcig6p0s4u0O3XT0kiU0ua8f-";
        private const string URL = "https://api.figma.com/v1/files/";

        [MenuItem("FigmaAutoLayout/Show")]
        public static void ShowWindow()
        {
            var window = (FigmaAutoLayout)GetWindow(typeof(FigmaAutoLayout));
            window.Show();
        }

        private void OnGUI()
        {
            _fileKey = EditorGUILayout.TextField(label: "File key", text: _fileKey);

            if (GUILayout.Button(text: "Get File"))
            {
                GetFile();
            }
        }
        private void GetFile()
        {
            var request = WebRequest.Create(URL + _fileKey);
            request.Headers["X-Figma-Token"] = Token;

            var response = request.GetResponse();
            string json = "";
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        json += line;
                    }

                }
            }

            Debug.Log(json);
            _file = JsonConvert.DeserializeObject<File>(json);
            response.Close();
        }
        private void CreatePrefab()
        {
            var frame = _file.document.children[0].children[0];

            var frameObj = new GameObject(name = frame.name);
            var frameRect = frameObj.AddComponent<RectTransform>();

            frameRect.pivot = new Vector2(x:0f, y:1f);
            frameRect.anchorMin = new Vector2(x: 0f, y: 1f);
            frameRect.anchorMax = new Vector2(x: 0f, y: 1f);
            frameRect.anchoredPosition = Vector2.zero;
            frameRect.sizeDelta = new Vector2(x: frame.absoluteBoundingBox.width, y: frame.absoluteBoundingBox.height);

            foreach(var child in frame.children)
                {
                    CreateLayer(child, frame, frameRect);            
                }

            PrefabUtility.SaveAsPrefabAsset(frameObj, assetPath:$"Assets/{frame.name}.prefab");
            }
        private void CreateLayer(Layer layer, Layer frame, Transform parent)
        {
            var obj = new GameObject(name = layer.name);
            var rect = obj.AddComponent<RectTransform>();

            var x = layer.absoluteBoundingBox.x - frame.absoluteBoundingBox.x;
            var y = frame.absoluteBoundingBox.y - layer.absoluteBoundingBox.y;

            rect.pivot = new Vector2(x: 0f, y: 1f);
            rect.anchorMin = new Vector2(x: 0f, y: 1f);
            rect.anchorMax = new Vector2(x: 0f, y: 1f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(x:layer.absoluteBoundingBox.width, y: layer.absoluteBoundingBox.height);

            rect.SetParent(parent, worldPositionStays:true);

            if(layer.children == null || layer.children.Length == 0)
                    return;
            
            foreach (var child in layer.children)
                {
                CreateLayer(child, frame, rect);
            }
        }
    }
}
