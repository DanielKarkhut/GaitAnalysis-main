// Daniel Karkhut
// How to Run:
// 1. cs -out:main.exe main.cs
// 2. mono main.exe
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;

public class ZScoreOutput {
    public List<double> input;
    public List<int> signals;
    public List<double> avgFilter;
    public List<double> filtered_stddev;
}

public static class ZScore {

    public static ZScoreOutput StartAlgo(List<double> input, int lag, double threshold, double influence) {
        // init variables!
        int[] signals = new int[input.Count];
        double[] filteredY = new List<double>(input).ToArray();
        double[] avgFilter = new double[input.Count];
        double[] stdFilter = new double[input.Count];

        var initialWindow = new List<double>(filteredY).Skip(0).Take(lag).ToList();

        avgFilter[lag - 1] = Mean(initialWindow);
        stdFilter[lag - 1] = StdDev(initialWindow);

        for (int i = lag; i < input.Count; i++) {
            if (Math.Abs(input[i] - avgFilter[i - 1]) > threshold * stdFilter[i - 1]) {
                signals[i] = (input[i] > avgFilter[i - 1]) ? 1 : -1;
                filteredY[i] = influence * input[i] + (1 - influence) * filteredY[i - 1];
            }
            else {
                signals[i] = 0;
                filteredY[i] = input[i];
            }

            // Update rolling average and deviation
            var slidingWindow = new List<double>(filteredY).Skip(i - lag).Take(lag+1).ToList();

            //var tmpMean = Mean(slidingWindow);
            //var tmpStdDev = StdDev(slidingWindow);

            avgFilter[i] = Mean(slidingWindow);
            stdFilter[i] = StdDev(slidingWindow);
        }

        // Copy to convenience class 
        var result = new ZScoreOutput();
        result.input = input;
        result.avgFilter       = new List<double>(avgFilter);
        result.signals         = new List<int>(signals);
        result.filtered_stddev = new List<double>(stdFilter);

        return result;
    }

    private static double Mean(List<double> list) {
        // Simple helper function! 
        return list.Average();
    }

    private static double StdDev(List<double> values) {
        double ret = 0;
        if (values.Count() > 0) {
            double avg = values.Average();
            double sum = values.Sum(d => Math.Pow(d - avg, 2));
            ret = Math.Sqrt((sum) / (values.Count()));
        }
        return ret;
    }
}

//Program.Main(null); //run it
public class Stride {

    double maxStrideLength = 0.0;
    double totalWalkLength = 0.0;
    int totalTime = 0;
    double averagePace = 0.0;
    string name = "";
    public Stride(List<int> TimeBetweenStrides, List<double> Peaks, List<double> Troughs, List <int> Time, string name) { //Constructor
        this.maxStrideLength = getMaxStrideLength(Troughs, Peaks);
        this.totalWalkLength = getTotalStrideLength(Troughs, Peaks);
        this.totalTime = Time[Time.Count - 1];
        this.averagePace = this.totalWalkLength / this.totalTime;
        this.name = name;
    }

    public double getMaxStrideLength(List<double> troughs, List<double> peaks) {
        double max_stride_length = 0;
        for(int i = 0; i < peaks.Count; i++) {
            double distance = Math.Abs(troughs[i] - peaks[i]);
            if (max_stride_length < distance) {
                max_stride_length = distance;
            }
        }
        return max_stride_length;
    }

    public double getTotalStrideLength(List<double> troughs, List<double> peaks) {
        double totalWalkLength = 0;
        for(int i = 0; i < peaks.Count; i++) {
            double distance = Math.Abs(troughs[i] - peaks[i]);
            totalWalkLength += distance;
        }
        return totalWalkLength;
    }

    public void report() {
        Console.WriteLine("______" + this.name + " Report _________");
        Console.WriteLine("Total Walk Length (mm): " + this.totalWalkLength);
        Console.WriteLine("Total Elapsed Time (ms): " + this.totalTime);
        Console.WriteLine("Max Stride Length (mm): " + this.maxStrideLength);
        Console.WriteLine("Average Pace (mm/ms): " + this.averagePace);
    }

}

