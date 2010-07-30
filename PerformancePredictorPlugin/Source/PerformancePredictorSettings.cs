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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ZoneFiveSoftware.Common.Data.Measurement;
using System.Globalization;
using SportTracksPerformancePredictorPlugin.Properties;
using SportTracksPerformancePredictorPlugin.Util;

namespace SportTracksPerformancePredictorPlugin.Source
{
    public partial class PerformancePredictorSettings : UserControl
    {
        public PerformancePredictorSettings()
        {
            InitializeComponent();
            InitControls();
            updateList();
        }

        void InitControls()
        {
            foreach (Length.Units s in Enum.GetValues(typeof(Length.Units)))
            {
                unitBox.Items.Add(Length.Label(s));
            }

            unitBox.SelectedItem = UnitUtil.Distance.Label;
            numericUpDown1.Value = Settings.PercentOfDistance;
        }
        public void ThemeChanged(ZoneFiveSoftware.Common.Visuals.ITheme visualTheme)
        {
            this.distanceBox.ThemeChanged(visualTheme);
        }
        public void UICultureChanged(System.Globalization.CultureInfo culture)
        {
            linkLabel1.Text = Resources.Webpage;
            resetSettings.Text = StringResources.ResetAllSettings;
            groupBox1.Text = Resources.DistancesUsed;
            addDistance.Text = "<- " + Resources.AddDistance;
            removeDistance.Text = Resources.RemoveDistance + " ->";
            groupBox2.Text = Resources.HighScorePluginIntegration;
            label1.Text = StringResources.Use;
            label2.Text = Resources.ProcDistUsed;
        }
        private void updateList()
        {
            distanceList.Items.Clear();
            foreach (double new_dist in Settings.Distances.Keys)
            {
                double length = new_dist;
                String str;
                if (Settings.Distances[new_dist].Values[0])
                {
                    str = UnitUtil.Distance.ToString(length, "u");
                }
                else
                {
                    str = UnitUtil.Distance.ToString(length, Settings.Distances[new_dist].Keys[0], "u");
                }
                distanceList.Items.Add(str);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo("IExplore",
                "http://code.google.com/p/gps-running/wiki/PerformancePredictor"));
        }

        private void resetSettings_Click(object sender, EventArgs e)
        {
            YesNoDialog dialog = new YesNoDialog(StringResources.ResetQuestion);
            dialog.ShowDialog();
            if (dialog.answer)
            {
                Settings.defaults();
                updateList();
            }
        }

        private void addDistance_Click(object sender, EventArgs e)
        {
            try
            {
                //Use ST parse routines to get handling of units for consistency,
                //even if there is a unit box (the risk to miss on precision is low)
                Length.Units unit = UnitUtil.Distance.Unit;
                //Some attempt to parse the unit box. This was easier in a previous version
                //unit = (Length.Units)Enum.Parse(typeof(Length.Units), (String)unitBox.SelectedItem);
                //However, with localised units, this is not possible
                //The box is not really needed when the unit now can be parsed in the box
                foreach (Length.Units s in Enum.GetValues(typeof(Length.Units)))
                {
                    if (((String)(unitBox.SelectedItem)).Equals(Length.Label(s)))
                    {
                        unit = s;
                    }
                }
                double d = UnitUtil.Distance.Parse(distanceBox.Text, ref unit);
                d = UnitUtil.Distance.ConvertFrom(d, unit);
                if (d <= 0) { throw new Exception(); }
                Settings.addDistance(d, unit, false);
                updateList();
            }
            catch (Exception)
            {
                new WarningDialog(Resources.PositiveNumber);
            }
        }

        private void removeDistance_Click(object sender, EventArgs e)
        {
            if (distanceList.SelectedIndex >= 0)
            {
                Settings.removeDistance(distanceList.SelectedIndex);
                updateList();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Settings.PercentOfDistance = (int)numericUpDown1.Value;
        }
    }
}