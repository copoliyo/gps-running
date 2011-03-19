/*
Copyright (C) 2010 Gerhard Olsson

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

/* This file is used in several plugins
 * */

using System;
using System.Globalization;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using GpsRunningPlugin;

namespace GpsRunningPlugin.Util
{
    class UnitUtil
    {
        public static IApplication GetApplication()
        {
#if GPSRUNNING_UNIQUEROUTES||GPSRUNNING_OVERLAY||GPSRUNNING_HIGHSCORE||GPSRUNNING_PERFORMANCEPREDICTOR||GPSRUNNING_ACCUMULATEDSUMMARY||GPSRUNNING_TRIMP
            return GpsRunningPlugin.Plugin.GetApplication();
#elif MATRIXPLUGIN
            return MatrixPlugin.Plugin.GetApplication();
#else // TRAILSPLUGIN
            return TrailsPlugin.Plugin.GetApplication();
#endif
        }
        
        //Helpfunction for common format
        private static string encPar(string p)
        {
            //Some units (like Km in Danish) has a trailing space
            char[] t = {' '};
            return " (" + p.TrimEnd(t) + ")";
        }

        /*********************************************************************************/
        public class Power
        {
            //private static Length.Units Unit { get { return null; } }
            public static int DefaultDecimalPrecision { get { return 0; } }
            private static string DefFmt { get { return "F" + DefaultDecimalPrecision; } }

            public static string ToString(double p)
            {
                return ToString(p, DefFmt);
            }
            public static string ToString(double p, string fmt)
            {
                string str = "";
                if (fmt.EndsWith("U")) { str = " " + Label; fmt = fmt.Remove(fmt.Length-1); }
                if (fmt.EndsWith("u")) { str = " " + LabelAbbr; fmt = fmt.Remove(fmt.Length-1); }
                return ConvertFrom(p).ToString((fmt));
            }

            public static double ConvertFrom(double p)
            {
                return p;
            }

            public static double Parse(string p)
            {
                return double.Parse(p, NumberFormatInfo.InvariantInfo);
            }

            public static String Label
            {
                get
                {
                    return CommonResources.Text.LabelWatts;
                }
            }
            public static String LabelAbbr
            {
                get
                {
                    return "W";
                }
            }
            public static String LabelAbbr2
            {
                get
                {
                    return encPar(LabelAbbr);
                }
            }
            public static String LabelAxis
            {
                get
                {
                    return CommonResources.Text.LabelPower + LabelAbbr2;
                }
            }
        }

        /*********************************************************************************/
        public class Cadence
        {
            //private static Length.Units Unit { get { return null; } }
            public static int DefaultDecimalPrecision { get { return 0; } }
            private static string DefFmt { get { return "F" + DefaultDecimalPrecision; } }

            public static string ToString(double p)
            {
                return ToString(p, DefFmt);
            }
            public static string ToString(double p, string fmt)
            {
                string str = "";
                if (fmt.EndsWith("U")) { str = " " + Label; fmt = fmt.Remove(fmt.Length-1); }
                if (fmt.EndsWith("u")) { str = " " + LabelAbbr; fmt = fmt.Remove(fmt.Length-1); }
                return ConvertFrom(p).ToString((fmt));
            }

            public static double ConvertFrom(double p)
            {
                return p;
            }

            private static double Parse(string p)
            {
                return double.Parse(p, NumberFormatInfo.InvariantInfo);
            }

            public static String Label
            {
                get
                {
                    return CommonResources.Text.LabelRPM;
                }
            }
            public static String LabelAbbr
            {
                get
                {
                    return CommonResources.Text.LabelRPM;
                }
            }
            public static String LabelAbbr2
            {
                get
                {
                    return encPar(LabelAbbr);
                }
            }
            public static String LabelAxis
            {
                get
                {
                    return CommonResources.Text.LabelCadence + LabelAbbr2;
                }
            }
        }

        /*********************************************************************************/
        public static class HeartRate
        {
            //private static Length.Units Unit { get { return null; } }
            public static int DefaultDecimalPrecision { get { return 0; } }
            private static string DefFmt { get { return "F" + DefaultDecimalPrecision; } }

