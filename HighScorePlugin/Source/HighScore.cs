/*
Copyright (C) 2007, 2008 Kristian Bisgaard Lassen
Copyright (C) 2010 Kristian Helkjaer Lassen

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Diagnostics;
//using ZoneFiveSoftware.Common.Visuals.Fitness;
//using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data;
using System.Data;
//using System.Windows.Forms;
//using GpsRunningPlugin.Properties;
using ZoneFiveSoftware.Common.Data.Measurement;
//using ZoneFiveSoftware.Common.Visuals;
//using GpsRunningPlugin.Util;

namespace GpsRunningPlugin.Source
{
    static class HighScore
    {
        //private HighScore() { }

        public static IList<IList<Object>> getFastestTimesOfDistances(IList<IActivity> activities, IList<double> distances, System.Windows.Forms.ProgressBar progressBar)
        {
            IList<Goal> goals = new List<Goal>();
            foreach (double distance in distances)
            { 
                goals.Add(new PointGoal(distance, false,
                            GoalParameter.Time, GoalParameter.Distance));
            }

            Result[] results = calculateInternal(activities, goals, progressBar);
            IList<IList<Object>> objects = new List<IList<Object>>();
            foreach (Result result in results)
            {
                if (result != null)
                {
                    IList<Object> s = new List<Object>();
                    objects.Add(s);
                    s.Add(result.Activity);
                    s.Add(result.Seconds);
                    s.Add(result.MeterStart);
                    s.Add(result.MeterEnd);
                    s.Add(result.TimeStart);
                }
            }
            return objects;
        }

        public static Result[] calculateInternal(IList<IActivity> activities, IList<Goal> goals, System.Windows.Forms.ProgressBar progressBar)
        {
            if (progressBar != null)
            {
                progressBar.Minimum = 0;
                progressBar.Value = 0;
                progressBar.Maximum = 0;//Set below
                progressBar.Visible = true;
                progressBar.BringToFront();
            }
            Result[] r = calculateActivities(activities, goals, progressBar);
            progressBar.Visible = false;
            return r;
        }

        public static Result[] calculateActivities(IList<IActivity> activities, IList<Goal> goals, System.Windows.Forms.ProgressBar progressBar)
        {
            Result[] results = new Result[goals.Count];
            if (activities != null && activities.Count > 0)
            {
                if (progressBar != null && progressBar.Maximum < progressBar.Value + activities.Count)
                {
                    progressBar.Maximum += activities.Count;
                }
                foreach (IActivity activity in activities)
                {
                    if (null != activity && activity.HasStartTime && 
                        (!Settings.IgnoreManualData || /*Settings.IgnoreManualData &&*/ !activity.UseEnteredData))
                    {
                        calculateActivity(activity, goals, results);
                    }
                    if (progressBar != null)
                    {
                        progressBar.Value++;
                    }
                }
            }
            return results;
        }

        private static void calculateActivity(IActivity activity, IList<Goal> goals, IList<Result> results)
        {
            ActivityInfo info = ActivityInfoCache.Instance.GetInfo(activity);
            double[] aDistance, aTime, aElevation, aPulse, aSpeed;
            DateTime[] aDateTime;
            int increment = 5;
            
            restart:
            {
                IList<LapDetailInfo> laps = info.DistanceLapDetailInfo(20 + increment);
                int length = laps.Count + 1;//info.MovingDistanceMetersTrack.Count;
                
                aDistance = new double[length];
                aDistance[0] = 0;

                aTime = new double[length];
                aTime[0] = 0;
                
                aElevation = new double[length];
                aPulse = new double[length];
                aSpeed = new double[length];
                aDateTime = new DateTime[length];

                DateTime dateTime = activity.StartTime;
                aDateTime[0] = dateTime;

                ITimeValueEntry<float> value = info.SmoothedElevationTrack.GetInterpolatedValue(dateTime);
                if (value != null)
                    aElevation[0] = value.Value;
                else
                    aElevation[0] = 0;

                value = info.SmoothedHeartRateTrack.GetInterpolatedValue(dateTime);
                if (info.SmoothedHeartRateTrack.Max > 0 &&
                     value != null)
                    aPulse[0] = value.Value;
                else
                    aPulse[0] = 0;

                value = info.SmoothedSpeedTrack.GetInterpolatedValue(dateTime);
                if (value != null)
                    aSpeed[0] = value.Value;
                else
                    aSpeed[0] = 0;

                //double d = 0;
                //TimeSpan t;
                //foreach (LapDetailInfo lap in laps)
                //{
                //    d += lap.LapDistanceMeters;
                //    t = lap.StartElapsed;
                //}
                int index = 1;
                foreach (LapDetailInfo lap in laps)
                {
                    aDistance[index] = lap.EndDistanceMeters;
                    aTime[index] = lap.EndElapsed.TotalSeconds;
                    if (aTime[index] < aTime[index - 1])
                    {
                        aTime[index] = aTime[index - 1];
                        increment += 5;
                        goto restart;
                    }

                    dateTime = lap.EndTime;
                    aDateTime[index] = dateTime;

                    value = info.SmoothedElevationTrack.GetInterpolatedValue(dateTime);
                    if (value != null)
                        aElevation[index] = value.Value;
                    else
                        aElevation[index] = 0;

                    value = info.SmoothedHeartRateTrack.GetInterpolatedValue(dateTime);
                    if (info.SmoothedHeartRateTrack.Max > 0 &&
                         value != null)
                        aPulse[index] = value.Value;
                    else
                        aPulse[index] = 0;

                    value = info.SmoothedSpeedTrack.GetInterpolatedValue(dateTime);
                    if (value != null)
                        aSpeed[index] = value.Value;
                    else
                        aSpeed[index] = 0;

                    index++;
                }
            }
            //foreach (TimeValueEntry<float> pair in info.MovingDistanceMetersTrack)
            //{
            //    distance[index] = pair.Value;
            //    time[index] = pair.ElapsedSeconds;
            //    DateTime dateTime = info.MovingDistanceMetersTrack.StartTime.AddSeconds(time[index]);//activity.StartTime.AddSeconds(pair.ElapsedSeconds);
            //    ITimeValueEntry<float> value = info.SmoothedElevationTrack.GetInterpolatedValue(dateTime);
            //    if (value != null)
            //        elevation[index] = value.Value;
            //    else
            //        elevation[index] = 0;
            //    value = info.SmoothedHeartRateTrack.GetInterpolatedValue(dateTime);
            //    if (info.SmoothedHeartRateTrack.Max > 0 &&
            //         value != null)
            //        pulse[index] = value.Value;
            //    else
            //        pulse[index] = 0;
            //    value = info.SmoothedSpeedTrack.GetInterpolatedValue(dateTime);
            //    if (value != null)
            //        speed[index] = value.Value;
            //    else
            //        speed[index] = 0;
            //    index++;
            //}

            //pad(elevation);
            //pad(pulse);
            //pad(speed);

            for (int i = 0; i < goals.Count; i++)
            {
                Result result = null;
                switch (goals[i].Image)
                {
                    case GoalParameter.PulseZone: 
                    case GoalParameter.SpeedZone:
                    case GoalParameter.PulseZoneSpeedZone:
                        if (info.SmoothedHeartRateTrack.Count > 0)
                            result = calculateActivityZoneGoal(activity, (IntervalsGoal)goals[i],
                                                getGoalTrack(goals[i].Domain, aDistance, aTime, aElevation),
                                                getGoalZoneTrack(goals[i].Image, aPulse, aSpeed),
                                                aTime, aDistance, aElevation, aPulse, aDateTime);
                        break; 
                    default:
                        result = calculateActivityGoal(activity, (PointGoal)goals[i],
                                            getGoalTrack(goals[i].Domain, aDistance, aTime, aElevation),
                                            getGoalTrack(goals[i].Image, aDistance, aTime, aElevation),
                                            aTime, aDistance, aElevation, aPulse, aDateTime);
                        break;
                }

                int upperBound = goals[i].UpperBound ? 1 : -1;
                if (result != null && (results[i] == null ||
                   (upperBound * result.DomainDiff > upperBound * results[i].DomainDiff)))
                {
                    results[i] = result;
                }
            }
        }

        //private static void pad(double[] ds)
        //{
        //    int index = 0;
        //    while (index < ds.Length && ds[index] == 0) index++;
        //    if (index >= ds.Length) return;
        //    double v = ds[index];
        //    while (index >= 0) ds[index--] = v;
        //    index = ds.Length - 1;
        //    while (index >= 0 && ds[index] == 0) index--;
        //    v = ds[index];
        //    while (index < ds.Length) ds[index++] = v;
        //}

        private static double[] getGoalTrack(GoalParameter goal, 
            double[] distance, double[] time, double[] elevation)
        {
            switch (goal)
            {
                case GoalParameter.Distance: 
                    return distance;
                case GoalParameter.Time: 
                    return time;
                case GoalParameter.Elevation: 
                    return elevation;
            }
            return null;
        }

        public static IList<double[]> getGoalZoneTrack(GoalParameter goal, 
            double[] pulse, double[] speed)
        {
            IList<double[]> result = new List<double[]>();
            switch (goal)
            {
                case GoalParameter.PulseZone:
                    result.Add(pulse); 
                    break;
                case GoalParameter.SpeedZone:
                    result.Add(speed); 
                    break;
                case GoalParameter.PulseZoneSpeedZone:
                    result.Add(pulse); result.Add(speed); 
                    break;
            }
            return result;
        }

        private static Result calculateActivityZoneGoal(IActivity activity, IntervalsGoal goal, 
            double[] domain, IList<double[]> image,
            double[] time, double[] distance, double[] elevation, double[] pulse, DateTime[] aDateTime)
        {
            bool foundAny = false;
            int back = 0, front = 0;
            int bestBack = 0, bestFront = 0;
            
            double best;
            if (goal.UpperBound) best = double.MinValue;
            else best = double.MaxValue;

            int length = image[0].Length;

            while (front < length)
            {
                bool inWindow = true;
                for (int i = 0; i < image.Count; i++)
                {
                    if (image[i][back] < goal.Intervals[i][0] ||
                        image[i][front] > goal.Intervals[i][1])
                    {
                        inWindow = false;
                        break;
                    }
                }
                if (inWindow)
                {
                    double domainDiff = domain[front] - domain[back];
                    int upperBound = goal.UpperBound ? 1 : -1;
                    if (upperBound*best < upperBound*domainDiff &&
                        (elevation[front] - elevation[back]) / (distance[front] - distance[back]) >= 
                        Settings.MinGrade)
                    {
                        foundAny = true;
                        best = domainDiff;
                        bestBack = back;
                        bestFront = front;
                    }
                    if (back == front || 
                        (front < length - 1 && isInZone(image, goal, front + 1))) 
                        front++;
                    else back++;
                }
                else
                {
                    if (back == front)
                    {
                        if (front < length - 1 && !isInZone(image, goal, front + 1)) 
                            back++;
                        front++;
                    }
                    else back++;
                }
            }

            if (foundAny)
            {
                return new Result(goal, activity, domain[bestBack], domain[bestFront], time[bestBack], time[bestFront],
                    distance[bestBack], distance[bestFront], elevation[bestBack], elevation[bestFront], 
                    averagePulse(pulse, time, bestBack, bestFront),
                    aDateTime[bestBack], aDateTime[bestFront]);
            }
            return null;
        }

        private static double averagePulse(double[] pulse, double[] time, int back, int front)
        {
            double result = 0;
            for (int i = back; i < front; i++)
                result += (pulse[i] + (pulse[i + 1] - pulse[i]) / 2) * (time[i + 1] - time[i]);
            result = result / (time[front] - time[back]);
            return result;
        }

        private static bool isInZone(IList<double[]> image, IntervalsGoal goal, int index)
        {
            for (int i = 0; i < image.Count; i++)
            {
                if (image[i][index] < goal.Intervals[i][0] ||
                    image[i][index] > goal.Intervals[i][1])
                {
                    return false;
                }
            }
            return true;
        }

        private static Result calculateActivityGoal(IActivity activity, PointGoal goal, 
            double[] domain, double[] image,
            double[] time, double[] distance, double[] elevation, double[] pulse, DateTime[] aDateTime)
        {
            bool foundAny = false;
            int back = 0, front = 0;
            int bestBack = 0, bestFront = 0;

            double best;
            if (goal.UpperBound) best = double.MinValue;
            else best = double.MaxValue;

            while (front < image.Length && back < image.Length)
            {
                if (image[front] - image[back] >= goal.Value)
                {
                    double domainDiff = domain[front] - domain[back];
                    int upperBound = goal.UpperBound ? 1 : -1;
                    if (upperBound*best < upperBound*domainDiff &&
                        (elevation[front] - elevation[back]) / (distance[front] - distance[back]) >=
                        Settings.MinGrade)
                    {
                        foundAny = true;
                        best = domainDiff;
                        bestBack = back;
                        bestFront = front;
                    }
                    back++;
                }
                else
                {
                    front++;
                }
            }
            if (foundAny)
            {
                return new Result(goal, activity, domain[bestBack], domain[bestFront], time[bestBack], time[bestFront],
                    distance[bestBack], distance[bestFront], elevation[bestBack], elevation[bestFront],
                    averagePulse(pulse, time, bestBack, bestFront),
                    aDateTime[bestBack], aDateTime[bestFront]);
            }
            return null;
        }

        public static void generateGoal(GoalParameter domain, GoalParameter image, bool upperBound,
            IList<Goal> goals)
        {
            switch (image)
            {
                case GoalParameter.Distance:
                    foreach (double distance in Settings.distances.Keys)
                    {
                        goals.Add(new PointGoal(distance, upperBound,
                                    domain, GoalParameter.Distance));
                    }
                    break;
                case GoalParameter.Time:
                    foreach (int time in Settings.times.Keys)
                    {
                        goals.Add(new PointGoal(time, upperBound,
                                    domain, GoalParameter.Time));
                    }
                    break;
                case GoalParameter.Elevation:
                    foreach (double elevation in Settings.elevations.Keys)
                    {
                        goals.Add(new PointGoal(elevation, upperBound,
                                    domain, GoalParameter.Elevation));
                    }
                    break;
                case GoalParameter.PulseZone:
                    foreach (double min in Settings.pulseZones.Keys)
                        foreach (double max in Settings.pulseZones[min].Keys)
                        {
                            IList<IList<double>> intervals = new List<IList<double>>();
                            IList<double> interval = new List<double>();
                            interval.Add(min);
                            interval.Add(max);
                            intervals.Add(interval);
                            goals.Add(new IntervalsGoal(intervals, upperBound,
                                        domain, GoalParameter.PulseZone));
                        }
                    break;
                case GoalParameter.SpeedZone:
                    foreach (double min in Settings.speedZones.Keys)
                        foreach (double max in Settings.speedZones[min].Keys)
                        {
                            IList<IList<double>> intervals = new List<IList<double>>();
                            IList<double> interval = new List<double>();
                            interval.Add(min);
                            interval.Add(max);
                            intervals.Add(interval);
                            goals.Add(new IntervalsGoal(intervals, upperBound,
                                        domain, GoalParameter.SpeedZone));
                        }
                    break;
                case GoalParameter.PulseZoneSpeedZone:
                    foreach (double minPulse in Settings.pulseZones.Keys)
                        foreach (double maxPulse in Settings.pulseZones[minPulse].Keys)
                            foreach (double minSpeed in Settings.speedZones.Keys)
                                foreach (double maxSpeed in Settings.speedZones[minSpeed].Keys)
                                {
                                    IList<IList<double>> intervals = new List<IList<double>>();
                                    IList<double> interval = new List<double>();
                                    interval.Add(minPulse);
                                    interval.Add(maxPulse);
                                    intervals.Add(interval);
                                    interval = new List<double>();
                                    interval.Add(minSpeed);
                                    interval.Add(maxSpeed);
                                    intervals.Add(interval);
                                    goals.Add(new IntervalsGoal(intervals, upperBound,
                                        domain, GoalParameter.PulseZoneSpeedZone));
                                }
                    break;
            }
        }

        public static IList<Goal> generateGoals()
        {
            IList<Goal> goals = new List<Goal>();
            generateGoal(Settings.Domain, Settings.Image, Settings.UpperBound, goals);
            return goals;
        }
    }
}