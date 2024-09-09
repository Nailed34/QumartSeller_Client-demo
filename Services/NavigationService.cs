using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;

namespace ClientWPF.Services
{
    internal interface INavigationVM
    {
        public void OnNavigated();
    }

    internal class NavigationService
    {
        private static Dictionary<ObservableObject, Page> CreatedLocators { get; set; } = new();
        private static Dictionary<Type, Type> RegisteredLocators { get; set; } = new();
        private static Dictionary<string, Frame> NavigationFrames { get; set; } = new();

        /// <summary>
        /// Create association between view model (VM) and view (V). This method doesn't create them until the Navigate method is called.
        /// </summary>
        public static void RegisterLocator<VM, V>() where VM : ObservableObject where V : Page
        {
            var page = typeof(V);
            var viewModel = typeof(VM);
            RegisteredLocators.TryAdd(viewModel, page);
        }

        /// <summary>
        /// Add frame for navigation to associated list. Use navigationFrameName in Navigate method
        /// </summary>
        public static void RegisterNavigationFrame(string navigationFrameName, Frame navigationFrame)
        {
            NavigationFrames.TryAdd(navigationFrameName, navigationFrame);
        }

        /// <summary>
        /// Navigate target frame to page. Page determines by associated view model by RegisterLocator method.
        /// Use view model type in T and Frame name registered by RegisterNavigationFrame method.
        /// Creates a new page and viewmodel if they were not created before the call
        /// </summary>
        public static void Navigate<T>(string targetNavigationFrame) where T : ObservableObject
        {
            // Find navigation frame
            Frame? targetFrame;
            if (!NavigationFrames.TryGetValue(targetNavigationFrame, out targetFrame))
                return;

            // Try find created locator
            var findLocator = CreatedLocators.Where(vm => vm.Key.GetType() == typeof(T));
            if (findLocator.Count() > 0)
            {
                // Navigate
                targetFrame.Navigate(findLocator.First().Value);

                // If VM has interface call it method
                var vm = findLocator.First().Key;
                if (vm is INavigationVM)
                    ((INavigationVM)vm).OnNavigated();
            }
            // Try find registered locator
            else
            {
                var findRegLocator = RegisteredLocators.Where(vm => typeof(T) == vm.Key);
                if (findRegLocator.Count() > 0)
                {
                    // Get page and vm types
                    var viewModel = findRegLocator.First().Key;
                    var page = findRegLocator.First().Value;

                    if (page != null && viewModel != null)
                    {
                        // Create new view model and page view
                        var newPage = Activator.CreateInstance(page);
                        var newViewModel = Activator.CreateInstance(viewModel);

                        if (newPage != null && newViewModel != null)
                        {
                            // Save to created locators
                            CreatedLocators.TryAdd((ObservableObject)newViewModel, (Page)newPage);

                            // Associate view with view model
                            ((Page)newPage).DataContext = newViewModel;

                            // Navigate
                            targetFrame.Navigate(newPage);

                            // If VM has interface call it method
                            if (newViewModel is INavigationVM)
                                ((INavigationVM)newViewModel).OnNavigated();
                        }
                    }
                }
            }
        }
    }
}
