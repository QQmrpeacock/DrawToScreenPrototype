using DrawToScreenPrototype.Core.Mvvm;
using DrawToScreenPrototype.Services.Interfaces;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace DrawToScreenPrototype.Modules.ModuleName.ViewModels
{
    public class RectItem : BindableBase
    {
        public RectItem(Brush colour, double width, double x)
        {
            Colour = colour;
            Width = width;
            X = x;
        }

        private double _height;
        public Brush Colour { get; set; }
        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }
        public double Width { get; set; }
        public double X { get; set; }

    }
    public class SimpleCanvasViewModel : RegionViewModelBase
    {

        private const int _CANVAS = 1000;
        private const double _TICKRATE = 1;
        private readonly Brush[] _colourLUT = new Brush[10] {
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f2636a")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dc4b4f")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c63333")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e94900")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ef6803")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f58606")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c1b225")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#72a85f")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#229e99")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#36a7a2"))
        };
        private const double _INTERVAL = 1 / _TICKRATE;
        private static readonly TimeSpan _SPAN = TimeSpan.FromSeconds(_INTERVAL);
        private RectItem[] _rectItems;

        public static int CANVAS { get => _CANVAS; }
        public static double TICKRATE { get => _TICKRATE; }

        public static Duration DURATION
        {
            get { return new Duration(_SPAN); }
        }
        public static KeyTime KEYTIME
        {
            get { return KeyTime.FromTimeSpan(_SPAN); }
            // this is why converters are needed.....
        }

        public RectItem[] RectItems
        {
            get { return _rectItems; }
            set { SetProperty(ref _rectItems, value); }
        }

        public SimpleCanvasViewModel(IRegionManager regionManager, IMessageService messageService) :
            base(regionManager)
        {
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            BuildBars(100);
            InfLoop();
            //Dispatcher.CurrentDispatcher.InvokeAsync( new Action(() =>  InfLoop()), DispatcherPriority.Background);
        }

        public void BuildBars(int n)
        {
            // n is limited to canvas size + 1
            // n cannot be less than 0

            if (n <= 0 || n > CANVAS) return;

            // force C# to calculate this as a double.
            // two ints will produce an int even if it results is stored to a double.
            // at least one must be cast to a double to get a double back from the division. 

            double padding = ((double)CANVAS / (double)10) / n;
            double width = ((double)CANVAS / (double)n) - padding;

            var localRectItems = new RectItem[n];

            for (int i = 0; i < n; i++)
            {
                double xPosition = (width + padding) * (double)i;
                localRectItems[i] = new RectItem(_colourLUT[i % 10],width, xPosition);
            }
            RectItems = localRectItems;

        }

        private async void InfLoop()
        {
            var rand = new Random();

            while (RectItems != null)
            {
                await Task.Delay(_SPAN); // 20hz

                for (int i = 0; i < RectItems.Length; i++)
                {
                    //RectItems[i].Height = x;

                    //use dispatcher to send updates async, at larger n of bars they will update sequentially at a slight offset from eachother like a wave.
                    int x = rand.Next(0, CANVAS + 1);
                    int iLocal = i;
                    int xLocal = x;
                    _ = Dispatcher.CurrentDispatcher.InvokeAsync(new Action(() => RectItems[iLocal].Height = xLocal), DispatcherPriority.Background);
                }

                
            }
        }

    }
}