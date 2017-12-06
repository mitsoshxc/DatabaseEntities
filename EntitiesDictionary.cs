using System.Collections.Generic;

namespace DatabaseEntitiesCLI
{
    partial class Entity
    {
        Dictionary<string, string> EntitiesDictionary = new Dictionary<string, string>()
        {
            {"bigint", "int"},
            {"int", "int"},
            {"smallint", "int"},
            {"char", "string"},
            {"nchar", "string"},
            {"ntext", "string"},
            {"nvarchar", "string"},
            {"text", "string"},
            {"varchar", "string"},
            {"decimal", "decimal"},
            {"float", "decimal"},
            {"money", "decimal"},
            {"numeric", "decimal"},
            {"real", "decimal"},
            {"smallmoney", "decimal"},
            {"date", "DateTime"},
            {"datetime", "DateTime"},
            {"datetime2", "DateTime"},
            {"smalldatetime", "DateTime"}
        };
    }
}