            public static string ToString(double p)
            {
                return ToString(p, DefFmt);
            }
            public static string ToString(double p, string fmt)
            {
                string str = "";
                if (fmt.EndsWith("U")) { str = " " + Label; fmt = fmt.Remove(fmt.Length-1); }
                if (fmt.EndsWith("u")) { str = " " + LabelAbbr; fmt = fmt.Remove(fmt.Length-1); }
                return ConvertFrom(p).ToString((fmt));
            }

            public static double ConvertFrom(double p)
            {
                return p;
            }

            public static double Parse(string p)
            {
                return double.Parse(p, NumberFormatInfo.InvariantInfo);
            }

            public static String Label
            {
                get
                {
                    return CommonResources.Text.LabelBPM;
                }
            }
            public static String LabelAbbr
            {
                get
                {
                    return CommonResources.Text.LabelBPM;
                }
            }
            public static String LabelAbbr2
            {
                get
                {
                    return encPar(LabelAbbr);
                }
            }
            public static String LabelAxis
            {
                get
                {
                    return CommonResources.Text.LabelHeartRate + LabelAbbr2;
                }
            }
        }

        /*********************************************************************************/
        public static class Energy
        {
            private static ZoneFiveSoftware.Common.Data.Measurement.Energy.Units Unit { get { return GetApplication().SystemPreferences.EnergyUnits; } }
            public static int DefaultDecimalPrecision { get { return 0; } }
            private static string DefFmt { get { return "F" + DefaultDecimalPrecision; } }

            public static string ToString(double p)
            {
                return ToString(p, DefFmt);
            }
            public static string ToString(double p, string fmt)
            {
                string str = "";
                if (fmt.EndsWith("U")) { str = " " + Label; fmt = fmt.Remove(fmt.Length - 1); }
                if (fmt.EndsWith("u")) { str = " " + LabelAbbr; fmt = fmt.Remove(fmt.Length - 1); }
                return ConvertFrom(p).ToString((fmt));
            }

            public static double ConvertFrom(double p)
            {
                return ZoneFiveSoftware.Common.Data.Measurement.Energy.Convert(p,
                    ZoneFiveSoftware.Common.Data.Measurement.Energy.Units.Kilocalorie,
                    Unit);
            }

            public static double Parse(string p)
            {
                ZoneFiveSoftware.Common.Data.Measurement.Energy.Units unit = Unit;
                return ZoneFiveSoftware.Common.Data.Measurement.Energy.ParseEnergyKilocalories(p, unit);
            }

            public static String Label
            {
                get
                {
                    return ZoneFiveSoftware.Common.Data.Measurement.Energy.Label(Unit);
                }
            }
            public static String LabelAbbr
            {
                get
                {
                    return ZoneFiveSoftware.Common.Data.Measurement.Energy.Label(Unit);
                }
            }
            public static String LabelAbbr2
            {
                get
                {
                    return encPar(LabelAbbr);
                }
            }
            public static String LabelAxis
            {
                //This is included for completeness only
                get
                {
                    return CommonResources.Text.LabelCalories + LabelAbbr2;
                }
            }
        }

        /*********************************************************************************/
        public static class Temperature
        {
            private static ZoneFiveSoftware.Common.Data.Measurement.Temperature.Units Unit { get { return GetApplication().SystemPreferences.TemperatureUnits; } }
            public static int DefaultDecimalPrecision { get { return 1; } }
            private static string DefFmt { get { return "F" + DefaultDecimalPrecision; } }

            public static string ToString(double p)
            {
                return ToString(p, DefFmt);
            }
            public static string ToString(double p, string fmt)
            {
                if (fmt.ToLower().Equals("u")) { fmt = DefFmt + fmt; }
                return ZoneFiveSoftware.Common.Data.Measurement.Temperature.ToString(ConvertFrom(p), Unit, fmt);
            }
            public static double ConvertFrom(double p)
            {
                return ZoneFiveSoftware.Common.Data.Measurement.Temperature.Convert(p,
                    ZoneFiveSoftware.Common.Data.Measurement.Temperature.Units.Celsius, Unit);
            }

            public static double Parse(string p)
            {
                return double.Parse(p, NumberFormatInfo.InvariantInfo);
            }

            public static String Label
            {
                get
                {
                    return ZoneFiveSoftware.Common.Data.Measurement.Temperature.Label(Unit);
                }
            }
            public static String LabelAbbr
            {
                get
                {
                    return ZoneFiveSoftware.Common.Data.Measurement.Temperature.LabelAbbr(Unit);
                }
            }
            public static String LabelAbbr2
            {
                get
                {
                    return encPar(LabelAbbr);
                }
            }
            public static String LabelAxis
            {
                //This is included for completeness only
                get
                {
                    return CommonResources.Text.LabelTemperature + LabelAbbr2;
                }
            }
        }

