#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Bus schedules.

.PARAMETER Notes
Path to the notes file.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $Notes,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-Notes {
  <#
  .SYNOPSIS
  Read and parse the notes.
  #>

  # Read the file
  $lines = @(Get-Content $Notes)
  if ($lines.Length -lt 2) {
    throw "Not enough lines in the notes"
  }

  # Parse the content (no validation)
  [PSCustomObject] @{
    EarliestTimestamp = [int] $lines[0]
    BusIds =  [int[]] @($lines[1].Split(",") | Where-Object { $_ -ne "x" })
  }
}

function Get-EarliestBusId {
  <#
  .SYNOPSIS
  Find and return the earliest bus id that can take us.
  #>
  param ([PSCustomObject] $N)

  # Start at the earliest timestamp
  $ts = $N.EarliestTimestamp
  while ($true) {
    Write-Host "Testing timestamp $ts"

    # Find a busid divisble by the timestamp
    $busId = $N.BusIds | Where-Object { ($ts % $_) -eq 0 }
    if ($busId) {
      return @{
        Timestamp = $ts
        BusId = $busId
      }
    }
    $ts += 1
  }
}

# Get the notes
$n = Get-Notes
$n | Format-Table

# Find the earliest bus we can take
$bus = Get-EarliestBusId $n
$bus | Format-Table

# Compute wait time
$waitTime = $bus.Timestamp - $n.EarliestTimestamp
Write-Host "Wait time = $waitTime"
Write-Host "Result = $($waitTime * $bus.BusId)"
