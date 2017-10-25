using Xamarin.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace brightcontrolunit2000
{
    public partial class brightcontrolunit2000Page : ContentPage
    {
        ControlManager manager = new ControlManager();
        public brightcontrolunit2000Page()
        {
            InitializeComponent();

            // Clear all
            UpButton.Clicked += async (sender, e) => await manager.MoveUp();
            DownButton.Clicked += async (sender, e) => await manager.MoveDown();
            LeftButton.Clicked += async (sender, e) => await manager.MoveLeft();
            RightButton.Clicked += async (sender, e) => await manager.MoveRight();
            ToggleButton.Clicked += async (sender, e) => await manager.Toggle();
        }
    }

    public class ControlManager
    {
        private int MAX_COLUMNS = 8;
        private int MAX_ROWS = 30;

        private int _currentX = 5;
        private int _currentY = 15;

        private int _lastX;
        private int _lastY;

        HttpClient client;

        private List<string> colors = new List<string>() { "0", "green", "red", "blue" };
        private int _currentColorIndex = 1;

        public ControlManager()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("http://whiteboardapi.azurewebsites.net/api/Whiteboard/");

            var r = new Random();
            _currentX = r.Next(0, MAX_COLUMNS);
            _currentY = r.Next(0, MAX_ROWS);
            Task.Run(async () => await TurnOn(_currentX, _currentY, _currentColorIndex));
        }

        private async Task TurnOn(int x, int y, int colorIndex)
        {
            var path = $"column/{x}/row/{y}/{colors[colorIndex]}";
            await client.GetStringAsync(path);
            await Task.Delay(300);
        }

        public async Task Clear()
        {
            var path = "http://whiteboardapi.azurewebsites.net/api/clear";
            await client.GetStringAsync(path);
        }

        public async Task MoveUp()
        {
            _lastX = _currentX;
            _lastY = _currentY;

            _currentY--;
            if (_currentY < 0)
                _currentY = MAX_ROWS;

            await TurnOn(_currentX, _currentY, _currentColorIndex);
            await TurnOn(_lastX, _lastY, 0);
        }

        public async Task MoveDown()
        {
            _lastX = _currentX;
            _lastY = _currentY;

            _currentY++;
            if (_currentY > MAX_ROWS)
                _currentY = 0;

            await TurnOn(_currentX, _currentY, _currentColorIndex);
            await TurnOn(_lastX, _lastY, 0); 
        }

        public async Task MoveLeft()
        {
            _lastX = _currentX;
            _lastY = _currentY;

            _currentX--;
            if (_currentX < 0)
                _currentX = MAX_COLUMNS;

            await TurnOn(_currentX, _currentY, _currentColorIndex);
            await TurnOn(_lastX, _lastY, 0); 
        }

        public async Task MoveRight()
        {
            _lastX = _currentX;
            _lastY = _currentY; 

            _currentX++;
            if (_currentX > MAX_COLUMNS)
                _currentX = 0;

            await TurnOn(_currentX, _currentY, _currentColorIndex);
            await TurnOn(_lastX, _lastY, 0);  
        }

        public async Task Toggle()
        {
            _currentColorIndex++;
            if (_currentColorIndex > 3)
                _currentColorIndex = 1;

            await TurnOn(_currentX, _currentY, _currentColorIndex);
        }
    }
}
