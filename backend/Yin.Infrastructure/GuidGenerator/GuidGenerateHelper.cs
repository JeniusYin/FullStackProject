using System;

namespace Yin.Infrastructure.GuidGenerator
{
    public class GuidGenerateHelper
    {
        public static Guid GenerateSequenceGuid()
        {
            IGuidGenerator generate = new SequentialGuidGenerator(new SequentialGuidGeneratorOptions()
            {
                DefaultSequentialGuidType = SequentialGuidType.SequentialAsString
            });
            return generate.Create();
        }
    }
}
