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
    if ($line -match "([a-z]+): ([0-9]+)\-([0-9]+) or ([0-9]+)\-([0-9]+)") {
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

function Get-NearbyInvalidValues {
  <#
  .SYNOPSIS
  Return the list of compltely invalid fields for nearby tickets.
  #>
  param ([PSCustomObject] $Notes)

  # Return the values that are out of range
  foreach ($ticket in $Notes.NearbyTickets) {
    $ticket | ForEach-Object {
      foreach ($field in $Notes.Fields.Keys) {
        foreach ($range in $Notes.Fields[$field]) {
          if ($_ -ge $range[0] -and $_ -le $range[1]) {
            return
          }
        }
      }
      $_
    }
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