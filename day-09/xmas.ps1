#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
XMAS cracking.

.PARAMETER Transmission
Path to the tramsmission file.

.PARAMETER Part2
Part two switch.

.PARAMETER PreambleSize
The size of the preamble.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $Transmission,
  [switch] $Part2,
  [int] $PreambleSize = 25
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-Transmission {
  <#`
  .SYNOPSIS
  Read the transmission file and extract the preamble from the rest.
  #>

  $lines = @(Get-Content $Transmission)
  if ($lines.Count -lt $PreambleSize) {
    throw "Transmission file does not contain enough information for a preamle"
  }

  # Return the object
  [PSCustomObject] @{
    Preamble      = [int64[]] $lines[0..($PreambleSize - 1)]
    Transmission  = [int64[]] $lines[$PreambleSize..($lines.Count - 1)]
  }
}

function Get-ValidSums {
  <#
  .SYNOPSIS
  Return the valid sums given a preamble.
  #>

  param (
    [int64[]] $Preamble
  )

  # Return all sum combinations
  for ($i = 0; $i -lt $Preamble.Length; $i++) {
    for ($j = $i + 1; $j -lt $Preamble.Length; $j++) {
      $Preamble[$i] + $Preamble[$j]
    }
  }
}

function Get-FirstError {
  <#
  .SYNOPSIS
  Return the first error in a given transmission.
  #>

  param (
    [PSCustomObject] $T
  )

  # Get initial preamble
  $preamble = $T.Preamble

  # Go through the transmission
  for ($i = 0; $i -lt $T.Transmission.Length; $i++) {
    # If the number of is not a valid sum, print it and exit
    if (-Not (@(Get-ValidSums $preamble) -contains $T.Transmission[$i])) {
      $T.Transmission[$i]
      break
    }

    # Shift the preamble
    $preamble = $preamble[1..($preamble.Length - 1)] + $T.Transmission[$i]
  }
}

function Find-Set {
  <#
  .SYNOPSIS
  Find a set of contiguous numbers in a given list that sums up to a given number.
  #>

  param (
    [int64[]] $List,
    [int64] $Sum
  )

  # Go through all valid positions for the beginning of the set
  for ($setStartIndex = 0; $setStartIndex -lt $List.Length - 1; $setStartIndex++) {
    $currentSum = 0
    for ($i = $setStartIndex; $i -lt $List.Length; $i++) {
      $currentSum += $List[$i]
      if ($currentSum -eq $Sum) {
        return $List[$setStartIndex..$i]
      } elseif ($currentSum -gt $Sum) {
        break
      }
    }
  }
}

# Read the transmission
$t = Get-Transmission
$firstError = Get-FirstError $t
Write-Host "First error = $firstError"

# Execute part 2
if ($Part2) {
  $set = @(Find-Set -List ($t.Preamble + $t.Transmission) -Sum $firstError)
  $minMax = $set | Measure-Object -Minimum -Maximum
  Write-Host "Weak set = $set"
  Write-Host "Minimum = $($minMax.Minimum)"
  Write-Host "Maximum = $($minMax.Maximum)"
  $weakness = $minMax.Minimum + $minMax.Maximum
  Write-Host "Weakness = $weakness"
}