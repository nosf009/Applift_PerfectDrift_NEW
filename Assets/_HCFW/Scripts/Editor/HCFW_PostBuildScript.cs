#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
#if UNITY_ANDROID
using UnityEditor.Android;
#endif
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEngine;

public class HCFW_PostBuildScript : MonoBehaviour
{
    static string appleAdmobAppId = "ca-app-pub-3940256099942544~3347511713";
    static string googleAdmobAppId = "ca-app-pub-3940256099942544~3347511713";

#if UNITY_IOS
    [PostProcessBuild]
    static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        // Read plist
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Add Admob mandatory parameter
        PlistElementDict rootDict = plist.root;
        rootDict.SetString("GADApplicationIdentifier", appleAdmobAppId);
        PlistElementArray skadArray = rootDict.CreateArray("SKAdNetworkItems");
        PlistElementDict skadDict = skadArray.AddDict();
        skadDict.SetString("SKAdNetworkIdentifier", "cstr6suwn9.skadnetwork");

        // Remove obsolete stuff, etc.
        var rootValues = rootDict.values;
        rootValues.Remove("UIApplicationExitsOnSuspend");
        // Write plist
        File.WriteAllText(plistPath, plist.WriteToString());

        /* // Won't work on workspaces
        string projectPath = path + "/Unity-iPhone.hxcodeworkspace/project.pbxproj";
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);

        string target = pbxProject.TargetGuidByName("Unity-iPhone");
        pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

        pbxProject.WriteToFile(projectPath);
        */

    }
#elif UNITY_ANDROID
    [PostProcessBuild]
    static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        string manifest = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
        var androidManifest = new AndroidManifest(manifest);
        androidManifest.AdMobData(googleAdmobAppId);
        androidManifest.Save();
    }
#endif
}

internal class AndroidXmlDocument : XmlDocument
{
    private string m_Path;
    protected XmlNamespaceManager nsMgr;
    public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

    public AndroidXmlDocument(string path)
    {
        m_Path = path;
        using (var reader = new XmlTextReader(m_Path))
        {
            reader.Read();
            Load(reader);
        }
        nsMgr = new XmlNamespaceManager(NameTable);
        nsMgr.AddNamespace("android", AndroidXmlNamespace);
    }

    public string Save()
    {
        return SaveAs(m_Path);
    }

    public string SaveAs(string path)
    {
        using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
        {
            writer.Formatting = Formatting.Indented;
            Save(writer);
        }
        return path;
    }
}

internal class AndroidManifest : AndroidXmlDocument
{
    private readonly XmlElement ManifestElement;
    private readonly XmlElement ApplicationElement;

    public AndroidManifest(string path) : base(path)
    {
        ManifestElement = SelectSingleNode("/manifest") as XmlElement;
        ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
    }

    private XmlAttribute CreateAndroidAttribute(string key, string value)
    {
        XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
        attr.Value = value;
        return attr;
    }

    internal XmlNode GetActivityWithLaunchIntent()
    {
        return
            SelectSingleNode(
                "/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and "
                + "intent-filter/category/@android:name='android.intent.category.LAUNCHER']",
                nsMgr);
    }


    internal void AdMobData(string googleAdmobAppId)
    {
        var manifest = SelectSingleNode("/manifest");
        var application = manifest.SelectSingleNode("application");
        var activity = GetActivityWithLaunchIntent() as XmlElement;
        if (activity.GetElementsByTagName("com.google.android.gms.ads.APPLICATION_ID", AndroidXmlNamespace) != null)
        {
            return;
        }
        XmlElement child = CreateElement("meta-data");
        application.AppendChild(child);
        XmlAttribute newAttribute = CreateAndroidAttribute("name", "com.google.android.gms.ads.APPLICATION_ID");
        child.Attributes.Append(newAttribute);
        newAttribute = CreateAndroidAttribute("value", googleAdmobAppId);
        child.Attributes.Append(newAttribute);
    }
}
