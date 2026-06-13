$controllersDir = "d:\ChuyenDeASPNet\Warehouse Management System\be-asp\QLKHO_PhanVanHoang\Controllers"

# Ensure using Attributes is present
Get-ChildItem -Path $controllersDir -Filter "*.cs" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -notmatch "using QLKHO_PhanVanHoang.Attributes;") {
        $content = $content -replace "using Microsoft.AspNetCore.Mvc;", "using Microsoft.AspNetCore.Mvc;`r`nusing QLKHO_PhanVanHoang.Attributes;"
        Set-Content -Path $_.FullName -Value $content
    }
}

function Update-ControllerPermissions {
    param (
        [string]$ControllerName,
        [string]$ViewPerm,
        [string]$EditPerm
    )
    $path = Join-Path $controllersDir "$ControllerName.cs"
    if (Test-Path $path) {
        $content = Get-Content $path -Raw

        # Replace GET endpoints with View Perm
        $content = $content -replace '\[HttpGet\("?([^"]*)"?\)\]\s*\[Authorize\(Roles\s*=\s*AppRoles\.All\)\]', "[HttpGet(`"`$1`")]`r`n        [HasPermission(`"$ViewPerm`")]"
        $content = $content -replace '\[HttpGet\]\s*\[Authorize\(Roles\s*=\s*AppRoles\.All\)\]', "[HttpGet]`r`n        [HasPermission(`"$ViewPerm`")]"
        
        # Replace POST/PUT/DELETE endpoints with Edit Perm
        $content = $content -replace '\[HttpPost\("?([^"]*)"?\)\]\s*\[Authorize\(Roles\s*=\s*(AppRoles\.All|AppRoles\.AdminOrManager)\)\]', "[HttpPost(`"`$1`")]`r`n        [HasPermission(`"$EditPerm`")]"
        $content = $content -replace '\[HttpPost\]\s*\[Authorize\(Roles\s*=\s*(AppRoles\.All|AppRoles\.AdminOrManager)\)\]', "[HttpPost]`r`n        [HasPermission(`"$EditPerm`")]"

        $content = $content -replace '\[HttpPut\("?([^"]*)"?\)\]\s*\[Authorize\(Roles\s*=\s*(AppRoles\.All|AppRoles\.AdminOrManager)\)\]', "[HttpPut(`"`$1`")]`r`n        [HasPermission(`"$EditPerm`")]"
        
        $content = $content -replace '\[HttpDelete\("?([^"]*)"?\)\]\s*\[Authorize\(Roles\s*=\s*(AppRoles\.All|AppRoles\.AdminOrManager)\)\]', "[HttpDelete(`"`$1`")]`r`n        [HasPermission(`"$EditPerm`")]"

        # Also replace class-level if any
        # ... but usually it's [Authorize] at class level.

        Set-Content -Path $path -Value $content
        Write-Host "Updated $ControllerName"
    }
}

Update-ControllerPermissions -ControllerName "ProductsController" -ViewPerm "PRODUCT_VIEW" -EditPerm "PRODUCT_EDIT"
Update-ControllerPermissions -ControllerName "CategoriesController" -ViewPerm "MASTER_DATA_VIEW" -EditPerm "MASTER_DATA_EDIT"
Update-ControllerPermissions -ControllerName "CustomersController" -ViewPerm "MASTER_DATA_VIEW" -EditPerm "MASTER_DATA_EDIT"
Update-ControllerPermissions -ControllerName "SuppliersController" -ViewPerm "MASTER_DATA_VIEW" -EditPerm "MASTER_DATA_EDIT"
Update-ControllerPermissions -ControllerName "WarehousesController" -ViewPerm "WAREHOUSE_VIEW" -EditPerm "MASTER_DATA_EDIT"

Update-ControllerPermissions -ControllerName "ReceivingVouchersController" -ViewPerm "INBOUND_VIEW" -EditPerm "INBOUND_CREATE"
# Approving requires manual update or just using EditPerm for now (we can do fine-tuning later, but INBOUND_CREATE covers creation)
Update-ControllerPermissions -ControllerName "DeliveryVouchersController" -ViewPerm "OUTBOUND_VIEW" -EditPerm "OUTBOUND_CREATE"
Update-ControllerPermissions -ControllerName "TransferVouchersController" -ViewPerm "TRANSFER_VIEW" -EditPerm "TRANSFER_CREATE"
Update-ControllerPermissions -ControllerName "CountingSheetsController" -ViewPerm "COUNTING_VIEW" -EditPerm "COUNTING_APPROVE"
Update-ControllerPermissions -ControllerName "InventoryAdjustmentsController" -ViewPerm "COUNTING_VIEW" -EditPerm "COUNTING_APPROVE"

Update-ControllerPermissions -ControllerName "InventoriesController" -ViewPerm "REPORT_VIEW" -EditPerm "REPORT_VIEW"
Update-ControllerPermissions -ControllerName "StockCardsController" -ViewPerm "STOCK_CARD_VIEW" -EditPerm "STOCK_CARD_VIEW"
Update-ControllerPermissions -ControllerName "ReportsController" -ViewPerm "REPORT_VIEW" -EditPerm "REPORT_VIEW"

# System
Update-ControllerPermissions -ControllerName "UsersController" -ViewPerm "USER_MANAGEMENT" -EditPerm "USER_MANAGEMENT"
Update-ControllerPermissions -ControllerName "RolesController" -ViewPerm "USER_MANAGEMENT" -EditPerm "USER_MANAGEMENT"
Update-ControllerPermissions -ControllerName "PermissionsController" -ViewPerm "USER_MANAGEMENT" -EditPerm "USER_MANAGEMENT"
Update-ControllerPermissions -ControllerName "AuditLogsController" -ViewPerm "SYSTEM_LOGS" -EditPerm "SYSTEM_LOGS"

Write-Host "Done"
