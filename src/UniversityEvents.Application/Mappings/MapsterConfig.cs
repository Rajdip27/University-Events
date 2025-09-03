using Mapster;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Core.Entities;

namespace UniversityEvents.Application.Mappings;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.NewConfig<Category, CategoryVm>();
             
    }
}
