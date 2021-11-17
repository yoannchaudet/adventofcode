#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Train tickets.

.PARAMETER NotesFile
Path to the notes file.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $NotesFile,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-Notes {
  <#
  .SYNOPSIS
  Parse the notes.
  #>

  $notes = [PSCustomObject] @{
    Fields        = @{}
    MyTicket      = $null
    NearbyTickets = @()
  }

  $nearbyTickets = $false
  foreach ($line in @(Get-Content $NotesFile)) {
    # Fields
    if ($line -match "([a-z\s]+): ([0-9]+)\-([0-9]+) or ([0-9]+)\-([0-9]+)") {
      $notes.Fields[$Matches[1]] = @(
        @([int] $Matches[2], [int] $Matches[3]),
        @([int] $Matches[4], [int] $Matches[5])
      )
    }

    # Tickets
    elseif ($line -match "[0-9]+(,[0-9])") {
      $ticket = [int[]] $line.Split(",")
      if (-Not $nearbyTickets) {
        $notes.MyTicket = $ticket
        $nearbyTickets = $true
      } else {
        $notes.NearbyTickets += ,@($ticket)
      }
    }

    # Catch all
    elseif ($line -in @("your ticket:", "nearby tickets:", "")) {
    } else {
      throw "Unable to parse line $line"
    }
  }

  $notes
}

function Test-ValueInRanges {
  <#
  .SYNOPSIS
  Test if a given value is within a given field's ranges.
  #>

  param (
    [int] $Value,
    [int[][]] $Ranges
  )

  # Test the ranges
  foreach ($range in $Ranges) {
    if ($Value -ge $range[0] -and $Value -le $range[1]) {
      return $true
    }
  }

  # Fallback
  $false
}

function Get-NearbyInvalidValues {
  <#
  .SYNOPSIS
  Return the list of compltely invalid fields for nearby tickets.

  For part 2 also update nearby tickets to remove invalid tickets.
  #>
  param ([PSCustomObject] $Notes)

  # List of valid nearby tickets
  $validNearbyTickets = @()

  # Return the values that are out of range
  foreach ($ticket in $Notes.NearbyTickets) {
    # Collect invalid values
    $invalidValues = @($ticket | ForEach-Object {
      foreach ($field in $Notes.Fields.Keys) {
        if (Test-ValueInRanges $_ $Notes.Fields[$field]) {
          return
        }
      }
      $_
    })

    # Update tickets
    if ($Part2 -and $invalidValues.Count -eq 0) {
      $validNearbyTickets += @(,$ticket)
    }

    # Return invalid values
    $invalidValues
  }

  # Update nearby tickets
  if ($Part2) {
    $Notes.NearbyTickets = $validNearbyTickets
  }
}

# Get the notes
$notes = Get-Notes
$notes | Format-Table

# Get values out of range
if (-Not $Part2) {
  $invalidValues = @(Get-NearbyInvalidValues $notes)
  Write-Host "Invalid values: $($invalidValues -Join ', ')"
  $result = ($invalidValues | Measure-Object -Sum).Sum
  Write-Host "Result = $result"
}

# Find which field is what
else {
  # Drop invalid tickets
  Write-Host "Total nearby tickets = $($notes.NearbyTickets.Count)"
  Get-NearbyInvalidValues $notes | Out-Null
  Write-Host "Valid nearby tickets = $($notes.NearbyTickets.Count)"

  # Assign each field, the indices that are within ranges
  $indicesInRanges = @{}
  foreach ($field in $Notes.Fields.Keys) {
    $indicesInRanges[$field] = @()
    for ($i = 0; $i -lt $Notes.MyTicket.Length; $i++) {
      $inRange = $true
      foreach ($nearbyTicket in $Notes.NearbyTickets) {
        if (-Not (Test-ValueInRanges $nearbyTicket[$i] $Notes.Fields[$field])) {
          $inRange = $false
          break
        }
      }
      if ($inRange) {
        $indicesInRanges[$field] += $i
      }
    }
  }
  $indicesInRanges | Format-Table

  # Remove ambiguities
  $identifiedField = @()
  while ($identifiedField.Count -lt $Notes.MyTicket.Length) {
    # Find an index that has been assigned to a field already
    $candidate = $indicesInRanges.Values | Where-Object { @($_).Count -eq 1 -and -not ($identifiedField -Contains @($_)[0]) }
    if (@($candidate).Count -eq 0) {
      throw "Unable to remove all ambiguities"
    }
    $candidate = $candidate[0][0]

    # Collect the index
    $identifiedField += $candidate

    # Update other fields not to reference that index anymore
    $fields = @($indicesInRanges.Keys)
    foreach ($field in $fields) {
      if ($indicesInRanges[$field].Count -gt 1) {
        $indicesInRanges[$field] = @($indicesInRanges[$field] | Where-Object { $_ -ne $candidate })
      }
    }
  }

  # Compute result
  Write-Host "Without ambiguities:"
  $indicesInRanges | Format-Table
  $result = 1
  foreach ($field in $Notes.Fields.Keys) {
    if ($field.Contains("departure")) {
      $result *= $notes.MyTicket[$indicesInRanges[$field][0]]
    }
  }
  Write-Host "Result = $result"
}