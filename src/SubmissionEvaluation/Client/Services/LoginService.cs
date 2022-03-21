namespace SubmissionEvaluation.Client.Services
{
    public class LoginService
    {
        public delegate void LoginChanged();

        public event LoginChanged OnLoginChange;

        public void InvokeEvent()
        {
            OnLoginChange?.Invoke();
        }
    }
}
