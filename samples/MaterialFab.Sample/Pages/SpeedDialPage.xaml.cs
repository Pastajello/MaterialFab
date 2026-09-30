namespace MaterialFab.Sample.Pages;

public partial class SpeedDialPage : ContentPage
{
	bool _isOpen;

	public SpeedDialPage()
	{
		InitializeComponent();
	}

	// Bottom to top - the order they appear in when opening.
	(FloatingActionButton Fab, View Label)[] Items => [(Mini3, Label3), (Mini2, Label2), (Mini1, Label1)];

	void OnMainClicked(object? sender, EventArgs e) => _ = SetOpenAsync(!_isOpen);

	void OnScrimTapped(object? sender, TappedEventArgs e) => _ = SetOpenAsync(false);

	void OnMiniClicked(object? sender, EventArgs e)
	{
		var index = Array.FindIndex(Items, i => i.Fab == sender);
		StatusLabel.Text = $"Picked action #{Items.Length - index}";
		_ = SetOpenAsync(false);
	}

	async Task SetOpenAsync(bool open)
	{
		if (_isOpen == open)
			return;
		_isOpen = open;

		MainFab.Icon = open ? "ic_close.png" : "ic_add.png";
		_ = MainFab.RotateToAsync(open ? 90 : 0, 200, Easing.CubicOut);

		if (open)
		{
			Scrim.IsVisible = true;
			_ = Scrim.FadeToAsync(0.4, 200);
		}
		else
		{
			_ = Scrim.FadeToAsync(0, 200).ContinueWith(_ => Dispatcher.Dispatch(() => Scrim.IsVisible = _isOpen));
		}

		var items = open ? Items : Items.Reverse().ToArray();
		foreach (var (fab, label) in items)
		{
			if (_isOpen != open)
				return; // tapped again mid-animation

			fab.IsShown = open;
			_ = label.FadeToAsync(open ? 1 : 0, 150);
			await Task.Delay(40);
		}
	}
}