        /*********************************************************************************/
        public static class Weight
        {
            private static ZoneFiveSoftware.Common.Data.Measurement.Weight.Units Unit { get { return GetApplication().SystemPreferences.WeightUnits; } }
            public static int DefaultDecimalPrecision { get { return 1; } }
            private static string DefFmt { get { return "F" + DefaultDecimalPrecision; } }

            public static string ToString(double p)
            {
                return ToString(p, DefFmt);
            }
            public static string ToString(double p, string fmt)
            {
                if (fmt.ToLower().Equals("u")) { fmt = DefFmt + fmt; }
                return ZoneFiveSoftware.Common.Data.Measurement.Weight.ToString(ConvertFrom(p), Unit, fmt);
            }

            public static double ConvertFrom(double p)
            {
                return ZoneFiveSoftware.Common.Data.Measurement.Weight.Convert(p,
                    ZoneFiveSoftware.Common.Data.Measurement.Weight.Units.Kilogram, Unit);
            }

            public static double Parse(string p)
            {
                return ZoneFiveSoftware.Common.Data.Measurement.Weight.ParseWeightKilograms(p, Unit);
            }

            public static String Label
            {
                get
                {
                    return ZoneFiveSoftware.Common.Data.Measurement.Weight.Label(Unit);
                }
            }
            public static String LabelAbbr
            {
                get
                {
                    return ZoneFiveSoftware.Common.Data.Measurement.Weight.LabelAbbr(Unit);
                }
            }
            public static String LabelAbbr2
            {
                get
                {
                    return encPar(LabelAbbr);
                }
            }
            public static String LabelAxis
            {
                get
                {
                    return CommonResources.Text.LabelWeight + LabelAbbr2;
                }
            }
        }

        /*********************************************************************************/
        public static class Elevation
        {
            private static Length.Units Unit { get { return GetApplication().SystemPreferences.ElevationUnits; } }
            public static int DefaultDecimalPrecision { get { return Length.DefaultDecimalPrecision(Unit); } }
            private static string DefFmt { get { return defFmt(Unit); } }
            private static string defFmt(Length.Units unit) { return "F" + Length.DefaultDecimalPrecision(unit); }
            public static Length.Units GetUnit(IActivity activity)
            {
                Length.Units du;
                if (activity != null)
                {
                    du = activity.Category.ElevationUnits;
                }
                else
                {
                    du = Unit;
                }
                return du;
            }

            public static string ToString(double p)
            {
                return ToString(p, DefFmt);
            }
            public static string ToString(double p, string fmt)
            {
                if (fmt.ToLower().Equals("u")) { fmt = DefFmt + fmt; }
                return Length.ToString(ConvertFrom(p), Unit, fmt);
            }
            public static string ToString(double p, IActivity activity, string fmt)
            {
                string str = "";
                if (fmt.EndsWith("U")) { str = " " + Label; fmt = fmt.Remove(fmt.Length - 1); }
                if (fmt.EndsWith("u")) { str = " " + LabelAbbr; fmt = fmt.Remove(fmt.Length - 1); }
                if (string.IsNullOrEmpty(fmt)) { fmt = DefFmt; }
                return ConvertFrom(p, activity).ToString((fmt)) + str;
            }

            public static double ConvertFrom(double p)
            {
                return ConvertFrom(p, Unit);
            }
            public static double ConvertFrom(double p, Length.Units unit)
            {
                return Length.Convert(p, Length.Units.Meter, unit);
            }
            public static double ConvertFrom(double value, IActivity activity)
            {
                return ConvertFrom(value, GetUnit(activity));
            }

            public static double Parse(string p)
            {
                Length.Units unit = Unit;
                return Length.ParseDistanceMeters(p, ref unit);
            }

            public static String Label
            {
                get
                {
                    return Length.Label(Unit);
                }
            }
            public static String LabelAbbr
            {
                get
                {
                    return Length.LabelAbbr(Unit);
                }
            }
            public static String LabelAbbr2
            {
                get
                {
                    return encPar(LabelAbbr);
                }
            }
            public static String LabelAxis
            {
                get
                {
                    return CommonResources.Text.LabelElevation + LabelAbbr2;
                }
            }
            public static string LabelAbbrAct(IActivity activity)
            {
                return Length.LabelAbbr(GetUnit(activity));
            }
            public static string LabelAbbrAct2(IActivity activity)
            {
                return encPar(LabelAbbrAct(activity));
            }
        }

