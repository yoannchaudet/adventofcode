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

function Get-DistinctChains2 {
  <#
  .SYNOPSIS
  Return the distinct number of chains given the longest chain.
  #>

  param (
    [PSCustomObject[]] $Chain
  )

  # Strategy: instead of trying to enumerate all solutions (that would take too long),
  # we notice (while doodling on actual paper) that by looking at the longest chain that
  # sections of adapters with a difference of 1, are creating branches in the tree.
  # These branches repeat in a fibonacci sequence.
  # For instance, if we have 0 (3), 1 (1), 2 (1), 3 (1), 6 (3):
  # The tree would be:
  #
  # 0 - 1 - 2 - 3 - 6
  #   - 2 - 3 - 6
  #   - 3 - 6
  #
  # The tree is productin exactly, 1 (current branch) + fibonacci (number of consecutives 1 in the chain - 1).
  # Here: 1 + f(2) = 1 + 2 + 1 = 3 (branches)
  #
  # On bigger trees, we observe that by multiplying the number of branches we obtain for each
  # consecutives 1s, we get the total number of branches, in a single iteration!

  # Start at one (we have the longest chain)
  $distinctChains = 1

  # Go through the longest chain
  $consecutiveOnes = 0
  for ($i = 0; $i -lt $Chain.Count; $i++) {
    # Count consecutive ones
    if ($Chain[$i].Difference -eq 1) {
      $consecutiveOnes += 1
    }

    # Aggregate new distinct chains
    else {
      Write-Host "$i consecutiveOnes = $consecutiveOnes"
      $distinctChains *= (Get-Fibonacci ($consecutiveOnes - 1)) + 1
      $consecutiveOnes = 0
    }
  }

  # Return the result
  $distinctChains
}

function Get-Fibonacci {
  <#
  .SYNOPSIS
  Return a fibonacci number for a given index.
  #>
  param ($I)

  # Just regular recurssive fibonacci
  if ($I -le 0) { # allow negative numbers here too
    0
  } elseif ($I -eq 1) {
    1
  } else {
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
  Write-Host "`nResult = $distinctChains"
}