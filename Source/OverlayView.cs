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
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Globalization;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Chart;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections;
using System.Reflection;
using System.ComponentModel;

using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Data.Algorithm;
#if !ST_2_1
using ZoneFiveSoftware.Common.Visuals.Forms;
#endif

using GpsRunningPlugin.Properties;
using GpsRunningPlugin.Util;

namespace GpsRunningPlugin.Source
{
    public partial class OverlayView : UserControl
    {
        //A wrapper for popupforms - could be called from IAction
        public OverlayView(IList<IActivity> activities, bool showDialog)
            : this()
        {
            if (showDialog)
            {
                //Theme and Culture must be set manually
                this.ThemeChanged(m_visualTheme);
                this.UICultureChanged(m_culture);
            }
            this.Activities = activities;
            if (showDialog)
            {
                _showPage = true;
            }
            RefreshPage();
            if (showDialog)
            {
                this.ShowDialog();
            }
        }

        public OverlayView()
        {
            InitializeComponent();
            InitControls();

            heartRate.Checked = Settings.ShowHeartRate;
            pace.Checked = Settings.ShowPace;
            speed.Checked = Settings.ShowSpeed;
            power.Checked = Settings.ShowPower;
            cadence.Checked = Settings.ShowCadence;
            elevation.Checked = Settings.ShowElevation;
            time.Checked = Settings.ShowTime;
            distance.Checked = Settings.ShowDistance;

            if (Settings.UseTimeXAxis)
            {
                useTime.Checked = true;
                useDistance.Checked = false;
            }
            else
            {
                useDistance.Checked = true;
                useTime.Checked = false;
            }

            RefreshColumns();
#if false
            TreeList.Column column = new TreeList.Column("Visible", StringResources.Visible, 50, StringAlignment.Center);
            treeListAct.Columns.Add(column);
            column = new TreeList.Column("Colour", StringResources.Colour, 50, StringAlignment.Center);
            treeListAct.Columns.Add(column);
            column = new TreeList.Column("StartTime", StringResources.ActDate, 200, StringAlignment.Center); 
            treeListAct.Columns.Add(column);
            column = new TreeList.Column("Offset", StringResources.Offset, 50, StringAlignment.Center);
            treeListAct.Columns.Add(column);
            column = new TreeList.Column("Name", StringResources.Name, 100, StringAlignment.Center); 
            treeListAct.Columns.Add(column);
#endif
            treeListAct.CheckBoxes = true;
            treeListAct.MultiSelect = true;
            treeListAct.RowDataRenderer.RowAlternatingColors = true;
            treeListAct.LabelProvider = new ActivityLabelProvider();
            treeListAct.CheckedChanged += new TreeList.ItemEventHandler(treeView_CheckedChanged);
            treeListAct.ContextMenuStrip = treeListContextMenuStrip;

            actionBanner1.Text = StringResources.OverlayChart;
            actionBanner1.MenuClicked += actionBanner1_MenuClicked;
            
            chart.SelectData += new ChartBase.SelectDataHandler(chart_SelectData);
            chart.SelectingData += new ChartBase.SelectDataHandler(chart_SelectingData);
            chart.Click += new EventHandler(chart_Click);
        }

        private void RefreshColumns()
        {

            treeListAct.Columns.Clear();
            foreach (string id in Settings.TreeListPermanentActColumns)
            {
                foreach (
#if ST_2_1
                    ListItemInfo
#else
IListColumnDefinition
#endif
 columnDef in OverlayColumnIds.PermanentColumnDefs())
                {
                    if (columnDef.Id == id)
                    {
                        TreeList.Column column = new TreeList.Column(
                            columnDef.Id,
                            columnDef.Text(columnDef.Id),
                            columnDef.Width,
                            columnDef.Align
                        );
                        treeListAct.Columns.Add(column);
                        break;
                    }
                }
            }

            foreach (string id in Settings.TreeListActColumns)
            {
                foreach (
#if ST_2_1
                    ListItemInfo
#else
                    IListColumnDefinition
#endif
                    columnDef in OverlayColumnIds.ColumnDefs())
                {
                    if (columnDef.Id == id)
                    {
                        TreeList.Column column = new TreeList.Column(
                            columnDef.Id,
                            columnDef.Text(columnDef.Id),
                            columnDef.Width,
                            columnDef.Align
                        );
                        treeListAct.Columns.Add(column);
                        break;
                    }
                }
            }
        }
        
        public void InitControls()
        {
            SizeChanged += new EventHandler(OverlayView_SizeChanged);
        }