        /*********************************************************************************/
        public static class Time
        {
            //This class handles Time as in "Time for activities" rather than "Time of day"
            public static int DefaultDecimalPrecision { get { return 1; } } //Unly used for time less than a minute
            private static string DefFmt { get { return ""; } }

            public static string ToString(double p)
            {
                return ToString(p, DefFmt);
            }
            public static String ToString(double sec, string fmt)
            {
                return ToString(new TimeSpan(0, 0, 0, 0, (int)(sec * 1000)), fmt);
            }
            public static string ToString(TimeSpan p)
            {
                return ToString(p, DefFmt);
            }
            public static String ToString(TimeSpan sec, string fmt)
            {
                string str = "";
                if (fmt.EndsWith("U")) { fmt = fmt.Remove(fmt.Length - 1); }
                if (fmt.EndsWith("u")) { fmt = fmt.Remove(fmt.Length - 1); }
                //.NET 2 has no built-in formatting for time
                //Currently remove millisec ("mm:ss" format)
                if (fmt.Equals("ss") && sec.TotalSeconds < 60 || fmt.Equals("s"))
                {
                    str = (sec.TotalMilliseconds / 1000).ToString(("F" + DefaultDecimalPrecision));
                }
                else
                {
                    //Truncate to seconds by creating new TimeSpan
                    str = (new TimeSpan(0, 0, (int)Math.Round(sec.TotalSeconds))).ToString();
                    if (fmt.Equals("mm:ss") && str.StartsWith("00:")) { str = str.Substring(3); }
                }

                return str;
            }

            public static double Parse(string p)
            {
                String[] pair = p.Split(':');
                double seconds;
                if (pair.Length >= 2)
                {
                    if (pair.Length == 2)
                    {
                        p = "00:" + p;
                    }
                    //This seem to be a regular time span, use standard metod
                    TimeSpan ts = TimeSpan.Parse(p);
                    seconds = ts.TotalMilliseconds / 1000;
                }
                else
                {
                    //Try parsing as double - there will be an exception if incorrect
                    seconds = double.Parse(p);
                }

                return seconds;
            }

            public static String LabelAxis
            {
                get
                {
                    return CommonResources.Text.LabelTime; //no unit info
                }
            }
        }

        /*********************************************************************************/
        public static class Distance
        {
            //Some lists uses unit when storing user entered data, why this is public
            public static Length.Units Unit { get { return GetApplication().SystemPreferences.DistanceUnits; } }
            public static int DefaultDecimalPrecision { get { return Length.DefaultDecimalPrecision(Unit); } }
            private static string DefFmt { get { return defFmt(Unit); } }
            private static string defFmt(Length.Units unit) { return "F" + Length.DefaultDecimalPrecision(unit); }
            public static Length.Units GetUnit(IActivity activity)
            {
                Length.Units du;
                if (activity != null)
                {
                    du = activity.Category.DistanceUnits;
                }
                else
                {
                    du = Unit;
                }
                return du;
            }

            public static string ToString(double p)
            {
                return ToString(p, DefFmt);
            }
            public static string ToString(double p, string fmt)
            {
                return ToString(p, Unit, fmt);
            }
            public static string ToString(double p, Length.Units unit, string fmt)
            {
                //The source is in system format (meter), but should be converted
                //defFmt should not be required, but ST applies it automatically if "u" is added
                if (fmt.ToLower().Equals("u")) { fmt = defFmt(unit) + fmt; }
                return Length.ToString(ConvertFrom(p, unit), unit, fmt);
            }
            public static string ToString(double p, IActivity activity, string fmt)
            {
                string str = "";
                if (fmt.EndsWith("U")) { str = " " + Label; fmt = fmt.Remove(fmt.Length - 1); }
                if (fmt.EndsWith("u")) { str = " " + LabelAbbr; fmt = fmt.Remove(fmt.Length - 1); }
                if (string.IsNullOrEmpty(fmt)) { fmt = DefFmt; }
                return ConvertFrom(p, activity).ToString((fmt)) + str;
            }

