using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

// Animates body tracking data and stores Lap_Body_Data.txt into 2-D Array
public class bodyDataVisual : MonoBehaviour
{
    
    private static string streamingAssetsPath = Application.streamingAssetsPath;
    private static string[] bodyComponents = new string[] {"time", "speed", "Run_State", "pos_X", "pos_Y", "pos_Z", "rot_X", "rot_Y", "rot_Z", "Left_Foot_X", "Left_Foot_Y", "Left_Foot_Z", 
                                                            "Left_Ankle_X", "Left_Ankle_Y", "Left_Ankle_Z", "Left_Knee_X", "Left_Knee_Y", "Left_Knee_Z", 
                                                            "Left_Hip_X" , "Left_Hip_Y", "Left_Hip_Z", "Right_Foot_X", "Right_Foot_Y", "Right_Foot_Z", 
                                                            "Right_Ankle_X", "Right_Ankle_Y", "Right_Ankle_Z", "Right_Knee_X", "Right_Knee_Y", "Right_Knee_Z", 
                                                            "Right_Hip_X", "Right_Hip_Y", "Right_Hip_Z", "Spine_Base_X", "Spine_Base_Y", "Spine_Base_Z", "Spine_Mid_X", "Spine_Mid_Y", "Spine_Mid_Z", 
                                                            "Spine_Shoulder_X", "Spine_Shoulder_Y", "Spine_Shoulder_Z", "Neck_X", "Neck_Y", "Neck_Z", "Head_X", "Head_Y", "Head_Z", 
                                                            "Left_Shoulder_X", "Left_Shoulder_Y", "Left_Shoulder_Z", "Left_Elbow_X", "Left_Elbow_Y", "Left_Elbow_Z", 
                                                            "Left_Wrist_X", "Left_Wrist_Y", "Left_Wrist_Z", "Left_Hand_X", "Left_Hand_Y", "Left_Hand_Z", 
                                                            "Left_Hand_Tip_X", "Left_Hand_Tip_Y", "Left_Hand_Tip_Z", "Left_Thumb_X", "Left_Thumb_Y", "Left_Thumb_Z", 
                                                            "Right_Shoulder_X", "Right_Shoulder_Y", "Right_Shoulder_Z", "Right_Elbow_X", "Right_Elbow_Y", "Right_Elbow_Z", 
                                                            "Right_Wrist_X", "Right_Wrist_Y", "Right_Wrist_Z", "Right_Hand_X", "Right_Hand_Y", "Right_Hand_Z", 
                                                            "Right_Hand_Tip_X", "Right_Hand_Tip_Y", "Right_Hand_Tip_Z", "Right_Thumb_X", "Right_Thumb_Y", "Right_Thumb_Z"};
    private string[,] bodyData; // Stores 2-Dimensional array of Lap Body Data
    private List<GameObject> spheres = new List<GameObject>(); // Stores all vector values of body points being tracked by Kinect Camera
    private int counter = 0; // Body Data Vector Elements counter
    private int currFrameIndex = 0; // Index counter of Lap Data

    // Start is called before the first frame updates and must press space to get past initial position
    void Start()
    {
        // Extract body data from txt and into arrays
        // PLACE BODY DATA TXT FILE INTO STREAMING ASSETS AND UPDATE TXT NAME TO ADD YOUR OWN
        var lines = File.ReadAllLines(streamingAssetsPath + "/User1_Back_and_Forth_Lap1_328174.2_07.21.2021_4.16.PM.txt"); // VIDEO WITH JUMPING AND A LOT OF NOISE, more recent, average delay time ~95 .. ~10 FPS
        //var lines = File.ReadAllLines(streamingAssetsPath + "/400_meter.txt"); // SMOOTHER VIDEO, 2 weeks ago video, average delay time ~35 .. ~30 FPS
        Debug.Log("Start of Body Data File Reading");

        // Store Lap Body Data into bodyData 2-dimensional array
        bodyData = new string[bodyComponents.Length, lines.Length]; 
        for(int x = 0; x < lines.Length; x++) { 
            var fields = lines[x].Split(',');
            for(int y = 0; y < fields.Length; y++) { 
                bodyData[y, x] = fields[y]; 
            }
        }

        // for loop captures inital position of body data, creates spheres at vector locations of bodyComponents from Left_Foot to Right_Thumb
        for(int x = 9; x < bodyData.GetLength(0); x = x + 3) {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = new Vector3(Int32.Parse(bodyData[x, currFrameIndex]), Int32.Parse(bodyData[x+1, currFrameIndex]), Int32.Parse(bodyData[x+2, currFrameIndex])); //position of sphere
            sphere.transform.localScale = new Vector3(100, 100, 100); // size of sphere
            sphere.name = bodyComponents[x]; // sphere GameObject name
            spheres.Add(sphere); // add sphere to sphere array
                //bodyData[x][0] // x
                //bodyData[x+1][0] // y 
                //bodyData[x+2][0] // z
        }
        currFrameIndex++;
        
        // Begin animating body data
        StartCoroutine(run()); 
        StopCoroutine(run());
        Application.Quit();
    }

    // run is updated according to the milliseconds of delay in body data, bodyData[0][x] = time
    IEnumerator run()
    {
        Debug.Log("press space to start");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
       
        Debug.Log("running");
        while(true) {

            // break out of loop if end of Lap Body Data txt
            if((bodyData.GetLength(1)-1) < currFrameIndex) {
                Debug.Log("End of Body Data File Reached");
                yield break;
            }

            // for loop captures all body data, moves spheres to new vector locations for all spheres from Left_Foot to Right_Thumb
            for(int x = 9; x < bodyData.GetLength(0); x = x + 3) {
                // 
                spheres[counter].transform.position = new Vector3(Int32.Parse(bodyData[x, currFrameIndex]), Int32.Parse(bodyData[x+1, currFrameIndex]), Int32.Parse(bodyData[x+2, currFrameIndex]));
                    //bodyData[x][0] // x
                    //bodyData[x+1][0] // y 
                    //bodyData[x+2][0] // z
                counter++;
            }
            counter = 0;
            yield return new WaitForSeconds(float.Parse(bodyData[0, currFrameIndex++].Insert(1, "."))); // Convert string to float. First add decimal point. BodyData.txt Data Ex. 0093 seconds convert to 0.090 seconds
        }
    }
}