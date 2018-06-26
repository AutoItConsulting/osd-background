//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutoIt.OSD.Background
{
    [XmlRoot("Options")]
    public class Options
    {
        [XmlElement("PasswordAdmin")]
        public string PasswordAdmin { get; set; }

        [XmlElement("PasswordUser")]
        public string PasswordUser { get; set; }

        [XmlElement("CustomBackground")]
        public OsdCustomBackground CustomBackground { get; set; }

        [XmlElement("Title")]
        public string Title { get; set; }

        [XmlElement("UserTools")]
        public UserTools UserTools { get; set; }

        [XmlElement("TaskSequenceVariables")]
        public OsdTaskSequenceVariables TaskSequenceVariables { get; set; }
    }

    public class OsdCustomBackground
    {
        [XmlElement("Enabled")]
        public bool Enabled { get; set; }

        [XmlElement("ImageFile")]
        public string ImageFile { get; set; }

        [XmlElement("ProgressBar")]
        public OsdProgressBar ProgressBar { get; set; }
    }

    public class OsdProgressBar
    {
        [XmlElement("Enabled")]
        public bool Enabled { get; set; }

        [XmlElement("ForeColor")]
        public string ForeColor { get; set; }

        [XmlElement("BackColor")]
        public string BackColor { get; set; }

        [XmlElement("Dock")]
        public string Dock { get; set; }

        [XmlElement("Offset")]
        public int Offset { get; set; }

        [XmlElement("Height")]
        public int Height { get; set; }
    }

    public class UserTools
    {
        [XmlArray("Tools")] [XmlArrayItem("Tool")]
        public List<UserTool> UserToolList = new List<UserTool>();

        [XmlElement("EnabledAdmin")]
        public bool EnabledAdmin { get; set; }

        [XmlElement("EnabledUser")]
        public bool EnabledUser { get; set; }
    }

    public class UserTool
    {
        public UserTool()
        {
            // Set defaults
            AdminOnly = true;
        }

        [XmlElement("AdminOnly")]
        public bool AdminOnly { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Program")]
        public string Program { get; set; }

        [XmlElement("Arguments")]
        public string Arguments { get; set; }

        [XmlElement("WorkingDirectory")]
        public string WorkingDirectory { get; set; }
    }

    public class OsdTaskSequenceVariables
    {
        [XmlElement("EnabledAdmin")]
        public bool EnabledAdmin { get; set; }

        [XmlElement("EnabledUser")]
        public bool EnabledUser { get; set; }

        [XmlElement("AllowEdit")]
        public bool AllowEdit { get; set; }
    }
}