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

function Get-NumberSpoken {
  <#
  .SYNOPSIS
  Get the number spoken at a given turn.
  #>

  param (
    [int] $Turn
  )

  # Init the context
  $ctx = @{}
  for ($i = 0; $i -lt $InitNumbers.Count; $i++) {
    $ctx[$InitNumbers[$i]] = @(0, ($i + 1))
  }

  # Set previous number
  $lastSpokenNumber = $InitNumbers[-1]

  # Iterate over the turns
  for ($currentTurn = $InitNumbers.Length + 1; $currentTurn -le $Turn; $currentTurn++) {
    if ($currentTurn % 100000 -eq 0) {
      Write-Host "Turn $currentTurn"
    }

    # Get the last turns
    [int[]] $lastTurns = $ctx.ContainsKey($lastSpokenNumber) ? $ctx[$lastSpokenNumber] : @(0, $currentTurn)

    # Compute next number
    if ($lastTurns[0] -ne 0) {
      $nextNumber = $lastTurns[1] - $lastTurns[0]
    } else {
      $nextNumber = 0
    }

    # Update context for this turn
    if ($ctx.ContainsKey($nextNumber)) {
      $ctx[$nextNumber] = @($ctx[$nextNumber][1], $currentTurn)
    } else {
      $ctx[$nextNumber] = @(0, $currentTurn)
    }

    $lastSpokenNumber = $nextNumber
  }

  $lastSpokenNumber
}

$turn = $Part2 ? 30000000 : 2020
$number = Get-NumberSpoken $turn
Write-Host "Result ($turn) = $number"

# 175594 too high