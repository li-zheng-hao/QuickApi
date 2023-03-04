using SqlSugar;

namespace QuickApi.WebapiSample.Sqlsugar.Mysql.Model;

public class SqlSugarModel
{
    //数据是自增需要加上IsIdentity 
    //数据库是主键需要加上IsPrimaryKey 
    //注意：要完全和数据库一致2个属性
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    public int? SchoolId { get; set; }
    public string Name { get; set; }
}