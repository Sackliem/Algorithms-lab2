using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sponge
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // The sponge's current depth;
        private int SpongeDepth = 3;

        // The camera.
        private PerspectiveCamera TheCamera;

        // The camera's current location.
        private double CameraPhi = Math.PI / 6.0;       // 30 degrees
        private double CameraTheta = Math.PI / 6.0;     // 30 degrees
        private double CameraR = 7.0;

        // The change in CameraPhi when you press the up and down arrows.
        private const double CameraDPhi = 0.1;

        // The change in CameraTheta when you press the left and right arrows.
        private const double CameraDTheta = 0.1;

        // The change in CameraR when you press + or -.
        private const double CameraDR = 0.5;

        // Create the scene.
        // MainViewport is the Viewport3D defined
        // in the XAML code that displays everything.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboDepth.SelectedIndex = 2;

            // Give the camera its initial position.
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            MainViewport.Camera = TheCamera;
            PositionCamera();

            // Create the main model including the lights.
            //Model3DGroup model_group = PrepareModel();
            PrepareModel();
        }

        // Make the model.
        private void PrepareModel()
        {
            // Make a model group.
            Model3DGroup model_group = new Model3DGroup();

            // Create lights.
            DefineLights(model_group);

            // Define the model's data.

            Stopwatch watch = new Stopwatch();
            watch.Start();
            MakeGeometryData(model_group, SpongeDepth);
            watch.Stop();
            Console.WriteLine(watch.Elapsed.TotalMilliseconds.ToString("0.00") + " Milliseconds");

            // Add the group of models to a ModelVisual3D.
            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = model_group;

            // Display the main visual in the viewportt.
            MainViewport.Children.Clear();
            MainViewport.Children.Add(model_visual);

            // Return the new model group.

            //return model_group;
        }

        // Define the lights.
        private void DefineLights(Model3DGroup model_group)
        {
            model_group.Children.Add(new AmbientLight(Colors.Gray));
            model_group.Children.Add(
                new DirectionalLight(Colors.Gray,
                    new Vector3D(3.0, -2.0, 1.0)));
            model_group.Children.Add(
                new DirectionalLight(Colors.Gray,
                    new Vector3D(-3.0, -2.0, -1.0)));
        }

        // Add the model to the Model3DGroup.
        private void MakeGeometryData(Model3DGroup model_group, int depth)
        {
            // Make a mesh to hold the surface.
            MeshGeometry3D mesh = new MeshGeometry3D();
            // Make the sponge.
            MakeSponge(mesh, -2, 2, -2, 2, -2, 2, depth);

            Console.WriteLine(mesh.Positions.Count + " points");
            Console.WriteLine(mesh.TriangleIndices.Count / 3 + " triangles");

            // Make the surface's material using a solid brush.
            DiffuseMaterial surface_material = new DiffuseMaterial(Brushes.LightGray); // цвет куба

            // Make the mesh's model.
            GeometryModel3D surface_model = new GeometryModel3D(mesh, surface_material);

            // Make the surface visible from both sides.
            surface_model.BackMaterial = surface_material;

            // Add the model to the model groups.
            model_group.Children.Add(surface_model);
        }


        private void MakeSponge(MeshGeometry3D mesh, double xmin, double xmax,
            double ymin, double ymax, double zmin, double zmax, int depth)
        {
            // See if this is depth 1.
            if (depth == 1)
            {
                // Just make a cube.
                AddCube(mesh, xmin, xmax, ymin, ymax, zmin, zmax);
            }
            else
            {
                // Divide the cube.
                double dx = (xmax - xmin) / 3.0;
                double dy = (ymax - ymin) / 3.0;
                double dz = (zmax - zmin) / 3.0;
                for (int ix = 0; ix < 3; ix++)
                {
                    for (int iy = 0; iy < 3; iy++)
                    {
                        if ((ix == 1) && (iy == 1)) continue;
                        for (int iz = 0; iz < 3; iz++)
                        {
                            if ((iz == 1) && ((ix == 1) || (iy == 1))) continue;
                            MakeSponge(mesh,
                                xmin + dx * ix, xmin + dx * (ix + 1),
                                ymin + dy * iy, ymin + dy * (iy + 1),
                                zmin + dz * iz, zmin + dz * (iz + 1),
                                depth - 1);
                        }
                    }
                }
            }
        }

        // Add a cube to the indicated mesh.
        private void AddCube(MeshGeometry3D mesh, double xmin, double xmax,
            double ymin, double ymax, double zmin, double zmax)
        {
            // Top.
            AddTriangle(mesh,
                new Point3D(xmin, ymax, zmax),
                new Point3D(xmax, ymax, zmax),
                new Point3D(xmax, ymax, zmin));
            AddTriangle(mesh,
                new Point3D(xmin, ymax, zmax),
                new Point3D(xmax, ymax, zmin),
                new Point3D(xmin, ymax, zmin));
            // Bottom.
            AddTriangle(mesh,
                new Point3D(xmin, ymin, zmax),
                new Point3D(xmin, ymin, zmin),
                new Point3D(xmax, ymin, zmin));
            AddTriangle(mesh,
                new Point3D(xmin, ymin, zmax),
                new Point3D(xmax, ymin, zmin),
                new Point3D(xmax, ymin, zmax));
            // Left.
            AddTriangle(mesh,
                new Point3D(xmin, ymax, zmax),
                new Point3D(xmin, ymax, zmin),
                new Point3D(xmin, ymin, zmin));
            AddTriangle(mesh,
                new Point3D(xmin, ymax, zmax),
                new Point3D(xmin, ymin, zmin),
                new Point3D(xmin, ymin, zmax));
            // Right.
            AddTriangle(mesh,
                new Point3D(xmax, ymax, zmax),
                new Point3D(xmax, ymin, zmax),
                new Point3D(xmax, ymin, zmin));
            AddTriangle(mesh,
                new Point3D(xmax, ymax, zmax),
                new Point3D(xmax, ymin, zmin),
                new Point3D(xmax, ymax, zmin));
            // Front.
            AddTriangle(mesh,
                new Point3D(xmin, ymax, zmax),
                new Point3D(xmin, ymin, zmax),
                new Point3D(xmax, ymin, zmax));
            AddTriangle(mesh,
                new Point3D(xmin, ymax, zmax),
                new Point3D(xmax, ymin, zmax),
                new Point3D(xmax, ymax, zmax));
            // Back.
            AddTriangle(mesh,
                new Point3D(xmin, ymin, zmin),
                new Point3D(xmax, ymin, zmin),
                new Point3D(xmax, ymax, zmin));
            AddTriangle(mesh,
                new Point3D(xmin, ymin, zmin),
                new Point3D(xmax, ymax, zmin),
                new Point3D(xmin, ymax, zmin));
        }

        // Add a triangle to the indicated mesh.
        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Get the points' indices.
            int index1 = AddPoint(mesh.Positions, point1);
            int index2 = AddPoint(mesh.Positions, point2);
            int index3 = AddPoint(mesh.Positions, point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }

        // Create the point and return its new index.
        private int AddPoint(Point3DCollection points, Point3D point)
        {
            // Create the point.
            points.Add(point);
            return points.Count - 1;
        }

        // Adjust the camera's position.
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    CameraPhi += CameraDPhi;
                    if (CameraPhi > Math.PI / 2.0) CameraPhi = Math.PI / 2.0;
                    e.Handled = true;
                    break;
                case Key.Down:
                    CameraPhi -= CameraDPhi;
                    if (CameraPhi < -Math.PI / 2.0) CameraPhi = -Math.PI / 2.0;
                    e.Handled = true;
                    break;
                case Key.Left:
                    CameraTheta += CameraDTheta;
                    e.Handled = true;
                    break;
                case Key.Right:
                    CameraTheta -= CameraDTheta;
                    e.Handled = true;
                    break;
                case Key.Add:
                case Key.OemPlus:
                    CameraR -= CameraDR;
                    if (CameraR < CameraDR) CameraR = CameraDR;
                    e.Handled = true;
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    CameraR += CameraDR;
                    e.Handled = true;
                    break;
            }

            // Update the camera's position.
            PositionCamera();
        }

        // Position the camera.
        private void PositionCamera()
        {
            // Calculate the camera's position in Cartesian coordinates.
            double y = CameraR * Math.Sin(CameraPhi);
            double hyp = CameraR * Math.Cos(CameraPhi);
            double x = hyp * Math.Cos(CameraTheta);
            double z = hyp * Math.Sin(CameraTheta);
            TheCamera.Position = new Point3D(x, y, z);

            // Look toward the origin.
            TheCamera.LookDirection = new Vector3D(-x, -y, -z);

            // Set the Up direction.
            TheCamera.UpDirection = new Vector3D(0, 1, 0);

            // Console.WriteLine("Camera.Position: (" + x + ", " + y + ", " + z + ")");
        }

        // Rebuild the model for the new depth.
        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // If the window isn't loaded, do nothing.
            if (MainViewport == null) return;

            // If the sponge's depth hasn't changed, do nothing.
            Label item = cboDepth.SelectedItem as Label;
            int new_depth = int.Parse(item.Content.ToString());
            if (new_depth == SpongeDepth) return;
            SpongeDepth = new_depth;

            // Rebuild the model.
            PrepareModel();

            // Get focus out of the ComboBox.
            MainViewport.Focus();
        }
    }
}
