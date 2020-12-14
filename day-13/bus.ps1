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

function Get-BusOffsets {
  <#
  .SYNOPSIS
  Read the notes and extract the bus offsets.
  #>

  # Read notes
  $lines = Get-Content $Notes
  if ($lines.Length -lt 2) {
    throw "Not enough lines in the notes"
  }

  # Extract bus offsets
  $busIds = $lines[1].Split(",")
  for ($i = 0; $i -lt $busIds.Length; $i++) {
    if ($busIds[$i] -ne "x") {
      # Return the object
      [PSCustomObject] @{
        BusId = [int] $busIds[$i]
        Offset = $i
      }
    }
  }
}

function Get-EarliestTimestamp {
  <#
  .SYNOPSIS
  Find the earliest timetstamp satisfying the entire schedule.

  .DESCRIPTION
  I got some help on this one.

  Only way to find a timestamp is to iterate.

  Start incrementing the timestamp by the first bus id (this way we know each timestamp will
  match the first bus). When we find a timestamp that also matches the second bus id, we multiply
  the counter by the bus id, and so on.

  The key is that bus ids are prime numbers so they can be divided by 1 and themselves only.

  That means incrementing the counter like we do will not cause us to miss any timestamp.
  #>
  param ([PSCustomObject] $Offsets, [uint64] $Start = 0)

  # Start with the first bus id
  $ts = $Offsets[0].BusId

  # Increment to use for the search
  $increment = $ts

  # Offset of the last bus id included in the increment
  $incrementOffset = 0

  # Start the loop
  while ($true) {
    Write-Host "Trying $ts"

    # End of the search flag
    $found = $true

    # Test the timestamp against all bus ids
    for ($i = 0; $i -lt $Offsets.Count; $i++) {
      # When the timestamp matches a new bus, increment the counter
      if (($ts + $Offsets[$i].Offset) % $Offsets[$i].BusId -eq 0) {
        if ($incrementOffset -lt $i) {
          $increment = $increment * $Offsets[$i].BusId
          $incrementOffset += 1
          Write-Host "Increment = $increment"
        }
      }

      # Exit the loop
      else {
        $found = $false
        break
      }
    }

    # Exit
    if ($found) {
      return $ts
    }

    # Increment the timestamp
    $ts += $increment
  }
}

# Part 1
if (-Not $Part2) {
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
}

# Part 2
else {
  # Read the offsets from the notes
  $offsets = Get-BusOffsets
  $offsets | Format-Table

  # Find earliest timestamp satisfying the entire schedule
  $ts = Get-EarliestTimestamp $offsets
  Write-Host "Timestamp = $ts"
}