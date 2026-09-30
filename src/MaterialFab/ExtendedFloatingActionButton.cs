namespace MaterialFab;

/// <summary>
/// A floating action button with an icon and a label that can shrink to icon-only with an animation.
/// Android: <c>Google.Android.Material.FloatingActionButton.ExtendedFloatingActionButton</c>.
/// </summary>
public class ExtendedFloatingActionButton : FabBase
{
	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(ExtendedFloatingActionButton));

	public static readonly BindableProperty TextColorProperty =
		BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ExtendedFloatingActionButton));

	public static readonly BindableProperty IsExtendedProperty =
		BindableProperty.Create(nameof(IsExtended), typeof(bool), typeof(ExtendedFloatingActionButton), true, BindingMode.TwoWay);

	/// <summary>The label.</summary>
	public string? Text
	{
		get => (string?)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	/// <summary>Label color. <see langword="null"/> uses the theme color.</summary>
	public Color? TextColor
	{
		get => (Color?)GetValue(TextColorProperty);
		set => SetValue(TextColorProperty, value);
	}

	/// <summary>
	/// true (default) shows icon and label, false shows the icon only.
	/// Changes are animated with the native Extend()/Shrink() morph.
	/// </summary>
	public bool IsExtended
	{
		get => (bool)GetValue(IsExtendedProperty);
		set => SetValue(IsExtendedProperty, value);
	}

	/// <summary>Animates to icon + label. Same as setting <see cref="IsExtended"/> to true.</summary>
	public void Extend() => IsExtended = true;

	/// <summary>Animates to icon only. Same as setting <see cref="IsExtended"/> to false.</summary>
	public void Shrink() => IsExtended = false;
}
