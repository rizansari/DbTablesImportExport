using System;
using System.Collections.Generic;
using System.Text;

namespace DbTablesImportExport.Config
{
    public class ExportConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseProvider { get; set; }
        public List<string> TableNames { get; set; }
        public string Destination { get; set; }
    }
}
