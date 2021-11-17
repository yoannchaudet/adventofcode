#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Train tickets.

.PARAMETER InitialStateFile
Path to the initial state file.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $InitialStateFile,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-InitialState {
  $state = @{}
  $state[0] = [char[][]] (Get-Content $InitialStateFile)
  $state
}

function Write-State {
  <#
  .SYNOPSIS
  Print the state.
  #>
  param([hashtable] $state)
  $keys = $state.Keys | Sort-Object
  foreach ($key in $keys) {
    Write-Host "z=$key"
    $cube = $state[$key]
    for($i = 0; $i -lt $cube.Length; $i++) {
      Write-Host $cube[$i]
    }
  }
}

function Expand-State {
  <#
  .SYNOPSIS
  Expand the state by one extra dimension.
  #>
  param([hashtable] $state)

  # Get the next side N of the state
  $N = $state[0].Count + 2

  # Add extra dimensions
  $keys = $state.Keys | Sort-Object
  $state[$keys[0] - 1] = [char[][]] (Get-BlankState $N)
  $state[$keys[-1] + 1] = [char[][]] (Get-BlankState $N)

  # Increase size of existing states
  foreach ($k in $keys) {
    $newK = @()
    $newK += @(,[char[]] ("." * $N))
    for ($i = 0; $i -lt ($N - 2); $i++) {
      $line = [char[]] (@('.') + $state[$k][$i] + @('.'))
      $newK += @(,$line)
    }
    $newK += @(,[char[]] ("." * $N))
    $state[$k] = [char[][]] $newK
  }
}

function Get-BlankState {
  <#
  .SYNOPSIS
  Return a blank state of the given size.
  #>
  param ([int] $N)
  for ($i = 0; $i -lt $N; $i++) {
    "." * $N
  }
}

function Get-State {
  <#
  .SYNOPSIS
  Return the state at a given position.
  #>
  param(
    [hashtable] $State,
    [int] $X, [int] $Y, [int] $Z
  )

  # Check all bounds
  if ($State.ContainsKey($Z)) {
    if ($Y -ge 0 -and $Y -lt $State[$Z].Length) {
      if ($X -ge 0 -and $X -lt $State[$Z][$Y].Length) {
        return $State[$Z][$Y][$X]
      }
    }
  }

  # Fallback
  "."
}

function Get-ActiveNeighbors {
  <#
  .SYNOPSIS
  Return the number of active neighbors at a given position.
  #>
  param(
    [hashtable] $State,
    [int] $X, [int] $Y, [int] $Z
  )

  # Lookup active neighbors
  $active = 0
  for ($dx = $X - 1; $dx -le $X + 1; $dx++) {
    for ($dy = $Y - 1; $dy -le $Y + 1; $dy++) {
      for ($dz = $Z - 1; $dz -le $Z + 1; $dz++) {
        if (-not ($dx -eq $X -and $dy -eq $Y -and $dz -eq $Z)) {
          $s = [char] (Get-State $State $dx $dy $dz)
          if ($s -eq '#') {
            $active += 1
          }
        }
      }
    }
  }
  $active
}

# Read the initial state
$state = Get-InitialState
Write-State $state

# Execute the cycles
for ($i = 1; $i -le 1; $i++) {
  # Logging and expand the state
  Write-Host "Cycle = $i"
  Expand-State $state

  # Mutate the state
  $nextState = $state.Clone()
  foreach ($z in $state.Keys) {
    for ($y = 0; $y -lt $state[$z].Length; $y++) {
      for ($x = 0; $x -lt $state[$z][$y].Length; $x++) {
        $s = [char] $state[$z][$y][$x]
        $activeNeighbors = Get-ActiveNeighbors $state $x $y $z
        if ($s -eq '#' -and ($activeNeighbors -lt 2 -or $activeNeighbors -gt 3)) {
          $nextState[$z][$y][$x] = '.'
        }
        elseif ($s -eq '.' -and $activeNeighbors -eq 3) {
          $nextState[$z][$y][$x] = '#'
        }
      }
    }
  }

  # Print state
  $state = $nextState
  Write-State $state
  Write-Host ""
}
