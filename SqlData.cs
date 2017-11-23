using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DatabaseEntitiesCLI
{
    public class SqlData
    {
        public string SqlConnection()
        {
            string _sqlServer, _sqlUser, _sqlPass = "";
            ConsoleKeyInfo _key;

            Console.WriteLine("\n## SQL Server location (IP / Domain)");
            Console.Write("#> ");
            _sqlServer = Console.ReadLine();
            Console.WriteLine("## SQL Server User Name");
            Console.Write("#> ");
            _sqlUser = Console.ReadLine();
            Console.WriteLine("## SQL User's Password");
            Console.Write("#> ");

            while (_key.Key != ConsoleKey.Enter)
            {
                _key = Console.ReadKey(true);

                if (_key.Key != ConsoleKey.Backspace)
                {
                    _sqlPass += _key.KeyChar;
                }
                else if (_key.Key == ConsoleKey.Backspace && _sqlPass.Length > 0)
                {
                    _sqlPass = _sqlPass.Remove(_sqlPass.Length - 1);
                }
            }

            Console.WriteLine();
            return "Server=" + _sqlServer + "; User ID=" + _sqlUser + "; Password=" + _sqlPass;
        }

        public string SqlDataBases(string _ConnectionString)
        {
            int _cntr = 0;
            string _choice;
            List<string> _DataBases = new List<string>();
            try
            {
                using (SqlConnection _conn = new SqlConnection(_ConnectionString))
                {
                    _conn.Open();
                    using (SqlCommand _cmd = new SqlCommand("SELECT name FROM SYS.DATABASES", _conn))
                    {
                        using (SqlDataReader _reader = _cmd.ExecuteReader())
                        {
                            while (_reader.Read())
                            {
                                _DataBases.Add(_reader["name"].ToString());
                                Console.WriteLine("## " + _cntr.ToString() + " --> " + _reader["name"].ToString());
                                _cntr++;
                            }
                        }
                    }
                }
                Console.WriteLine("## Select Database to connect to (by number)");
                Console.Write("#> ");
                _choice = Console.ReadLine();

                return _DataBases[int.Parse(_choice)];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public IEnumerable<string> SqlTables(string _ConnectionString)
        {
            int _cntr = 0, _lineCntr = 0;
            string[] _selected;
            Dictionary<int, string> _table = new Dictionary<int, string>();
            List<string> _returnTable = new List<string>();

            string _command = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
                                    WHERE TABLE_TYPE='BASE TABLE'
                                UNION
                                SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS 
                                ORDER BY TABLE_NAME";

            try
            {
                using (SqlConnection _conn = new SqlConnection(_ConnectionString))
                {
                    _conn.Open();
                    using (SqlCommand _cmd = new SqlCommand(_command, _conn))
                    {
                        using (SqlDataReader _reader = _cmd.ExecuteReader())
                        {
                            Console.Write("## ");
                            while (_reader.Read())
                            {
                                Console.Write(_cntr.ToString() + " --> " +
                                    _reader["TABLE_NAME"].ToString());
                                _table.Add(_cntr, _reader["TABLE_NAME"].ToString());
                                _lineCntr++;

                                for (int i = _reader["TABLE_NAME"].ToString().Length; i < 30; i++)
                                {
                                    Console.Write(" ");
                                }

                                if (_lineCntr < 3)
                                {

                                    Console.Write("\t");
                                }
                                else
                                {
                                    Console.Write("\n## ");
                                    _lineCntr = 0;
                                }

                                _cntr++;
                            }
                        }
                    }
                }

                Console.WriteLine("\n## Select table/view rows' numbers seperated by commas ','");
                Console.Write("#> ");
                _selected = Console.ReadLine().Replace(" ", "").Split(',');

                foreach (var element in _selected)
                {
                    _returnTable.Add(_table[int.Parse(element)]);
                }

                return _returnTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnTable.Clear();
                _returnTable.TrimExcess();
                return _returnTable;
            }
        }
    }
}