        public void ShowDialog()
        {
            popupForm = new Form();
            popupForm.Controls.Add(this);
            popupForm.Size = Settings.WindowSize;
            //6 is the distance between controls
            popupForm.MinimumSize = new System.Drawing.Size(6 + elevation.Width + elevation.Left + this.Width - btnSaveImage.Left, 0);
            this.Size = new Size(Parent.Size.Width - 17, Parent.Size.Height - 38);
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom)));
            popupForm.SizeChanged += new EventHandler(form_SizeChanged);
            setSize();
            if (activities.Count == 1)
                popupForm.Text = Resources.O1;
            else
                popupForm.Text = String.Format(Resources.O2, activities.Count);
            popupForm.Icon = Icon.FromHandle(Properties.Resources.Image_32_Overlay.GetHicon());
            Parent.SizeChanged += new EventHandler(Parent_SizeChanged);
            popupForm.StartPosition = FormStartPosition.CenterScreen;
            popupForm.ShowDialog();
        }

        public IList<IActivity> Activities
        {
            get
            {
                return activities;
            }
            set
            {
                if (activities != null)
                {
                    foreach (IActivity activity in activities)
                    {
#if ST_2_1
                        activity.DataChanged -= new NotifyDataChangedEventHandler(activity_DataChanged);
#else
                        activity.PropertyChanged -= new PropertyChangedEventHandler(Activity_PropertyChanged);
#endif
                    }
                }
                activities.Clear();
                foreach (IActivity activity in value)
                {
                    activities.Add(activity);
#if ST_2_1
                    activity.DataChanged += new NotifyDataChangedEventHandler(activity_DataChanged);
#else
                    activity.PropertyChanged += new PropertyChangedEventHandler(Activity_PropertyChanged);
#endif

                }
                activities.Sort(new ActivityDateComparer());
                nextIndex = 0;
                actWrappers.Clear();
                foreach (IActivity activity in activities)
                {
                    actWrappers.Add(new ActivityWrapper(activity, newColor()));
                }

                CommonData.refActWrapper = null;
                RefreshPage();
            }
        }
        public void RefreshPage()
        {
            if (_showPage)
            {
                updateActivities();
                updateChart();
            }
        }
        private void updateActivities()
        {
            //Temporary
            if (Plugin.Verbose > 100 && Settings.uniqueRoutes != null)
            {
                MethodInfo methodInfo = Settings.uniqueRoutes.GetMethod("findSimilarStretch");
                IDictionary<IActivity, IList<IList<int>>> result =
                    (IDictionary<IActivity, IList<IList<int>>>)methodInfo.Invoke(this, new object[] { activities[0], activities });

            }
            activities.Sort(new ActivityDateComparer());

            nextIndex = 0;

            treeListAct.RowData = actWrappers;
            foreach(ActivityWrapper wrapper in actWrappers)
            {
                treeListAct.SetChecked(wrapper, true);
            }
        }

        private class ActivityDateComparer : Comparer<IActivity>
        {
            public override int Compare(IActivity x, IActivity y)
            {
                return x.StartTime.CompareTo(y.StartTime);
            }
        }

        public bool HidePage()
        {
            _showPage = false;
            return true;
        }
        public void ShowPage(string bookmark)
        {
            bool changed = !_showPage;
            _showPage = true;
            if (changed) { RefreshPage(); }
        }
        public void ThemeChanged(ITheme visualTheme)
        {
            m_visualTheme = visualTheme;

            this.chart.ThemeChanged(visualTheme);
            this.panel1.ThemeChanged(visualTheme);
            this.panel2.ThemeChanged(visualTheme);
            this.actionBanner1.ThemeChanged(visualTheme);
            this.treeListAct.ThemeChanged(visualTheme);
            //Non ST controls
            chartBackgroundPanel.BackColor = visualTheme.Window;
            this.BackColor = visualTheme.Control;
        }

        public void UICultureChanged(CultureInfo culture)
        {
            m_culture = culture;
            actionBanner1.Text = StringResources.OverlayChart;
            showMeanMenuItem.Text = Resources.BCA;
            showRollingAverageMenuItem.Text = Resources.BMA;

            tableSettingsMenuItem.Text = StringResources.TableSettings;

            labelXaxis.Text = StringResources.XAxis + ":";
            labelYaxis.Text = StringResources.YAxis + ":";
            useTime.Text = CommonResources.Text.LabelTime;
            useDistance.Text = CommonResources.Text.LabelDistance;
            heartRate.Text = CommonResources.Text.LabelHeartRate;
            pace.Text = CommonResources.Text.LabelPace;
            speed.Text = CommonResources.Text.LabelSpeed;
            power.Text = CommonResources.Text.LabelPower;
            cadence.Text = CommonResources.Text.LabelCadence;
            elevation.Text = CommonResources.Text.LabelElevation;
            time.Text = CommonResources.Text.LabelTime;
            distance.Text = CommonResources.Text.LabelDistance;

            showMenuItem.Text = StringResources.Show;
            showDiffMenuItem.Text = StringResources.Show + " " + StringResources.Difference.ToLower();

            showHRMenuItem.Text = StringResources.Show + " " + CommonResources.Text.LabelHeartRate;
            showPaceMenuItem.Text = StringResources.Show + " " + CommonResources.Text.LabelPace;
            showSpeedMenuItem.Text = StringResources.Show + " " + CommonResources.Text.LabelSpeed;
            showPowerMenuItem.Text = StringResources.Show + " " + CommonResources.Text.LabelPower;
            showCadenceMenuItem.Text = StringResources.Show + " " + CommonResources.Text.LabelCadence;
            showElevationMenuItem.Text = StringResources.Show + " " + CommonResources.Text.LabelElevation;
            showTimeMenuItem.Text = StringResources.Show + " " + CommonResources.Text.LabelTime;
            showDistanceMenuItem.Text = StringResources.Show + " " + CommonResources.Text.LabelDistance;

            showHRDiffMenuItem.Text = StringResources.Show + " " + CommonResources.Text.LabelHeartRate.ToLower() + " " + StringResources.Difference;
            showTimeDiffMenuItem.Text = StringResources.Show + " " + CommonResources.Text.LabelTime.ToLower() + " " + StringResources.Difference;
            showDistDiffMenuItem.Text = StringResources.Show + " " + CommonResources.Text.LabelDistance.ToLower() + " " + StringResources.Difference;

            offsetStripTextBox.Text = CommonResources.Text.LabelActivity + " " + StringResources.Offset.ToLower();
            averageStripTextBox.Text = StringResources.MovingAverage + " " + StringResources.Width;
            
            int max = Math.Max(labelXaxis.Location.X + labelXaxis.Size.Width,
                                labelYaxis.Location.X + labelYaxis.Size.Width) + 5;
            useTime.Location = new Point(max, labelXaxis.Location.Y);
            correctUI(new Control[] { useTime, useDistance });
            heartRate.Location = new Point(max, labelYaxis.Location.Y);
            correctUI(new Control[] { heartRate, pace, speed, power, cadence, elevation, time, distance });

            RefreshPage();
        }
        private void correctUI(IList<Control> comp)
        {
            Control prev = null;
            foreach (Control c in comp)
            {
                if (prev != null)
                {
                    c.Location = new Point(prev.Location.X + prev.Size.Width,
                                           prev.Location.Y);
                }
                prev = c;
            }
        }
        private void setSize()
        {
          // Using SplitContainers eliminates the adjustions previously required
        }

        /*************************************************/
        private int nextIndex;

        private Color getColor(int color)
        {
            switch (color)
            {
                case 0: return Color.Blue;
                case 1: return Color.Red;
                case 2: return Color.Green;
                case 3: return Color.Orange;
                case 4: return Color.Plum;
                case 5: return Color.HotPink;
                case 6: return Color.Gold;
                case 7: return Color.Silver;
                case 8: return Color.YellowGreen;
                case 9: return Color.Turquoise;
            }
            return Color.Black;
        }

        private Color newColor()
        {
            int color = nextIndex;
            nextIndex = (nextIndex + 1) % 10;
            return getColor(color);
        }

        private void addSeries(Interpolator interpolator, 
            CanInterpolater canInterpolator, IAxis axis,
            GetDataSeries getDataSeries)
        {
            IList<ChartDataSeries> list = buildSeries(interpolator, canInterpolator, axis, getDataSeries);
            IList<ChartDataSeries> averages = new List<ChartDataSeries>();
            foreach (ChartDataSeries series in list)
            {
                series.ValueAxis = axis;
                chart.DataSeries.Add(series);
                if (Settings.ShowMovingAverage)
                {
                    averages.Add(makeMovingAverage(series, axis));
                }
            }
            if (Settings.ShowCategoryAverage && activities.Count > 1)
            {
                chart.DataSeries.Add(getCategoryAverage(axis,list));
                if (Settings.ShowMovingAverage)
                {
                    ChartDataSeries average = getCategoryAverage(axis, averages);
                    average.LineWidth = 2;
                    chart.DataSeries.Add(average);
                }
            }
        }

        private ChartDataSeries makeMovingAverage(ChartDataSeries series, IAxis axis)
        {
            if (series.Points.Count == 0) return new ChartDataSeries(chart, axis);
            double size;
            if (Settings.UseTimeXAxis)
            {
                size = Settings.MovingAverageTime; //No ConvertFrom, time is always in seconds
            }
            else
            {
                size = UnitUtil.Distance.ConvertFrom(Settings.MovingAverageLength);
            }
            ChartDataSeries average = new ChartDataSeries(chart, axis);
            Queue<double> queueX = new Queue<double>(), queueSum = new Queue<double>();
            double sum = 0;
            double lastX = 0, lastY = 0, firstX=0;
            bool first = true;
            foreach (PointF point in series.Points.Values)
            {
                if (!first)
                {
                    double diffX = point.X - lastX;
                    double diffY = point.Y - lastY;
                    double area = diffX * lastY + diffY * diffX / 2.0; 
                    sum += area;
                    if (size > 0)
                    {
                        queueX.Enqueue(point.X);
                        queueSum.Enqueue(area);
                    }
                }
                else
                {
                    firstX = point.X;
                }
                float y = float.NaN;
                if (first && size == 0)
                {
                    y = point.Y;
                }
                else
                {
                    if (size == 0)
                    {
                        y = (float)(sum / (point.X - firstX));
                    }
                    else
                    {
                        if (queueX.Count > 0 && 
                            size <= point.X - queueX.Peek())
                        {
                            float diffX = (float)(point.X - queueX.Dequeue());
                            sum -= queueSum.Dequeue();
                            y = (float)(sum / diffX);
                        }
                    }
                }
                if (!y.Equals(float.NaN) && 
                    !average.Points.ContainsKey(point.X))
                {
                    average.Points.Add(point.X, new PointF(point.X, y));
                }
                lastX = point.X;
                lastY = point.Y;
                if (first) first = false;                
            }
            chart.DataSeries.Add(average);
            average.LineColor = series.LineColor;
            average.LineWidth = 2;
            series2activity.Add(average, series2activity[series]);
            return average;
        }
        
        private ChartDataSeries getCategoryAverage(IAxis axis,
            IList<ChartDataSeries> list)
        {
            ChartDataSeries average = new ChartDataSeries(chart, axis);
            SortedList<float, bool> xs = new SortedList<float, bool>();
            foreach (ChartDataSeries series in list)
            {
                foreach (PointF point in series.Points.Values)
                {
                    if (!xs.ContainsKey(point.X))
                    {
                        xs.Add(point.X, true);
                    }
                }
            }
            foreach (float x in xs.Keys)
            {
                int seen = 0;
                float y = 0;
                foreach (ChartDataSeries series in list)
                {
                    float theX = x;
                    float theY = series.GetYValueAtX(ref theX);
                    if (!theY.Equals(float.NaN))
                    {
                        y += theY;
                        seen++;
                    }
                }
                if (seen > 1 &&
                    average.Points.IndexOfKey(x) == -1)
                {
                    average.Points.Add(x, new PointF(x, y / seen));
                }
             }
            return average;
        }

        private void updateChart()
        {
            //TODO: add show working
            chart.Visible = false;
            chart.UseWaitCursor = true;
//            this.chart.BackgroundImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Hourglass16;
//            this.chart.BackgroundImageLayout = ImageLayout.Center;
            chart.BeginUpdate();
            chart.AutozoomToData(false);
            chart.DataSeries.Clear();
            chart.YAxisRight.Clear();
            series2activity.Clear();
            bool useRight = false;

            if (Settings.UseTimeXAxis)
            {
                chart.XAxis.Formatter = new Formatter.SecondsToTime();
                chart.XAxis.Label = UnitUtil.Time.LabelAxis;
            }
            else
            {
                chart.XAxis.Formatter = new Formatter.General(UnitUtil.Distance.DefaultDecimalPrecision);
                chart.XAxis.Label = UnitUtil.Distance.LabelAxis; ;
            }

            if (Settings.ShowHeartRate)
            {
                nextIndex = 0;
                useRight = true;
                chart.YAxis.Formatter = new Formatter.General(UnitUtil.HeartRate.DefaultDecimalPrecision);
                chart.YAxis.Label = UnitUtil.HeartRate.LabelAxis;
                addSeries(
                    delegate(float value)
                    {
                        return value;
                    },
                    delegate(ActivityInfo info)
                    {
                        return info.Activity.HeartRatePerMinuteTrack != null;
                    },
                    chart.YAxis,
                    delegate(ActivityInfo info)
                    {
                        return info.Activity.HeartRatePerMinuteTrack;
                    }
                    );
            }
            if (Settings.ShowPace)
            {
                IAxis axis;
                if (useRight)
                {
                    axis = new RightVerticalAxis(chart);
                    chart.YAxisRight.Add(axis);
                }
                else
                {
                    axis = chart.YAxis;
                    useRight = true;
                }
                nextIndex = 0;
                axis.Formatter = new Formatter.SecondsToTime();
                axis.Label = UnitUtil.Pace.LabelAxis;
                axis.SmartZoom = true;
                addSeries(
                    delegate(float value)
                    {
                        return UnitUtil.Pace.ConvertFrom(value);
                    },
                    delegate(ActivityInfo info)
                    {
                        return info.SmoothedSpeedTrack.Count > 0;
                    },
                    axis,
                    delegate(ActivityInfo info)
                    {
                        return info.SmoothedSpeedTrack;
                    });
            }
            if (Settings.ShowSpeed)
            {
                IAxis axis;
                if (useRight)
                {
                    axis = new RightVerticalAxis(chart);
                    chart.YAxisRight.Add(axis);
                }
                else
                {
                    axis = chart.YAxis;
                    useRight = true;
                }
                nextIndex = 0;
                axis.Formatter = new Formatter.General(UnitUtil.Speed.DefaultDecimalPrecision);
                axis.Label = UnitUtil.Speed.LabelAxis;
                addSeries(
                    delegate(float value)
                    {
                        return UnitUtil.Speed.ConvertFrom(value);
                    },
                    delegate(ActivityInfo info) 
                    { 
                        return info.SmoothedSpeedTrack.Count > 0; 
                    },
                    axis,
                    delegate(ActivityInfo info)
                    {
                        return info.SmoothedSpeedTrack;
                    });
            }
            if (Settings.ShowPower)
            {
                IAxis axis;
                if (useRight)
                {
                    axis = new RightVerticalAxis(chart);
                    chart.YAxisRight.Add(axis);
                }
                else
                {
                    axis = chart.YAxis;
                    useRight = true;
                }
                nextIndex = 0;
                axis.Formatter = new Formatter.General(UnitUtil.Power.DefaultDecimalPrecision);
                axis.Label = UnitUtil.Power.LabelAxis;
                addSeries(
                    delegate(float value)
                    {
                        return value;
                    },
                    delegate(ActivityInfo info) { return info.SmoothedPowerTrack.Count > 0; },
                    axis,
                    delegate(ActivityInfo info)
                    {
                        return info.SmoothedPowerTrack;
                    });
            }
            if (Settings.ShowCadence)
            {
                IAxis axis;
                if (useRight)
                {
                    axis = new RightVerticalAxis(chart);
                    chart.YAxisRight.Add(axis);
                }
                else
                {
                    axis = chart.YAxis;
                    useRight = true;
                }
                nextIndex = 0;
                axis.Formatter = new Formatter.General(UnitUtil.Cadence.DefaultDecimalPrecision);
                axis.Label = UnitUtil.Cadence.LabelAxis;
                addSeries(
                     delegate(float value)
                     {
                         return value;
                     },
                     delegate(ActivityInfo info) { return info.SmoothedCadenceTrack.Count > 0; },
                     axis,
                    delegate(ActivityInfo info)
                    {
                        return info.SmoothedCadenceTrack;
                    });
            }
            if (Settings.ShowElevation)
            {
                IAxis axis;
                if (useRight)
                {
                    axis = new RightVerticalAxis(chart);
                    chart.YAxisRight.Add(axis);
                }
                else
                {
                    axis = chart.YAxis;
                    useRight = true;
                }
                nextIndex = 0;
                axis.Formatter = new Formatter.General(UnitUtil.Elevation.DefaultDecimalPrecision);
                axis.ChangeAxisZoom(new Point(0, 0), new Point(10, 10));
                axis.Label = UnitUtil.Elevation.LabelAxis;
                addSeries(
                    delegate(float value)
                    {
                        return UnitUtil.Elevation.ConvertFrom(value);
                    },
                    delegate(ActivityInfo info)
                    {
                        return info.SmoothedElevationTrack.Count > 0;
                    },
                    axis,
                    delegate(ActivityInfo info)
                    {
                        return info.SmoothedElevationTrack;
                    });
            }
            if (Settings.ShowTime)
            {
                IAxis axis;
                if (useRight)
                {
                    axis = new RightVerticalAxis(chart);
                    chart.YAxisRight.Add(axis);
                }
                else
                {
                    axis = chart.YAxis;
                    useRight = true;
                }
                nextIndex = 0;
                axis.Formatter = new Formatter.SecondsToTime();
                axis.ChangeAxisZoom(new Point(0, 0), new Point(10, 10));
                axis.Label = UnitUtil.Time.LabelAxis;

                addSeries(
                    delegate(float value)
                    {
                        return value;                        
                    },
                    delegate(ActivityInfo info)
                    {
                        return info.SmoothedSpeedTrack. Count > 0;
                    },
                    axis,
                    delegate(ActivityInfo info)
                    {
                        INumericTimeDataSeries TimeTrack = new ZoneFiveSoftware.Common.Data.NumericTimeDataSeries(info.ActualDistanceMetersTrack);
                        INumericTimeDataSeries ModTimeTrack;
                        bool includeStopped = false;
#if ST_2_1
                        // If UseEnteredData is set, exclude Stopped
                        if (info.Activity.UseEnteredData == false && info.Time.Equals(info.ActualTrackTime))
                        {
                            includeStopped = true;
                        }
#else
                        includeStopped = Plugin.GetApplication().SystemPreferences.AnalysisSettings.IncludeStopped;
#endif
                        CorrectTimeDataSeriesForPauses(info, includeStopped, TimeTrack, out ModTimeTrack);

                        // Copy the modified times into the value of TimeTrack - the time values of TimeTrack will be modified later 
                        for (int i = 0; i < ModTimeTrack.Count; i++)
                        {
                            TimeValueEntry<float> entry = (TimeValueEntry<float>)TimeTrack[i];
                            entry.Value = ModTimeTrack[i].ElapsedSeconds;
                        }
                        //TimeValueEntry<float> lastEntry = (TimeValueEntry<float>)TimeTrack[TimeTrack.Count-1];
                        //lastEntry.Value = ModTimeTrack[ModTimeTrack.Count-1].ElapsedSeconds;
                        
                        return TimeTrack;
                    });
            }
            if (Settings.ShowDistance)
            {
                IAxis axis;
                if (useRight)
                {
                    axis = new RightVerticalAxis(chart);
                    chart.YAxisRight.Add(axis);
                }
                else
                {
                    axis = chart.YAxis;
                    useRight = true;
                }
                nextIndex = 0;
                axis.Formatter = new Formatter.General(UnitUtil.Distance.DefaultDecimalPrecision);
                axis.Label = UnitUtil.Distance.LabelAxis;
                addSeries(
                    delegate(float value)
                    {
                        return UnitUtil.Distance.ConvertFrom(value);
                    },
                    delegate(ActivityInfo info)
                    {
                        bool includeStopped = false;
#if ST_2_1
                        // If UseEnteredData is set, exclude Stopped
                        if (info.Activity.UseEnteredData == false && info.Time.Equals(info.ActualTrackTime))
                        {
                            includeStopped = true;
                        }
#else
                        includeStopped = Plugin.GetApplication().SystemPreferences.AnalysisSettings.IncludeStopped;
#endif
                        if (includeStopped) 
                        { 
                            return info.ActualDistanceMetersTrack.Count > 0; 
                        }
                        else 
                        { 
                            return info.MovingDistanceMetersTrack.Count > 0;
                        }
                    },
                    axis,
                    delegate(ActivityInfo info)
                    {
                        bool includeStopped = false;
#if ST_2_1
                        // If UseEnteredData is set, exclude Stopped
                        if (info.Activity.UseEnteredData == false && info.Time.Equals(info.ActualTrackTime))
                        {
                            includeStopped = true;
                        }
#else
                        includeStopped = Plugin.GetApplication().SystemPreferences.AnalysisSettings.IncludeStopped;
#endif
                        if (includeStopped) 
                        { 
                            return info.ActualDistanceMetersTrack; 
                        }
                        else 
                        { 
                            return info.MovingDistanceMetersTrack; 
                        }
                    });
            }
            if (Settings.ShowDiffHeartRate)
            {
                IAxis axis;
                if (useRight)
                {
                    axis = new RightVerticalAxis(chart);
                    chart.YAxisRight.Add(axis);
                }
                else
                {
                    axis = chart.YAxis;
                    useRight = true;
                }
                nextIndex = 0;
                axis.Formatter = new Formatter.General(UnitUtil.HeartRate.DefaultDecimalPrecision);
                axis.Label = UnitUtil.HeartRate.LabelAxis;
                addSeries(
                    delegate(float value)
                    {
                        return value;
                    },
                    delegate(ActivityInfo info)
                    {
                        if (CommonData.refActWrapper == null)
                            return false;
                        else
                            return info.Activity.HeartRatePerMinuteTrack != null &&
                                   CommonData.refActWrapper.Activity.HeartRatePerMinuteTrack != null;
                    },
                    chart.YAxis,
                    delegate(ActivityInfo info)
                    {

                        bool includeStopped = false;
#if ST_2_1
                        // If UseEnteredData is set, exclude Stopped
                        if (info.Activity.UseEnteredData == false && info.Time.Equals(info.ActualTrackTime))
                        {
                            includeStopped = true;
                        }
#else
                        includeStopped = Plugin.GetApplication().SystemPreferences.AnalysisSettings.IncludeStopped;
#endif
                        ActivityInfo refInfo = ActivityInfoCache.Instance.GetInfo(CommonData.refActWrapper.Activity);

                        INumericTimeDataSeries refTrack, actTrack, modRefTrack, modActTrack, modTrack;
                        actTrack = info.Activity.HeartRatePerMinuteTrack;
                        refTrack = refInfo.Activity.HeartRatePerMinuteTrack;

                        // Remove pauses in order to be able to calculate difference
                        CorrectTimeDataSeriesForPauses(info, includeStopped, actTrack, out modActTrack);
                        CorrectTimeDataSeriesForPauses(refInfo, includeStopped, refTrack, out modRefTrack);
                        INumericTimeDataSeries track = new ZoneFiveSoftware.Common.Data.NumericTimeDataSeries();
                        foreach (ITimeValueEntry<float> p in modActTrack)
                        {
                            DateTime refActualTime = modRefTrack.StartTime.AddSeconds(p.ElapsedSeconds);
                            DateTime actualTime = modActTrack.StartTime.AddSeconds(p.ElapsedSeconds);

                            ITimeValueEntry<float> refEntry = modRefTrack.GetInterpolatedValue(refActualTime);
                            if (refEntry != null)
                            {
                                track.Add(actualTime, p.Value - refEntry.Value);
                            }
                        }
                        AddPausesToTimeDataSeries(info, includeStopped, track, out modTrack);
                        return modTrack;
                    }
                    );
            }
            if (Settings.ShowDiffDistance)
            {
                IAxis axis;
                if (useRight)
                {
                    axis = new RightVerticalAxis(chart);
                    chart.YAxisRight.Add(axis);
                }
                else
                {
                    axis = chart.YAxis;
                    useRight = true;
                }
                nextIndex = 0;
                axis.Formatter = new Formatter.General(UnitUtil.Distance.DefaultDecimalPrecision);
                axis.Label = UnitUtil.Distance.LabelAxis;
                addSeries(
                    delegate(float value)
                    {
                        return UnitUtil.Distance.ConvertFrom(value);
                    },
                    delegate(ActivityInfo info)
                    {
                        bool includeStopped = false;
#if ST_2_1
                        // If UseEnteredData is set, exclude Stopped
                        if (info.Activity.UseEnteredData == false && info.Time.Equals(info.ActualTrackTime))
                        {
                            includeStopped = true;
                        }
#else
                        includeStopped = Plugin.GetApplication().SystemPreferences.AnalysisSettings.IncludeStopped;
#endif

                        if (CommonData.refActWrapper == null)
                            return false;
                        else
                        {
                            ActivityInfo refInfo = ActivityInfoCache.Instance.GetInfo(CommonData.refActWrapper.Activity);

                            if (includeStopped)
                            {
                                return info.ActualDistanceMetersTrack.Count > 0 &&
                                       refInfo.ActualDistanceMetersTrack.Count > 0;
                            }
                            else
                            {
                                return info.MovingDistanceMetersTrack.Count > 0 &&
                                       refInfo.MovingDistanceMetersTrack.Count > 0;
                            }
                        }
                    },
                    axis,
                    delegate(ActivityInfo info)
                    {
                        bool includeStopped = false;
#if ST_2_1
                        // If UseEnteredData is set, exclude Stopped
                        if (info.Activity.UseEnteredData == false && info.Time.Equals(info.ActualTrackTime))
                        {
                            includeStopped = true;
                        }
#else
                        includeStopped = Plugin.GetApplication().SystemPreferences.AnalysisSettings.IncludeStopped;
#endif
                        ActivityInfo refInfo = ActivityInfoCache.Instance.GetInfo(CommonData.refActWrapper.Activity);

                        INumericTimeDataSeries refTrack, actTrack, modRefTrack, modActTrack, modTrack;
                        if (includeStopped)
                        {
                            actTrack = info.ActualDistanceMetersTrack;
                            refTrack = refInfo.ActualDistanceMetersTrack;
                        }
                        else
                        {
                            actTrack = info.MovingDistanceMetersTrack;
                            refTrack = refInfo.MovingDistanceMetersTrack;
                        }
                       
                        // Remove pauses in order to be able to calculate difference
                        CorrectTimeDataSeriesForPauses(info, includeStopped, actTrack, out modActTrack);
                        CorrectTimeDataSeriesForPauses(refInfo, includeStopped, refTrack, out modRefTrack);
                        INumericTimeDataSeries track = new ZoneFiveSoftware.Common.Data.NumericTimeDataSeries();
                        foreach (ITimeValueEntry<float> p in modActTrack)
                        {
                            DateTime refActualTime = modRefTrack.StartTime.AddSeconds(p.ElapsedSeconds);
                            DateTime actualTime = modActTrack.StartTime.AddSeconds(p.ElapsedSeconds);

                            ITimeValueEntry<float> refDist = modRefTrack.GetInterpolatedValue(refActualTime);
                            if (refDist != null)
                            {
                                track.Add(actualTime, p.Value - refDist.Value);
                            }
                        }
                        AddPausesToTimeDataSeries(info, includeStopped, track, out modTrack);
                        return modTrack;

                    });
            }


            //chart.AutozoomToData is the slowest part of this plugin
            chart.AutozoomToData(true);
            chart.Refresh();
            chart.EndUpdate();
            chart.UseWaitCursor = false;
            chart.Visible = true;
        }

        private void CorrectTimeDataSeriesForPauses(ActivityInfo info, bool includeStopped, INumericTimeDataSeries dataSeries, out INumericTimeDataSeries newDataSeries)
        {
            IValueRangeSeries<DateTime> pauses;
            if (includeStopped)
            {
                pauses = info.Activity.TimerPauses;
            }
            else
            {
                pauses = info.NonMovingTimes;
            }
            newDataSeries = new ZoneFiveSoftware.Common.Data.NumericTimeDataSeries();
            newDataSeries.AllowMultipleAtSameTime = true;
            foreach (TimeValueEntry<float> entry in dataSeries)
            {
                DateTime entryTime = dataSeries.EntryDateTime(entry);
                TimeSpan elapsed = DateTimeRangeSeries.TimeNotPaused(info.Activity.StartTime, entryTime, pauses);                
                newDataSeries.Add(dataSeries.StartTime.Add(elapsed), entry.Value);
            }
        }

        private void AddPausesToTimeDataSeries(ActivityInfo info, bool includeStopped, INumericTimeDataSeries dataSeries, out INumericTimeDataSeries newDataSeries)
        {
            IValueRangeSeries<DateTime> pauses;
            if (includeStopped)
            {
                pauses = info.Activity.TimerPauses;
            }
            else
            {
                pauses = info.NonMovingTimes;
            }
            newDataSeries = new ZoneFiveSoftware.Common.Data.NumericTimeDataSeries();
            newDataSeries.AllowMultipleAtSameTime = true;
            foreach (TimeValueEntry<float> entry in dataSeries)
            {
                DateTime newEntryTime = DateTimeRangeSeries.AddTimeAndPauses(info.Activity.StartTime, new TimeSpan(0,0, (int)entry.ElapsedSeconds), pauses);
                newDataSeries.Add(newEntryTime, entry.Value);
            }
        }
        
        
        private IList<ChartDataSeries> buildSeries(
            Interpolator interpolator, CanInterpolater canInterpolator, IAxis axis,
            GetDataSeries getDataSeriess)
        {
            IList<ChartDataSeries> list = new List<ChartDataSeries>();
            int index = 0;            
            
            foreach (ActivityWrapper actWrapper in actWrappers)
            {
                IActivity activity = actWrapper.Activity;
                ArrayList checkedWrappers = (ArrayList)treeListAct.CheckedElements;
                if (checkedWrappers.Contains(actWrapper))
                {
                    double offset=0;
                    if (Settings.UseTimeXAxis)
                    {
                        offset = actWrapper.TimeOffset.TotalSeconds;
                    }
                    else
                    {
                        offset = actWrapper.DistanceOffset;
                    }
                    ChartDataSeries series = getDataSeries(
                        interpolator, 
                        canInterpolator, 
                        ActivityInfoCache.Instance.GetInfo(activity),
                        axis,
                        getDataSeriess,
                        offset);
                    series2activity.Add(series, activity);
                    list.Add(series);
                }
                else
                {
                    newColor();
                }
                index++;
            }
            return list;
        }

        private delegate double Interpolator(float value);
        private delegate bool CanInterpolater(ActivityInfo info);
        private delegate INumericTimeDataSeries GetDataSeries(ActivityInfo info);

        private ChartDataSeries getDataSeries(Interpolator interpolator,
            CanInterpolater canInterpolater, ActivityInfo info, IAxis axis,
            GetDataSeries getDataSeries,
            double offset)
        {
            ChartDataSeries series = new ChartDataSeries(chart, axis);
            if (!canInterpolater(info))
            {
                newColor();
                return series;
            }
            bool first = true;
            float priorElapsed = float.NaN;
            //This should be retrieved per activity, if that is changed in ST
            bool includeStopped = false;
#if ST_2_1
            // If UseEnteredData is set, exclude Stopped
            if (info.Activity.UseEnteredData == false && info.Time.Equals(info.ActualTrackTime))
            {
                includeStopped = true;
            }
#else
            includeStopped = Plugin.GetApplication().SystemPreferences.AnalysisSettings.IncludeStopped;
#endif
            INumericTimeDataSeries data, timeModData;
            CorrectTimeDataSeriesForPauses( info, includeStopped, getDataSeries(info), out timeModData);
            if (Settings.UseTimeXAxis)
                data = timeModData; // x-axis is time, use time with pauses excluded
            else
                data = getDataSeries(info); // x-axis is distance. Distance need to be looked up using original time.
            foreach (ITimeValueEntry<float> entry in data)
            {
                float elapsed = entry.ElapsedSeconds;
				if ( elapsed != priorElapsed )
				{
					float x = float.NaN;
                    if (Settings.UseTimeXAxis)
					{
						x = (float)( elapsed + offset );
					}
					else
					{
                        ITimeValueEntry<float> entryMoving;
                        if (includeStopped)
                        {
                            entryMoving = info.ActualDistanceMetersTrack.GetInterpolatedValue(info.Activity.StartTime.AddSeconds(entry.ElapsedSeconds));
                        }
                        else
                        {
                            entryMoving = info.MovingDistanceMetersTrack.GetInterpolatedValue(info.Activity.StartTime.AddSeconds(entry.ElapsedSeconds));
                        }
                        if (entryMoving != null && (first || (!first && entryMoving.Value > 0)))
						{
							x = (float)UnitUtil.Distance.ConvertFrom( entryMoving.Value + offset );
						}
					}
					float y = (float)interpolator( entry.Value );
					if ( !x.Equals( float.NaN ) && !float.IsInfinity( y ) &&
						series.Points.IndexOfKey( x ) == -1 )
					{
						series.Points.Add( x, new PointF( x, y ) );
					}
				}
                priorElapsed = elapsed;
                first = false;
            }
            series.LineColor = newColor();
            return series;
        }
        
        private void form_SizeChanged(object sender, EventArgs e)
        {
            if (popupForm != null)
            {
                Settings.WindowSize = popupForm.Size;
            }
            OverlayView_SizeChanged(sender, e);
        }

        private void OverlayView_SizeChanged(object sender, EventArgs e)
        {
            setSize();
        }