            //From SI unit (ST internal) to display
            public static double ConvertFrom(double p)
            {
                return ConvertFrom(p, Unit);
            }
            public static double ConvertFrom(double p, Length.Units unit)
            {
                return Length.Convert(p, Length.Units.Meter, unit);
            }
            public static double ConvertFrom(double value, IActivity activity)
            {
                return ConvertFrom(value, GetUnit(activity));
            }

            public static double ConvertTo(double value, Length.Units du)
            {
                return Length.Convert(value, du, Length.Units.Meter);
            }
            public static double ConvertTo(double value, IActivity activity)
            {
                return ConvertTo(value, GetUnit(activity));
            }

            public static double Parse(string p)
            {
                Length.Units unit = Unit;
                return Parse(p, ref unit);
            }
            public static double Parse(string p, ref Length.Units unit)
            {
                return Length.ParseDistanceMeters(p, ref unit);
            }

            public static String Label
            {
                get
                {
                    return Length.Label(Unit);
                }
            }
            public static String LabelAbbr
            {
                get
                {
                    return Length.LabelAbbr(Unit);
                }
            }
            public static String LabelAbbr2
            {
                get
                {
                    return encPar(LabelAbbr);
                }
            }
            public static String LabelAxis
            {
                get
                {
                    return CommonResources.Text.LabelDistance + LabelAbbr2;
                }
            }
            public static string LabelAbbrAct(IActivity activity)
            {
                return Length.LabelAbbr(GetUnit(activity));
            }
            public static string LabelAbbrAct2(IActivity activity)
            {
                return encPar(LabelAbbrAct(activity));
            }
        }

        /*********************************************************************************/
        //Pace/speed handling
        //Limit distance to units where we have pace/speed labels
        //(meter is available for distance as well, but no labels are available)

        private static Length.Units getDistUnit(bool isPace)
        {
            Length.Units distUnit = GetApplication().SystemPreferences.DistanceUnits;
            //Add all known statute length units, in case they are added to ST
            if (distUnit.Equals(Length.Units.Mile) || distUnit.Equals(Length.Units.Inch) ||
                distUnit.Equals(Length.Units.Foot) || distUnit.Equals(Length.Units.Yard))
            {
                distUnit = Length.Units.Mile;
            }
            else
            {
                distUnit = Length.Units.Kilometer;
            }
            return distUnit;
        }

        /*********************************************************************************/
        public static class Speed
        {
            //Convert pace/speed from system type (m/s) to used value
            // all speed units are per hour
            private static Length.Units Unit { get { return getDistUnit(false); } }
            public static int DefaultDecimalPrecision { get { return 1; } }
            private static string DefFmt { get { return "F" + DefaultDecimalPrecision; } }
            public static Length GetLength(IActivity activity)
            {
                Length du;
                if (activity != null)
                {
                    du = activity.Category.SpeedDistance;
                }
                else
                {
                    du = new Length(1, GetApplication().SystemPreferences.DistanceUnits);
                }
                return du;
            }

            public static string ToString(double p)
            {
                return ToString(p, DefFmt);
            }
            public static string ToString(double p, string fmt)
            {
                return ToString(p, null, fmt);
            }
            public static string ToString(double p, IActivity activity, string fmt)
            {
                string str = "";
                if (fmt.EndsWith("U")) { str = " " + Label; fmt = fmt.Remove(fmt.Length-1); }
                if (fmt.EndsWith("u")) { str = " " + LabelAbbr; fmt = fmt.Remove(fmt.Length-1); }
                if (string.IsNullOrEmpty(fmt)) { fmt = DefFmt; }
                return ConvertFrom(p, activity).ToString((fmt)) + str;
            }

            public static double ConvertFrom(double p)
            {
                return ConvertFrom(p, Unit);
            }
            public static double ConvertFrom(double p, Length.Units unit)
            {
                return Length.Convert(p, Length.Units.Meter, unit) * 60 * 60;
            }
            public static double ConvertFrom(double value, IActivity activity)
            {
                return PaceOrSpeed.ConvertFrom(false, value, activity);
            }

            public static double Parse(string p)
            {
                return Length.Convert(double.Parse(p, NumberFormatInfo.InvariantInfo), Unit, Length.Units.Meter) / (60 * 60);
            }

