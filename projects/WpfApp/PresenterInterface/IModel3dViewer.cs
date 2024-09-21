using System;
using System.Windows.Media.Media3D;

namespace DicomApp.PresenterInterface
{
    public interface IModel3dViewer
    {
        void SetModel(Model3DGroup model);
        void Show();
    }
}
