using AdvCrypExam.Exercises;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

SolveAlgorithms.AddRoutes(app);
SolveRsa.AddRoutes(app);
SolveAdditiveElgamal.AddRoutes(app);
SolveMultiplicativeElgamal.AddRoutes(app);
SolveShamirSecret.AddRoutes(app);
SolveCipolla.AddRoutes(app);
app.MapGet("/", static () => Results.Redirect("/swagger/index.html", true));

app.Run();