using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace StudentCourseEnrollmentApp.UI.Services
{
    public class LoadingService
    {
        public event Action<bool, int, string>? LoadingStateChanged;
        
        private bool _isLoading = true;
        private int _progress = 0;
        private string _loadingMessage = "Initializing application...";
        
        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    LoadingStateChanged?.Invoke(_isLoading, _progress, _loadingMessage);
                }
            }
        }
        
        public int Progress
        {
            get => _progress;
            private set
            {
                if (_progress != value)
                {
                    _progress = value;
                    LoadingStateChanged?.Invoke(_isLoading, _progress, _loadingMessage);
                }
            }
        }
        
        public string LoadingMessage
        {
            get => _loadingMessage;
            private set
            {
                if (_loadingMessage != value)
                {
                    _loadingMessage = value;
                    LoadingStateChanged?.Invoke(_isLoading, _progress, _loadingMessage);
                }
            }
        }
        
        public async Task StartLoadingAsync()
        {
            // Ensure splash screen is visible immediately
            IsLoading = true;
            Progress = 0;
            LoadingMessage = "Initializing application...";
            
            // Simulate initialization steps with shorter delays
            await SimulateLoadingStep("Checking authentication...", 20);
            await SimulateLoadingStep("Loading user preferences...", 40);
            await SimulateLoadingStep("Preparing course data...", 60);
            await SimulateLoadingStep("Setting up navigation...", 80);
            await SimulateLoadingStep("Finalizing setup...", 100);
        }
        
        private async Task SimulateLoadingStep(string message, int targetProgress)
        {
            LoadingMessage = message;
            
            // Gradually increase progress to target with faster updates
            while (Progress < targetProgress)
            {
                Progress = Math.Min(Progress + 10, targetProgress);
                await Task.Delay(100); // Reduced delay for faster progress
            }
        }
        
        public void SetProgress(int progress, string message = null)
        {
            Progress = Math.Clamp(progress, 0, 100);
            if (!string.IsNullOrEmpty(message))
            {
                LoadingMessage = message;
            }
        }
        
        public void SetLoadingMessage(string message)
        {
            LoadingMessage = message;
        }
        
        public async Task CompleteLoadingAsync()
        {
            Progress = 100;
            LoadingMessage = "Ready!";
            
            // Show completion message briefly
            await Task.Delay(500); // Reduced delay
            
            // Fade out
            IsLoading = false;
        }
        
        public void HideSplashScreen()
        {
            IsLoading = false;
        }
        
        public void ShowLoading(string message = "Loading...")
        {
            IsLoading = true;
            LoadingMessage = message;
        }
        
        public void HideLoading()
        {
            IsLoading = false;
        }
    }
}
