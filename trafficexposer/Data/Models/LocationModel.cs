/*
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
    /// Model for saving Details of an Location (ex. City)
    /// </summary>
    public struct Location
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
