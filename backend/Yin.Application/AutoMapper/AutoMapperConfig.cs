using AutoMapper;
using System.Collections.Generic;

namespace Yin.Application.AutoMapper
{
    public class AutoMapperConfig
    {
        public static IEnumerable<Profile> GetAllMaProfiles()
        {
            return new List<Profile>()
            {
                new DomainToDtoMappingProfile(),
                new DtoToDomainMappingProfile(),
                new NormalMappingProfile(),
                new DynamicMappingProfile()
            };
        }
    }
}
