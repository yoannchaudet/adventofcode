#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Boarding pass magic.

.DESCRIPTION
The 7 first characters of the boarding pass are just regular binary with F = 0 and B = 1.

  FBFBBFFxxx = (44)10
  (0101100)2  = (44)10

The last 3 characters are binary too with L = 0 and R = 1.

  RLR    = (5)10
  (101)2  = (5)10

.PARAMETER BoardingPasses
Path to the list of boarding passes.

.PARAMETER Part2
Part two: ?
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $BoardingPasses,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-Seats {
  <#
  .SYNOPSIS
  Return the seats (Row, Column and ID) from the list of boarding passes.
  #>

  # Read the boarding passes
  Get-Content $BoardingPasses | ForEach-Object {
    $pass = $_

    # Validate length
    if ($pass.Length -ne 10) {
      Write-Warning "Invalid boarding pass: $pass"
    }

    # Decode row
    $binaryRow = $pass.Substring(0, 7).Replace('F', '0').Replace('B', '1')
    $row = [Convert]::ToInt32($binaryRow, 2)

    # Decode column
    $binaryColumn = $pass.Substring(7, 3).Replace('L', '0').Replace('R', '1')
    $column = [Convert]::ToInt32($binaryColumn, 2)

    # Return the seat information
    [PSCustomObject] @{
      Row = $row
      Column = $column
      ID = ($row * 8 + $column)
    }
  }
}

# Get all seats
$seats = @(Get-Seats)
$seats | Format-Table

# Find the seat with the maximum seat ID
if (-Not $Part2) {
  $maxSeatID = ($seats | Measure-Object -Property ID -Maximum).Maximum
  Write-Host "Maximum seat ID = $maxSeatID"
}

# Find my seat
else {
  # We know our seat is not at the beginning or end
  # Seat +1 and -1 exists
  # Flight is full

  # Get all seats ids, sorted (asc)
  $seatIds = @($seats.ID | Sort-Object)

  # There should be at least 3 seats in there
  if ($seatIds.Count -lt 3) {
    throw "Not enough seats"
  }

  # Find the seat
  for ($i = 1; $i -lt $seatIds.Count - 1; $i++) {
    if ($seatIds[$i] -ne $seatIds[$i - 1] + 1) {
      Write-Host "My seat ID = $($seatIds[$i] - 1)"
      break
    }
  }
}