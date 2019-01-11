using System;
using System.Windows.Media;

namespace GazRouter.Flobus.Utilites
{
    public static class GeometryParser
    {
        public static Geometry GetGeometry(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            var geometry = GetEllipseGeometry(path);
            if (geometry != null) return geometry;

            geometry = GetRectGeometry(path);
            if (geometry != null) return geometry;

            geometry = GetLineGeometry(path);
            if (geometry != null) return geometry;

            return GeometryExtensions.GetPathGeometry(path);
        }

        private static Geometry GetLineGeometry(string path)
        {
            if (!path.StartsWith("L", StringComparison.Ordinal)) return null;
            throw new NotImplementedException();
        }

        private static Geometry GetRectGeometry(string path)
        {
            if (!path.StartsWith("R", StringComparison.Ordinal)) return null;

            throw new NotImplementedException();
        }

        private static Geometry GetEllipseGeometry(string path)
        {
            if (!path.StartsWith("E", StringComparison.Ordinal)) return null;

            throw new NotImplementedException();
        }
    }
}