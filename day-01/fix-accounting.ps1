#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Find and print the two entries adding up to 2020 in the expense report
and multiply them.

.PARAMETER ExpenseReport
Path to the expense report.

.PARAMETER Part2
Part two: find and print three entries adding up to 2020.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $ExpenseReport,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-TwoEntries2020 {
  param ([int[]]$Entries)

  # a + b = 2020
  # go through the list (fix a) and check if b is in it
  foreach ($a in $entries) {
    # Deduct b
    $b = 2020 - $a

    # If b is in the list, we are done
    if ($Entries -contains $b) {
      return [PSCustomObject]@{
        A = $a
        B = $b
        Result = $a * $b
      }
    }
  }
}

function Get-ThreeEntries2020 {
  param ([int[]]$Entries)

  # a + b + c = 2020
  # go through the list (fix a), fix b (with b < 2020 - a), deduct c and check if in the list
  foreach ($a in $entries) {
    $bCandidates = $entries | Where-Object { $_ -lt 2020 - $a }
    foreach ($b in $bCandidates) {
      $c = 2020 - $a - $b
      # If b is in the list, we are done
      if ($Entries -contains $c) {
        return [PSCustomObject]@{
          A = $a
          B = $b
          C = $c
          Result = $a * $b * $c
        }
      }
    }
  }
}

# Read the expense report, convert lines to int and sort everything
# Assume all entries are unique
$entries = Get-Content $ExpenseReport | ForEach-Object { [int] $_ } | Sort-Object

# Find either 2 or 3 entries adding up to 2020
$result = $Part2 ? (Get-ThreeEntries2020 $entries) : (Get-TwoEntries2020 $entries)
if ($result) {
  $result | Format-Table
} else {
  Write-Host "No results found"
}