            private static String getLabel()
            {
                return getLabel(null);
            }
            private static String getLabel(IActivity activity)
            {
                String unit;
#if ST_2_1
                if (Unit.Equals(Length.Units.Mile))
                {
                    unit = CommonResources.Text.LabelMilePerHour;
                }
                else
                {
                    unit = CommonResources.Text.LabelKmPerHour;
                }
#else
                unit = ZoneFiveSoftware.Common.Data.Measurement.Speed.Label(
                    ZoneFiveSoftware.Common.Data.Measurement.Speed.Units.Speed,
                    GetLength(activity));
#endif
                return unit;
            }
            public static String Label
            {
                get
                {
                    return getLabel();
                }
            }
            public static String LabelAbbr
            {
                get
                {
                    return getLabel();
                }
            }
            public static String LabelAbbr2
            {
                get
                {
                    return encPar(LabelAbbr);
                }
            }
            public static String LabelAxis
            {
                get
                {
                    return CommonResources.Text.LabelSpeed + LabelAbbr2;
                }
            }
            public static string LabelAbbrAct(IActivity activity)
            {
                return getLabel(activity);
            }
            public static string LabelAbbrAct2(IActivity activity)
            {
                return encPar(LabelAbbrAct(activity));
            }
        }

        /*********************************************************************************/
        public static class Pace
        {
            //Pace units are in "unspecified time" per distance
            //Display is done as time, input parsing expects time in minutes or "minute time"

            private static Length.Units Unit { get { return getDistUnit(true); } }
            public static int DefaultDecimalPrecision { get { return 1; } } //Not really applicable
            private static string DefFmt { get { return ""; } }
            public static Length GetLength(IActivity activity)
            {
                Length du;
                if (activity != null)
                {
                    du = activity.Category.PaceDistance;
                }
                else
                {
                    du = new Length(1, GetApplication().SystemPreferences.DistanceUnits);
                }
                return du;
            }

            public static string ToString(double p)
            {
                return ToString(p, DefFmt);
            }
            public static String ToString(double speedMS, string fmt)
            {
                return ToString(speedMS, null, fmt);
            }
            public static String ToString(double speedMS, IActivity activity, string fmt)
            {
                //The only formatting handled is "u"/"U"
                double pace = ConvertFrom(speedMS, activity);
                string str = "";
                if (fmt.EndsWith("U")) { str = " " + Label; fmt = fmt.Remove(fmt.Length - 1); }
                if (fmt.EndsWith("u")) { str = " " + LabelAbbr; fmt = fmt.Remove(fmt.Length - 1); }
                if (Math.Abs(speedMS) == double.MinValue)//"divide by zero" check. Or some hardcoded value?
                {
                    str = "-" + str;
                }
                else
                {
                    str = new TimeSpan(0, 0, (int)Math.Round(pace)).ToString() + str;
                    if (str.StartsWith("00:")) { str = str.Substring(3); }
                }
                return str;
            }

            public static double ConvertFrom(double p)
            {
                return ConvertFrom(p, Unit);
            }
            public static double ConvertFrom(double p, Length.Units unit)
            {
                return 1 / Length.Convert(p, Length.Units.Meter, unit);
            }
            public static double ConvertFrom(double value, IActivity activity)
            {
                return PaceOrSpeed.ConvertFrom(true, value, activity);
            }

            public static double Parse(string p)
            {
                //Almost the same as parsing time, except that no colons is interpreted as minutes
                String[] pair = p.Split(':');
                double seconds;
                if (pair.Length == 3 || pair.Length == 2)
                {
                    if (pair.Length == 2)
                    {
                        p = "00:" + p;
                    }
                    //This seem to be a regular time span, use standard metod
                    TimeSpan ts = TimeSpan.Parse(p);
                    seconds = ts.TotalMilliseconds / 1000;
                }
                else
                {
                    seconds = 60 * double.Parse(p);
                }

                return seconds;
            }

