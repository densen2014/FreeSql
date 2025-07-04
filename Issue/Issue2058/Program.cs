using FreeSql.DataAnnotations;
using PropertyChanged;
using System.ComponentModel;

class Program
{
    private static IFreeSql fsql = new FreeSql.FreeSqlBuilder()
       .UseConnectionString(FreeSql.DataType.SqlServer, "Data Source=.;Integrated Security=True;Initial Catalog=ds_shop;Pooling=true;Max Pool Size=3")
       //.UseAutoSyncStructure(true)
       .UseNoneCommandParameter(true)
       .UseMonitorCommand(
           cmd => Console.WriteLine("\r\n" + cmd.CommandText)
       )
       .Build();


    static void Main(string[] args)
    {
        var list =new BindingList<OrderDetails001>();

        var repo = fsql.GetRepository<OrderDetails001>();
        //repo.BeginEdit(list.ToList()); //开始对 list 进行编辑

        list.Add(new OrderDetails001
        {
            UserCode = "Cod001",
            Quantity = 120
        });
        list[0].Bultos = list[0].Quantity / 12;
        repo.EndEdit(list.ToList());

        Console.WriteLine($"ID\tQuantity\tBultos");
        foreach (var item in list)
        {
            Console.WriteLine($"{item.ID}\t{item.Quantity,-8}\t{item.Bultos,-6}");
        }

        repo.BeginEdit(list.ToList());  

        list.Add(new OrderDetails001
        {
            UserCode = "Cod002",
            Quantity = 100,
            Bultos = 20
        });
        list[0].UserCode = "Cod003";
        list[0].Quantity =1000;
        list[0].Bultos = list[0].Quantity/12;
 
        repo.EndEdit(list.ToList());

        Console.WriteLine($"ID\tQuantity\tBultos");
        foreach (var item in list)
        {
            Console.WriteLine($"{item.ID}\t{item.Quantity,-8}\t{item.Bultos,-6}");
        }

    }
}

[AddINotifyPropertyChangedInterface]
public class OrderDetails001
{
    [Column(IsIdentity = true)]
    public int ID { get; set; }

    public string UserCode { get; set; }
   
    public int Quantity { get; set; } = 1;

    public int? Bultos { get; set; } = 0;

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