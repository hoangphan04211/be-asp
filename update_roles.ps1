$dir = "d:\ChuyenDeASPNet\Warehouse Management System\be-asp\QLKHO_PhanVanHoang\Controllers"
$files = Get-ChildItem -Path $dir -Filter "*.cs" -Recurse

foreach ($f in $files) {
    $content = Get-Content $f.FullName -Raw
    
    if ($content -match "\[Authorize") {
        $modified = $false
        
        if ($content -notmatch "using QLKHO_PhanVanHoang\.Constants;") {
            $content = "using QLKHO_PhanVanHoang.Constants;`r`n" + $content
            $modified = $true
        }
        
        if ($content -match '\[Authorize\(Roles\s*=\s*"Admin,WarehouseManager,Staff"\)\]') {
            $content = $content -replace '\[Authorize\(Roles\s*=\s*"Admin,WarehouseManager,Staff"\)\]', '[Authorize(Roles = AppRoles.All)]'
            $modified = $true
        }
        if ($content -match '\[Authorize\(Roles\s*=\s*"Admin,WarehouseManager,Employee"\)\]') {
            $content = $content -replace '\[Authorize\(Roles\s*=\s*"Admin,WarehouseManager,Employee"\)\]', '[Authorize(Roles = AppRoles.All)]'
            $modified = $true
        }
        if ($content -match '\[Authorize\(Roles\s*=\s*"Admin,WarehouseManager"\)\]') {
            $content = $content -replace '\[Authorize\(Roles\s*=\s*"Admin,WarehouseManager"\)\]', '[Authorize(Roles = AppRoles.AdminOrManager)]'
            $modified = $true
        }
        if ($content -match '\[Authorize\(Roles\s*=\s*"Admin"\)\]') {
            $content = $content -replace '\[Authorize\(Roles\s*=\s*"Admin"\)\]', '[Authorize(Roles = AppRoles.Admin)]'
            $modified = $true
        }
        
        if ($modified) {
            Set-Content -Path $f.FullName -Value $content -Encoding UTF8
        }
    }
}