            private static String getLabel()
            {
                return getLabel(null);
            }
            private static String getLabel(IActivity activity)
            {
                String unit;
#if ST_2_1
                if (Unit.Equals(Length.Units.Mile))
                {
                    unit = CommonResources.Text.LabelMinPerMile;
                }
                else
                {
                    unit = CommonResources.Text.LabelMinPerKm;
                }
#else
                unit = ZoneFiveSoftware.Common.Data.Measurement.Speed.Label(
                    ZoneFiveSoftware.Common.Data.Measurement.Speed.Units.Pace,
                    new Length(1, GetApplication().SystemPreferences.DistanceUnits));
#endif
                return unit;
            }
            public static String Label
            {
                get
                {
                    return getLabel();
                }
            }
            public static String LabelAbbr
            {
                get
                {
                    return getLabel();
                }
            }
            public static String LabelAbbr2
            {
                get
                {
                    return encPar(LabelAbbr);
                }
            }
            public static String LabelAxis
            {
                get
                {
                    return CommonResources.Text.LabelPace + LabelAbbr2;
                }
            }
            //Some forms uses the label to find if pace/speed is used, get a way to find how
            public static bool isLabelPace(string label)
            {
                //All possible labels should be checked here
                //The "min/" is for changed languages, not foolproof, but that was the way HighScore checked it
                return (
#if ST_2_1
                    label.Equals(CommonResources.Text.LabelMinPerKm) || label.Equals(CommonResources.Text.LabelMinPerMile)
#else
                    label.Equals(ZoneFiveSoftware.Common.Data.Measurement.Speed.Label(
                    ZoneFiveSoftware.Common.Data.Measurement.Speed.Units.Pace,
                    new Length(1, GetApplication().SystemPreferences.DistanceUnits)))
#endif
                    || label.Contains("min/")
                    );
            }
            public static string LabelAbbrAct(IActivity activity)
            {
                return getLabel(activity);
            }
            public static string LabelAbbrAct2(IActivity activity)
            {
                return encPar(LabelAbbrAct(activity));
            }
        }

        /*********************************************************************************/
        public static class PaceOrSpeed
        {
            public static float ConvertFrom(bool isPace, double value, IActivity activity)
            {
                //speed is in m/s
                double speed = value;
                Length.Units du;
                if (activity != null)
                {
#if ST_2_1
                    du = activity.Category.DistanceUnits;
#else
                    du = (isPace) ? 
                        activity.Category.PaceDistance.ValueUnits : activity.Category.SpeedDistance.ValueUnits;
                    //scale, custom unit may be other than one
                    speed = speed /
                        (float)((isPace) ? 
                        activity.Category.PaceDistance.Value : activity.Category.SpeedDistance.Value);
#endif
                }
                else
                {
                    du = GetApplication().SystemPreferences.DistanceUnits;
                }
                //convert from (x*)m/s to (x*)<unit>/s
                speed = Distance.ConvertFrom(speed, du);

                if (isPace)
                {
                    //pace is <time (s)>/<unit>
                    speed = 1 / speed;
                }
                else
                {
                    //speed is <unit>/h
                    speed = speed * 60 * 60;
                }
                return (float)speed;
            }
            public static int DefaultDecimalPrecision(bool isPace)
            {
                if (isPace)
                {
                    return Pace.DefaultDecimalPrecision;
                }
                return Speed.DefaultDecimalPrecision;
            }
            public static String ToString(bool isPace, double speedMS)
            {
                if (isPace)
                {
                    return Pace.ToString(speedMS);
                }
                return Speed.ToString(speedMS);
            }
            public static String ToString(bool isPace, double speedMS, string fmt)
            {
                if (isPace)
                {
                    return Pace.ToString(speedMS, fmt);
                }
                return Speed.ToString(speedMS, fmt);
            }
            public static double ConvertFrom(bool isPace, double speedMS)
            {
                if (isPace)
                {
                    return Pace.ConvertFrom(speedMS);
                }
                return Speed.ConvertFrom(speedMS);
            }

            public static double Parse(bool isPace, string p)
            {
                if (isPace)
                {
                    return Pace.Parse(p);
                }
                return Speed.Parse(p);
            }

            public static String Label(bool isPace)
            {
                if (isPace)
                {
                    return Pace.Label;
                }
                return Speed.Label;
            }
            public static String LabelAbbr(bool isPace)
            {
                if (isPace)
                {
                    return Pace.LabelAbbr;
                }
                return Speed.LabelAbbr;
            }
            public static String LabelAbbr2(bool isPace)
            {
                if (isPace)
                {
                    return Pace.LabelAbbr2;
                }
                return Speed.LabelAbbr2;
            }
            public static String LabelAxis(bool isPace)
            {
                if (isPace)
                {
                    return Pace.LabelAxis;
                }
                return Speed.LabelAxis;
            }
        }
    }
}
