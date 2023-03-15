using MarketData.ContributionGatewayApi;
using MarketData.ContributionGatewayApi.Application;
using MarketData.ContributionGatewayApi.Domain;
using MarketData.ContributionGatewayApi.Infrastructure;
using SurrealDb.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddJsonConverters();
builder.Services.AddSurrealClient();
builder.Services.AddTransient<IContributionService, ContributionService>();
builder.Services.AddTransient<IValidationService, MockValidationService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

IResult MapErrors(ApplicationError error) => error.Match<IResult>(
    Results.BadRequest,
    dbError => Results.Problem(dbError.Error), 
    validationServiceFail => Results.Problem(validationServiceFail.Message)
);

app.MapGet("/contribution",
        async (IContributionService service,
            CancellationToken cancellationToken) =>
        {
            var res = await service.GetContributions(cancellationToken);
            return res.Match(Results.Ok,
                dbError => Results.Problem(dbError.Error));
        })
    .WithName("GetContributions")
    .WithTags("Contributions")
    .Produces<MarketDataContributions>();

app.MapPost("/contribution",
        async (ContributionRequest request,
                IContributionService service,
                CancellationToken cancellationToken) =>
        {
            var some = await request
                .ToMarketDataContribution().ToAsync()
                .Bind(mdc => 
                    service.CreateContribution(mdc, cancellationToken).ToAsync()
                );

            return some.Match<IResult>(
                record => Results .Created($"contribution/{record.Id}", record),
                error => MapErrors(error));
        })
    .WithName("CreateContribution")
    .WithTags("Contributions")
    .Produces<MarketDataContribution>();

app.Run();

public partial class Program
{
}
