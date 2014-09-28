﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TimeZoneInfoExplorer
{
    public partial class MainForm : Form
    {
        private static readonly string[] OrdinalWeeks = { "", "1st", "2nd", "3rd", "4th", "Last" }; 

        public MainForm()
        {
            InitializeComponent();
        }

        private void AdjustToSelectedTimeZone(object sender, EventArgs e)
        {
            var zone = (TimeZoneInfo) timeZones.SelectedValue;

            idValue.Text = zone.Id;
            displayNameValue.Text = zone.DisplayName;
            standardNameValue.Text = zone.StandardName;
            daylightNameValue.Text = zone.DaylightName;
            supportsDstValue.Text = zone.SupportsDaylightSavingTime ? "Yes" : "No";
            standardOffsetValue.Text = zone.BaseUtcOffset.ToString();

            adjustmentRules.DataSource = zone.GetAdjustmentRules().Select(rule => new
            {
                Delta = rule.DaylightDelta,
                Start = rule.DateStart,
                End = rule.DateEnd,
                DstStart = FormatTransition(rule.DaylightTransitionStart),
                DstEnd = FormatTransition(rule.DaylightTransitionEnd),
            }).ToList();
            adjustmentRules.ClearSelection();

            // Not sure of the best way to handle this, basically... could definitely
            // do with some work.
            layoutPanel.ColumnStyles[0].SizeType = SizeType.Absolute;
            layoutPanel.ColumnStyles[0].Width = adjustmentRules.PreferredSize.Width;

            AdjustOffsets(sender, e);
        }

        private void AdjustOffsets(object sender, EventArgs e)
        {
            var zone = (TimeZoneInfo) timeZones.SelectedValue;
            var offsetList = new[] { new { Utc = "", Offset = TimeSpan.Zero } }.ToList();
            offsetList.Clear();
            var start = DateTime.SpecifyKind(offsetsFrom.Value, DateTimeKind.Utc);
            var end = DateTime.SpecifyKind(offsetsTo.Value, DateTimeKind.Utc);
            // Limit it to 100 values... it could be very large temporarily while changing from/to
            for (DateTime utc = start; utc <= end && offsetList.Count < 100; utc = utc.AddHours(1))
            {
                offsetList.Add(new { Utc = utc.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), Offset = zone.GetUtcOffset(utc) });
            }

            utcOffsets.DataSource = offsetList;
            utcOffsets.ClearSelection();
        }

        private void PopulateTimeZones(object sender, EventArgs e)
        {
            var zones = TimeZoneInfo.GetSystemTimeZones()
                .OrderBy(zone => zone.BaseUtcOffset)
                .ThenBy(zone => zone.Id)
                .ToList();
            timeZones.DataSource = zones;
            timeZones.SelectedIndex = zones.IndexOf(TimeZoneInfo.Utc);
        }

        private static string FormatTransition(TimeZoneInfo.TransitionTime transition)
        {
            return transition.IsFixedDateRule ?
                string.Format("{0:MMMM dd} at {1:HH:mm}", new DateTime(2000, transition.Month, transition.Day), transition.TimeOfDay) :
                string.Format("{0} {1} of {2:MMMM} at {3:HH:mm}", OrdinalWeeks[transition.Week], transition.DayOfWeek, new DateTime(2000, transition.Month, 1), transition.TimeOfDay);
        }
    }
}
