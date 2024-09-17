using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace DicomApp.Models
{
    public class BloodVessel3DRegion
    {
        public HashSet<Point3D> SelectedVoxels { get; private set; }

        public BloodVessel3DRegion()
        {
            SelectedVoxels = new HashSet<Point3D>();
        }

        public void AddVoxel(Point3D voxel)
        {
            SelectedVoxels.Add(voxel);
        }

        public void RemoveVoxel(Point3D voxel)
        {
            SelectedVoxels.Remove(voxel);
        }

        public bool ContainsVoxel(Point3D voxel)
        {
            return SelectedVoxels.Contains(voxel);
        }

        public void Clear()
        {
            SelectedVoxels.Clear();
        }
    }
}