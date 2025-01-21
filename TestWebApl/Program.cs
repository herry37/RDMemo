using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TestWebApl.Application.Data;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 將配置注入到 DI 容器中
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);


        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        #region
        // 自動註冊所有接口及其實現
        var serviceCollection = builder.Services;
        var executingAssembly = Assembly.GetExecutingAssembly();
        // 自動掃描程序集中的類別
        serviceCollection.Scan(scan => scan
            .FromAssemblies(executingAssembly) // 從目前執行的程序集開始掃描
            .AddClasses(classes => classes // 將符合條件的類別加入服務容器
                .Where(type => type.Name.EndsWith("Service") // 選擇類別名稱包含 
                    || (/*type.GetInterfaces().Any() &&*/ type.IsPublic))) // 選擇公開類別且實現了介面
                    .AsSelf() // 將類別註冊為自己
                .AsImplementedInterfaces()// 將類別註冊為實現的介面
                .WithScopedLifetime());
        //builder.Services.AddScoped<ProductService>();
        //// 如果您希望註冊所有的接口和實現，可以這樣做：
        //serviceCollection.Scan(scan => scan
        //    .FromAssemblies(executingAssembly)
        //    .AddClasses() // 自動找到所有類
        //    .AsImplementedInterfaces() // 將類與其實現的接口一起註冊
        //    .WithScopedLifetime()); // 設定生命周期

        #endregion

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}