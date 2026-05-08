using MindCabinet.Shared.Utility;

namespace MindCabinet.Utility;


public class SimpleTableBuilder(
                string name,
                SimpleTableBuilder.ColumnDefinition[] columnDefinitions,
                string[] postFixs ) {
    public class ColumnDefinition( string name, string type, string? details ) {
        public string Name { get; } = name;
        public string Type { get; } = type;
        public string? Details { get; } = details;


        public string GetDefinitionSql() {
            string sql = $"{this.Name} {this.Type}";
            if( this.Details is not null ) {
                sql += $" {this.Details}";
            }
            return sql;
        }
    }



    public string Name { get; } = name;
    public ColumnDefinition[] ColumnDefinitions { get; } = columnDefinitions;
    public string[] PostFixs { get; } = postFixs;
    


    public string GetCreateTableSql() {
        string sql = $"CREATE TABLE {this.Name} (\n";
        
        sql += string.Join(
            ",\n    ",
            this.ColumnDefinitions.Select( def => def.GetDefinitionSql() )
        );

        if( this.PostFixs.Count() > 0 ) {
            sql += ",\n     ";
            sql += string.Join( ",\n     ", this.PostFixs );
        }

        sql += "\n);";

        return sql;
    }
}
