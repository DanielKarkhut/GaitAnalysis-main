using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Microsoft.Win32.Registry;

public class checkUserFiles : MonoBehaviour
{
    private static string configPath = Application.streamingAssetsPath + "/Configuration";

    private static string currentUserLocation = @"SOFTWARE\XPRience\Users"; 
    private static string usersLocation = @"SOFTWARE\XPRience\Users\";

    //DEFAULT SETTINGS
    private string[] settingNames = { "KinectLocation", "MaxHeight", "MaxSpeed", "TurnSens", "SpeedChangeSens", "ArmStopSens", "SlowDownRate", "SpeedUpRate", "TurnRate" }; //"SceneNum", "MaxFPS", "VideoQuality", "Weight" };
    private float[] defaultSettings = { 0, 32f, 7f, 0.1f, 0.35f, 0.6f, 40f, 10f, 100f };

    private string[] sceneKeys = { "Bots", "SavedBots", "PresetLocations", "SavedLocations"};
    public int numberOfScenes = 12;
    // Start is called before the first frame update
    void Start()
    {
        //Check if the current User exists
        checkUserKeys();
        checkUserPhotoFolders();
    }

    private void createSceneKeys(RegistryKey sceneKey)
    {
        for (int j = 0; j < sceneKeys.Length; j++)
            sceneKey.CreateSubKey(sceneKeys[j]);
    }

    //Gets the Current username
    private string GetCurrentUser()
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey(currentUserLocation);
        if(key == null)
        {
            key = Registry.CurrentUser.CreateSubKey(currentUserLocation);
        }

        string userName = "Guest";
        object o = key.GetValue("UserName", null);

        if(o != null)
        {
            userName = o.ToString();
        }

        key.Close();
        return userName;
    }

    private void checkUserKeys()
    {
        //Check if the current User exists
        string userName = GetCurrentUser();
        RegistryKey newUser = Registry.CurrentUser.OpenSubKey(usersLocation + userName, true);
        if (newUser != null) //user Exists
        {
            //check if the user settings exist if not make them
           // RegistryKey settingsKey = newUser.OpenSubKey(@"\UserSettings", true);
            if (newUser.GetValueNames().Length != settingNames.Length + 3) //settings + age, height, weight
            {
                for (int i = 0; i < defaultSettings.Length; i++)
                    newUser.SetValue(settingNames[i], defaultSettings[i]);
            }
            else
            {
                Debug.Log("Settings Exists");
            }

            //checks if all the scene keys exist
            for (int i = 0; i <= numberOfScenes; i++)
            {
                RegistryKey sceneKey = newUser.OpenSubKey(@"\Scene" + i, true);
                if (sceneKey == null)
                {
                    sceneKey = newUser.CreateSubKey(@"\Scene" + i, true);
                    createSceneKeys(sceneKey);
                }
                else
                {
                    Debug.Log("Scene" + i + " Exists");

                    //checks if all proper keys exist within Scene + i                   
                    foreach(string key in sceneKeys) 
                    {
                        RegistryKey sceneSubFolderKey = sceneKeys.OpenSubKey(key, true); //opens bots, savebots, presetlocations, savedlocations
                        if(sceneSubFolderKey == null)
                        {
                            sceneSubFolderKey = sceneKeys.CreateSubKey(key, true); //creates Bots, Saved Bots, PresetLocations, and/or SavedLocations if does not already exist 
                        }
                    }
                    
                    sceneSubFolderKey.Close();
                    
                }
                sceneKey.Close();
            }

            //settingsKey.Close();
            Debug.Log("User Already Exists");
        }
        else //user doesn't exist so make it and all subkeys
        {
            newUser = Registry.CurrentUser.CreateSubKey(usersLocation + userName, true);
            //RegistryKey settingsKey = newUser.CreateSubKey(@"\UserSettings", true);
            for (int j = 0; j < defaultSettings.Length; j++)
                newUser.SetValue(settingNames[j], defaultSettings[j]);
            for (int i = 0; i <= numberOfScenes; i++)
            {
                RegistryKey sceneKey = newUser.CreateSubKey(@"\Scene" + i, true);
                createSceneKeys(sceneKey);
                sceneKey.Close();
            }
            Debug.Log("Created User: " + userName);
        }
        newUser.Close();
    }


    private static string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    private string[] photoFolderNames = { "SavedScreenshots", "PresetScreenshots" };
    private void checkUserPhotoFolders()
    {
        string userName = GetCurrentUser();

        DirectoryInfo userPhotos = new DirectoryInfo(picturesPath + "/" + userName);
        if (userPhotos.Exists)
        {
            checkPhotoSubFolders(userPhotos);
        }
        else
        {
            userPhotos.Create();
            checkPhotoSubFolders(userPhotos);
        }
    }


    private bool foundFolder = false;
    private void checkPhotoSubFolders(DirectoryInfo userPhotos)
    {
        for (int i = 0; i < photoFolderNames.Length; i++)
        {
            foreach(DirectoryInfo dinfo in userPhotos.GetDirectories())
            {
                if(dinfo.Name == photoFolderNames[i])
                {
                    foundFolder = true;
                    break;
                }
            }
            if(!foundFolder)
                userPhotos.CreateSubdirectory(photoFolderNames[i]);

            foundFolder = false;
        }
    }
}
