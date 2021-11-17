#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Find the number of trees one would land into following a given slope.

.PARAMETER Map
Path to the map.

.PARAMETER Part2
Part two: use more slopes.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $Map,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-TreeCount {
  <#
  .SYNOPSIS
  Get the number of trees encoutered for a given terrain and slope.
  #>
  param (
    [string[]] $Terrain,
    [int] $SlopeDown,
    [int] $SlopeRight
  )

  # Number of trees we found
  $trees = 0

  # Left (x) position, 0-indexed
  $x = 0

  # Slide down!
  $y = $SlopeDown
  while ($y -lt $terrain.Count) {
    $x += $SlopeRight
    if ($terrain[$y][$x % $terrain[$y].Length] -eq '#') {
      $trees += 1
    }
    $y += $SlopeDown
  }

  # Return the number of trees
  $trees
}

# Read the map
$terrain = Get-Content $Map

# Get the number of trees encountered for a fixed slope
if (-Not $Part2) {
  $trees = Get-TreeCount -Terrain $terrain -SlopeDown 1 -SlopeRight 3
  Write-Host "Number of encoutered tree(s): $trees"
}

# Part 2
else {
  $result = 1
  @(
    @(1, 1),
    @(1, 3),
    @(1, 5),
    @(1, 7),
    @(2, 1)
  ) | ForEach-Object {
    # Get number of trees for given slope
    $trees = Get-TreeCount -Terrain $terrain -SlopeDown $_[0] -SlopeRight $_[1]
    Write-Host "[down: $($_[0]), right: $($_[1])] Number of encoutered tree(s): $trees"

    # Aggregate result
    $result *= $trees
  }

  # Print result
  Write-Host "Result = $result"
}
