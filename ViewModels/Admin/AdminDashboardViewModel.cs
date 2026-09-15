namespace exam_system.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalRegistredUsers { get; set; }
        public int TotalActiveUsers { get; set; }
        public int TotalNumberOfDiplomas { get; set; }
        public int TotalNumberOfQuizes { get; set; }
        public int TotalAttembts { get; set; }
        public float AveragePassRate { get; set; }
    }
}
