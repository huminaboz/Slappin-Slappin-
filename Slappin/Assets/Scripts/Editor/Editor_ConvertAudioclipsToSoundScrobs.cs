using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Editor_ConvertAudioclipsToSoundScrobs : MonoBehaviour
{
    //Debug helper scripts
    [MenuItem("Slappin/Sound/Test if folder paths exist")]
    public static void DebugTestFolderPaths()
    {
        string[] testPaths =
        {
            "Assets/Resources/SFXScrobs/",
            "Assets/AudioFiles/SFXs",
            "Assets/Resources/MusicScrobs/",
            "Assets/AudioFiles/Music"
        };
        AllFolderPathsExist(testPaths);
    }


    [MenuItem("Slappin/Sound/Convert New Audioclips (in the SFX and BGM Folders) to AudioScrobs")]
    public static void ConvertAllNewAudioclipsToAudioScrobs()
    {
        int totalNewCreated = 0;
        int totalSkipped = 0;

        ConvertSFXAudioclipsToSoundScrobs<SFXScrob>(ref totalNewCreated, ref totalSkipped);
        ConvertMusicAudioclipsToSoundScrobs<MusicScrob>(ref totalNewCreated, ref totalSkipped);

        Debug.Log($"Created {totalNewCreated}. Skipped {totalSkipped}.");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void ConvertSFXAudioclipsToSoundScrobs<T>(ref int totalNewCreated, ref int totalSkipped)
        where T : AudioScrob
    {
        const string sfxAudioClipsPath = "Assets/AudioFiles/SFXs";
        const string sfxScrobOutputPath = "Assets/Resources/SFXScrobs/";

        string[] inputPath = { sfxAudioClipsPath };
        CreateScrobsOfType<T>(inputPath, sfxScrobOutputPath, ref totalNewCreated, ref totalSkipped);
    }

    private static void ConvertMusicAudioclipsToSoundScrobs<T>(ref int totalNewCreated, ref int totalSkipped)
        where T : AudioScrob
    {
        const string musicAudioClipsPath = "Assets/AudioFiles/Music";
        const string musicScrobOutputPath = "Assets/Resources/MusicScrobs/";

        string[] inputPath = { musicAudioClipsPath };
        CreateScrobsOfType<T>(inputPath, musicScrobOutputPath, ref totalNewCreated, ref totalSkipped);
    }

    private static void CreateScrobsOfType<T>(string[] audioclipsInputPath, string outputPath,
        ref int totalNewCreated, ref int totalSkipped) where T : AudioScrob
    {
        if (!AllFolderPathsExist(audioclipsInputPath))
        {
            Debug.LogWarning("Not all folder paths existed, aborting. Double check your paths.");
            return;
        }

        const string type = "t: audioclip";
        List<string> audioClipsWithExistingSoundScrobs =
            PrepareAListOfAllGUIDsOfClipsWithExistingSoundScrobs<T>();

        string[]
            guids = AssetDatabase.FindAssets(type,
                audioclipsInputPath); //NOTE:: FindAssets is recursive and will go through folders for you
        if (guids.Length < 1)
        {
            Debug.Log($"No sfx found in {audioclipsInputPath[0]}.");
            return;
        }

        foreach (string guid in guids)
        {
            if (string.IsNullOrEmpty(guid)) continue;

            if (audioClipsWithExistingSoundScrobs.Contains(guid))
            {
                totalSkipped++;
                continue;
            }

            CreateScrob<T>(guid, outputPath, ref totalNewCreated);
        }
    }

    private static List<string> PrepareAListOfAllGUIDsOfClipsWithExistingSoundScrobs<T>() where T : AudioScrob
    {
        List<string> result = new();
        string[] guids = AssetDatabase.FindAssets($"t: {typeof(T).ToString()}");

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (asset == null) continue;

            result.Add(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset.clip)));
        }

        return result;
    }

    private static void CreateScrob<T>(string guid, string scrobPath, ref int totalCreated) where T : AudioScrob
    {
        AudioClip clip =
            AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(AudioClip)) as AudioClip;

        if (clip == null) return;
        if (AssetDatabase.FindAssets(clip.name + " SFXScrob_").Length > 0)
        {
            Debug.Log("Already have an AudioScrob for " + clip.name + ", skipping...");
            return;
        }

        T scrob = (T)ScriptableObject.CreateInstance(typeof(T));
        scrob.clip = clip;
        scrob.name = clip.name;
        //if (scrob is MusicScrob musicScrob) musicScrob.looping = true; 

        AssetDatabase.CreateAsset(scrob, scrobPath + "/" + scrob.name + ".asset");
        Debug.Log($"Found new sfx: {scrob.name}");
        totalCreated++;
    }


    private static bool AllFolderPathsExist(string[] testPaths)
    {
        foreach (string testPath in testPaths)
        {
            if (DoesFolderPathExist(testPath))
            {
                Debug.Log(testPath + " \nWhen the above was checked as a valid path it returned: " +
                          DoesFolderPathExist(testPath));
            }
            else
            {
                Debug.LogError(testPath + " \nWhen the above was checked as a valid path it returned: " +
                               DoesFolderPathExist(testPath));
                return false;
            }
        }

        return true;
    }

    private static bool DoesFolderPathExist(string _path)
    {
        // Strip off trailing /s, they'll mess it up
        while (_path.LastIndexOf("/") == _path.Length - 1)
        {
            _path = _path.Substring(0, _path.Length - 1);
        }

        return AssetDatabase.IsValidFolder(_path);
    }
}