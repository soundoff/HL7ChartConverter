using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartConverter
{
    public enum Action
    {
        /// <summary>
        /// A request to process charts and create HL7 XML file.
        /// </summary>
        ProcessCharts,
        /// <summary>
        /// A request to convert charts to plain text.
        /// </summary>
        ConvertCharts,
        /// <summary>
        /// A request to convert a doc to a tif.
        /// </summary>
        ConvertDocToTif
    }
}
