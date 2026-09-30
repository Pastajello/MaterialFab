namespace MaterialFab;

/// <summary>What <see cref="FabScrollBehavior"/> does with the FAB while scrolling.</summary>
public enum FabScrollAction
{
	/// <summary>Do nothing.</summary>
	None,
	/// <summary>Scrolling down hides the FAB, scrolling up shows it again.</summary>
	Hide,
	/// <summary>
	/// Scrolling down shrinks an <see cref="ExtendedFloatingActionButton"/> to its icon, scrolling up extends it.
	/// Behaves like <see cref="Hide"/> for a regular <see cref="FloatingActionButton"/>.
	/// </summary>
	Shrink,
}

/// <summary>
/// Hides or shrinks a FAB in response to scrolling, the MAUI equivalent of Android's
/// <c>CoordinatorLayout</c> behaviors. MAUI pages are not hosted in a <c>CoordinatorLayout</c>,
/// so the scroll reaction has to be wired up explicitly.
/// Attach it to an <see cref="ItemsView"/> (CollectionView, CarouselView) or a <see cref="ScrollView"/>.
/// </summary>
public class FabScrollBehavior : Behavior<View>
{
	public static readonly BindableProperty FabProperty =
		BindableProperty.Create(nameof(Fab), typeof(FabBase), typeof(FabScrollBehavior));

	public static readonly BindableProperty ActionProperty =
		BindableProperty.Create(nameof(Action), typeof(FabScrollAction), typeof(FabScrollBehavior), FabScrollAction.Shrink);

	public static readonly BindableProperty ThresholdProperty =
		BindableProperty.Create(nameof(Threshold), typeof(double), typeof(FabScrollBehavior), 24d);

	/// <summary>The FAB to control, usually set with <c>{x:Reference}</c>.</summary>
	public FabBase? Fab
	{
		get => (FabBase?)GetValue(FabProperty);
		set => SetValue(FabProperty, value);
	}

	/// <summary>What to do on scroll. Defaults to <see cref="FabScrollAction.Shrink"/>.</summary>
	public FabScrollAction Action
	{
		get => (FabScrollAction)GetValue(ActionProperty);
		set => SetValue(ActionProperty, value);
	}

	/// <summary>How far to scroll in one direction before reacting. Defaults to 24.</summary>
	public double Threshold
	{
		get => (double)GetValue(ThresholdProperty);
		set => SetValue(ThresholdProperty, value);
	}

	double _lastScrollY;
	double _accumulated;

	protected override void OnAttachedTo(View bindable)
	{
		base.OnAttachedTo(bindable);

		if (bindable is ItemsView itemsView)
			itemsView.Scrolled += OnItemsViewScrolled;
		else if (bindable is ScrollView scrollView)
			scrollView.Scrolled += OnScrollViewScrolled;
		else
			throw new InvalidOperationException($"{nameof(FabScrollBehavior)} can only be attached to an ItemsView or a ScrollView.");
	}

	protected override void OnDetachingFrom(View bindable)
	{
		if (bindable is ItemsView itemsView)
			itemsView.Scrolled -= OnItemsViewScrolled;
		else if (bindable is ScrollView scrollView)
			scrollView.Scrolled -= OnScrollViewScrolled;

		base.OnDetachingFrom(bindable);
	}

	void OnItemsViewScrolled(object? sender, ItemsViewScrolledEventArgs e) =>
		OnScrolled(e.VerticalDelta, e.VerticalOffset);

	void OnScrollViewScrolled(object? sender, ScrolledEventArgs e)
	{
		var delta = e.ScrollY - _lastScrollY;
		_lastScrollY = e.ScrollY;
		OnScrolled(delta, e.ScrollY);
	}

	void OnScrolled(double delta, double offset)
	{
		if (Fab is null || Action == FabScrollAction.None)
			return;

		// Always show the full FAB at the very top.
		if (offset <= 0)
		{
			_accumulated = 0;
			Expand(Fab);
			return;
		}

		// A change of direction resets the counter so small jitters don't toggle the FAB.
		if (Math.Sign(delta) != Math.Sign(_accumulated))
			_accumulated = 0;

		_accumulated += delta;

		if (_accumulated > Threshold)
			Collapse(Fab);
		else if (_accumulated < -Threshold)
			Expand(Fab);
	}

	void Collapse(FabBase fab)
	{
		if (Action == FabScrollAction.Shrink && fab is ExtendedFloatingActionButton efab)
			efab.IsExtended = false;
		else
			fab.IsShown = false;
	}

	void Expand(FabBase fab)
	{
		if (Action == FabScrollAction.Shrink && fab is ExtendedFloatingActionButton efab)
			efab.IsExtended = true;

		fab.IsShown = true;
	}
}
