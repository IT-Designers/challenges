namespace SubmissionEvaluation.Client.Services
{
    public class LoginService
    {
        public delegate void loginChanged();

        public event loginChanged OnLoginChange;

        public void InvokeEvent()
        {
            OnLoginChange?.Invoke();
        }
    }
}