#if ST_2_1
        private void activity_DataChanged(object sender, NotifyDataChangedEventArgs e)
        {
            updateChart();
        }
#else
        private void Activity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            updateChart();
        }
#endif

        private void treeView_CheckedChanged(object sender, EventArgs e)
        {
            updateChart();
        }

        void chart_Click(object sender, EventArgs e)
        {
            treeListAct.SelectedItems = new object[] { };
            bSelectDataFlag = false;

			if ( bSelectingDataFlag )
			{
				bSelectingDataFlag = false;
				return;
			}
        }

		void chart_SelectingData(object sender, ChartBase.SelectDataEventArgs e)
		{
            if ((lastSelectedSeries != null) && (lastSelectedSeries != e.DataSeries))
            {
                treeListAct.SelectedItems = new object[] { };
            }
			lastSelectedSeries = e.DataSeries;
			bSelectingDataFlag = true;
		}

        void chart_SelectData(object sender, ChartBase.SelectDataEventArgs e)
        {
            if (e != null && e.DataSeries != null)
            {
                // Select the row of the treeview
                if (series2activity.ContainsKey(e.DataSeries))
                {
                    treeListAct.SelectedItems = new object[] { actWrappers[activities.IndexOf(series2activity[e.DataSeries])] };
                }
                else
                {
                    treeListAct.SelectedItems = new object[] { };
                }
                bSelectingDataFlag = false;
                if (bSelectDataFlag)
                    chart_SelectingData(sender, e);
                bSelectDataFlag = true;
            }
        }

        private void Parent_SizeChanged(object sender, EventArgs e)
        {
            setSize();
        }

        private void useTime_CheckedChanged(object sender, EventArgs e)
        {
            if (!Settings.UseTimeXAxis)
            {
                Settings.UseTimeXAxis = true;
                updateChart();
                treeListAct.Refresh();
            }
        }

        private void useDistance_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.UseTimeXAxis)
            {
                Settings.UseTimeXAxis = false;
                updateChart();
                treeListAct.Refresh();
            }
        }

        private void heartRate_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ShowHeartRate = heartRate.Checked;
            updateChart();
        }

        private void pace_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ShowPace = pace.Checked;
            updateChart();
        }

        private void speed_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ShowSpeed = speed.Checked;
            updateChart();
        }

        private void power_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ShowPower = power.Checked;
            updateChart();
        }

        private void cadence_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ShowCadence = cadence.Checked;
            updateChart();
        }

        private void elevation_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ShowElevation = elevation.Checked;
            updateChart();
        }

        private void time_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ShowTime = time.Checked;
            updateChart();
        }

        private void distance_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ShowDistance = distance.Checked;
            updateChart();
        }

		private void btnSaveImage_Click( object sender, EventArgs e )
		{
#if ST_2_1
            OverlaySaveImageInfoPage siiPage = new OverlaySaveImageInfoPage();
            siiPage.UICultureChanged(m_culture); //Should be in ST3 too...
#else
            SaveImageDialog siiPage = new SaveImageDialog();
#endif
            siiPage.ThemeChanged(m_visualTheme);

            if (string.IsNullOrEmpty(saveImageProperties_fileName))
            {
                saveImageProperties_fileName = String.Format("{0} {1}", "Overlay", DateTime.Now.ToShortDateString());
                char[] cInvalid = Path.GetInvalidFileNameChars();
                for (int i = 0; i < cInvalid.Length; i++)
                    saveImageProperties_fileName = saveImageProperties_fileName.Replace(cInvalid[i], '-');
            }
            if (string.IsNullOrEmpty(Settings.SavedImageFolder))
            {
                Settings.SavedImageFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
                siiPage.FileName = Settings.SavedImageFolder + Path.DirectorySeparatorChar + saveImageProperties_fileName;
            siiPage.ImageSize = Settings.SavedImageSize;
			siiPage.ImageFormat = Settings.SavedImageFormat;

			siiPage.ShowDialog();

			if ( siiPage.DialogResult == DialogResult.OK )
			{
				saveImageProperties_fileName = Path.GetFileName(siiPage.FileName);
                Settings.SavedImageFolder = Path.GetDirectoryName(siiPage.FileName);
                Settings.SavedImageSize = siiPage.ImageSize;
				Settings.SavedImageFormat = siiPage.ImageFormat;
#if ST_2_1
                if ((!System.IO.File.Exists(siiPage.FileName)) ||
                    (MessageBox.Show(String.Format(SaveImageResources.FileAlreadyExists, siiPage.FileName),
                                        SaveImageResources.SaveImage, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes))
					chart.SaveImage( siiPage.ImageSize, siiPage.FileName, siiPage.ImageFormat ); 
#else
                chart.SaveImage(siiPage.ImageSizes[siiPage.ImageSize], siiPage.FileName, siiPage.ImageFormat); 
#endif
            }
		}

        void actionBanner1_MenuClicked(object sender, System.EventArgs e)
        {
            //actionBanner1.ContextMenuStrip.Width = 100;
            actionBanner1.ContextMenuStrip.Show(actionBanner1.Parent.PointToScreen(new System.Drawing.Point(actionBanner1.Right - actionBanner1.ContextMenuStrip.Width - 2,
                actionBanner1.Bottom + 1)));
        }

        private void bannerContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            showMeanMenuItem.Checked = Settings.ShowCategoryAverage;
            showRollingAverageMenuItem.Checked = Settings.ShowMovingAverage;
            offsetStripTextBox.Enabled = (treeListAct.SelectedItems.Count > 0);
            if (offsetStripTextBox.Enabled)
            {
                ActivityWrapper wrapper = (ActivityWrapper)treeListAct.SelectedItems[0];
                if (Settings.UseTimeXAxis)
                    offsetStripTextBox.Text = Util.UnitUtil.Time.ToString(wrapper.TimeOffset);
                else
                    offsetStripTextBox.Text = wrapper.DistanceOffset.ToString();
            }

            setRefActMenuItem.Enabled = (treeListAct.SelectedItems.Count == 1);
            showDiffMenuItem.Enabled = (CommonData.refActWrapper != null);
        }

        private void bannerContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            // Set the offset of the selected activities here            
            foreach (ActivityWrapper wrapper in treeListAct.SelectedItems)
            {
                try
                {
                    if (!Settings.UseTimeXAxis)
                        wrapper.DistanceOffset = UnitUtil.Distance.Parse(offsetStripTextBox.Text);
                    else
                        wrapper.TimeOffset = Time.ParseTimeSpan(offsetStripTextBox.Text, false);
                }
                catch
                {
                    // No valid value in the textbox, ignore it
                }
            }
            try
            {
                if (!Settings.UseTimeXAxis)
                    Settings.MovingAverageLength = UnitUtil.Distance.Parse(averageStripTextBox.Text);
                else
                    Settings.MovingAverageTime = UnitUtil.Time.Parse(averageStripTextBox.Text);
            }
            catch
            {
                // No valid value in the textbox, ignore it
            }
            treeListAct.Refresh();
            updateChart();
        }

        private void ShowMeanMenuItem_Click(object sender, EventArgs e)
        {
            Settings.ShowCategoryAverage = !Settings.ShowCategoryAverage;
            updateChart();
        }

        private void rollingAverageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.ShowMovingAverage = !Settings.ShowMovingAverage;
            updateChart();
        }

        private void setRefActMenuItem_Click(object sender, EventArgs e)
        {
            if (treeListAct.SelectedItems.Count == 1)
            {
                CommonData.refActWrapper = (ActivityWrapper)treeListAct.SelectedItems[0];
            }
            treeListAct.Refresh();
            updateChart();
        }

        // TODO: Fix handling of the user entering the offset
        private void offsetStripTextBox_TextChanged(object sender, EventArgs e)
        {
            if (treeListAct.SelectedItems.Count >= 1)
            {
                try
                {
                    foreach (ActivityWrapper wrapper in treeListAct.SelectedItems)
                    {
                        // This parsing is done just to verify that they can be done and that the text in the box is valid
                        if (Settings.UseTimeXAxis)
                        {
                            wrapper.TimeOffset = Time.ParseTimeSpan(offsetStripTextBox.Text, false);
                            offsetStripTextBox.Text = Util.UnitUtil.Time.ToString(wrapper.TimeOffset);
                        }
                        else
                        {
                            wrapper.DistanceOffset = double.Parse(offsetStripTextBox.Text);
                        }
                    }

                }
                catch (Exception Ex)
                {
                    ActivityWrapper wrapper = (ActivityWrapper)treeListAct.SelectedItems[0];
                    if (Settings.UseTimeXAxis)
                        offsetStripTextBox.Text = Util.UnitUtil.Time.ToString(wrapper.TimeOffset);
                    else
                        offsetStripTextBox.Text = wrapper.DistanceOffset.ToString();
                }
            }
        }

        private void bannerShowContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            showHRMenuItem.Checked = Settings.ShowHeartRate;
            showPaceMenuItem.Checked = Settings.ShowPace;
            showSpeedMenuItem.Checked = Settings.ShowSpeed;
            showPowerMenuItem.Checked = Settings.ShowPower;
            showCadenceMenuItem.Checked = Settings.ShowCadence;
            showElevationMenuItem.Checked = Settings.ShowElevation;
            showTimeMenuItem.Checked = Settings.ShowTime;
            showDistanceMenuItem.Checked = Settings.ShowDistance;
        }

        private void showXXXMenuItem_click(object sender, EventArgs e)
        {
            // Change the corresponding check box. Its event handler will change the settings.
            if (sender == showHRMenuItem)
            {
                heartRate.Checked = !heartRate.Checked;
            }
            else if (sender == showPaceMenuItem)
            {
                pace.Checked = !pace.Checked;
            }
            else if (sender == showSpeedMenuItem)
            {
                speed.Checked = !speed.Checked;
            }
            else if (sender == showPowerMenuItem)
            {
                power.Checked = !power.Checked;
            }
            else if (sender == showCadenceMenuItem)
            {
                cadence.Checked = !cadence.Checked;
            }
            else if (sender == showElevationMenuItem)
            {
                elevation.Checked = !elevation.Checked;
            }
            else if (sender == showTimeMenuItem)
            {
                time.Checked = !time.Checked;
            }
            else if (sender == showDistanceMenuItem)
            {
                distance.Checked = !distance.Checked;
            }
        }

        private void bannerShowDiffContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            showTimeDiffMenuItem.Checked = Settings.ShowDiffTime;
            showDistDiffMenuItem.Checked = Settings.ShowDiffDistance;
            showHRDiffMenuItem.Checked = Settings.ShowDiffHeartRate;
        }

        private void showDiffXXXMenuItem_click(object sender, EventArgs e)
        {
            if (sender == showTimeDiffMenuItem)
                Settings.ShowDiffTime = !Settings.ShowDiffTime;
            else if (sender == showDistDiffMenuItem)
                Settings.ShowDiffDistance = !Settings.ShowDiffDistance;
            else if (sender == showHRDiffMenuItem)
                Settings.ShowDiffHeartRate = !Settings.ShowDiffHeartRate;
            updateChart();
        }


        private void treeListContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            setRefTreeListMenuItem.Enabled = (treeListAct.SelectedItems.Count == 1);
        }

        private void tableSettingsMenuItem_Click(object sender, EventArgs e)
        {
#if ST_2_1
            ListSettings dialog = new ListSettings();
			dialog.ColumnsAvailable = OverlayColumnIds.ColumnDefs();
#else
            ListSettingsDialog dialog = new ListSettingsDialog();
            dialog.AvailableColumns = OverlayColumnIds.ColumnDefs(); // TrailResultColumnIds.ColumnDefs(m_controller.CurrentActivity);
#endif
            dialog.ThemeChanged(m_visualTheme);
            dialog.AllowFixedColumnSelect = false;
            dialog.SelectedColumns = Settings.TreeListActColumns; // PluginMain.Settings.ActivityPageColumns;
            dialog.NumFixedColumns = 3; // PluginMain.Settings.ActivityPageNumFixedColumns;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int numFixedColumns = dialog.NumFixedColumns;
                Settings.TreeListActColumns = dialog.SelectedColumns;
                RefreshColumns();
            }

        }

        private ITheme m_visualTheme =
#if ST_2_1
                Plugin.GetApplication().VisualTheme;
#else
 Plugin.GetApplication().SystemPreferences.VisualTheme;
#endif
        private CultureInfo m_culture =
#if ST_2_1
                new System.Globalization.CultureInfo("en");
#else
 Plugin.GetApplication().SystemPreferences.UICulture;
#endif

        private bool _showPage = false;
        private ChartDataSeries lastSelectedSeries = null;

        private List<IActivity> activities = new List<IActivity>();
        private List<ActivityWrapper> actWrappers = new List<ActivityWrapper>();
        private IDictionary<ChartDataSeries, IActivity> series2activity = new Dictionary<ChartDataSeries, IActivity>();

        //bSelectingDataFlag and bSelectDataFlag are used to coordinate the chart 
        //click/select/selecting events to minimize 'movingAverage' and 'box' control flicker.
        //I'm sure there's a better way, but at this time this is all I've got.
        private bool bSelectingDataFlag = false;
        private bool bSelectDataFlag = false;

        private string saveImageProperties_fileName = "";





    }

    static class CommonData
    {
        public static ActivityWrapper refActWrapper = null;
    }
}