using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DatabaseEntitiesCLI
{
    partial class Entity
    {
        public void NewEntity()
        {
            string _ConnectionString = "", _Database = "";
            char _key;
            IEnumerable<string> _table = Enumerable.Empty<string>();

            SqlData _SqlData = new SqlData();

            while (_Database.Length == 0)
            {
                _ConnectionString = _SqlData.SqlConnection();
                _Database = _SqlData.SqlDataBases(_ConnectionString);

                if (_Database.Length == 0)
                {
                    Console.WriteLine("## Press 'c' to Continue or any other key to Quit");
                    Console.Write("#> ");
                    _key = Console.ReadKey().KeyChar;
                    if (_key != 'c' && _key != 'C')
                    {
                        Environment.Exit(1);
                    }
                }
            }

            _ConnectionString += ";Database=" + _Database;

            while (!_table.Any())
            {
                _table = _SqlData.SqlTables(_ConnectionString);

                if (!_table.Any())
                {
                    Console.WriteLine("## Press 'c' to Continue or any other key to Quit");
                    Console.Write("#> ");
                    _key = Console.ReadKey().KeyChar;
                    if (_key != 'c' && _key != 'C')
                    {
                        Environment.Exit(1);
                    }
                }
            }

            PropertyCreation(_table, _ConnectionString);
        }

        private void PropertyCreation(IEnumerable<string> _tables, string _ConnectionString)
        {
            string _command = "", _path;

            foreach (var _table in _tables)
            {
                using (SqlConnection _conn = new SqlConnection(_ConnectionString))
                {
                    _conn.Open();
                    _command = @"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS
                                    WHERE TABLE_NAME='" + _table + "'";

                    using (SqlCommand _cmd = new SqlCommand(_command, _conn))
                    {
                        using (SqlDataReader _reader = _cmd.ExecuteReader())
                        {
                            while (_reader.Read())
                            {
                                try
                                {
                                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Classes");
                                    _path = Directory.GetCurrentDirectory() + "/Classes/" + _table + "s.cs";
                                    using (TextWriter _File = new StreamWriter(_path))
                                    {
                                        _File.WriteLine("using System.ComponentModel.DataAnnotations.Schema;");
                                        _File.WriteLine("\n[Table(\"" + _table + "\")]");
                                        _File.WriteLine("public class " + _table + "s");
                                        _File.WriteLine("{");

                                        while (_reader.Read())
                                        {
                                            _File.WriteLine("\tpublic " +
                                                            EntitiesDictionary[_reader["DATA_TYPE"].ToString()]
                                                            + " " + _reader["COLUMN_NAME"] +
                                                            " { get; set; }\n");
                                        }

                                        _File.WriteLine("}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("## All done!");
        }
    }
}