#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Jolts joys.

.PARAMETER JoltageRatings
Path to the joltage ratings file.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $JoltageRatings,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-Chain {
  <#
  .SYNOPSIS
  Return a chain of adapters.
  #>

  param (
    [int[]] $ratings
  )

  # Rating of the initial outlet
  $lastRating = 0

  # Go through the adapter ratings
  foreach ($adapterRating in $ratings) {
    # Make sure the chain is valid
    if ($adapterRating - $lastRating -gt 3) {
      throw "Unable to complete the chain"
    }

    # Output an object
    [PSCustomObject] @{
      AdapterRating = $adapterRating
      Difference = $adapterRating - $lastRating
    }

    # Update last rating
    $lastRating = $adapterRating
  }

  # Add last rating
  [PSCustomObject] @{
    AdapterRating = $lastRating + 3
    Difference = 3
  }
}

# Read joltage ratings from the file and sort them
$ratings = [int[]] @(Get-Content $JoltageRatings) | Sort-Object

# Get the chain
$chain = Get-Chain $ratings
$chain | Format-Table

# Count the distributions
$dist = @()
for ($i = 0; $i -lt 3; $i++) {
  $dist += @($chain | Where-Object { $_.Difference -eq ($i + 1) }).Count
}
Write-Host "Distribution: $($dist -Join ", ")"
Write-Host "Result = " ($dist[0] * $dist[2])