using Android.Opengl;
using Android.Views;

namespace AndroidTestApp
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public static readonly object ActivityLock = new object();

        private OpenTKGLSurfaceView glView;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Title = "Test";

            RequestWindowFeature(WindowFeatures.NoTitle);
            Window?.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            Window?.SetFlags(WindowManagerFlags.TranslucentNavigation, WindowManagerFlags.TranslucentNavigation);
            Window.DecorView.SystemUiFlags = SystemUiFlags.Fullscreen | SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.LayoutHideNavigation | SystemUiFlags.ImmersiveSticky;

            glView = new OpenTKGLSurfaceView(this);
            SetContentView(glView);
        }
    }
}
