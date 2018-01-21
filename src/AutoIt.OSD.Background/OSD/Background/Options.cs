//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System.Xml.Serialization;

namespace AutoIt.OSD.Background
{
    [XmlRoot("Options")]
    public class Options
    {
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
    }
}
