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
        [XmlElement("Title")]
        public string Title;

        [XmlElement("Password")]
        public string Password;
        
        [XmlElement("CustomBackgroundEnabled")]
        public string CustomBackgroundEnabled;
        
        [XmlElement("ProgressBarEnabled")]
        public string ProgressBarEnabled;

        [XmlElement("ProgressBarForeColor")]
        public string ProgressBarForeColor;

        [XmlElement("ProgressBarBackColor")]
        public string ProgressBarBackColor;

        [XmlElement("ProgressBarDock")]
        public string ProgressBarDock;

        [XmlElement("ProgressBarOffset")]
        public int ProgressBarOffset;

        [XmlElement("ProgressBarHeight")]
        public int ProgressBarHeight;

        [XmlArray("UserTools")]
        [XmlArrayItem("UserTool")]
        public List<UserTool> UserTools = new List<UserTool>();

    }

    public class UserTool
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Program")]
        public string Program { get; set; }

        [XmlElement("Arguments")]
        public string Arguments { get; set; }

        [XmlElement("WorkingDirectory")]
        public string WorkingDirectory { get; set; }
    }
}
