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
    public class Import
    {
        public Import()
        {

        }

        public Import(ImportConfig Config)
        {
            this.Config = Config;
        }

        public ImportConfig Config { get; set; }

        public void ImportFromFile()
        {
            try
            {
                if (Config == null ||
                    string.IsNullOrWhiteSpace(Config.ConnectionString) ||
                    string.IsNullOrWhiteSpace(Config.Source) ||
                    !File.Exists(Config.Source))
                {
                    throw new ArgumentException("Config is null or incorrect", "Config");
                }

                string json = File.ReadAllText(Config.Source);

                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                using (var db = new SqlConnection(Config.ConnectionString))
                {
                    foreach (var table in data.Keys)
                    {
                        Console.WriteLine("table:{0}", table);

                        string query = @"SELECT Col.Column_Name from 
                            INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab, 
                            INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col 
                        WHERE 
                            Col.Constraint_Name = Tab.Constraint_Name
                            AND Col.Table_Name = Tab.Table_Name
                            AND Constraint_Type = 'PRIMARY KEY'
                            AND Col.Table_Name = '" + table + "'";

                        var pks = db.Query<string>(query).ToList();
                        if (pks != null && pks.Count > 0)
                        {
                            var tableData = data[table];
                            var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tableData.ToString());
                            foreach (var row in rows)
                            {
                                var updateQuery1 = new StringBuilder();
                                var updateQuery2 = new StringBuilder();

                                var insertQuery1 = new StringBuilder();
                                var insertQuery2 = new StringBuilder();

                                insertQuery1.Append(string.Format("insert into [{0}] (", table));
                                insertQuery2.Append("values (");

                                updateQuery1.Append(string.Format("update [{0}] set ", table));
                                updateQuery2.Append(" where ");

                                bool first = true;
                                bool firstUpdate1 = true;
                                bool firstUpdate2 = true;
                                foreach (var field in row.Keys)
                                {
                                    if (first)
                                    {
                                        first = false;
                                    }
                                    else
                                    {
                                        insertQuery1.Append(",");
                                        insertQuery2.Append(",");
                                    }

                                    insertQuery1.Append(string.Format("[{0}]", field));

                                    if (row[field] == null)
                                    {
                                        insertQuery2.Append(string.Format("null"));
                                    }
                                    else
                                    {
                                        insertQuery2.Append(string.Format(" N'{0}'", row[field].ToString().Replace("'", "''")));
                                    }
                                    


                                    if (pks.Contains(field))
                                    {
                                        if (firstUpdate2)
                                        {
                                            firstUpdate2 = false;
                                        }
                                        else
                                        {
                                            updateQuery2.Append(" and ");
                                        }

                                        if (row[field] == null)
                                        {
                                            updateQuery2.Append(string.Format("[{0}] = null", field));
                                        }
                                        else
                                        {
                                            updateQuery2.Append(string.Format("[{0}] = '{1}'", field, row[field].ToString().Replace("'", "''")));
                                        }
                                    }
                                    else
                                    {
                                        if (firstUpdate1)
                                        {
                                            firstUpdate1 = false;
                                        }
                                        else
                                        {
                                            updateQuery1.Append(", ");
                                        }

                                        if (row[field] == null)
                                        {
                                            updateQuery1.Append(string.Format("[{0}] = null", field));
                                        }    
                                        else
                                        {
                                            updateQuery1.Append(string.Format("[{0}] = N'{1}'", field, row[field].ToString().Replace("'", "''")));
                                        }
                                    }
                                }

                                insertQuery1.Append(")");
                                insertQuery2.Append(");");

                                updateQuery2.Append(";");

                                string insertQuery = insertQuery1.ToString() + " " + insertQuery2.ToString();

                                if (firstUpdate1)
                                {
                                    updateQuery1.Append(string.Format("[{0}] = N'{1}'", row.Keys.First(), row[row.Keys.First()]));
                                }
                                string updateQuery = updateQuery1.ToString() + " " + updateQuery2.ToString();

                                //Console.WriteLine(insertQuery);
                                //Console.WriteLine(updateQuery);

                                int rowsAffected = db.Execute(updateQuery);
                                if (rowsAffected == 0)
                                {
                                    try
                                    {
                                        rowsAffected = db.Execute(insertQuery);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex.Message.Contains("IDENTITY"))
                                        {
                                            string q1 = "SET IDENTITY_INSERT " + table + " ON; ";
                                            string q2 = "SET IDENTITY_INSERT " + table + " OFF; ";
                                            rowsAffected = db.Execute(q1 + insertQuery + q2);
                                        }
                                    }
                                    
                                }
                            }
                        }
                        else
                        {
                            // no pk found
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class Data
    {
        
    }
}
