using Microsoft.AspNetCore.Mvc;
using SmartInventory.Application.Products;
using SmartInventory.Domain.Common;

namespace SmartInventory.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
public sealed class ProductsController(ProductCatalogService productCatalog) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> Search(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var products = await productCatalog.SearchAsync(search, page, pageSize, cancellationToken);
        return Ok(products);
    }

    [HttpPost]
    [ProducesResponseType<ProductDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await productCatalog.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Search), new { search = product.Sku }, product);
        }
        catch (DomainException exception)
        {
            return BadRequest(new ProblemDetails { Title = "Datos de producto no válidos", Detail = exception.Message, Status = StatusCodes.Status400BadRequest });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Title = "El producto ya existe", Detail = exception.Message, Status = StatusCodes.Status409Conflict });
        }
    }
}
