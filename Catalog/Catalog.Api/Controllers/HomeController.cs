using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController : ControllerBase
{
    [Route("/")]
    public IActionResult Index() => new RedirectResult("~/swagger");
}