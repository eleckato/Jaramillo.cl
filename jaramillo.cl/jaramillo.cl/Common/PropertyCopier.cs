
namespace jaramillo.cl.Common
{
    public static class PropertyCopier
    {
        public static dynamic Copy(dynamic parent, dynamic child)
        {
            var parentProperties = parent.GetType().GetProperties();
            var childProperties = child.GetType().GetProperties();

            if (parentProperties == childProperties) return null;

            foreach (var parentProperty in parentProperties)
            {
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                    {
                        childProperty.SetValue(child, parentProperty.GetValue(parent));
                        break;
                    }
                }
            }

            return child;
        }
    }

}