using System;

namespace KohonenNet
{
    public class PropertyCountGroupCountRelationViolationException : Exception
    {
        private readonly int propertyCount;
        private readonly int groupCount;

        public int PropertyCount => propertyCount;
        public int GroupCount => groupCount;

        public PropertyCountGroupCountRelationViolationException(int propertyCount, int groupCount):
            base($"Property count / group count violation: property count = {propertyCount}, group count = {groupCount}")
        {
            this.propertyCount = propertyCount;
            this.groupCount = groupCount;
        }
    }
}
