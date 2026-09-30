# MaterialFab

Material Design **Floating Action Button** (FAB) and **Extended FAB** for .NET MAUI.

On Android the controls are backed by the real Material Components widgets
(`com.google.android.material.floatingactionbutton.*`), so you get the native ripple, elevation,
shape theming and the shrink / extend / show / hide animations for free.
On other platforms the controls render as empty 0x0 placeholder views, so shared XAML compiles and runs everywhere.

> **Version 0.1** – early preview. The API may change.

| Platform | Status |
|---|---|
| Android | Native Material `FloatingActionButton` / `ExtendedFloatingActionButton` |
| iOS, Mac Catalyst, Windows | Placeholder (empty 0x0 view) |

Requires .NET 11 / .NET MAUI 11.

## Getting started

Reference the library project (it is not published to NuGet yet), or build the package with `dotnet pack src/MaterialFab`.

Register the handlers in `MauiProgram.cs`:

```csharp
using MaterialFab;

builder
    .UseMauiApp<App>()
    .UseMaterialFab();
```

Add the XML namespace to your page:

```xml
xmlns:fab="clr-namespace:MaterialFab;assembly=MaterialFab"
```

## Usage

A FAB is a regular view. To make it "float", put it in the same `Grid` cell as the content and align it to a corner:

```xml
<Grid>
    <CollectionView ... />

    <fab:FloatingActionButton Icon="ic_add.png"
                              HorizontalOptions="End" VerticalOptions="End" Margin="16"
                              SemanticProperties.Description="Add item"
                              Command="{Binding AddCommand}" />
</Grid>
```

### Sizes, colors, shape

```xml
<fab:FloatingActionButton Icon="ic_add.png" Size="Mini" />      <!-- 40dp -->
<fab:FloatingActionButton Icon="ic_add.png" Size="Normal" />    <!-- 56dp, default -->
<fab:FloatingActionButton Icon="ic_add.png" Size="Large" />     <!-- 96dp -->

<fab:FloatingActionButton Icon="ic_edit.png"
                          BackgroundColor="#E91E63"
                          IconColor="White"
                          RippleColor="#FFEB3B"
                          CornerRadius="16"
                          Elevation="12" />
```

### Extended FAB

```xml
<fab:ExtendedFloatingActionButton x:Name="ComposeFab"
                                  Icon="ic_edit.png" Text="Compose"
                                  HorizontalOptions="End" VerticalOptions="End" Margin="16"
                                  Clicked="OnComposeClicked" />
```

```csharp
ComposeFab.IsExtended = false; // animated morph to icon only (native Shrink())
ComposeFab.IsExtended = true;  // animated morph back to icon + label (native Extend())
```

### Show / hide

`IsShown` uses the native `Show()` / `Hide()` animation (scale + fade).
`IsVisible` still works, but switches the view instantly.

```csharp
fab.IsShown = false; // or fab.Hide()
fab.IsShown = true;  // or fab.Show()
```

## Reacting to scroll

Material apps usually hide the FAB, or shrink the extended FAB, while the user scrolls down, and bring it back when they scroll up.
On native Android that is done by `CoordinatorLayout` behaviors. MAUI pages are not hosted in a `CoordinatorLayout`,
so MaterialFab ships `FabScrollBehavior`, which you attach to the scrolling view.

### Shrink an extended FAB on a CollectionView

```xml
<Grid>
    <CollectionView ItemsSource="{Binding Messages}">
        <CollectionView.Behaviors>
            <fab:FabScrollBehavior Fab="{x:Reference ComposeFab}" Action="Shrink" />
        </CollectionView.Behaviors>
        ...
    </CollectionView>

    <fab:ExtendedFloatingActionButton x:Name="ComposeFab"
                                      Icon="ic_edit.png" Text="Compose"
                                      HorizontalOptions="End" VerticalOptions="End" Margin="16" />
</Grid>
```

Scrolling down shrinks the FAB to its icon, scrolling up extends it again. At the top of the list it is always extended.

### Hide a FAB on a ScrollView

```xml
<Grid>
    <ScrollView>
        <ScrollView.Behaviors>
            <fab:FabScrollBehavior Fab="{x:Reference EditFab}" Action="Hide" Threshold="48" />
        </ScrollView.Behaviors>
        ...
    </ScrollView>

    <fab:FloatingActionButton x:Name="EditFab" Icon="ic_edit.png"
                              HorizontalOptions="End" VerticalOptions="End" Margin="16" />
</Grid>
```

`FabScrollBehavior` properties:

| Property | Default | Description |
|---|---|---|
| `Fab` | – | The FAB to control, usually `{x:Reference}`. |
| `Action` | `Shrink` | `Shrink` shrinks an extended FAB (a regular FAB is hidden instead), `Hide` hides it, `None` disables the behavior. |
| `Threshold` | `24` | Distance to scroll in one direction before reacting, so small jitters don't toggle the FAB. |

