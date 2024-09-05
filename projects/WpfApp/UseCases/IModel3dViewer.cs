using System;
using System.Windows.Media.Media3D;

namespace DicomApp.UseCases
{
    public interface IModel3dViewer
    {
        void SetModel(Model3DGroup model);
        void Show();
    }
}
