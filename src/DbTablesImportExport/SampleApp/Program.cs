using DbTablesImportExport;
using DbTablesImportExport.Config;
using System;
using System.Linq;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Export();
            Import();
        }

        static void Import()
        {
            var config = new ImportConfig()
            {
                ConnectionString = "",
                DatabaseProvider = "System.Data.SqlClient",
                Source = @"D:\temp\db\backup.bak",
                IdentityInsert = true,
                Overwrite = true
            };
            var import = new Import(config);
            import.ImportFromFile();
        }

        static void Export()
        {
            var config = new ExportConfig()
            {
                ConnectionString = "",
                DatabaseProvider = "System.Data.SqlClient",
                Destination = @"D:\temp\db\backup.bak",
                TableNames = (new string[] { "TABLE1" }).ToList()
            };
            var export = new Export(config);
            export.ExportToFile();
        }
    }
}