The behavior works with any `ItemsView` (`CollectionView`, `CarouselView`) and with `ScrollView`.

### "Scroll to top" FAB

For custom rules, set `IsShown` / `IsExtended` yourself from the `Scrolled` event.
This example shows a mini FAB once the list is scrolled past the first 15 items:

```xml
<CollectionView x:Name="List" Scrolled="OnListScrolled" ... />

<fab:FloatingActionButton x:Name="ScrollTopFab" Icon="ic_arrow_up.png" Size="Mini" IsShown="False"
                          HorizontalOptions="Center" VerticalOptions="End" Margin="16"
                          Clicked="OnScrollTopClicked" />
```

```csharp
void OnListScrolled(object? sender, ItemsViewScrolledEventArgs e) =>
    ScrollTopFab.IsShown = e.FirstVisibleItemIndex > 15;

void OnScrollTopClicked(object? sender, EventArgs e) =>
    List.ScrollTo(0, position: ScrollToPosition.Start, animate: true);
```

`IsShown` and `IsExtended` are two-way bindable, so the same thing can be driven from a view model.

## API

### `FabBase` (shared)

| Property | Type | Default | Description |
|---|---|---|---|
| `Icon` | `ImageSource` | `null` | Any image source (file, font, stream, URI). |
| `IconColor` | `Color` | theme | Icon tint. |
| `BackgroundColor` / `Background` | `Color` / `Brush` | theme | Container color. Only solid colors are supported. |
| `RippleColor` | `Color` | theme | Touch ripple color. |
| `CornerRadius` | `double` | `-1` | `-1` uses the theme shape (circle in M2, rounded square in M3). |
| `Elevation` | `double` | `-1` | Resting elevation. `-1` uses the theme value. |
| `IsShown` | `bool` | `true` | Animated show / hide. |
| `Command`, `CommandParameter` | | | Executed on tap. |
| `Clicked` | event | | Raised on tap. |

Methods: `Show()`, `Hide()`.

### `FloatingActionButton`

| Property | Type | Default |
|---|---|---|
| `Size` | `FabSize` (`Mini`, `Normal`, `Large`) | `Normal` |

### `ExtendedFloatingActionButton`

| Property | Type | Default | Description |
|---|---|---|---|
| `Text` | `string` | `null` | The label. |
| `TextColor` | `Color` | theme | Label color. |
| `IsExtended` | `bool` | `true` | `false` shrinks to icon only (animated). |

Methods: `Extend()`, `Shrink()`.

## Material 3

The FABs follow the app theme. To get Material 3 styling, enable it in the app project:

```xml
<UseMaterial3>true</UseMaterial3>
```

## Customizing the native view

The handlers expose the native widget as `NativeFab`, so anything the MAUI API doesn't cover can be set through the mapper:

```csharp
FloatingActionButtonHandler.Mapper.AppendToMapping("CompatPadding", (handler, view) =>
{
#if ANDROID
    handler.NativeFab.UseCompatPadding = true;
#endif
});
```

## Known limitations

- iOS, Mac Catalyst and Windows only get placeholder views.
- No `CoordinatorLayout` integration: the FAB does not move up automatically for a Snackbar, and scroll reactions go through `FabScrollBehavior`.
- `Background` supports solid colors only (gradients are ignored).
- On Android the FAB is wrapped in a `FrameLayout` so MAUI layout can follow the native animations. To keep the shadow from being clipped,
  the wrapper turns off `clipChildren` on its direct parent view.

## Sample app

`samples/MaterialFab.Sample` has four tabs:

- **Variants**: sizes, colors, shapes, elevation, the extended FAB, and a playground with live controls.
- **List**: a `CollectionView` with an extended FAB that shrinks (or hides) on scroll, plus a mini "scroll to top" FAB.
- **Scroll**: a `ScrollView` with a FAB that hides on scroll and an adjustable threshold.
- **Speed dial**: a speed-dial menu built from mini FABs with staggered native animations.

Run it on Android:

```bash
dotnet build samples/MaterialFab.Sample -f net11.0-android -t:Run
```

## Project layout

```
MaterialFab.slnx
src/MaterialFab/                 the library
  FabBase.cs, FloatingActionButton.cs, ExtendedFloatingActionButton.cs
  FabScrollBehavior.cs
  AppHostBuilderExtensions.cs    UseMaterialFab()
  FabHandlers.Stub.cs            placeholder handlers for non-Android platforms
  Platforms/Android/             native handlers
samples/MaterialFab.Sample/      sample app
```

## Author

Pastajello
