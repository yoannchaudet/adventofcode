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

function Get-90DegreesRotatedPoint {
  <#
  .SYNOPSIS
  Rotate a given point by 90 degrees (clockwise).
  #>
  param ([PSCustomObject] $Point, [switch] $Counterclockwise)

  if ($Counterclockwise) {
    [PSCustomObject] @{
      East  = -$Point.North
      North = $Point.East
    }
  } else {
    [PSCustomObject] @{
      East  = $Point.North
      North = -$Point.East
    }
  }
}

# Get the instructions
$instructions = Get-Instructions

# Ship location
$shipLocation = [PSCustomObject] @{
  East  = 0
  North = 0
}

# Waypoint location (part 2 only)
$waypointLocation = [PSCustomObject] @{
  East  = 10
  North = 1
}

# Initial direction of the ship (in degrees), part 1 only
# - North: 90
# - East: 0
# - South: 270
# - West: 180
$shipDirection = 0

# Direction vectors
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
      if (-Not $Part2) {
        $shipLocation.North += $instruction.Value
      } else {
        $waypointLocation.North += $instruction.Value
      }
      break
    }
    "S" {
      if (-Not $Part2) {
        $shipLocation.North -= $instruction.Value
      } else {
        $waypointLocation.North -= $instruction.Value
      }
      break
    }
    "E" {
      if (-Not $Part2) {
        $shipLocation.East += $instruction.Value
      } else {
        $waypointLocation.East += $instruction.Value
      }
      break
    }
    "W" {
      if (-Not $Part2) {
        $shipLocation.East -= $instruction.Value
      } else {
        $waypointLocation.East -= $instruction.Value
      }
      break
    }

    # Left/Right
    "L" {
      if (-Not $Part2) {
        $shipDirection = Get-NewDirection $shipDirection $instruction.Value
      } else {
        for ($i = 0; $i -lt ($instruction.Value / 90); $i++) {
          $waypointLocation = Get-90DegreesRotatedPoint $waypointLocation -Counterclockwise
        }
      }
      break
    }
    "R" {
      if (-Not $Part2) {
        $shipDirection = Get-NewDirection $shipDirection (-1 * $instruction.Value)
      } else {
        for ($i = 0; $i -lt ($instruction.Value / 90); $i++) {
          $waypointLocation = Get-90DegreesRotatedPoint $waypointLocation
        }
      }
      break
    }

    # Forward
    "F" {
      if (-Not $Part2) {
        $vector = $directionVectors[$shipDirection]
        $shipLocation.East += $vector[0] * $instruction.Value
        $shipLocation.North += $vector[1] * $instruction.Value
      } else {
        $shipLocation.East += $waypointLocation.East * $instruction.Value
        $shipLocation.North += $waypointLocation.North * $instruction.Value
      }
      break
    }
  }
  Write-Host "$($Instruction.Action)$($Instruction.Value) `tship`t$($shipLocation.East),$($shipLocation.North)`twaypoint`t$($waypointLocation.East),$($waypointLocation.North)"
}

# Final location
$shipLocation | Format-Table
Write-Host "Manhattan distance = $(Get-ManhattanDistance $shipLocation)"
