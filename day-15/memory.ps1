#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Memory game.

.PARAMETER StartingNumbers
Init numbers.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [int[]] $InitNumbers,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

# Init the context
$ctx = @{}
for ($i = 0; $i -lt $InitNumbers.Count; $i++) {
  $ctx[$InitNumbers[$i]] = @(0, ($i + 1))
}

function Get-NumberSpoken {
  <#
  .SYNOPSIS
  Get the number spoken at a given turn.
  #>

  param (
    [int] $Turn
  )

  # The turn is one of the init number
  if ($Turn -le $InitNumbers.Length) {
    $InitNumbers[$Turn - 1]
  }

  # Handle regular turn
  else {
    # Get the last spoken number
    $lastSpokenNumber = Get-NumberSpoken ($Turn - 1)

    # Get the last turns
    [int[]] $lastTurns = $ctx.ContainsKey($lastSpokenNumber) ? $ctx[$lastSpokenNumber] : @(0, $Turn)

    # Compute next number
    if ($lastTurns[0] -ne 0) {
      $nextNumber = $lastTurns[1] - $lastTurns[0]
    } else {
      $nextNumber = 0
    }

    # Update context for this turn
    if ($ctx.ContainsKey($nextNumber)) {
      $ctx[$nextNumber] = @($ctx[$nextNumber][1], $Turn)
    } else {
      $ctx[$nextNumber] = @(0, $Turn)
    }

    # Return next number
    $nextNumber
  }
}

$number = Get-NumberSpoken 2020
Write-Host "Result = $number"