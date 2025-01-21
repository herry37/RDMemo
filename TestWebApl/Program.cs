using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TestWebApl.Application.Data;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // �N�t�m�`�J�� DI �e����
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);


        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        #region
        // �۰ʵ��U�Ҧ����f�Ψ��{
        var serviceCollection = builder.Services;
        var executingAssembly = Assembly.GetExecutingAssembly();
        // �۰ʱ��y�{�Ƕ��������O
        serviceCollection.Scan(scan => scan
            .FromAssemblies(executingAssembly) // �q�ثe���檺�{�Ƕ��}�l���y
            .AddClasses(classes => classes // �N�ŦX�������O�[�J�A�Ȯe��
                .Where(type => type.Name.EndsWith("Service") // ������O�W�٥]�t 
                    || (/*type.GetInterfaces().Any() &&*/ type.IsPublic))) // ��ܤ��}���O�B��{�F����
                    .AsSelf() // �N���O���U���ۤv
                .AsImplementedInterfaces()// �N���O���U����{������
                .WithScopedLifetime());
        //builder.Services.AddScoped<ProductService>();
        //// �p�G�z�Ʊ���U�Ҧ������f�M��{�A�i�H�o�˰��G
        //serviceCollection.Scan(scan => scan
        //    .FromAssemblies(executingAssembly)
        //    .AddClasses() // �۰ʧ��Ҧ���
        //    .AsImplementedInterfaces() // �N���P���{�����f�@�_���U
        //    .WithScopedLifetime()); // �]�w�ͩR�P��

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