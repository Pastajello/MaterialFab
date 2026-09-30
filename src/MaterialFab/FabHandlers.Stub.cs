#if !ANDROID
// Placeholder handlers for platforms other than Android: an empty 0x0 view,
// so XAML using the FABs compiles and runs everywhere and the FABs are simply not shown.
#if IOS || MACCATALYST
using PlatformView = UIKit.UIView;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.Controls.Grid;
#else
using PlatformView = System.Object;
#endif
using Microsoft.Maui.Handlers;

namespace MaterialFab;

public partial class FloatingActionButtonHandler : ViewHandler<FloatingActionButton, PlatformView>
{
	public static IPropertyMapper<FloatingActionButton, FloatingActionButtonHandler> Mapper =
		new PropertyMapper<FloatingActionButton, FloatingActionButtonHandler>(ViewMapper);

	public FloatingActionButtonHandler() : base(Mapper)
	{
	}

	protected override PlatformView CreatePlatformView() => new();

	public override Size GetDesiredSize(double widthConstraint, double heightConstraint) => Size.Zero;
}

public partial class ExtendedFloatingActionButtonHandler : ViewHandler<ExtendedFloatingActionButton, PlatformView>
{
	public static IPropertyMapper<ExtendedFloatingActionButton, ExtendedFloatingActionButtonHandler> Mapper =
		new PropertyMapper<ExtendedFloatingActionButton, ExtendedFloatingActionButtonHandler>(ViewMapper);

	public ExtendedFloatingActionButtonHandler() : base(Mapper)
	{
	}

	protected override PlatformView CreatePlatformView() => new();

	public override Size GetDesiredSize(double widthConstraint, double heightConstraint) => Size.Zero;
}
#endif
