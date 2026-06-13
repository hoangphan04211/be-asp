namespace QLKHO_PhanVanHoang.Constants
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string WarehouseManager = "WarehouseManager";
        public const string Employee = "Employee";
        
        public const string AdminOrManager = Admin + "," + WarehouseManager;
        public const string All = Admin + "," + WarehouseManager + "," + Employee;
    }
}

