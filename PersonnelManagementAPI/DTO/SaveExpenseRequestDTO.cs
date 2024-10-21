namespace PersonnelManagementAPI.DTO
{
    public class SaveExpenseRequestDTO
    {

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExpenseDocument { get; set; }
    }
}
