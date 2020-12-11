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

function Get-DistinctChains {
  <#
  .SYNOPSIS
  Return the number of distinct chains.
  #>

  param (
    [int[]] $Ratings,
    $LastRating = 0
  )

  # Count the number of branches we can do at this point in the tree
  $nextSet = @($Ratings | Where-Object { $_ -gt $LastRating -and $_ -le ($LastRating + 3) })

  # Output 1 (we have a complete chain)
  if ($nextSet.Count -eq 0) {
    1
  }

  # Recursively discover other branches
  else {
    $nextSet | ForEach-Object { Get-DistinctChains $Ratings $_ }
  }
}

function Get-DistinctChains2 {
  param (
    [PSCustomObject[]] $Chain
  )

  # Start at one
  $distinctChains = 1

  # Go through the chain
  $consecutiveOnes = 0
  for ($i = 0; $i -lt $Chain.Count; $i++) {
    if ($Chain[$i].Difference -eq 1) {
      $consecutiveOnes += 1
    } else {
      Write-Host "$i consecutiveOnes = $consecutiveOnes"
      $distinctChains *= (Get-Fibonacci ($consecutiveOnes - 1)) + 1
      $consecutiveOnes = 0
    }
  }
  $distinctChains
}

function Get-Fibonacci {
  <#
  .SYNOPSIS
  Return a fibonacci number for a given index.
  #>
  param ($I)

  if ($I -le 0) {
    0
  }
  elseif ($I -eq 1) {
    1
  }
  else {
    $I + (Get-Fibonacci ($I - 1))
  }
}

# Read joltage ratings from the file and sort them
$ratings = [int[]] @(Get-Content $JoltageRatings) | Sort-Object

# Part 1
if (-Not $Part2) {
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
}

# Part 2
if ($Part2) {
  $chain = Get-Chain $ratings
  $distinctChains = Get-DistinctChains2 $chain
  Write-Host "Result = $distinctChains"
}