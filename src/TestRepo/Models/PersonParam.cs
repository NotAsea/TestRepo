// ReSharper disable ClassNeverInstantiated.Global

namespace TestRepo.Models;

internal record PersonRouteDefaultParam(ILogger Logger, IPersonService Service, HttpContext HttpContext);

