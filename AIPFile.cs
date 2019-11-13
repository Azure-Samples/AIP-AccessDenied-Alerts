// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIPAccessDeniedAlerts
{
    public class AIPFile
    {


        public class Rootobject
        {
            public Result[] results { get; set; }
            public object render { get; set; }
            public object statistics { get; set; }
            public Table[] tables { get; set; }
        }

        public class Result
        {
            public string ContentId_g { get; set; }
            public string FileName { get; set; }
            public string LabelName_s { get; set; }
            public string UserId_s { get; set; }
            public string ProtectionOwner_s { get; set; }
            public string TimeGenerated { get; set; }
            public string ProtectionTime_t { get; set; }
            public string IPv4_s { get; set; }
            public string Activity_s { get; set; }
            public string Operation_s { get; set; }
            public string AccessCount { get; set; }
        }

        public class Table
        {
            public string name { get; set; }
            public Column[] columns { get; set; }
            public object[][] rows { get; set; }
        }

        public class Column
        {
            public string name { get; set; }
            public string type { get; set; }
        }


    }
}