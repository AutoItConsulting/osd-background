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
        public string Title { get; set; }

        [XmlElement("Password")]
        public string Password { get; set; }
        
        [XmlElement("CustomBackgroundEnabled")]
        public string CustomBackgroundEnabled { get; set; }
        
        [XmlElement("ProgressBarEnabled")]
        public string ProgressBarEnabled { get; set; }

        [XmlElement("ProgressBarForeColor")]
        public string ProgressBarForeColor { get; set; }

        [XmlElement("ProgressBarBackColor")]
        public string ProgressBarBackColor { get; set; }

        [XmlElement("ProgressBarDock")]
        public string ProgressBarDock { get; set; }

        [XmlElement("ProgressBarOffset")]
        public int ProgressBarOffset { get; set; }

        [XmlElement("ProgressBarHeight")]
        public int ProgressBarHeight { get; set; }

        [XmlElement("UserTools")] 
        public UserTools UserTools { get; set; }
    }

    public class UserTools
    {
        [XmlElement("Enabled")]
        public string Enabled { get; set; }

        [XmlArray("Tools")]
        [XmlArrayItem("Tool")]
        public List<UserTool> UserToolList = new List<UserTool>();
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
