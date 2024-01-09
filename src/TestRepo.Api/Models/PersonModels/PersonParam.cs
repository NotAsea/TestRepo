namespace TestRepo.Api.Models.PersonModels;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable once NotAccessedPositionalProperty.Global
internal record PersonRouteDefaultParam(
    ILogger Logger,
    IPersonService Service,
    HttpContext HttpContext
);