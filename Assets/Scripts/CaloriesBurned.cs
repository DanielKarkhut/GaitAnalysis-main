using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Formula found here
// http://www.shapesense.com/fitness-exercise/calculators/running-calorie-burn-calculator.shtml

public class CaloriesBurned : MonoBehaviour
{
    private int CB; // Calories Burned
    //public int G; // Grade of running surface, i.e. -10 for 10% grade down
    public List<int> G = new List<int>(); // History of grade of running surface, i.e. -10 for 10% grade down
    public int WKG; // Weight, kilograms
    public double DRK; // Distance Run, kilometers
    private double CFF; // Cardiorespiratory Fitness Factor
    private double TF = 0; // Treadmill Factor, set at 0 because running on treadmill, no air resistance is factored in 
    private double GFactor1; // used for calories formula, taken from website
    private double GFactor2; // used for calories formula, taken from website
    private int userAge; // Age of user
    private double MHR; // Maximum heart rate (beats/minute)
    private double RHR; // Resting heart rate (beats/minute)
    private double restingHeartRate; // 20 Second Resting Heart Rate
    private double VO2max; // Maximum oxygen consumption (in mL*kg^-1*min^-1)
    
    // Constructor
    public CaloriesBurned(int weight, double distance, int age, double twentySecondRestingHeartRate) {
        WKG = weight;
        DRK = distance;
        userAge = age;
        restingHeartRate = twentySecondRestingHeartRate;
        getCFF();
    }
    
    
    public int getCaloriesBurned(int grade) 
    {
        G.Add(grade);

        getGFactors();

        CB = (int)((((GFactor1 * G[G.Count - 1]) + GFactor2) * WKG + TF) * DRK * CFF);
        
        return CB;
    }

    private void getGFactors() {   
        int lastIndexVal = G[G.Count - 1];
        if(-20 <= lastIndexVal  && lastIndexVal <= -15) {
            GFactor1 = -0.01;
            GFactor2 = 0.50;
        }
        else if(-15 < lastIndexVal && lastIndexVal <= -10) {
            GFactor1 = -0.02;
            GFactor2 = 0.35;
        }
        else if(-10 < lastIndexVal && lastIndexVal <= 0){
            GFactor1 = 0.04;
            GFactor2 = 0.95;
        }
        else if(0 < lastIndexVal && lastIndexVal <= 10){
            GFactor1 = 0.05;
            GFactor2 = 0.95;
        }
        else if(10 < lastIndexVal && lastIndexVal <= 15){
            GFactor1 = 0.07;
            GFactor2 = 0.75;
        }
        else { 
            GFactor1 = 0.07;
            GFactor2 = 0.75;
        }
    }

    private void getCFF() {
        MHR = 208 - (0.7 * userAge);
        RHR = restingHeartRate * 3;
        VO2max = 15.3 * (MHR/RHR);

        if(VO2max >= 56) {
            CFF = 1.00;
        }
        else if(56 > VO2max && VO2max >= 54) {
            CFF = 1.01;
        }
        else if(54 > VO2max && VO2max >= 52){
            CFF = 1.02;
        }
        else if(52 > VO2max && VO2max >= 50){
            CFF = 1.03;
        }
        else if(50 > VO2max && VO2max >= 48){
            CFF = 1.04;
        } 
        else if(48 > VO2max && VO2max >= 46){
            CFF = 1.05;
        } 
        else if(46 > VO2max && VO2max >= 44){
            CFF = 1.06;
        } 
        else if(VO2max < 44){
            CFF = 1.07;
        } 
    }
}
