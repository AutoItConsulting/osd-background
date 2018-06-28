// 
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System.Xml.Serialization;

namespace AutoIt.OSD.Background
{
    /// <summary>
    ///     This is an object that is serialized into xml and sent via a named pipe.
    /// </summary>
    [XmlRoot("NamedPipeXmlPayload")]
    public class NamedPipeXmlPayload
    {
        /// <summary>
        ///     Constructor. Needs a parameterless constructor for serializing.
        /// </summary>
        public NamedPipeXmlPayload()
        {
            SignalQuit = false;
            Options = new Options();
            OsdBackgroundDir = string.Empty;
        }

        /// <summary>
        ///     An Options object.
        /// </summary>
        [XmlElement("Options")]
        public Options Options { get; set; }

        /// <summary>
        ///     If set to true this is simply a request to terminate
        /// </summary>
        [XmlElement("SignalQuit")]
        public bool SignalQuit { get; set; }

        /// <summary>
        ///     The script dir that the exe was launched from.
        /// </summary>
        [XmlElement("OsdBackgroundDir")]
        public string OsdBackgroundDir { get; set; }

        /// <summary>
        ///     The working directory at time of launch.
        /// </summary>
        [XmlElement("OsdBackgroundWorkingDir")]
        public string OsdBackgroundWorkingDir { get; set; }
    }
}
