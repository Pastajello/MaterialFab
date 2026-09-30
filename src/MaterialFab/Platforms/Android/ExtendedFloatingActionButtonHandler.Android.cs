using Android.Content.Res;
using Android.Widget;
using Google.Android.Material.Shape;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using AExtendedFab = Google.Android.Material.FloatingActionButton.ExtendedFloatingActionButton;

namespace MaterialFab;

public partial class ExtendedFloatingActionButtonHandler : ViewHandler<ExtendedFloatingActionButton, FrameLayout>
{
	public static IPropertyMapper<ExtendedFloatingActionButton, ExtendedFloatingActionButtonHandler> Mapper =
		new PropertyMapper<ExtendedFloatingActionButton, ExtendedFloatingActionButtonHandler>(ViewMapper)
		{
			[nameof(IView.Background)] = MapBackground,
			[nameof(IView.IsEnabled)] = MapIsEnabled,
			[nameof(IView.Semantics)] = MapSemantics,

			[nameof(ExtendedFloatingActionButton.Text)] = MapText,
			[nameof(ExtendedFloatingActionButton.TextColor)] = MapTextColor,
			[nameof(FabBase.Icon)] = MapIcon,
			[nameof(FabBase.IconColor)] = MapIconColor,
			[nameof(FabBase.RippleColor)] = MapRippleColor,
			[nameof(FabBase.CornerRadius)] = MapCornerRadius,
			[nameof(FabBase.Elevation)] = MapElevation,
			// Order matters: text and icon must be set before the first shrink/extend.
			[nameof(ExtendedFloatingActionButton.IsExtended)] = MapIsExtended,
			[nameof(FabBase.IsShown)] = MapIsShown,
		};

	AExtendedFab? _fab;
	int _iconVersion;

	ColorStateList? _defaultBackgroundTint;
	ColorStateList? _defaultIconTint;
	ColorStateList? _defaultTextColor;
	ColorStateList? _defaultRipple;
	ShapeAppearanceModel? _defaultShape;
	float _defaultElevation;

	public ExtendedFloatingActionButtonHandler() : base(Mapper)
	{
	}

	/// <summary>The native Material extended FAB (inside the <see cref="ViewHandler{TVirtualView, TPlatformView}.PlatformView"/> container).</summary>
	public AExtendedFab NativeFab => _fab ?? throw new InvalidOperationException("The handler is not connected.");

	protected override FrameLayout CreatePlatformView()
	{
		_fab = new AExtendedFab(Context);

		_defaultBackgroundTint = _fab.BackgroundTintList;
		_defaultIconTint = _fab.IconTint;
		_defaultTextColor = _fab.TextColors;
		_defaultRipple = _fab.RippleColor;
		_defaultShape = _fab.ShapeAppearanceModel;
		_defaultElevation = _fab.Elevation;

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

	public static void MapBackground(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view) =>
		handler.NativeFab.BackgroundTintList = FabPlatformHelpers.BackgroundToTint(view, handler._defaultBackgroundTint);

	public static void MapIsEnabled(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view)
	{
		ViewHandler.MapIsEnabled(handler, view);
		handler.NativeFab.Enabled = view.IsEnabled;
	}

	public static void MapSemantics(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view)
	{
		ViewHandler.MapSemantics(handler, view);
		handler.NativeFab.ContentDescription = ((IView)view).Semantics?.Description ?? view.Text;
	}

	public static void MapText(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view)
	{
		handler.NativeFab.Text = view.Text;
		((IView)view).InvalidateMeasure();
	}

	public static void MapTextColor(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view)
	{
		var color = view.TextColor.ToColorStateList(handler._defaultTextColor);
		if (color is not null)
			handler.NativeFab.SetTextColor(color);
	}

	public static void MapIcon(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view) =>
		_ = handler.LoadIconAsync(view.Icon);

	async Task LoadIconAsync(ImageSource? source)
	{
		var version = ++_iconVersion;
		var drawable = await FabPlatformHelpers.LoadDrawableAsync(source, MauiContext);

		if (version != _iconVersion || _fab is null || VirtualView is null)
			return;

		_fab.Icon = drawable;
		((IView)VirtualView).InvalidateMeasure();
	}

	public static void MapIconColor(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view) =>
		handler.NativeFab.IconTint = view.IconColor.ToColorStateList(handler._defaultIconTint);

	public static void MapRippleColor(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view) =>
		handler.NativeFab.RippleColor = view.RippleColor.ToColorStateList(handler._defaultRipple);

	public static void MapCornerRadius(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view)
	{
		var shape = FabPlatformHelpers.ToShape(handler.Context, view.CornerRadius, handler._defaultShape);
		if (shape is not null)
			handler.NativeFab.ShapeAppearanceModel = shape;
	}

	public static void MapElevation(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view) =>
		handler.NativeFab.Elevation = view.Elevation < 0
			? handler._defaultElevation
			: handler.Context.ToPixels(view.Elevation);

	public static void MapIsExtended(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view)
	{
		var fab = handler.NativeFab;

		if (fab.Extended == view.IsExtended)
			return;

		// No animation before the first layout; after that, the native morph (width + label fade).
		if (!fab.IsLaidOut)
			fab.Extended = view.IsExtended;
		else if (view.IsExtended)
			fab.Extend();
		else
			fab.Shrink();
	}

	public static void MapIsShown(ExtendedFloatingActionButtonHandler handler, ExtendedFloatingActionButton view)
	{
		var fab = handler.NativeFab;
		FabPlatformHelpers.ApplyIsShown(fab, view.IsShown, fab.Show, fab.Hide);
	}
}
