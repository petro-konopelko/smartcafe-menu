using FluentValidation;
using SmartCafe.Menu.Application.Features.Categories.CreateCategory;

namespace SmartCafe.Menu.API.Endpoints.Categories;

public static class CreateCategoryEndpoint
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters");
                
            RuleFor(x => x.IconUrl)
                .MaximumLength(1000).WithMessage("Icon URL must not exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.IconUrl));
        }
    }

    public static RouteGroupBuilder MapCreateCategory(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateCategoryRequest request,
            CreateCategoryHandler handler,
            IValidator<CreateCategoryRequest> validator,
            CancellationToken ct) =>
        {
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var result = await handler.HandleAsync(request, ct);
            return Results.Created($"/api/categories/{result.Id}", result);
        })
        .WithName("CreateCategory")
        .WithSummary("Create a new custom category")
        .Produces<CategoryDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        return group;
    }
}
