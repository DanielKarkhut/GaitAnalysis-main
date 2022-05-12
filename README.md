# GaitAnalysis-main/Assets/Scripts/bodyDataVisual.cs
 Takes Kinect text file body data, stores body data into 2-D array, creates an animation of each tracked point from front and side view, press space to start. Used to check whether other tracking features like step counter and jump checker are working properly. Useful for fine-tuning parameters and perfecting body tracking.

# GaitAnalysis-main/Assets/Scripts/CaloriesBurned.cs
 Takes user weight, distance traveled, age, and twenty second resting heart beat and returns a value for estimated calories burned based on online calorie burn calculator. 

# GaitAnalysis-main/Assets/Scripts/PaceStrideTracker.cs
 Takes Kinect text file body data and uses a Z-score function to calculate when each step is made and stores how many steps the user takes for both their right and left leg. Approach explained in Report.pdf.

# GaitAnalysis-main/Assets/Scripts/checkUserFiles.cs
  Accesses windows registry keys to ensure validity of user data folder tree. Creates folders if any are missing and ensures recorded run data is properly stored in the data structure.
