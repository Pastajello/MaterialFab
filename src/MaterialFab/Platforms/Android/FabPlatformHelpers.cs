using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Shape;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;

namespace MaterialFab;

static class FabPlatformHelpers
{
	/// <summary>
	/// The native FAB is wrapped in a FrameLayout. The ExtendedFAB shrink/extend animation works by changing
	/// LayoutParams.width, and MAUI layouts ignore their children's LayoutParams. FrameLayout honors them,
	/// so the container follows the animation and MAUI re-measures on every requestLayout.
	/// Hide() also sets the FAB to GONE, so the container then measures as 0x0.
	/// </summary>
	public static FrameLayout CreateContainer(Context context, AView fab)
	{
		var container = new FrameLayout(context);
		container.SetClipChildren(false);
		container.SetClipToPadding(false);
		container.AddView(fab, new FrameLayout.LayoutParams(
			ViewGroup.LayoutParams.WrapContent,
			ViewGroup.LayoutParams.WrapContent,
			GravityFlags.Center));
		container.ViewAttachedToWindow += OnContainerAttached;
		return container;
	}

	// The FAB shadow extends past the container bounds. MAUI layouts use clipChildren=false, but e.g. Border
	// (ContentViewGroup) does not, which would crop the shadow to the container's rectangle. Without the wrapper
	// the parent would draw the shadow itself, so we only unclip the direct parent.
	static void OnContainerAttached(object? sender, AView.ViewAttachedToWindowEventArgs e)
	{
		if (sender is AView { Parent: ViewGroup parent })
			parent.SetClipChildren(false);
	}

	public static ColorStateList? ToColorStateList(this Color? color, ColorStateList? fallback) =>
		color is null ? fallback : ColorStateList.ValueOf(color.ToPlatform());

	public static ColorStateList? BackgroundToTint(IView view, ColorStateList? fallback) =>
		view.Background is SolidPaint { Color: { } color }
			? ColorStateList.ValueOf(color.ToPlatform())
			: fallback;

	public static ShapeAppearanceModel? ToShape(Context context, double cornerRadius, ShapeAppearanceModel? fallback) =>
		cornerRadius < 0
			? fallback
			: ShapeAppearanceModel.InvokeBuilder()
				.SetAllCorners(CornerFamily.Rounded, context.ToPixels(cornerRadius))
				.Build();

	/// <summary>
	/// Material skips the Show() animation for a view that was never laid out (and a view that is GONE from the
	/// start never is). So the initial hidden state is INVISIBLE: the view still gets laid out and the first Show()
	/// animates.
	/// </summary>
	public static void ApplyIsShown(AView fab, bool isShown, Action show, Action hide)
	{
		if (!fab.IsLaidOut && fab.Visibility == ViewStates.Visible && !isShown)
		{
			fab.Visibility = ViewStates.Invisible;
			return;
		}

		if (isShown)
			show();
		else
			hide();
	}

	public static async Task<Drawable?> LoadDrawableAsync(ImageSource? source, IMauiContext? mauiContext)
	{
		if (source is null || mauiContext is null)
			return null;

		try
		{
			var result = await source.GetPlatformImageAsync(mauiContext);
			return result?.Value;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[MaterialFab] Failed to load icon: {ex}");
			return null;
		}
	}
}
