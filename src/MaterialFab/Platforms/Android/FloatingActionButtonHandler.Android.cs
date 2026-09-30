using Android.Content.Res;
using Android.Widget;
using Google.Android.Material.Shape;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using AFab = Google.Android.Material.FloatingActionButton.FloatingActionButton;

namespace MaterialFab;

public partial class FloatingActionButtonHandler : ViewHandler<FloatingActionButton, FrameLayout>
{
	public static IPropertyMapper<FloatingActionButton, FloatingActionButtonHandler> Mapper =
		new PropertyMapper<FloatingActionButton, FloatingActionButtonHandler>(ViewMapper)
		{
			// Override the ViewMapper entries: the defaults would apply background/enabled to the container
			// instead of the native FAB (and replace its MaterialShapeDrawable).
			[nameof(IView.Background)] = MapBackground,
			[nameof(IView.IsEnabled)] = MapIsEnabled,
			[nameof(IView.Semantics)] = MapSemantics,

			[nameof(FloatingActionButton.Size)] = MapSize,
			[nameof(FabBase.Icon)] = MapIcon,
			[nameof(FabBase.IconColor)] = MapIconColor,
			[nameof(FabBase.RippleColor)] = MapRippleColor,
			[nameof(FabBase.CornerRadius)] = MapCornerRadius,
			[nameof(FabBase.Elevation)] = MapElevation,
			[nameof(FabBase.IsShown)] = MapIsShown,
		};

	AFab? _fab;
	int _iconVersion;

	// Native defaults from the theme (M2 or M3), restored when a MAUI property goes back to null/-1.
	ColorStateList? _defaultBackgroundTint;
	ColorStateList? _defaultIconTint;
	ColorStateList? _defaultRipple;
	ShapeAppearanceModel? _defaultShape;
	float? _defaultElevation;
	int _defaultMaxImageSize;

	public FloatingActionButtonHandler() : base(Mapper)
	{
	}

	/// <summary>The native Material FAB (inside the <see cref="ViewHandler{TVirtualView, TPlatformView}.PlatformView"/> container).</summary>
	public AFab NativeFab => _fab ?? throw new InvalidOperationException("The handler is not connected.");

	protected override FrameLayout CreatePlatformView()
	{
		_fab = new AFab(Context);

		_defaultBackgroundTint = _fab.BackgroundTintList;
		_defaultIconTint = _fab.SupportImageTintList;
		_defaultRipple = _fab.RippleColorStateList;
		_defaultShape = _fab.ShapeAppearanceModel;
		_defaultMaxImageSize = (int)Context.ToPixels(24);

		return FabPlatformHelpers.CreateContainer(Context, _fab);
	}

	protected override void ConnectHandler(FrameLayout platformView)
	{
		base.ConnectHandler(platformView);
		NativeFab.Click += OnClick;
	}

	protected override void DisconnectHandler(FrameLayout platformView)
	{
		if (_fab is not null)
			_fab.Click -= OnClick;

		base.DisconnectHandler(platformView);
	}

	void OnClick(object? sender, EventArgs e) => VirtualView?.SendClicked();

	public static void MapBackground(FloatingActionButtonHandler handler, FloatingActionButton view) =>
		handler.NativeFab.BackgroundTintList = FabPlatformHelpers.BackgroundToTint(view, handler._defaultBackgroundTint);

	public static void MapIsEnabled(FloatingActionButtonHandler handler, FloatingActionButton view)
	{
		ViewHandler.MapIsEnabled(handler, view);
		handler.NativeFab.Enabled = view.IsEnabled;
	}

	public static void MapSemantics(FloatingActionButtonHandler handler, FloatingActionButton view)
	{
		ViewHandler.MapSemantics(handler, view);
		handler.NativeFab.ContentDescription = ((IView)view).Semantics?.Description;
	}

	public static void MapSize(FloatingActionButtonHandler handler, FloatingActionButton view)
	{
		var fab = handler.NativeFab;
		var context = handler.Context;

		switch (view.Size)
		{
			case FabSize.Mini:
				fab.CustomSize = AFab.NoCustomSize;
				fab.Size = AFab.SizeMini;
				fab.SetMaxImageSize(handler._defaultMaxImageSize);
				break;
			case FabSize.Large:
				fab.CustomSize = (int)context.ToPixels(96);
				fab.SetMaxImageSize((int)context.ToPixels(36));
				break;
			default:
				fab.CustomSize = AFab.NoCustomSize;
				fab.Size = AFab.SizeNormal;
				fab.SetMaxImageSize(handler._defaultMaxImageSize);
				break;
		}

		((IView)view).InvalidateMeasure();
	}

	public static void MapIcon(FloatingActionButtonHandler handler, FloatingActionButton view) =>
		_ = handler.LoadIconAsync(view.Icon);

	async Task LoadIconAsync(ImageSource? source)
	{
		var version = ++_iconVersion;
		var drawable = await FabPlatformHelpers.LoadDrawableAsync(source, MauiContext);

		// The icon may have changed again, or the handler may have disconnected, while loading.
		if (version != _iconVersion || _fab is null || VirtualView is null)
			return;

		_fab.SetImageDrawable(drawable);
	}

	public static void MapIconColor(FloatingActionButtonHandler handler, FloatingActionButton view) =>
		handler.NativeFab.SupportImageTintList = view.IconColor.ToColorStateList(handler._defaultIconTint);

	public static void MapRippleColor(FloatingActionButtonHandler handler, FloatingActionButton view)
	{
		var ripple = view.RippleColor.ToColorStateList(handler._defaultRipple);
		if (ripple is not null)
			handler.NativeFab.SetRippleColor(ripple);
	}

	public static void MapCornerRadius(FloatingActionButtonHandler handler, FloatingActionButton view)
	{
		var shape = FabPlatformHelpers.ToShape(handler.Context, view.CornerRadius, handler._defaultShape);
		if (shape is not null)
			handler.NativeFab.ShapeAppearanceModel = shape;
	}

	public static void MapElevation(FloatingActionButtonHandler handler, FloatingActionButton view)
	{
		var fab = handler.NativeFab;

		// CompatElevation reads 0 until the StateListAnimator has run, so it can't be captured in
		// CreatePlatformView. Leave the theme value alone for -1 and only remember a default on the first change.
		if (view.Elevation < 0)
		{
			if (handler._defaultElevation is { } elevation)
				fab.CompatElevation = elevation;
			return;
		}

		handler._defaultElevation ??= fab.CompatElevation > 0 ? fab.CompatElevation : handler.Context.ToPixels(6);
		fab.CompatElevation = handler.Context.ToPixels(view.Elevation);
	}

	public static void MapIsShown(FloatingActionButtonHandler handler, FloatingActionButton view)
	{
		var fab = handler.NativeFab;
		FabPlatformHelpers.ApplyIsShown(fab, view.IsShown, fab.Show, fab.Hide);
	}
}
