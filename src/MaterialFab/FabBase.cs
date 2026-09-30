using System.Windows.Input;

namespace MaterialFab;

/// <summary>
/// Properties shared by <see cref="FloatingActionButton"/> and <see cref="ExtendedFloatingActionButton"/>.
/// Rendered natively (Material Components) on Android; an empty 0x0 view on other platforms.
/// </summary>
public abstract class FabBase : View
{
	public static readonly BindableProperty IconProperty =
		BindableProperty.Create(nameof(Icon), typeof(ImageSource), typeof(FabBase));

	public static readonly BindableProperty IconColorProperty =
		BindableProperty.Create(nameof(IconColor), typeof(Color), typeof(FabBase));

	public static readonly BindableProperty RippleColorProperty =
		BindableProperty.Create(nameof(RippleColor), typeof(Color), typeof(FabBase));

	public static readonly BindableProperty CornerRadiusProperty =
		BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(FabBase), -1d);

	public static readonly BindableProperty ElevationProperty =
		BindableProperty.Create(nameof(Elevation), typeof(double), typeof(FabBase), -1d);

	public static readonly BindableProperty IsShownProperty =
		BindableProperty.Create(nameof(IsShown), typeof(bool), typeof(FabBase), true, BindingMode.TwoWay);

	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FabBase));

	public static readonly BindableProperty CommandParameterProperty =
		BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FabBase));

	/// <summary>The icon. Any <see cref="ImageSource"/> works (file, font, stream, URI).</summary>
	public ImageSource? Icon
	{
		get => (ImageSource?)GetValue(IconProperty);
		set => SetValue(IconProperty, value);
	}

	/// <summary>Icon tint. <see langword="null"/> uses the theme color.</summary>
	public Color? IconColor
	{
		get => (Color?)GetValue(IconColorProperty);
		set => SetValue(IconColorProperty, value);
	}

	/// <summary>Touch ripple color. <see langword="null"/> uses the theme color.</summary>
	public Color? RippleColor
	{
		get => (Color?)GetValue(RippleColorProperty);
		set => SetValue(RippleColorProperty, value);
	}

	/// <summary>
	/// Corner radius in device-independent units. -1 (default) uses the theme shape:
	/// a circle in Material 2, a rounded square in Material 3.
	/// </summary>
	public double CornerRadius
	{
		get => (double)GetValue(CornerRadiusProperty);
		set => SetValue(CornerRadiusProperty, value);
	}

	/// <summary>Resting elevation (shadow) in device-independent units. -1 (default) uses the theme elevation.</summary>
	public double Elevation
	{
		get => (double)GetValue(ElevationProperty);
		set => SetValue(ElevationProperty, value);
	}

	/// <summary>
	/// Animated show/hide using the native Show()/Hide() (scale + fade).
	/// Unlike <see cref="VisualElement.IsVisible"/>, which switches the view instantly.
	/// </summary>
	public bool IsShown
	{
		get => (bool)GetValue(IsShownProperty);
		set => SetValue(IsShownProperty, value);
	}

	/// <summary>Executed when the FAB is tapped, if <see cref="ICommand.CanExecute"/> returns true.</summary>
	public ICommand? Command
	{
		get => (ICommand?)GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}

	public object? CommandParameter
	{
		get => GetValue(CommandParameterProperty);
		set => SetValue(CommandParameterProperty, value);
	}

	/// <summary>Raised when the FAB is tapped.</summary>
	public event EventHandler? Clicked;

	/// <summary>Shows the FAB with the native animation. Same as setting <see cref="IsShown"/> to true.</summary>
	public void Show() => IsShown = true;

	/// <summary>Hides the FAB with the native animation. Same as setting <see cref="IsShown"/> to false.</summary>
	public void Hide() => IsShown = false;

	internal void SendClicked()
	{
		Clicked?.Invoke(this, EventArgs.Empty);

		if (Command?.CanExecute(CommandParameter) == true)
			Command.Execute(CommandParameter);
	}
}
