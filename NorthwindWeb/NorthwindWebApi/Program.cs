using NorthwindWebApi.Repositorio.DAO;
using NorthwindWebApi.Repositorio.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IHome, homeDAO>();
builder.Services.AddScoped<ICategory, categoryDAO>();
builder.Services.AddScoped<IProduct, productDAO>();
builder.Services.AddScoped<ICustomer, customerDAO>();
builder.Services.AddScoped<ICargo, cargoDAO>();
builder.Services.AddScoped<IPais, paisDAO>();
builder.Services.AddScoped<IShipper, shipperDAO>();
builder.Services.AddScoped<ISupplier, supplierDAO>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUsuarioDAO, UsuarioDAO>();

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
