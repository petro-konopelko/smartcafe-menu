using FluentValidation;
using SmartCafe.Menu.Application.Features.Categories.UpdateCategory;

namespace SmartCafe.Menu.API.Endpoints.Categories;

public static class UpdateCategoryEndpoint
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters");
                
            RuleFor(x => x.IconUrl)
                .MaximumLength(1000).WithMessage("Icon URL must not exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.IconUrl));
        }
    }

    public static RouteGroupBuilder MapUpdateCategory(this RouteGroupBuilder group)
    {
        group.MapPut("/{categoryId:guid}", async (
            Guid categoryId,
            UpdateCategoryRequest request,
            UpdateCategoryHandler handler,
            IValidator<UpdateCategoryRequest> validator,
            CancellationToken ct) =>
        {
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var result = await handler.HandleAsync(categoryId, request, ct);
                return Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("UpdateCategory")
        .WithSummary("Update a custom category")
        .Produces<CategoryDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        return group;
    }
}