public class main {
    public static void Main (string[] args) {

        // Store the path of the textfile in your system
        string file = @"PersonalLap1.txt";

        // Check if File exists
        if (!File.Exists(file)) {
            Console.Write("Error");
        }

        // Read and store lines of data from file
        string[] readData = File.ReadAllLines(file);

        // Create and populate lists with left foot, right foot, and time data
        int indexOfLeftFootX = 9; //from PersonalLap1.txt Format
        int indexOfRightFootX = 21; //from PersonalLap1.txt Format

        List<double> leftFootX = new List<double>(); // List of Left foot X points
        List<double> rightFootX = new List<double>(); // List of Right foot X points
        List<int> time = new List<int>(); // List of Time differences between indices

        // Add .txt data to Lists
        foreach (string s in readData) { 
            string[] split = s.Split(',');
            leftFootX.Add(int.Parse(split[indexOfLeftFootX]));
            rightFootX.Add(int.Parse(split[indexOfRightFootX]));
            time.Add(int.Parse(split[0]));
        }

        // Update time List to increment with each change 
        for (int i = 0; i < time.Count; i++) {
            if(i == 0) {
                time[i] = time[i];
            } else {
                time[i] = time[i] + time[i-1];
            }
        }
    
        
        // Peak and Trough Detection
        // Left Leg

        //MODIFY THESE PARAMETERS
        int lag = 8; // How much your data will be smoothed and how adaptive the algorithm is to changes in the long-term average of the data.
        double threshold = 1; //The number of standard deviations from the moving mean above which the algorithm will classify a new datapoint as being a signal.
        double influence = 0.0; // The influence of signals on the algorithm's detection threshold.

        var zScoresLeft = ZScore.StartAlgo(leftFootX, lag, threshold, influence);

       
        List<double> peaksLeft = new List<double>(); // Store calculated peaks
        List<double> troughsLeft = new List<double>(); // Store calculated peaks
        List<int> timePeriodsLeft = new List<int>(); // Store calculated peaks

        double max = 0.0;
        double min = 0.0;

        // Find x position of peaks and troughs by iterating over zScores.signals and find local max for each change in signal [from -1 to 1]
        for(int i = 0; i < zScoresLeft.signals.Count; i++) {
            if(zScoresLeft.signals[i] == 0) { // if Z-score.signal is not finding peaks/troughs
                continue;
            }
            max = 0; // local max within range
            min = 0; // local min within range
            for(int y = i+1; y < zScoresLeft.signals.Count; y++) { // Search further in indices to find local max and min
                
                if ((leftFootX[i] > max) | (max == 0)) { // Find local Max
                    max = leftFootX[i];
                }
                if ((leftFootX[i] < min) | (min == 0)) { // Find local min
                    min = leftFootX[i];
                }
                if(zScoresLeft.signals[i] != zScoresLeft.signals[y]) { // If signal changes
                    if ((zScoresLeft.signals[i] == 1) & (troughsLeft.Count > 0)) { // If signal is 1 and the first trough has already been found then add to peaks
                        peaksLeft.Add(max); 
                        timePeriodsLeft.Add(time[y]);        
                    } else if(zScoresLeft.signals[i] == -1) { // If signal is -1 
                        troughsLeft.Add(min);
                        timePeriodsLeft.Add(time[y]); 
                    }
                    i = y;
                    break;
                }
                i++;
            }
        }

        // Find time between full stride
        List<int> timeBetweenTroughAndPeakLeft = new List<int>();
        for(int i = 0; i < timePeriodsLeft.Count; i=i+2) {
            timeBetweenTroughAndPeakLeft.Add(timePeriodsLeft[i] - timePeriodsLeft[i+1]);
        }


        // Peak and Trough Detection
        // Right Leg

        //MODIFY THESE PARAMETERS
        lag = 7;
        threshold = 0.8;
        influence = 0.0;
        var zScoresRight = ZScore.StartAlgo(rightFootX, lag, threshold, influence);

        List<double> peaksRight = new List<double>();
        List<double> troughsRight  = new List<double>();
        List<int> timePeriodsRight = new List<int>();

        max = 0.0;
        min = 0.0;


        for(int i = 0; i < zScoresRight.signals.Count; i++) {
            if(zScoresRight.signals[i] == 0) {
                continue;
            }
            max = 0;
            min = 0;
            for(int y = i+1; y < zScoresRight.signals.Count; y++) {
                
                if ((rightFootX[i] > max) | (max == 0)) {
                    max = rightFootX[i];
                }
                if ((rightFootX[i] < min) | (min == 0)) {
                    min = rightFootX[i];
                }
                if(zScoresRight.signals[i] != zScoresRight.signals[y]) {
                    if ((zScoresRight.signals[i] == 1) & (troughsRight.Count > 0)) {
                        peaksRight.Add(max); 
                        timePeriodsRight.Add(time[y]);        
                    } else if(zScoresRight.signals[i] == -1) {
                        troughsRight.Add(min);
                        timePeriodsRight.Add(time[y]); 
                    }
                    i = y;
                    break;
                }
                i++;
            }
        }

        List<int> timeBetweenTroughAndPeakRight = new List<int>();
        for(int i = 0; i < timePeriodsRight.Count; i=i+2) {
            timeBetweenTroughAndPeakRight.Add(timePeriodsRight[i] - timePeriodsRight[i+1]);
        }

        // Console Printing

        Console.WriteLine("Troughs Right: " + troughsRight.Count);
        Console.WriteLine("Peaks Right: " + peaksRight.Count);

        Console.WriteLine("Troughs Left: " + troughsLeft.Count);
        Console.WriteLine("Peaks Left: " + peaksLeft.Count);

        Stride rightLeg = new Stride(timeBetweenTroughAndPeakRight, peaksRight, troughsRight, time, "Right Leg");
        Stride leftLeg = new Stride(timeBetweenTroughAndPeakLeft, peaksLeft, troughsLeft, time, "Left Leg");

        rightLeg.report();
        leftLeg.report();
    }
}