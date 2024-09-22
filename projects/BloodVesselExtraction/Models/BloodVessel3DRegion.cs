using System.Windows.Media.Media3D;

namespace DicomApp.BloodVesselExtraction.Models
{
    public class BloodVessel3DRegion
    {
        public HashSet<(int X, int Y, int Z)> SelectedVoxels { get; set; }

        public BloodVessel3DRegion()
        {
            SelectedVoxels = new HashSet<(int, int, int)>();
        }

        public void AddVoxel(Point3D voxel)
        {
            SelectedVoxels.Add((
                (int)Math.Round(voxel.X),
                (int)Math.Round(voxel.Y),
                (int)Math.Round(voxel.Z)
            ));
        }

        public void RemoveVoxel(Point3D voxel)
        {
            SelectedVoxels.Remove((
                (int)Math.Round(voxel.X),
                (int)Math.Round(voxel.Y),
                (int)Math.Round(voxel.Z)
            ));
        }

        public bool ContainsVoxel(Point3D voxel)
        {
            return SelectedVoxels.Contains((
                (int)Math.Round(voxel.X),
                (int)Math.Round(voxel.Y),
                (int)Math.Round(voxel.Z)
            ));
        }

        public void Clear()
        {
            SelectedVoxels.Clear();
        }
    }
}
