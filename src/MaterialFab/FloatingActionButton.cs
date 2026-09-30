namespace MaterialFab;

/// <summary>Size of a <see cref="FloatingActionButton"/>.</summary>
public enum FabSize
{
	/// <summary>40dp.</summary>
	Mini,
	/// <summary>56dp (default).</summary>
	Normal,
	/// <summary>96dp (Material 3 large FAB).</summary>
	Large,
}

/// <summary>
/// A round, icon-only floating action button.
/// Android: <c>Google.Android.Material.FloatingActionButton.FloatingActionButton</c>.
/// </summary>
public class FloatingActionButton : FabBase
{
	public static readonly BindableProperty SizeProperty =
		BindableProperty.Create(nameof(Size), typeof(FabSize), typeof(FloatingActionButton), FabSize.Normal);

	/// <summary>Button size. Defaults to <see cref="FabSize.Normal"/>.</summary>
	public FabSize Size
	{
		get => (FabSize)GetValue(SizeProperty);
		set => SetValue(SizeProperty, value);
	}
}
