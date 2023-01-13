using Diagram4.Adorners;
using Diagram4.Models;
using Diagram4.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Diagram4.ViewModels
{
    internal class DragInfo
    {
        public StrategySetViewModel? StrategySetFrom { get; set; }
        public StrategySetViewModel? StrategySetTo { get; set;}
        public StrategyViewModel? StrategyFrom { get; set; }
        public bool Dragging { get; set; }
    }
    internal class DiagramViewModel:ViewModelBase
    {
        private Model _model;
        private readonly DragInfo _dragInfo = new DragInfo();
        public Canvas Canvas { get; set; }
        public ObservableCollection<UserControl> DiagramItems { get; }
        public List<ViewModelBase> DiagramItemViewModels { get; }
        public DisplayTileViewModel DisplayTile0 { get; }
        public DisplayTileViewModel DisplayTile1 { get; }
        public DisplayTileViewModel DisplayTile2 { get; }
        public DisplayTileViewModel DisplayTile3 { get; }
        public Command DropCommand { get; }
        public Command ClickCommand { get; }
        public Command MouseLeftButtonUpCommand { get; }
        public event Action? CanvasClicked;
        public event Action<object>? KeyDown;

        public DiagramViewModel(Model model)
        {
            _model = model;
            DisplayTile0 = new DisplayTileViewModel() { Text = "HS", ImageName = "../Images/114514.jpeg" };
            DisplayTile1 = new DisplayTileViewModel() { Text = "PS", ImageName = "../Images/fufu.jpg" };
            DisplayTile2 = new DisplayTileViewModel() { Text = "H", ImageName = "../Images/53.jpg" };
            DisplayTile3 = new DisplayTileViewModel() { Text = "P", ImageName = "../Images/gua.jpg" };
            DropCommand = new Command(OnDrop);
            ClickCommand = new Command(OnClick);
            MouseLeftButtonUpCommand= new Command(OnMouseLeftButtonUp);
            DiagramItems = new ObservableCollection<UserControl>();
            DiagramItemViewModels = new List<ViewModelBase>();
        }
        public void OnDrop(object? obj)
        {
            DragEventArgs e = (obj as DragEventArgs)!;
            DisplayTileViewModel displayTile = (e.Data.GetData(typeof(DisplayTileViewModel)) as DisplayTileViewModel)!;
            if (displayTile != null)
            {
                Canvas canvas = (e.Source as Canvas)!;
                if (canvas != null)
                {
                    Point pos = e.GetPosition(canvas);
                    switch(displayTile.Text)
                    {
                        default:
                            {
                                StrategySetView strategySetView = new StrategySetView();
                                StrategySetViewModel strategySetViewModel = new StrategySetViewModel();
                                strategySetViewModel.Text = "Strategy Set Name Here!";
                                strategySetViewModel.ImageName = "../Images/114514.jpeg";
                                strategySetViewModel.DragStarted += OnDragStarted;
                                CanvasClicked += strategySetViewModel.OnCanvasClicked;
                                KeyDown += strategySetViewModel.OnKeyDown;
                                strategySetView.DataContext= strategySetViewModel;
                                DiagramItems.Add(strategySetView);
                                DiagramItemViewModels.Add(strategySetViewModel);
                                
                                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(strategySetView);
                                if (adornerLayer==null)
                                {
                                    Console.WriteLine("Failed to get adorner layer!");
                                }
                                else
                                {
                                    MoveAdorner moveAdorner = new MoveAdorner(strategySetView);
                                    adornerLayer.Add(moveAdorner);
                                }
                                Canvas.SetLeft(strategySetView, pos.X);
                                Canvas.SetTop(strategySetView, pos.Y);
                                break;
                            }
                    }
                }
                else
                {
                    Console.WriteLine("The item doesn't drop on canvas!");
                }
            }
        }
        public void OnClick(object? obj)
        {
            Console.WriteLine("Clicked on canvas!");
            CanvasClicked?.Invoke();
        }
        public void OnKeyDown(object obj)
        {
            KeyDown?.Invoke(obj);
        }
        void OnMouseLeftButtonUp(object? obj)
        {
            Console.WriteLine("Mouse released, drag interrupted");
            _dragInfo.Dragging = false;
        }
        void OnDragStarted(StrategySetViewModel strategySetViewModel,
            StrategyViewModel strategyViewModel,
            MouseButtonEventArgs e)
        {
            _dragInfo.Dragging = true;
            _dragInfo.StrategySetFrom = strategySetViewModel;
            _dragInfo.StrategyFrom = strategyViewModel;
            e.GetPosition()
            Console.WriteLine()
        }
    }
}
