using System.Globalization;

namespace MaterialFab.Sample.Pages;

public partial class ListPage : ContentPage
{
	static readonly string[] Names = ["Anna", "Bob", "Carol", "Dave", "Eve", "Frank", "Grace", "Heidi", "Ivan", "Judy"];

	public ListPage()
	{
		Resources.Add("FirstLetter", new FirstLetterConverter());
		InitializeComponent();

		List.ItemsSource = Enumerable.Range(1, 200).Select(i => $"{Names[i % Names.Length]} - message #{i}").ToList();

		ActionPicker.ItemsSource = Enum.GetNames<FabScrollAction>();
		ActionPicker.SelectedIndex = (int)FabScrollAction.Shrink;
	}

	void OnActionChanged(object? sender, EventArgs e)
	{
		if (ActionPicker.SelectedIndex < 0)
			return;

		ScrollBehavior.Action = (FabScrollAction)ActionPicker.SelectedIndex;

		// Reset the FAB when the mode changes.
		ComposeFab.IsExtended = true;
		ComposeFab.IsShown = true;
	}

	void OnListScrolled(object? sender, ItemsViewScrolledEventArgs e) =>
		ScrollTopFab.IsShown = e.FirstVisibleItemIndex > 15;

	void OnScrollTopClicked(object? sender, EventArgs e) =>
		List.ScrollTo(0, position: ScrollToPosition.Start, animate: true);

	async void OnComposeClicked(object? sender, EventArgs e) =>
		await DisplayAlertAsync("Compose", "The primary screen action was tapped.", "OK");

	sealed class FirstLetterConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
			value is string { Length: > 0 } s ? s[..1] : "";

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
			throw new NotSupportedException();
	}
}
