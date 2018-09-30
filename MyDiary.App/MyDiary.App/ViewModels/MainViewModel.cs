using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using MvvmHelpers;
using ReactiveUI;
using Splat;

namespace MyDiary.App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            

            MenuItems = GetMenuItems();

            NavigateToMenuItem = ReactiveCommand.CreateFromObservable<IRoutableViewModel, Unit>(
                routableVm => Router.NavigateAndReset.Execute(routableVm).Select(_ => Unit.Default));

            this.WhenAnyValue(x => x.Selected)
                .Where(x => x != null)
                .StartWith(MenuItems.First())
                .Select(x => Locator.Current.GetService<IRoutableViewModel>(x.TargetType.FullName))
                .InvokeCommand(NavigateToMenuItem);
        }

        private MasterCellViewModel _selected;
        public MasterCellViewModel Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public ReactiveCommand<IRoutableViewModel, Unit> NavigateToMenuItem { get; }

        public IEnumerable<MasterCellViewModel> MenuItems { get; }

        public RoutingState Router { get; }

        private IEnumerable<MasterCellViewModel> GetMenuItems()
        {
            return new[]
            {
                  new MasterCellViewModel { Title = "My Diary Entries", IconSource = "reminders.png", TargetType = typeof(DiaryViewModel) },
                new MasterCellViewModel { Title = "About", IconSource = "entry.png", TargetType = typeof(AboutViewModel) },
              
                //new MasterCellViewModel { Title = "Personal Information", IconSource = "todo.png", TargetType = typeof(LetterStreamViewModel) },
            };
        }
    }
}