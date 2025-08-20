using Microsoft.AspNetCore.Components;

namespace StudentCourseEnrollmentApp.UI.Services
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }

    public class ToastMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ToastType Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsVisible { get; set; } = true;
    }

    public interface IToastService
    {
        event Action<List<ToastMessage>> OnToastsChanged;
        List<ToastMessage> Toasts { get; }
        void ShowSuccess(string title, string message);
        void ShowError(string title, string message);
        void ShowWarning(string title, string message);
        void ShowInfo(string title, string message);
        void RemoveToast(Guid id);
        void ClearAll();
    }

    public class ToastService : IToastService
    {
        private readonly List<ToastMessage> _toasts = new();
        private readonly int _maxToasts = 5;

        public event Action<List<ToastMessage>> OnToastsChanged = null!;
        public List<ToastMessage> Toasts => _toasts.ToList();

        public void ShowSuccess(string title, string message)
        {
            AddToast(new ToastMessage { Title = title, Message = message, Type = ToastType.Success });
        }

        public void ShowError(string title, string message)
        {
            AddToast(new ToastMessage { Title = title, Message = message, Type = ToastType.Error });
        }

        public void ShowWarning(string title, string message)
        {
            AddToast(new ToastMessage { Title = title, Message = message, Type = ToastType.Warning });
        }

        public void ShowInfo(string title, string message)
        {
            AddToast(new ToastMessage { Title = title, Message = message, Type = ToastType.Info });
        }

        public void RemoveToast(Guid id)
        {
            var toast = _toasts.FirstOrDefault(t => t.Id == id);
            if (toast != null)
            {
                toast.IsVisible = false;
                _toasts.Remove(toast);
                OnToastsChanged?.Invoke(_toasts);
            }
        }

        public void ClearAll()
        {
            _toasts.Clear();
            OnToastsChanged?.Invoke(_toasts);
        }

        private void AddToast(ToastMessage toast)
        {
            if (_toasts.Count >= _maxToasts)
            {
                _toasts.RemoveAt(0);
            }

            _toasts.Add(toast);
            OnToastsChanged?.Invoke(_toasts);

            // Auto-remove after 5 seconds
            _ = Task.Delay(5000).ContinueWith(_ =>
            {
                RemoveToast(toast.Id);
            });
        }
    }
}
