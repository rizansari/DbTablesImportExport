using System;
using System.Collections.Generic;
using System.Text;

namespace DbTablesImportExport.Config
{
    public class ImportConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseProvider { get; set; }
        public string Source { get; set; }
        public bool Overwrite { get; set; }
        public bool IdentityInsert { get; set; }
    }
}
