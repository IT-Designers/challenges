namespace SubmissionEvaluation.Client.Services
{
    public class MaintenanceService
    {
        public delegate void MaintenanceChange();

        public event MaintenanceChange OnMaintenanceChange;

        public void InvokeEvent()
        {
            OnMaintenanceChange?.Invoke();
        }
    }
}
