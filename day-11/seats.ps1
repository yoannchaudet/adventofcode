#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Simulate the waiting room.

.PARAMETER WaitingRoomLayout
Path to the waiting room layout file.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $WaitingRoomLayout,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-AdjacentOccupiedSeats {
  <#
  .SYNOPSIS
  Return the number of adjacent seats that are occupied.
  #>
  param (
    [char[][]] $Layout,
    [int] $X,
    [int] $Y
  )


  # Adjacent cells are:
  # xxx
  # x.x
  # xxx
  $adjacentCells = 0
  $adjacentOffsets = @(
    @{ X =  -1; Y = -1},
    @{ X =   0; Y = -1},
    @{ X =  +1; Y = -1},
    @{ X =  -1 ;Y =  0},
    @{ X =  +1 ;Y =  0},
    @{ X =  -1 ;Y = +1},
    @{ X =   0 ;Y = +1},
    @{ X =  +1 ;Y = +1}
  )

  # Collect the occupied adjacent cells
  foreach ($offset in $adjacentOffsets) {
    $adjacentX = $X + $offset.X
    $adjacentY = $Y + $offset.Y
    if ($adjacentX -In 0..($Layout[0].Length - 1) -and $adjacentY -In 0..($Layout.Length - 1)) {
      if ($Layout[$adjacentY][$adjacentX] -eq $OCCUPIED_SEAT) {
        $adjacentCells += 1
      }
    }
  }
  $adjacentCells
}

function Get-VisibleOccupiedSeats {
 <#
  .SYNOPSIS
  Return the number of visible seats that are occupied.
  #>
  param (
    [char[][]] $Layout,
    [int] $X,
    [int] $Y
  )


  # Adjacent cells are:
  # xxx
  # x.x
  # xxx
  $adjacentCells = 0
  $adjacentOffsets = @(
    @{ X =  -1; Y = -1},
    @{ X =   0; Y = -1},
    @{ X =  +1; Y = -1},
    @{ X =  -1 ;Y =  0},
    @{ X =  +1 ;Y =  0},
    @{ X =  -1 ;Y = +1},
    @{ X =   0 ;Y = +1},
    @{ X =  +1 ;Y = +1}
  )

  # Collect the visible occupied cells
  foreach ($offset in $adjacentOffsets) {
    $adjacentX = $X
    $adjacentY = $Y
    # The offsets are now vectors
    while ($true) {
      $adjacentX += $offset.X
      $adjacentY += $offset.Y
      if ($adjacentX -In 0..($Layout[0].Length - 1) -and $adjacentY -In 0..($Layout.Length - 1)) {
        if ($Layout[$adjacentY][$adjacentX] -eq $OCCUPIED_SEAT) {
          $adjacentCells += 1
          break
        } else {
          # Stop when seeing a chair
          if ($Layout[$adjacentY][$adjacentX] -eq $EMPTY_SEAT) {
            break
          }
        }
      }

      # Stop when hitting a wall
      else {
        break
      }
    }
  }
  $adjacentCells
}

function Copy-Layout {
  <#
  .SYNOPSIS
  Return a copy of the layout.
  #>
  param (
    [char[][]] $Layout
  )
  $newLayout = @()
  foreach ($row in $Layout) {
    $newRow = $row | ForEach-Object { $_ }
    $newLayout += @(,$newRow)
  }
  $newLayout
}

function Get-OccupiedSeats {
  <#
  .SYNOPSIS
  Return the number of occupied seats in a layout.
  #>
  param (
    [char[][]] $Layout
  )
  $occupiedSeats = 0
  foreach ($row in $Layout) {
    $occupiedSeats += @($row | Where-Object { $_ -eq $OCCUPIED_SEAT }).Count
  }
  $occupiedSeats
}

function Invoke-Simulation {
  <#
  .SYNOPSIS
  Invoke the simulation on a given layout until the total number of occupied seats stabilizes.
  #>
  param (
    [char[][]] $Layout,
    [int] $OccupiedToEmptyThreshold = 4,
    [scriptblock] $GetOccupiedSeatsBlock = { param ([char[][]] $Layout, [int] $X, [int] $Y) Get-AdjacentOccupiedSeats $Layout $X $Y }
  )

  # Get the previous occupied seats
  $layoutOccupiedSeats = Get-OccupiedSeats $Layout
  Write-Host "Step #0"
  Write-Host "Occupied seats = $layoutOccupiedSeats"

  # Step
  $step = 1

  # Loop
  while ($true) {
    Write-Host "Step #$Step"

    # Copy the layout
    $stepLayout = Copy-Layout $Layout

    # Loop through the cells
    foreach ($y in 0..($Layout.Length - 1)) {
      foreach ($x in 0..($Layout[0].Length - 1)) {

        # Empty seats and no adjacents occupied seats -> occupied
        if ($Layout[$y][$x] -eq $EMPTY_SEAT -and (& $GetOccupiedSeatsBlock $Layout $x $y) -eq 0) {
          $stepLayout[$y][$x] = $OCCUPIED_SEAT
        }

        # Occupied and 4 or more adjacent seats occupied -> empty
        if ($Layout[$y][$x] -eq $OCCUPIED_SEAT -and (& $GetOccupiedSeatsBlock $Layout $x $y) -ge $OccupiedToEmptyThreshold) {
          $stepLayout[$y][$x] = $EMPTY_SEAT
        }
      }
    }

    # End state
    $stepOccupiedSeats = Get-OccupiedSeats $stepLayout
    Write-Host "Occupied seats = $stepOccupiedSeats"
    if ($stepOccupiedSeats -eq $layoutOccupiedSeats) {
      break
    }
    $Layout = $stepLayout
    $layoutOccupiedSeats = $stepOccupiedSeats
    $Step += 1
  }

  # Return the occupied seats
  $layoutOccupiedSeats
}

# Constants
$EMPTY_SEAT     =  'L'
$OCCUPIED_SEAT  =  '#'
$FLOOR          =  '.'

# Read the layout
$layout = [char[][]] @(Get-Content $WaitingRoomLayout)
$layout | Write-Host

# Get the stable occupied seats
$options = @{
  Layout = $layout
}
if ($Part2) {
  $options['OccupiedToEmptyThreshold'] = 5
  $options['GetOccupiedSeatsBlock'] = { param ([char[][]] $Layout, [int] $X, [int] $Y) Get-VisibleOccupiedSeats $Layout $X $Y }
}
$stableOccupiedSeats = Invoke-Simulation @options
Write-Host
Write-Host "Stable occupied seats = $stableOccupiedSeats"