﻿/*
 *  Created by: Hakuryuu
 *  www.hakuryuu.net
 *  info@hakuryuu.net
 *  
 *  Copyright (c) 2023 Hakuryuu
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trafficexposer.Data
{
    /// <summary>
    /// Area which is defined from the User where he wants to be informed of Traffic Events.
    /// </summary>
    public struct Area
    {
        public Location StartLocation { get; set; }
        public Location Destiny { get; set; }
        public TimeSpan? EstimatedLeave { get; set; }
    }
}
