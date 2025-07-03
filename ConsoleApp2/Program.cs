using FreeSql.DataAnnotations;
using System.ComponentModel;

class Program
{
    private static IFreeSql fsql = new FreeSql.FreeSqlBuilder()
       .UseConnectionString(FreeSql.DataType.SqlServer, "Data Source=.;Integrated Security=True;Initial Catalog=ds_shop;Pooling=true;Max Pool Size=3")
       //.UseAutoSyncStructure(true)
       .UseMonitorCommand(
           cmd => Console.WriteLine("\r\n" + cmd.CommandText)
       )
       .Build();


    static void Main(string[] args)
    {
        fsql.Select<OrderDetails001>()
                            .Include(a => a.OrdersLite)
                            .LeftJoin(a => a.OrdersLite!.Customers!.ID == a.OrdersLite!.CustomerID)
                            .ToList();

        fsql.Select<OrderDetails001>()
                            .Include(a => a.OrdersLite)
                            .LeftJoin(a => a.OrdersLite!.Customers!.ID == a.OrdersLite!.CustomerID)
                            .ToChunk(100, done =>
                            {
                                var list = done.Object;
                            });
    }
}

public class OrderDetails001
{
    public int ID { get; set; }
    public DateTime OrderDate { get; set; }

    [Navigate(nameof(ID))]
    public virtual Orders001? OrdersLite { get; set; }

    [Column(IsIgnore = true)]
    [DisplayName("Cliente")]
    public string CompanyName { get => companyName ?? (OrdersLite?.Customers?.CompanyName ?? "**"); set => companyName = value; }
    string? companyName;
}


public partial class Orders001
{
    public int ID { get; set; }
    public int CustomerID { get; set; }
    public string? ProductName { get; set; }

    [Navigate(nameof(CustomerID))]
    public virtual Customers001? Customers { get; set; }
}

public class Customers001
{
    public int ID { get; set; }

    public string? CompanyName { get; set; }
}