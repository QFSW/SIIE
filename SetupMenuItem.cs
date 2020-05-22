using System.Collections.Generic;
using System.IO;
using UnityEditor;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

#if UNITY_EDITOR
public static class SetupMenuItem
{
    [MenuItem("QFSW/SSIE/Setup")]
    public static void Setup()
    {
        string text =
            File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "ProjectSettings/TagManager.asset"));
        text = text.Replace("%YAML 1.1", "");
        text = text.Replace("%TAG !u! tag:unity3d.com,2011:", "");
        text = text.Replace("--- !u!78 &1", "");
        var input = new StringReader(text);

        var deserializer = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();

        var obj = deserializer.Deserialize<Dictionary<string, object>>(input);
        var tagManager = obj["TagManager"] as Dictionary<object, object>;
        if (tagManager != null)
        {
            List<object> layers = tagManager["layers"] as List<object>;
            if (layers != null && !layers.Contains("SelectableInversion"))
            {
                layers.Add("SelectableInversion");

                var serializer = new SerializerBuilder().Build();
                var yaml = serializer.Serialize(obj);

                File.WriteAllLines(Path.Combine(Directory.GetCurrentDirectory(), "ProjectSettings/TagManager.asset"),
                    new[] {"%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n--- !u!78 &1", yaml});

                EditorUtility.DisplayDialog("SSIE", "Layer has been added to your project.", "Okay");
            }
            else
            {
                EditorUtility.DisplayDialog("SSIE", "Layer has already been added to your project.", "Okay");   
            }
           
        }
    }
}
#endif
