using FluentAssertions;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Core.Entities;
using UniversityEvents.UnitTests.Common;

namespace UniversityEvents.UnitTests.Repositories;

public class CategoryRepositoryTests : TestBase
{
    private readonly CategoryRepository _repository;

    public CategoryRepositoryTests()
    {
        //_repository = new CategoryRepository(Context, CacheMock.Object);
    }

    [Fact]
    public async Task CreateOrUpdateCategoryAsync_ShouldAddCategory()
    {
        var vm = new CategoryVm { Name = "Music", Description = "Music events" };

        var result = await _repository.CreateOrUpdateCategoryAsync(vm, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        (await Context.Categories.FindAsync(result.Id)).Name.Should().Be("Music");
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ShouldReturnCategory()
    {
        var category = new Category { Name = "Tech" };
        await Context.Categories.AddAsync(category);
        await Context.SaveChangesAsync();

        var result = await _repository.GetCategoryByIdAsync(category.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Tech");
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldMarkIsDelete_WhenExists()
    {
        var category = new Category { Name = "Health" };
        await Context.Categories.AddAsync(category);
        await Context.SaveChangesAsync();

        var result = await _repository.DeleteCategoryAsync(category.Id, CancellationToken.None);

        result.Should().BeTrue();
        (await Context.Categories.FindAsync(category.Id)).IsDelete.Should().BeTrue();
    }

    [Fact]
    public async Task GetCategoriesAsync_ShouldReturnPaginatedList()
    {
        var categories = Enumerable.Range(1, 10).Select(i => new Category { Name = $"Category {i}" }).ToList();
        await Context.Categories.AddRangeAsync(categories);
        await Context.SaveChangesAsync();

        var filter = new Filter { Page = 1, PageSize = 5 };
        var result = await _repository.GetCategoriesAsync(filter, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(5);
        result.TotalItems.Should().Be(10);
    }
}
