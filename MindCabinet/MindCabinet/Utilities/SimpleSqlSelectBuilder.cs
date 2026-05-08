using MindCabinet.Shared.Utility;

namespace MindCabinet.Utility;


public class SimpleSqlSelectBuilder {
    public static string Build(
                string tableName,
                IEnumerable<string> columnNames,
                string? joinClause,
                IEnumerable<string> whereClause,
                string? groupByClause = null,
                string? havingClause = null,
                string? orderByClause = null,
                string? limitClause = null,
                bool wrapWithCount = false ) {
        string sql = $"SELECT {string.Join(", ", columnNames)}";
        sql += $"\nFROM {tableName}";

        if( !string.IsNullOrEmpty(joinClause) ) {
            sql += $"\n{joinClause}";
        }

        if( whereClause.Count() > 0 ) {
            sql += $"\nWHERE {string.Join("\n    AND ", whereClause)}";
        }

        if( !string.IsNullOrEmpty(groupByClause) ) {
            sql += $"\nGROUP BY {groupByClause}";
        }

        if( !string.IsNullOrEmpty(havingClause) ) {
            sql += $"\nHAVING {havingClause}";
        }

        if( !string.IsNullOrEmpty(orderByClause) ) {
            sql += $"\nORDER BY {orderByClause}";
        }

        if( !string.IsNullOrEmpty(limitClause) ) {
            sql += $"\nLIMIT {limitClause}";
        }
        
        if( wrapWithCount ) {
            sql = $"SELECT COUNT(*) FROM (\n{sql}\n) AS TotalCount";
        }

        return sql + ";";
    }



    public string TableName;
    public string[] ColumnNames;
    public string? JoinClause;
    public List<string> WhereClause;
    public string? GroupByClause;
    public string? HavingClause;
    public string? OrderByClause;
    public string? LimitClause;
    public bool WrapWithCount = false;

    
    public SimpleSqlSelectBuilder( string tableName, IEnumerable<string> columnNames ) {
        this.TableName = tableName;
        this.ColumnNames = columnNames.ToArray();
        this.JoinClause = null;
        this.WhereClause = new List<string>();
        this.OrderByClause = null;
        this.LimitClause = null;
    }

    public SimpleSqlSelectBuilder( 
                string tableName,
                string[] columnNames,
                string? joinClause,
                string[] whereClause,
                string? groupByClause = null,
                string? havingClause = null,
                string? orderByClause = null,
                string? limitClause = null,
                string? offsetClause = null,
                bool wrapWithCount = false ) {
        this.TableName = tableName;
        this.ColumnNames = columnNames;
        this.JoinClause = joinClause;
        this.WhereClause = whereClause.ToList();
        this.GroupByClause = groupByClause;
        this.HavingClause = havingClause;
        this.OrderByClause = orderByClause;
        this.LimitClause = limitClause;
        this.WrapWithCount = wrapWithCount;
    }


    public void AddWhereClause( string clause ) {
        this.WhereClause.Add( clause );
    }


    public string Build() {
        string sql = SimpleSqlSelectBuilder.Build(
            tableName: this.TableName,
            columnNames: this.ColumnNames,
            joinClause: this.JoinClause,
            whereClause: this.WhereClause,
            groupByClause: this.GroupByClause,
            havingClause: this.HavingClause,
            orderByClause: this.OrderByClause,
            limitClause: this.LimitClause,
            wrapWithCount: this.WrapWithCount
        );
        return sql;
    }
}
