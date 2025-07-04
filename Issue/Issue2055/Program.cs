using FreeSql;
using FreeSql.DataAnnotations;
using System.Diagnostics; 
class Program
{
    private static Lazy<IFreeSql> sqlserverLazy = new Lazy<IFreeSql>(() => new FreeSql.FreeSqlBuilder()
       .UseConnectionString(FreeSql.DataType.SqlServer, "Data Source=.;Integrated Security=True;Initial Catalog=ds_shop;Pooling=true;Max Pool Size=3")
       .UseAutoSyncStructure(true)
       .UseMonitorCommand(
           cmd => Trace.WriteLine("\r\n线程" + Thread.CurrentThread.ManagedThreadId + ": " + cmd.CommandText) //监听SQL命令对象，在执行前  
       )
       .UseLazyLoading(true)
       .Build());

    private static IInsert<Topic> insert => sqlserverLazy.Value.Insert<Topic>();

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var items = new List<Topic>();
        for (var a = 0; a < 10; a++) items.Add(new Topic { Id = a + 1, Title = $"newtitle{a}", Clicks = a * 100, CreateTime = DateTime.Now });

        insert.AppendData(items).InsertIdentity().ExecuteSqlBulkCopy();

        sqlserverLazy.Value.Select<Topic>()
            .ToList()
            .ForEach(a => Console.WriteLine($"Id: {a.Id}, Title: {a.Title}, Clicks: {a.Clicks}, CreateTime: {a.CreateTime}"));
    }
}

[Table(Name = "tb topic")]
class Topic
{
    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }
    public int Clicks { get; set; }
    public int TypeGuid { get; set; }
    public string Title { get; set; }
    public DateTime CreateTime { get; set; }
}
