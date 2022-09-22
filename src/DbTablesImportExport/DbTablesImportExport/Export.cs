using Dapper;
using DbTablesImportExport.Config;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DbTablesImportExport
{
    public class Export
    {
        public Export()
        {

        }

        public Export(ExportConfig Config)
        {
            this.Config = Config;
        }

        public ExportConfig Config { get; set; }

        public void ExportToFile()
        {
            try
            {
                if (Config == null ||
                    string.IsNullOrWhiteSpace(Config.ConnectionString) ||
                    Config.TableNames == null ||
                    Config.TableNames.Count == 0)
                {
                    throw new ArgumentException("Config is null or incorrect", "Config");
                }

                bool isFirst = true;

                var output = new StringBuilder();

                output.Append("{");

                using (var db = new SqlConnection(Config.ConnectionString))
                {
                    foreach (string table in Config.TableNames)
                    {
                        string query = @"select * from " + table;

                        var data = db.Query(query).ToList();

                        var json = JsonConvert.SerializeObject(data);

                        if (isFirst)
                        {
                            output.Append(string.Format("\"{0}\": ", table));
                            isFirst = false;
                        }
                        else
                        {
                            output.Append(string.Format(",\"{0}\": ", table));
                        }

                        output.Append(json);
                    }
                }

                output.Append("}");

                File.WriteAllText(Config.Destination, output.ToString());
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
