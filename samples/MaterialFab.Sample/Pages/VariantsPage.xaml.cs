using System.Windows.Input;

namespace MaterialFab.Sample.Pages;

public partial class VariantsPage : ContentPage
{
	static readonly string[] Icons = ["ic_add.png", "ic_edit.png", "ic_navigation.png", "ic_image.png", "ic_arrow_up.png"];

	int _clicks;
	int _iconIndex;

	public VariantsPage()
	{
		PlayCommand = new Command<string>(p => ShowStatus($"Command with parameter \"{p}\""));
		InitializeComponent();
		BindingContext = this;

		SizePicker.ItemsSource = Enum.GetNames<FabSize>();
		SizePicker.SelectedIndex = (int)FabSize.Normal;
	}

	public ICommand PlayCommand { get; }

	void ShowStatus(string text) => StatusLabel.Text = $"#{++_clicks}: {text}";

	void OnFabClicked(object? sender, EventArgs e) => ShowStatus(sender switch
	{
		ExtendedFloatingActionButton efab => $"Extended FAB \"{efab.Text}\"",
		FloatingActionButton fab => $"FAB {fab.Size}",
		_ => "?",
	});

	void OnToggleExtended(object? sender, EventArgs e)
	{
		ComposeFab.IsExtended = !ComposeFab.IsExtended;
		NavigateFab.IsExtended = !NavigateFab.IsExtended;
	}

	void OnToggleExtendedShown(object? sender, EventArgs e)
	{
		ComposeFab.IsShown = !ComposeFab.IsShown;
		NavigateFab.IsShown = !NavigateFab.IsShown;
	}

	void OnToggleExtendedVisible(object? sender, EventArgs e)
	{
		ComposeFab.IsVisible = !ComposeFab.IsVisible;
		NavigateFab.IsVisible = !NavigateFab.IsVisible;
	}

	void OnChangeText(object? sender, EventArgs e) =>
		ComposeFab.Text = ComposeFab.Text == "Compose" ? "Write a new message" : "Compose";

	void OnSizeChanged(object? sender, EventArgs e)
	{
		if (SizePicker.SelectedIndex >= 0)
			PlayFab.Size = (FabSize)SizePicker.SelectedIndex;
	}

	void OnCornerChanged(object? sender, ValueChangedEventArgs e)
	{
		PlayFab.CornerRadius = Math.Round(e.NewValue);
		CornerLabel.Text = $"CornerRadius: {PlayFab.CornerRadius}";
	}

	void OnElevationChanged(object? sender, ValueChangedEventArgs e)
	{
		PlayFab.Elevation = Math.Round(e.NewValue);
		ElevationLabel.Text = $"Elevation: {PlayFab.Elevation}";
	}

	void OnTogglePlayShown(object? sender, EventArgs e) => PlayFab.IsShown = !PlayFab.IsShown;

	void OnTogglePlayEnabled(object? sender, EventArgs e) => PlayFab.IsEnabled = !PlayFab.IsEnabled;

	void OnRandomColor(object? sender, EventArgs e)
	{
		var bg = Color.FromHsla(Random.Shared.NextDouble(), 0.7, 0.5);
		PlayFab.BackgroundColor = bg;
		PlayFab.IconColor = bg.GetLuminosity() > 0.5 ? Colors.Black : Colors.White;
	}

	void OnResetColor(object? sender, EventArgs e)
	{
		PlayFab.ClearValue(BackgroundColorProperty);
		PlayFab.ClearValue(FabBase.IconColorProperty);
	}

	void OnChangeIcon(object? sender, EventArgs e) =>
		PlayFab.Icon = Icons[++_iconIndex % Icons.Length];

	async void OnRotate(object? sender, EventArgs e)
	{
		await PlayFab.RotateToAsync(360, 400, Easing.CubicInOut);
		PlayFab.Rotation = 0;
	}
}
