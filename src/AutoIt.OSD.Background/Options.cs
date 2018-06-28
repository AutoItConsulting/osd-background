//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutoIt.OSD.Background
{
    /// <summary>
    ///     Class that describes the options.xml file that can be configured.
    /// </summary>
    [XmlRoot("Options")]
    public class Options
    {
        /// <summary>
        ///     Password to give admin level access.
        /// </summary>
        [XmlElement("PasswordAdmin")]
        public string PasswordAdmin { get; set; }

        /// <summary>
        ///     Password to give user level access.
        /// </summary>
        [XmlElement("PasswordUser")]
        public string PasswordUser { get; set; }

        /// <summary>
        ///     CustomBackground options.
        /// </summary>
        [XmlElement("CustomBackground")]
        public OsdCustomBackground CustomBackground { get; set; }

        /// <summary>
        ///     Title of the user tools menu.
        /// </summary>
        [XmlElement("Title")]
        public string Title { get; set; }

        /// <summary>
        ///     UserTools options.
        /// </summary>
        [XmlElement("UserTools")]
        public UserTools UserTools { get; set; }

        /// <summary>
        ///     TaskSequenceVariables options.
        /// </summary>
        [XmlElement("TaskSequenceVariables")]
        public OsdTaskSequenceVariables TaskSequenceVariables { get; set; }

        /// <summary>
        ///     Runs a check of the data in a fully populated Options object to fix the values so they make sense. If the values
        ///     are too broken it will exception.
        /// </summary>
        /// <returns></returns>
        public void SanityCheck()
        {
            // Can't have progress bar if no background
            if (!CustomBackground.Enabled)
            {
                CustomBackground.ProgressBar.Enabled = false;
            }
        }
    }

    /// <summary>
    ///     CustomBackground class.
    /// </summary>
    public class OsdCustomBackground
    {
        /// <summary>
        ///     Gets or sets a value indicating if the background should be customised.
        /// </summary>
        [XmlElement("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets a value for the background image path.
        /// </summary>
        [XmlElement("ImageFile")]
        public string ImageFile { get; set; }

        /// <summary>
        ///     ProgressBar options.
        /// </summary>
        [XmlElement("ProgressBar")]
        public OsdProgressBar ProgressBar { get; set; }
    }

    /// <summary>
    ///     ProgressBar class.
    /// </summary>
    public class OsdProgressBar
    {
        /// <summary>
        ///     Gets or sets a value indicating if the progress bar is enabled.
        /// </summary>
        [XmlElement("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the foreground color.
        /// </summary>
        [XmlElement("ForeColor")]
        public string ForeColor { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the background color.
        /// </summary>
        [XmlElement("BackColor")]
        public string BackColor { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the dock status of the progress bar.
        /// </summary>
        [XmlElement("Dock")]
        public string Dock { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the offset of the progress bar from the edge of the screen.
        /// </summary>
        [XmlElement("Offset")]
        public int Offset { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the height of the progress bar.
        /// </summary>
        [XmlElement("Height")]
        public int Height { get; set; }
    }

    /// <summary>
    ///     UserTools class.
    /// </summary>
    public class UserTools
    {
        /// <summary>
        ///     A list of user tools.
        /// </summary>
        [XmlArray("Tools")] [XmlArrayItem("Tool")]
        public List<UserTool> UserToolList = new List<UserTool>();

        /// <summary>
        ///     Gets or sets a value indicating if the user tools tab is enabled.
        /// </summary>
        [XmlElement("EnabledAdmin")]
        public bool EnabledAdmin { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating if the user tools tab is enabled.
        /// </summary>
        [XmlElement("EnabledUser")]
        public bool EnabledUser { get; set; }
    }

    /// <summary>
    ///     UserTool class.
    /// </summary>
    public class UserTool
    {
        /// <summary>
        ///     Initialises a new instance of the UserTool class.
        /// </summary>
        public UserTool()
        {
            // Set defaults
            AdminOnly = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating if this tool should only be shown to admins.
        /// </summary>
        [XmlElement("AdminOnly")]
        public bool AdminOnly { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the display name of the tool.
        /// </summary>
        [XmlElement("Name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the command line of the tool.
        /// </summary>
        [XmlElement("Program")]
        public string Program { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the command line arguments of the tool.
        /// </summary>
        [XmlElement("Arguments")]
        public string Arguments { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the working directory of the tool.
        /// </summary>
        [XmlElement("WorkingDirectory")]
        public string WorkingDirectory { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    ///     TaskSequenceVariabes class.
    /// </summary>
    public class OsdTaskSequenceVariables
    {
        /// <summary>
        ///     Gets or sets a value indicating if the task sequence tab is enabled.
        /// </summary>
        [XmlElement("EnabledAdmin")]
        public bool EnabledAdmin { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating if the user tools tab is enabled.
        /// </summary>
        [XmlElement("EnabledUser")]
        public bool EnabledUser { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating edits are allowed.
        /// </summary>
        [XmlElement("AllowEdit")]
        public bool AllowEdit { get; set; }
    }
}