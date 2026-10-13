# Fails (exit 1) when combined line coverage of the named packages is below the threshold.
# Reads every coverage.cobertura.xml under -Path, merges hits per source line across
# files, and counts each (file, line) once.
param(
    [Parameter(Mandatory)] [double] $Threshold,
    [Parameter(Mandatory)] [string[]] $Packages,
    [Parameter(Mandatory)] [string] $Path
)

$Packages = $Packages | ForEach-Object { $_ -split ',' } | Where-Object { $_ }

$reports = Get-ChildItem -Path $Path -Recurse -Filter coverage.cobertura.xml
if (-not $reports) {
    Write-Error "No coverage reports found under $Path"
    exit 1
}

$hits = @{}   # "<package>|<file>|<line>" -> max hits
foreach ($report in $reports) {
    [xml]$xml = Get-Content $report.FullName
    foreach ($package in $xml.coverage.packages.package) {
        if ($Packages -notcontains $package.name) { continue }
        foreach ($class in $package.classes.class) {
            foreach ($line in $class.lines.line) {
                # Different test projects report the same source file with different
                # prefixes ("Game.cs" vs "Blackjack.Game\Game.cs"); normalise so each
                # line is counted once.
                $file = ($class.filename -replace '/', '\') -replace "^$([regex]::Escape($package.name))\\", ''
                $key = "$($package.name)|$file|$($line.number)"
                $count = [int]$line.hits
                if (-not $hits.ContainsKey($key) -or $hits[$key] -lt $count) { $hits[$key] = $count }
            }
        }
    }
}

$failed = $false
foreach ($package in $Packages) {
    $keys = $hits.Keys | Where-Object { $_.StartsWith("$package|") }
    $total = @($keys).Count
    if ($total -eq 0) {
        # A package whose project does not exist yet is skipped; an existing project
        # without coverage data is a failure so the gate can never pass silently.
        if (Test-Path (Join-Path (Get-Location) "src/$package")) {
            Write-Host "::error::No coverage data for existing package $package"
            $failed = $true
        } else {
            Write-Host "::notice::Package $package not present yet, skipped"
        }
        continue
    }
    $covered = @($keys | Where-Object { $hits[$_] -gt 0 }).Count
    $pct = [math]::Round(100.0 * $covered / $total, 2)
    $status = if ($pct -ge $Threshold) { "OK" } else { "BELOW THRESHOLD" }
    Write-Host ("{0,-24} {1,6}%  ({2}/{3} lines)  {4}" -f $package, $pct, $covered, $total, $status)
    if ($pct -lt $Threshold) { $failed = $true }
}

if ($failed) {
    Write-Error "Line coverage below $Threshold%."
    exit 1
}
