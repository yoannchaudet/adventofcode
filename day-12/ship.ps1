#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Ship's directions.

.PARAMETER NavInstructions
Path to the navigation instructions file.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $NavInstructions,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-Instructions {
  <#
  .SYNOPSIS
  Parse and return the instructions.
  #>
  Get-Content $NavInstructions | ForEach-Object {
    $line = $_
    if ($line -match "(N|S|E|W|L|R|F)([0-9]+)") {
      [PSCustomObject] @{
        Action  = $Matches[1]
        Value   = [int] $Matches[2]
      }
    } else {
      throw "Invalid instruction: $line"
    }
  }
}

function Get-ManhattanDistance {
  <#
  .SYNOPSIS
  Return the Manhattan distance of a given location.
  #>
  param ([PSCustomObject] $Location)

  # Return distance
  [Math]::Abs($Location.East) + [Math]::Abs($Location.North)
}

function Get-NewDirection {
  <#
  .SYNOPSIS
  Return a direction + a given offset.
  #>
  param ([int] $Direction, [int] $Offset)

  # Sum the two (modulus 360)
  $Direction = ($Direction + $Offset) % 360

  # Fix the direction if it becomes negative
  if ($Direction -lt 0) {
    $Direction = (360 - [Math]::Abs($Direction)) % 360
  }

  # Return the direction
  $Direction
}

# Get the instructions
$instructions = Get-Instructions

# Initial location
$location = [PSCustomObject] @{
  East  = 0
  North = 0
}

# Initial direction (in degrees)
# - North: 90
# - East: 0
# - South: 270
# - West: 180
$direction = 0
$directionVectors = @{
  #      (East,North)
  0   = @(+1, 0)
  90  = @(0, +1)
  180 = @(-1, 0)
  270 = @(0, -1)
}

# Process the instructions
foreach ($instruction in $instructions) {
  switch ($instruction.Action) {

    # Simple directions
    "N" {
      $location.North += $instruction.Value
      break
    }
    "S" {
      $location.North -= $instruction.Value
      break
    }
    "E" {
      $location.East += $instruction.Value
      break
    }
    "W" {
      $location.East -= $instruction.Value
      break
    }

    # Left/Right
    "L" {
      $direction = Get-NewDirection $direction $instruction.Value
      break
    }
    "R" {
      $direction = Get-NewDirection $direction (-1 * $instruction.Value)
      break
    }

    # Forward
    "F" {
      $vector = $directionVectors[$direction]
      $location.East += $vector[0] * $instruction.Value
      $location.North += $vector[1] * $instruction.Value
      break
    }
  }
  Write-Host "$($Instruction.Action)$($Instruction.Value) `t`t $($location.East),$($location.North)"

}

# Final location
$location | Format-Table
Write-Host "Manhattan distance = $(Get-ManhattanDistance $location)"
