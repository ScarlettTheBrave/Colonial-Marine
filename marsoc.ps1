# marsoc.ps1 - copy MARSOC-related files into Colonial-Marine, one commit per work day, then push.
# Reads from the source repo only. Writes only to the target repo.

$sourceRepo = "D:\Learning\CM14\space-stories-cm14"
$targetRepo = "D:\Learning\CM14\Colonial-Marine"
$keyword    = "marsoc"
$branch     = "main"

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$ErrorActionPreference = "Continue"

function Stop-Script($msg) {
    Write-Host ""
    Write-Host "STOP: $msg" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

# ---- 0. Checks ----
if (-not (Test-Path -LiteralPath (Join-Path $sourceRepo ".git"))) { Stop-Script "Source repo not found: $sourceRepo" }
if (-not (Test-Path -LiteralPath (Join-Path $targetRepo ".git"))) { Stop-Script "Target repo not found: $targetRepo" }

$email = (& git -C $targetRepo config user.email)
Write-Host "Git email in target repo: $email"
Write-Host "(it must be one of the emails in GitHub -> Settings -> Emails, or the graph will not count commits)"
Write-Host ""

# ---- 1. Find files ----
Write-Host "Scanning source repo..."
$all = & git -c core.quotepath=off -C $sourceRepo ls-files -co --exclude-standard
$all = @($all | Where-Object { $_ })
Write-Host "Files in source repo: $($all.Count)"

$byName = @($all | Where-Object { $_ -match $keyword })

$exts = @(".cs", ".yml", ".yaml", ".ftl", ".json", ".xaml")
$byContent = @($all | Where-Object { $exts -contains [IO.Path]::GetExtension($_).ToLower() } | Where-Object {
    $p = Join-Path $sourceRepo $_
    (Test-Path -LiteralPath $p) -and (Select-String -LiteralPath $p -Pattern $keyword -Quiet)
})

$rels = @($byName + $byContent | Sort-Object -Unique | Where-Object {
    Test-Path -LiteralPath (Join-Path $sourceRepo $_)
})
Write-Host "MARSOC files found: $($rels.Count)"
if ($rels.Count -eq 0) { Stop-Script "No files found. Check the path or the keyword." }

# ---- 2. Group by the day each file was last modified ----
$items = foreach ($r in $rels) {
    $f = Get-Item -LiteralPath (Join-Path $sourceRepo $r)
    [pscustomobject]@{
        Rel  = $r
        When = $f.LastWriteTime
        Day  = $f.LastWriteTime.ToString("yyyy-MM-dd")
    }
}
$plan = @($items | Group-Object Day | Sort-Object Name)

Write-Host ""
Write-Host "Plan (one commit per day):"
foreach ($g in $plan) { Write-Host ("  {0}: {1} files" -f $g.Name, $g.Count) }
Write-Host "Total commits planned: $($plan.Count)"

if ($plan.Count -lt 3) {
    Write-Host ""
    Write-Host "WARNING: very few distinct days. File dates may have been reset by copying or unpacking." -ForegroundColor Yellow
}

Write-Host ""
$answer = Read-Host "Create these commits and push to origin/$branch? (y/n)"
if ($answer -ne "y") { Stop-Script "Cancelled by user." }

# ---- 3. Copy + commit, oldest day first ----
foreach ($g in $plan) {
    foreach ($it in $g.Group) {
        $dst = Join-Path $targetRepo $it.Rel
        New-Item -ItemType Directory -Force -Path (Split-Path $dst -Parent) | Out-Null
        Copy-Item -LiteralPath (Join-Path $sourceRepo $it.Rel) -Destination $dst -Force
        & git -C $targetRepo add -- $it.Rel | Out-Null
    }

    & git -C $targetRepo diff --cached --quiet
    if ($LASTEXITCODE -ne 0) {
        $latest = ($g.Group | Sort-Object When | Select-Object -Last 1).When
        $stamp  = $latest.ToString("yyyy-MM-ddTHH:mm:ss")
        $env:GIT_AUTHOR_DATE    = $stamp
        $env:GIT_COMMITTER_DATE = $stamp
        & git -C $targetRepo commit -q -m "feat(marsoc): update MARSOC files ($($g.Count) files)"
        Remove-Item Env:\GIT_AUTHOR_DATE    -ErrorAction SilentlyContinue
        Remove-Item Env:\GIT_COMMITTER_DATE -ErrorAction SilentlyContinue
        Write-Host "Committed $($g.Name): $($g.Count) files" -ForegroundColor Green
    } else {
        Write-Host "Skipped $($g.Name): nothing new (already in repo)"
    }
}

# ---- 4. Push (no --force) ----
Write-Host ""
Write-Host "Pushing..."
& git -C $targetRepo push origin $branch
if ($LASTEXITCODE -ne 0) {
    Write-Host "Push rejected, trying pull --rebase first..." -ForegroundColor Yellow
    & git -C $targetRepo pull --rebase origin $branch
    & git -C $targetRepo push origin $branch
}

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "DONE. Check https://github.com/ScarlettTheBrave/Colonial-Marine" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "Push failed. Copy the git messages above and send them to me." -ForegroundColor Red
}
Read-Host "Press Enter to exit"
