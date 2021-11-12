using System;
using Volo.Abp.Guids;

namespace Yin.Infrastructure.Abp.Guids
{
    public class GuidGenerateHelper
    {
        public static Guid GenerateSequenceGuid()
        {
            IGuidGenerator generate = new SequentialGuidGenerator(new AbpSequentialGuidGeneratorOptions()
            {
                DefaultSequentialGuidType = SequentialGuidType.SequentialAsString
            });
            return generate.Create();
        }
    }
}
